use crate::board::Board;
use crate::connectivity::{is_connected, sealed_rows_connected};

/// Precompute combined pool+sealed compatible list for depth=1.
/// valid_first_pair[a] = rows that are pool-compatible AND sealed-compatible with row a.
pub fn precompute_valid_first_pairs(
    all_rows: &[u16],
    compatible: &[Vec<u16>],
    sealed_2: &[Vec<bool>],
) -> Vec<Vec<u16>> {
    all_rows
        .iter()
        .map(|&a| {
            compatible[a as usize]
                .iter()
                .copied()
                .filter(|&b| sealed_2[a as usize][b as usize])
                .collect()
        })
        .collect()
}

/// Single-threaded solver. Returns the count of valid n×n Nurikabe patterns.
pub fn solve(
    n: usize,
    all_rows: &[u16],
    compatible: &[Vec<u16>],
    first_pairs: &[Vec<u16>],
    count: &mut u64,
) {
    let mut board = Board::new(n);
    backtrack(&mut board, all_rows, compatible, first_pairs, count);
}

/// Recursive backtracking with pruning.
fn backtrack(
    board: &mut Board,
    all_rows: &[u16],
    compatible: &[Vec<u16>],
    first_pairs: &[Vec<u16>],
    count: &mut u64,
) {
    let n = board.n;
    let depth = board.depth;

    if depth == n {
        if is_connected(&board.rows, n, n) {
            *count += 1;
        }
        return;
    }

    let candidates: &[u16] = if depth == 0 {
        all_rows
    } else if depth == 1 {
        // Pre-merged pool+sealed candidates for depth=1
        &first_pairs[board.rows[0] as usize]
    } else {
        &compatible[board.rows[depth - 1] as usize]
    };

    for &row in candidates {
        board.rows[depth] = row;
        board.depth = depth + 1;

        // At depth >= 2, check sealed-rows connectivity (depth 0,1 already handled)
        if depth >= 2 && !sealed_rows_connected(&board.rows, n, depth + 1) {
            board.depth = depth;
            continue;
        }

        backtrack(board, all_rows, compatible, first_pairs, count);
        board.depth = depth;
    }
}

/// Solve starting from a given first row. Used by the parallel solver.
pub fn solve_from_first_row(
    n: usize,
    first_row: u16,
    all_rows: &[u16],
    compatible: &[Vec<u16>],
    first_pairs: &[Vec<u16>],
) -> u64 {
    let mut board = Board::new(n);
    board.rows[0] = first_row;
    board.depth = 1;
    let mut count = 0u64;
    backtrack(&mut board, all_rows, compatible, first_pairs, &mut count);
    count
}

/// Solve starting from two given rows. Used by deeper parallel partitioning.
pub fn solve_from_first_two_rows(
    n: usize,
    row0: u16,
    row1: u16,
    all_rows: &[u16],
    compatible: &[Vec<u16>],
    first_pairs: &[Vec<u16>],
) -> u64 {
    let mut board = Board::new(n);
    board.rows[0] = row0;
    board.rows[1] = row1;
    board.depth = 2;
    let mut count = 0u64;
    backtrack(&mut board, all_rows, compatible, first_pairs, &mut count);
    count
}
