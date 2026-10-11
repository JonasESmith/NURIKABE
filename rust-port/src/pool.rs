/// Check if two adjacent rows form a 2x2 all-water pool.
/// A pool exists when columns j and j+1 are both water in both rows.
#[inline(always)]
pub fn has_pool(a: u16, b: u16) -> bool {
    let a_adj = a & (a >> 1); // columns where a has water at j AND j+1
    let b_adj = b & (b >> 1); // columns where b has water at j AND j+1
    (a_adj & b_adj) != 0      // overlap = 2x2 pool
}

/// For each possible row value, precompute which other rows are pool-compatible.
/// compatible[r] = list of rows that don't form a pool when placed below row r.
pub fn precompute_compatible(all_rows: &[u16]) -> Vec<Vec<u16>> {
    all_rows
        .iter()
        .map(|&a| {
            all_rows
                .iter()
                .copied()
                .filter(|&b| !has_pool(a, b))
                .collect()
        })
        .collect()
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn no_pool_disjoint() {
        // rows 0b11 and 0b00 — no overlap
        assert!(!has_pool(0b11, 0b00));
    }

    #[test]
    fn pool_detected() {
        // rows 0b11 and 0b11 — 2x2 pool at columns 0-1
        assert!(has_pool(0b11, 0b11));
    }

    #[test]
    fn no_pool_staggered() {
        // rows 0b10 and 0b01 — no shared adjacent pair
        assert!(!has_pool(0b10, 0b01));
    }

    #[test]
    fn pool_in_middle() {
        // 0b0110 and 0b0110 — pool at columns 1-2
        assert!(has_pool(0b0110, 0b0110));
    }

    #[test]
    fn compatible_count_2x2() {
        let rows = crate::board::all_rows(2);
        let compat = precompute_compatible(&rows);
        // row 0b11 should be incompatible with 0b11 only
        let full_row = 0b11u16;
        let full_compat = &compat[full_row as usize];
        assert!(!full_compat.contains(&full_row));
    }
}
