//! Parallel transfer-matrix engine.
//!
//! Each cell step expands every frontier state into at most two successors
//! (white / black), written into a preallocated buffer in parallel. The buffer
//! is then sorted by key and equal keys are merged.

use rayon::prelude::*;

use crate::count::Count;
use crate::frontier::{decode, normalize, Cell, Force, Key, Src, WrapBit, DIAG, DIAG2, FRESH, MAX_SLOTS};

/// Never produced (slots use bits 0..115, flags bits 126..127), so it marks
/// unused successor slots and sorts last.
const SENTINEL: Key = Key::MAX;
const MIN_CHUNK: usize = 4096;

pub struct Dp<C> {
    /// Live frontier states after the last cell, sorted by key.
    pub states: Vec<(Key, C)>,
    /// Grids whose single black component closed early (rest forced white).
    pub finished: C,
    /// Largest number of live states seen at any step.
    pub max_states: usize,
}

/// Apply drops and closure rules to a successor; returns its key, or `None`
/// if it is invalid or was counted as finished.
#[inline]
#[allow(clippy::too_many_arguments)]
fn settle<C: Count>(
    cell: &Cell,
    nslots: usize,
    allow_close: bool,
    mut lab: [u8; MAX_SLOTS],
    removed: u8,
    ndiag: Key,
    cnt: &C,
    fin: &mut C,
) -> Option<Key> {
    let mut gone = [0u8; 2];
    let mut ng = 0;
    if let Some(d) = cell.drop {
        if lab[d] != 0 {
            gone[ng] = lab[d];
            ng += 1;
        }
        lab[d] = 0;
    }
    if removed != 0 && !(ng == 1 && gone[0] == removed) {
        gone[ng] = removed;
        ng += 1;
    }
    let live = &lab[..nslots];
    let closed = gone[..ng].iter().filter(|l| !live.contains(l)).count();
    if closed == 0 {
        return Some(normalize(live) | ndiag);
    }
    // A component left the frontier for good. Valid only if it was the sole
    // black component and every remaining cell is white.
    if closed == 1 && allow_close && live.iter().all(|&x| x == 0) {
        fin.add_assign(cnt);
    }
    None
}

/// Expand one state across one cell into `out[0]` (white) and `out[1]` (black).
#[inline]
fn step<C: Count>(
    cell: &Cell,
    nslots: usize,
    allow_close: bool,
    s: Key,
    cnt: C,
    out: &mut [(Key, C)],
    fin: &mut C,
) {
    let lab = decode(s, nslots);
    let up = lab[cell.slot];
    let left = match cell.left {
        Src::None => 0,
        Src::Slot(j) => lab[j],
        Src::SameAsUp => up,
        Src::Bit => unreachable!(),
    };
    let diag_black = match cell.diag {
        Src::None => false,
        Src::Bit => s & DIAG != 0,
        Src::Slot(j) => lab[j] != 0,
        Src::SameAsUp => unreachable!(),
    };
    let mut ndiag = if cell.set_bit && up != 0 { DIAG } else { 0 };
    ndiag |= match cell.wrap_bit {
        WrapBit::Clear => 0,
        WrapBit::SetFromUp => if up != 0 { DIAG2 } else { 0 },
        WrapBit::Keep => s & DIAG2,
    };
    // Cylinder seam: the block (above, above column 0, this cell, column 0).
    let right = cell.right.map_or(0, |j| lab[j]);
    let wrap_block = cell.right.is_some() && s & DIAG2 != 0 && up != 0 && right != 0;
    let (can_white, can_black) = match cell.force {
        Force::Free => (true, true),
        Force::White => (true, false),
        Force::SameAs(j) => (lab[j] == 0, lab[j] != 0),
    };

    if can_white {
        let mut l2 = lab;
        l2[cell.slot] = 0;
        if let Some(k) = settle(cell, nslots, allow_close, l2, up, ndiag, &cnt, fin) {
            out[0] = (k, cnt);
        }
    }
    // Black is forbidden if it would complete a 2x2 black block.
    if can_black && !(diag_black && up != 0 && left != 0) && !wrap_block {
        let mut l2 = lab;
        // A SameAsUp left neighbour is the mirror of `up`: same component.
        let left_lab = match cell.left {
            Src::Slot(j) => l2[j],
            _ => 0,
        };
        // Join every black neighbour's component into the first one found.
        let nbrs = [up, left_lab, right];
        let newl = match nbrs.iter().find(|&&x| x != 0) {
            None => FRESH,
            Some(&t) => {
                for &o in &nbrs {
                    if o != 0 && o != t {
                        for l in l2[..nslots].iter_mut() {
                            if *l == o {
                                *l = t;
                            }
                        }
                    }
                }
                t
            }
        };
        l2[cell.slot] = newl;
        if let Some(p) = cell.pin {
            l2[p] = newl;
        }
        if let Some(k) = settle(cell, nslots, allow_close, l2, 0, ndiag, &cnt, fin) {
            out[1] = (k, cnt);
        }
    }
}

