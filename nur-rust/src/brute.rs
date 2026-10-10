//! Direct enumeration, used only to cross-check the transfer matrix on small grids.

/// Is this h x w grid (bit r*w+c = black) a valid pattern?
pub fn is_valid(g: u64, h: usize, w: usize) -> bool {
    let black = |r: usize, c: usize| g >> (r * w + c) & 1 == 1;
    for r in 0..h.saturating_sub(1) {
        for c in 0..w.saturating_sub(1) {
            if black(r, c) && black(r, c + 1) && black(r + 1, c) && black(r + 1, c + 1) {
                return false;
            }
        }
    }
    if g == 0 {
        return true;
    }
    // Flood fill from the lowest black cell; valid if it reaches every black cell.
    let mut seen = g & g.wrapping_neg();
    let mut stack = vec![seen.trailing_zeros() as usize];
    while let Some(p) = stack.pop() {
        let (r, c) = (p / w, p % w);
        let mut visit = |q: usize| {
            if g >> q & 1 == 1 && seen >> q & 1 == 0 {
                seen |= 1 << q;
                stack.push(q);
            }
        };
        if r > 0 { visit(p - w); }
        if r + 1 < h { visit(p + w); }
        if c > 0 { visit(p - 1); }
        if c + 1 < w { visit(p + 1); }
    }
    seen == g
}

pub fn count_rect(h: usize, w: usize) -> u64 {
    assert!(h * w <= 30, "brute force is only for tiny grids");
    (0u64..1 << (h * w)).filter(|&g| is_valid(g, h, w)).count() as u64
}

/// Fixed counts on n x n: [identity, rot90, rot180, reflect, diag].
pub fn fixed_counts(n: usize) -> [u64; 5] {
    assert!(n <= 5);
    let at = |g: u64, r: usize, c: usize| g >> (r * n + c) & 1;
    let fixed = |g: u64, t: &dyn Fn(usize, usize) -> (usize, usize)| {
        (0..n).all(|r| (0..n).all(|c| { let (a, b) = t(r, c); at(g, r, c) == at(g, a, b) }))
    };
    let mut f = [0u64; 5];
    for g in 0u64..1 << (n * n) {
        if !is_valid(g, n, n) {
            continue;
        }
        f[0] += 1;
        f[1] += fixed(g, &|r, c| (c, n - 1 - r)) as u64;
        f[2] += fixed(g, &|r, c| (n - 1 - r, n - 1 - c)) as u64;
        f[3] += fixed(g, &|r, c| (r, n - 1 - c)) as u64;
        f[4] += fixed(g, &|r, c| (c, r)) as u64;
    }
    f
}

/// Valid patterns on an h x w cylinder (column w-1 is adjacent to column 0).
pub fn count_cylinder(h: usize, w: usize) -> u64 {
    assert!(h * w <= 30 && w >= 3);
    let black = |g: u64, r: usize, c: usize| g >> (r * w + c % w) & 1 == 1;
    (0u64..1 << (h * w))
        .filter(|&g| {
            for r in 0..h.saturating_sub(1) {
                for c in 0..w {
                    if black(g, r, c) && black(g, r, c + 1) && black(g, r + 1, c) && black(g, r + 1, c + 1) {
                        return false;
                    }
                }
            }
            if g == 0 {
                return true;
            }
            let mut seen = g & g.wrapping_neg();
            let mut stack = vec![seen.trailing_zeros() as usize];
            while let Some(p) = stack.pop() {
                let (r, c) = (p / w, p % w);
                let mut nbrs = vec![r * w + (c + 1) % w, r * w + (c + w - 1) % w];
                if r > 0 { nbrs.push(p - w); }
                if r + 1 < h { nbrs.push(p + w); }
                for q in nbrs {
                    if g >> q & 1 == 1 && seen >> q & 1 == 0 {
                        seen |= 1 << q;
                        stack.push(q);
                    }
                }
            }
            seen == g
        })
        .count() as u64
}
