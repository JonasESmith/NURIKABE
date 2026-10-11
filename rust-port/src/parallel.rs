use rayon::prelude::*;

use crate::solver::{solve_from_first_row, solve_from_first_two_rows};

/// Parallel solver with adaptive partitioning depth.
pub fn solve_parallel(
    n: usize,
    all_rows: &[u16],
    compatible: &[Vec<u16>],
    first_pairs: &[Vec<u16>],
) -> u64 {
    if n >= 7 {
        solve_parallel_deep(n, all_rows, compatible, first_pairs)
    } else {
        all_rows
            .par_iter()
            .map(|&first_row| {
                solve_from_first_row(n, first_row, all_rows, compatible, first_pairs)
            })
            .sum()
    }
}

/// Partition by first two rows for n>=7.
fn solve_parallel_deep(
    n: usize,
    all_rows: &[u16],
    compatible: &[Vec<u16>],
    first_pairs: &[Vec<u16>],
) -> u64 {
    // Generate all valid (row0, row1) pairs using the pre-merged first_pairs
    let pairs: Vec<(u16, u16)> = all_rows
        .iter()
        .flat_map(|&r0| first_pairs[r0 as usize].iter().map(move |&r1| (r0, r1)))
        .collect();

    pairs
        .par_iter()
        .map(|&(r0, r1)| {
            solve_from_first_two_rows(n, r0, r1, all_rows, compatible, first_pairs)
        })
        .sum()
}