/// Run the scan from the empty frontier.
///
/// `allow_close`: a component leaving the frontier with nothing else black
/// is a finished grid (counted in `finished`). When false, closure is invalid
/// (used by rotation counts, whose seam is checked after the scan).
pub fn run<C: Count>(cells: &[Cell], nslots: usize, allow_close: bool) -> Dp<C> {
    run_from(cells, nslots, allow_close, vec![(0, C::one())])
}

pub fn run_from<C: Count>(
    cells: &[Cell],
    nslots: usize,
    allow_close: bool,
    init: Vec<(Key, C)>,
) -> Dp<C> {
    assert!(nslots <= MAX_SLOTS, "width {nslots} exceeds {MAX_SLOTS} slots");
    let mut cur = init;
    let mut finished = C::zero();
    let mut max_states = cur.len();
    let threads = rayon::current_num_threads();
    // Two buffers swapped every step, so the allocator is not asked for a
    // fresh multi-GB block per cell.
    let mut out: Vec<(Key, C)> = Vec::new();

    for cell in cells {
        let chunk = (cur.len() / (threads * 8)).max(MIN_CHUNK);
        out.clear();
        // Exact growth instead of Vec's doubling keeps each buffer at 2x the
        // largest state count.
        out.reserve_exact(cur.len() * 2);
        out.resize(cur.len() * 2, (SENTINEL, C::zero()));
        let fin = out
            .par_chunks_mut(2 * chunk)
            .zip(cur.par_chunks(chunk))
            .map(|(o, inp)| {
                let mut f = C::zero();
                for (i, &(s, c)) in inp.iter().enumerate() {
                    step(cell, nslots, allow_close, s, c, &mut o[2 * i..2 * i + 2], &mut f);
                }
                f
            })
            .reduce(C::zero, |mut a, b| {
                a.add_assign(&b);
                a
            });
        finished.add_assign(&fin);

        out.par_sort_unstable_by_key(|e| e.0);
        let mut w = 0;
        for i in 0..out.len() {
            let (k, c) = out[i];
            if k == SENTINEL {
                break;
            }
            if w > 0 && out[w - 1].0 == k {
                out[w - 1].1.add_assign(&c);
            } else {
                out[w] = (k, c);
                w += 1;
            }
        }
        out.truncate(w);
        std::mem::swap(&mut cur, &mut out);
        max_states = max_states.max(cur.len());
    }
    Dp { states: cur, finished, max_states }
}

/// Total of grids with at most one black component.
pub fn single_component<C: Count>(dp: &Dp<C>, nslots: usize) -> C {
    let mut t = dp.finished;
    for (s, c) in &dp.states {
        if decode(*s, nslots)[..nslots].iter().all(|&l| l <= 1) {
            t.add_assign(c);
        }
    }
    t
}
