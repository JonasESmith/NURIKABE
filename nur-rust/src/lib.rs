//! Counting Nurikabe patterns: h x w black/white grids whose black cells
//! (water) are 4-connected or absent and contain no 2x2 black block.
//!
//! Uses a cell-by-cell transfer matrix over frontier connectivity states
//! instead of enumerating patterns, so cost grows with the number of frontier
//! states (~2.7x per unit of width) rather than with the count (~1.75^(n^2)).

pub mod brute;
pub mod count;
pub mod counts;
pub mod engine;
pub mod frontier;
pub mod growth;
