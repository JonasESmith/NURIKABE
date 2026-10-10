pub const MAX_N: usize = 16;

/// A partial or complete Nurikabe board.
/// Each row is a u16 bitmask where bit j set = water at column j.
#[derive(Clone)]
pub struct Board {
    pub rows: [u16; MAX_N],
    pub n: usize,
    pub depth: usize,
}

impl Board {
    pub fn new(n: usize) -> Self {
        Board {
            rows: [0u16; MAX_N],
            n,
            depth: 0,
        }
    }

}

/// Generate all possible row patterns for a grid of width n.
/// Returns values 0..2^n (each a u16 bitmask).
pub fn all_rows(n: usize) -> Vec<u16> {
    (0..(1u32 << n)).map(|r| r as u16).collect()
}
