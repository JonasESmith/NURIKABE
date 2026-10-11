use crate::board::MAX_N;

/// Check if all water cells in rows[0..depth] form a single connected component.
/// Uses bitmask flood-fill — zero heap allocation.
pub fn is_connected(rows: &[u16; MAX_N], n: usize, depth: usize) -> bool {
    let total_water: u32 = rows[..depth].iter().map(|r| r.count_ones()).sum();
    if total_water <= 1 {
        return true;
    }

    let reached = flood_fill(rows, n, depth);
    reached == total_water
}

/// Check if the "sealed" portion of the board is still viable.
///
/// Rows 0..depth-2 are sealed — they can never gain new neighbors below.
/// Row depth-1 is the frontier — it may connect to future rows.
///
/// Pruning logic:
/// - If frontier has water: flood upward from frontier. Any sealed-row water
///   unreachable from frontier is permanently isolated -> prune.
/// - If frontier has no water: sealed water must form one connected component.
pub fn sealed_rows_connected(rows: &[u16; MAX_N], n: usize, depth: usize) -> bool {
    if depth <= 1 {
        return true;
    }

    let sealed_water: u32 = rows[..depth - 1].iter().map(|r| r.count_ones()).sum();
    if sealed_water == 0 {
        return true;
    }

    let frontier = rows[depth - 1];

    if frontier == 0 {
        // No frontier water — sealed water must be self-connected
        return is_connected(rows, n, depth - 1);
    }

    // Full flood from frontier upward
    sealed_rows_connected_full(rows, n, depth)
}

/// Full flood-fill based sealed check.
fn sealed_rows_connected_full(rows: &[u16; MAX_N], _n: usize, depth: usize) -> bool {
    let mut visited = [0u16; MAX_N];
    visited[depth - 1] = rows[depth - 1];

    loop {
        let mut changed = false;

        for r in (0..depth).rev() {
            let prev = visited[r];
            if r + 1 < depth {
                visited[r] |= visited[r + 1] & rows[r];
            }
            expand_horizontal(&mut visited[r], rows[r]);
            changed |= visited[r] != prev;
        }

        for r in 0..depth {
            let prev = visited[r];
            if r > 0 {
                visited[r] |= visited[r - 1] & rows[r];
            }
            expand_horizontal(&mut visited[r], rows[r]);
            changed |= visited[r] != prev;
        }

        if !changed {
            break;
        }

        // Early exit: all sealed water reached
        if (0..depth - 1).all(|r| rows[r] & !visited[r] == 0) {
            return true;
        }
    }

    for r in 0..depth - 1 {
        if (rows[r] & !visited[r]) != 0 {
            return false;
        }
    }
    true
}

/// Precompute 2-row sealed compatibility table.
/// sealed_2[a][b] = true iff placing row b below sealed row a passes the sealed check.
pub fn precompute_sealed_2(all_rows: &[u16], n: usize) -> Vec<Vec<bool>> {
    let num = all_rows.len();
    let mut table = vec![vec![false; num]; num];
    let mut board = [0u16; MAX_N];

    for &a in all_rows {
        for &b in all_rows {
            board[0] = a;
            board[1] = b;
            table[a as usize][b as usize] = sealed_rows_connected(&board, n, 2);
        }
    }
    table
}

/// Flood-fill from the first water cell, return count of reached cells.
fn flood_fill(rows: &[u16; MAX_N], _n: usize, depth: usize) -> u32 {
    let mut visited = [0u16; MAX_N];
    flood_fill_into(rows, depth, &mut visited);
    visited[..depth].iter().map(|r| r.count_ones()).sum()
}

/// Flood-fill from the first water cell into the visited array.
fn flood_fill_into(rows: &[u16; MAX_N], depth: usize, visited: &mut [u16; MAX_N]) {
    let mut start_r = 0;
    let mut start_bit = 0u16;
    for r in 0..depth {
        if rows[r] != 0 {
            start_r = r;
            start_bit = rows[r] & rows[r].wrapping_neg();
            break;
        }
    }
    if start_bit == 0 {
        return;
    }

    visited[start_r] = start_bit;

    loop {
        let mut changed = false;

        for r in 0..depth {
            let prev = visited[r];
            if r > 0 {
                visited[r] |= visited[r - 1] & rows[r];
            }
            expand_horizontal(&mut visited[r], rows[r]);
            changed |= visited[r] != prev;
        }

        for r in (0..depth).rev() {
            let prev = visited[r];
            if r + 1 < depth {
                visited[r] |= visited[r + 1] & rows[r];
            }
            expand_horizontal(&mut visited[r], rows[r]);
            changed |= visited[r] != prev;
        }

        if !changed {
            break;
        }
    }
}

/// Expand visited bits horizontally within a row's water cells until stable.
#[inline(always)]
fn expand_horizontal(visited: &mut u16, water: u16) {
    loop {
        let expanded = *visited | ((*visited << 1) & water) | ((*visited >> 1) & water);
        if expanded == *visited {
            break;
        }
        *visited = expanded;
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    fn make_board(rows: &[u16]) -> [u16; MAX_N] {
        let mut b = [0u16; MAX_N];
        for (i, &r) in rows.iter().enumerate() {
            b[i] = r;
        }
        b
    }

    #[test]
    fn single_cell_connected() {
        let board = make_board(&[0b01]);
        assert!(is_connected(&board, 2, 1));
    }

    #[test]
    fn horizontal_line_connected() {
        let board = make_board(&[0b111]);
        assert!(is_connected(&board, 3, 1));
    }

    #[test]
    fn vertical_line_connected() {
        let board = make_board(&[0b1, 0b1, 0b1]);
        assert!(is_connected(&board, 3, 3));
    }

    #[test]
    fn disconnected() {
        let board = make_board(&[0b101]);
        assert!(!is_connected(&board, 3, 1));
    }

    #[test]
    fn l_shape_connected() {
        let board = make_board(&[0b11, 0b01]);
        assert!(is_connected(&board, 2, 2));
    }

    #[test]
    fn sealed_rows_prune() {
        let board = make_board(&[0b101, 0b000]);
        assert!(!sealed_rows_connected(&board, 3, 2));
    }

    #[test]
    fn sealed_isolated_from_frontier() {
        let board = make_board(&[0b110, 0b001]);
        assert!(!sealed_rows_connected(&board, 3, 2));
    }

    #[test]
    fn sealed_reaches_frontier() {
        let board = make_board(&[0b101, 0b101]);
        assert!(sealed_rows_connected(&board, 3, 2));
    }

    #[test]
    fn sealed_no_frontier_water_connected() {
        let board = make_board(&[0b110, 0b000]);
        assert!(sealed_rows_connected(&board, 3, 2));
    }

    #[test]
    fn sealed_no_frontier_water_disconnected() {
        let board = make_board(&[0b101, 0b000]);
        assert!(!sealed_rows_connected(&board, 3, 2));
    }

    #[test]
    fn three_row_connected() {
        let board = make_board(&[0b111, 0b001, 0b011]);
        assert!(sealed_rows_connected(&board, 3, 3));
    }

    #[test]
    fn three_row_disconnected() {
        let board = make_board(&[0b100, 0b100, 0b001]);
        assert!(!sealed_rows_connected(&board, 3, 3));
    }

    #[test]
    fn empty_middle_with_water_above() {
        let board = make_board(&[0b010, 0b000, 0b010]);
        assert!(!sealed_rows_connected(&board, 3, 3));
    }
}
