//! Pattern counts: rectangles, and the symmetry-fixed counts that give
//! equivalence classes on n x n via Burnside's lemma.

use crate::count::{crt, BigUint, Count, ModPair, PRIMES, U256};
use crate::engine::{run, single_component, Dp};
use crate::frontier::{rect_cells, slot, Cell, Force, Src, WrapBit};

/// Number of valid h x w patterns (empty grid included).
pub fn count_rect(h: usize, w: usize) -> (U256, usize) {
    let dp: Dp<U256> = run(&rect_cells(h, w, &|_, _| Force::Free), w, true);
    (single_component(&dp, w), dp.max_states)
}

/// Bits of headroom the CRT result must leave below the prime product. A
/// count too large for the primes reconstructs to an essentially random value
/// in [0, M), which lands this close to the top with probability ~2^-32.
const CRT_SLACK_BITS: u32 = 32;

/// Number of valid h x w patterns by modular counting: one engine pass per
/// pair of 61-bit primes, combined with CRT. No size limit and 32 B per state
/// (vs 48 B for `count_rect`), at the cost of one pass per 122 bits.
pub fn count_rect_mod(h: usize, w: usize) -> (BigUint, usize) {
    // ln a ~ 0.5605 hw (bulk growth), i.e. ~0.81 bits per cell; the slack covers
    // the error of this estimate on small grids, and the check below catches it.
    let est_bits = (0.81 * (h * w) as f64).ceil() as u32 + 2 * CRT_SLACK_BITS;
    let pairs = (est_bits as usize).div_ceil(122);
    assert!(2 * pairs <= PRIMES.len(), "{h}x{w} needs more than {} primes", PRIMES.len());

    let mut residues = vec![];
    let mut max_states = 0;
    for pass in 0..pairs {
        let (r, st) = mod_pass(h, w, 2 * pass);
        residues.extend(r);
        max_states = max_states.max(st);
    }
    let x = crt(&residues);
    let m_bits: u32 = PRIMES[..residues.len()].iter().map(|&p| 64 - p.leading_zeros() - 1).sum();
    assert!(
        x.bits() + CRT_SLACK_BITS <= m_bits,
        "{h}x{w}: CRT result uses {} of {m_bits} bits; too few primes",
        x.bits()
    );
    (x, max_states)
}

fn mod_pass(h: usize, w: usize, off: usize) -> ([u64; 2], usize) {
    fn go<const OFF: usize>(h: usize, w: usize) -> ([u64; 2], usize) {
        let dp: Dp<ModPair<OFF>> = run(&rect_cells(h, w, &|_, _| Force::Free), w, true);
        (single_component(&dp, w).0, dp.max_states)
    }
    match off {
        0 => go::<0>(h, w),
        2 => go::<2>(h, w),
        4 => go::<4>(h, w),
        6 => go::<6>(h, w),
        8 => go::<8>(h, w),
        10 => go::<10>(h, w),
        12 => go::<12>(h, w),
        14 => go::<14>(h, w),
        _ => unreachable!(),
    }
}

/// Patterns fixed by left-right reflection.
pub fn fixed_reflect(n: usize) -> U256 {
    let half = n.div_ceil(2);
    let cells = rect_cells(n, n, &|_, c| if c >= half { Force::SameAs(n - 1 - c) } else { Force::Free });
    single_component(&run(&cells, n, true), n)
}

/// Patterns fixed by transposition (reflection in the main diagonal).
///
/// Scans the upper triangle as a staircase. Every non-empty symmetric pattern
/// must touch the diagonal (its black cells off the diagonal come in mirror
/// pairs that can only connect through it), so we count all staircase fills
/// minus those with a white diagonal, then add back the empty grid.
pub fn fixed_diag(n: usize) -> U256 {
    let build = |diag_white: bool| {
        let mut v = vec![];
        for r in 0..n {
            for c in r..n {
                let on_diag = c == r;
                v.push(Cell {
                    slot: c,
                    left: match (on_diag, r > 0) {
                        (true, true) => Src::SameAsUp,
                        (true, false) => Src::None,
                        _ => Src::Slot(c - 1),
                    },
                    diag: match (on_diag, r > 0) {
                        (true, true) => Src::Slot(r - 1),
                        (true, false) => Src::None,
                        _ => Src::Bit,
                    },
                    set_bit: c + 1 < n,
                    force: if on_diag && diag_white { Force::White } else { Force::Free },
                    pin: None,
                    drop: if on_diag && r > 0 { Some(r - 1) } else { None },
                    right: None,
                    wrap_bit: WrapBit::Clear,
                });
            }
        }
        v
    };
    let all: U256 = single_component(&run(&build(false), n, true), n);
    let white_diag: U256 = single_component(&run(&build(true), n, true), n);
    let mut r = all.checked_sub(&white_diag).unwrap();
    r.add_assign(&U256::one());
    r
}

struct UnionFind(Vec<usize>);

impl UnionFind {
    fn new(n: usize) -> Self {
        UnionFind((0..n).collect())
    }
    fn find(&mut self, x: usize) -> usize {
        let mut r = x;
        while self.0[r] != r {
            r = self.0[r];
        }
        let mut y = x;
        while self.0[y] != r {
            let next = self.0[y];
            self.0[y] = r;
            y = next;
        }
        r
    }
    fn union(&mut self, a: usize, b: usize) {
        let (a, b) = (self.find(a), self.find(b));
        self.0[a] = b;
    }
    fn all_joined(&mut self, nodes: impl IntoIterator<Item = usize>) -> bool {
        let mut it = nodes.into_iter();
        let Some(first) = it.next() else { return true };
        let r = self.find(first);
        it.all(|x| self.find(x) == r)
    }
}

/// Patterns fixed by 180-degree rotation.
///
/// Scans the top half (plus the middle row for odd n, its right half mirrored),
/// then glues the scanned half to its rotated copy along the seam.
pub fn fixed_rot180(n: usize) -> U256 {
    let m = n / 2;
    let odd = n % 2 == 1;
    let rows = if odd { m + 1 } else { m };
    let cells = rect_cells(rows, n, &|r, c| {
        if odd && r == m && c > m { Force::SameAs(n - 1 - c) } else { Force::Free }
    });
    let dp: Dp<U256> = run(&cells, n, false);
    let mut tot = U256::ZERO;
    for (s, cnt) in &dp.states {
        let l: Vec<usize> = (0..n).map(|i| slot(*s, i) as usize).collect();
        let k = *l.iter().max().unwrap();
        if k == 0 {
            tot.add_assign(cnt); // empty grid
            continue;
        }
        // Nodes 0..k: scanned components; k..2k: their rotated copies.
        let mut uf = UnionFind::new(2 * k);
        let mut ok = true;
        for c in 0..n {
            if l[c] == 0 {
                continue;
            }
            let mc = n - 1 - c;
            if l[mc] != 0 {
                // Odd: (m,c) is the image of (m,mc). Even: (m,c), below (m-1,c), is the image of (m-1,mc).
                uf.union(l[c] - 1, k + l[mc] - 1);
            }
            // Even n: 2x2 block straddling the seam.
            if !odd && c + 1 < n && l[c + 1] != 0 && l[mc] != 0 && l[mc - 1] != 0 {
                ok = false;
                break;
            }
        }
        if ok && uf.all_joined(0..2 * k) {
            tot.add_assign(cnt);
        }
    }
    tot
}

/// Patterns fixed by 90-degree rotation.
///
/// Scans one quadrant (m x m, or m x (m+1) for odd n), pinning the labels of
/// its last column, then glues four rotated copies (plus the centre cell for
/// odd n) and checks connectivity and seam 2x2 blocks.
pub fn fixed_rot90(n: usize) -> U256 {
    let m = n / 2;
    let odd = n % 2 == 1;
    if m == 0 {
        return U256::from_u64(2); // 1x1: both colourings
    }
    let (qh, qw) = if odd { (m, m + 1) } else { (m, m) };
    // Slots 0..qw: frontier. Slots qw..qw+qh: pinned last column (r, qw-1).
    let nslots = qw + qh;
    let mut cells = rect_cells(qh, qw, &|_, _| Force::Free);
    for (i, cell) in cells.iter_mut().enumerate() {
        let (r, c) = (i / qw, i % qw);
        if c == qw - 1 {
            cell.pin = Some(qw + r);
        }
    }
    let dp: Dp<U256> = run(&cells, nslots, false);
    let mut tot = U256::ZERO;
    for (s, cnt) in &dp.states {
        let lr: Vec<usize> = (0..qw).map(|i| slot(*s, i) as usize).collect(); // last row (m-1, c)
        let pc: Vec<usize> = (0..qh).map(|r| slot(*s, qw + r) as usize).collect(); // last column (r, qw-1)
        let k = (0..nslots).map(|i| slot(*s, i) as usize).max().unwrap();
        for centre_black in [false, true].into_iter().take(if odd { 2 } else { 1 }) {
            if k == 0 {
                tot.add_assign(cnt); // empty quadrant: empty grid, or lone centre cell
                continue;
            }
            let mut ok = (0..qh.saturating_sub(1))
                .all(|r| !(pc[r] != 0 && pc[r + 1] != 0 && lr[r] != 0 && lr[r + 1] != 0));
            if !odd && pc[m - 1] != 0 {
                ok = false; // centre 2x2 = four copies of the corner cell
            }
            if odd && centre_black && pc[m - 1] != 0 && lr[m - 1] != 0 {
                ok = false;
            }
            if !ok {
                continue;
            }
            let mut uf = UnionFind::new(4 * k + 1);
            for q in 0..4 {
                let q2 = (q + 1) % 4;
                for r in 0..qh {
                    if pc[r] != 0 && lr[r] != 0 {
                        uf.union(q * k + pc[r] - 1, q2 * k + lr[r] - 1);
                    }
                }
                if centre_black && pc[m - 1] != 0 {
                    uf.union(q * k + pc[m - 1] - 1, 4 * k);
                }
            }
            let nodes = 0..4 * k + usize::from(centre_black);
            if uf.all_joined(nodes) {
                tot.add_assign(cnt);
            }
        }
    }
    tot
}

/// Counts fixed by each symmetry of the square, and the resulting number of
/// equivalence classes under rotation and reflection.
#[derive(Debug, Clone)]
pub struct Classes {
    pub identity: U256,
    pub rot90: U256,
    pub rot180: U256,
    pub reflect: U256,
    pub diag: U256,
    pub classes: U256,
}

/// Burnside over the dihedral group D4: (id + rot180 + 2 rot90 + 2 reflect + 2 diag) / 8.
pub fn classes(n: usize, identity: U256) -> Classes {
    let (rot90, rot180, reflect, diag) = (fixed_rot90(n), fixed_rot180(n), fixed_reflect(n), fixed_diag(n));
    let mut sum = identity;
    sum.add_assign(&rot180);
    for x in [&rot90, &reflect, &diag] {
        sum.add_assign(x);
        sum.add_assign(x);
    }
    let (classes, rem) = sum.div_rem_small(8);
    assert_eq!(rem, 0, "Burnside sum not divisible by 8 for n={n}");
    Classes { identity, rot90, rot180, reflect, diag, classes }
}
