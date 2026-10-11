//! Frontier state encoding and the per-cell specification used by the engine.
//!
//! The grid is scanned cell by cell. The frontier holds one slot per column
//! (plus optional pinned slots). Each slot is 0 for white, or a component
//! label for black. Labels are renumbered 1, 2, ... in order of first
//! appearance, so equivalent frontiers share one key.
//!
//! Key layout (u128): slot i occupies bits 4i..4i+3; bit 127 holds the colour
//! of the cell diagonally up-left of the next cell (needed for the 2x2 rule).

pub type Key = u128;

/// Most slots a key can hold. A row of w slots has at most ceil(w/2) distinct
/// components and labels must fit in 4 bits (1..=15).
pub const MAX_SLOTS: usize = 29;

pub const DIAG: Key = 1 << 127;
/// Cylinder only: colour of the cell above column 0 in the current row, for
/// the 2x2 block that wraps around the seam.
pub const DIAG2: Key = 1 << 126;

/// Placeholder label for a brand-new component, replaced by `normalize`.
pub const FRESH: u8 = 255;

#[inline]
pub fn slot(s: Key, i: usize) -> u8 {
    ((s >> (4 * i)) & 0xF) as u8
}

#[inline]
pub fn decode(s: Key, nslots: usize) -> [u8; MAX_SLOTS] {
    let mut lab = [0u8; MAX_SLOTS];
    for (i, l) in lab.iter_mut().enumerate().take(nslots) {
        *l = slot(s, i);
    }
    lab
}

/// Canonically relabel and pack the slots (without the DIAG bit).
#[inline]
pub fn normalize(lab: &[u8]) -> Key {
    let mut map = [0u8; 256];
    let mut next = 1u8;
    let mut s: Key = 0;
    for (i, &l) in lab.iter().enumerate() {
        if l != 0 {
            if map[l as usize] == 0 {
                assert!(next < 16, "more than 15 frontier components");
                map[l as usize] = next;
                next += 1;
            }
            s |= (map[l as usize] as Key) << (4 * i);
        }
    }
    s
}

/// Where a neighbour's colour/label comes from.
#[derive(Clone, Copy, Debug)]
pub enum Src {
    None,
    Slot(usize),
    /// The DIAG bit of the key.
    Bit,
    /// Mirror of the cell above (diagonal-reflection seam).
    SameAsUp,
}

/// Colour constraint on a cell.
#[derive(Clone, Copy, PartialEq, Debug)]
pub enum Force {
    Free,
    White,
    /// Same colour as the cell currently held in this slot (mirror symmetry).
    SameAs(usize),
}

/// One step of the scan.
#[derive(Clone, Copy, Debug)]
pub struct Cell {
    /// Frontier slot this cell overwrites; before the step it holds the cell above.
    pub slot: usize,
    /// Left neighbour.
    pub left: Src,
    /// Up-left neighbour (only its colour matters).
    pub diag: Src,
    /// Record the colour of the cell above into the DIAG bit for the next cell.
    pub set_bit: bool,
    pub force: Force,
    /// Also copy this cell's label into a pinned slot.
    pub pin: Option<usize>,
    /// After this cell, clear this slot (its cell leaves the frontier).
    pub drop: Option<usize>,
    /// Cylinder: right neighbour slot (already placed this row) for the last column.
    pub right: Option<usize>,
    /// Cylinder: how this cell updates DIAG2.
    pub wrap_bit: WrapBit,
}

#[derive(Clone, Copy, PartialEq, Debug)]
pub enum WrapBit {
    Clear,
    /// Record the colour of the cell above (first column of a cylinder row).
    SetFromUp,
    Keep,
}

/// Row-major cells for an h x w rectangle. `force(r, c)` imposes symmetry.
pub fn rect_cells(h: usize, w: usize, force: &dyn Fn(usize, usize) -> Force) -> Vec<Cell> {
    let mut v = Vec::with_capacity(h * w);
    for r in 0..h {
        for c in 0..w {
            v.push(Cell {
                slot: c,
                left: if c > 0 { Src::Slot(c - 1) } else { Src::None },
                diag: if c > 0 { Src::Bit } else { Src::None },
                set_bit: c + 1 < w,
                force: force(r, c),
                pin: None,
                drop: None,
                right: None,
                wrap_bit: WrapBit::Clear,
            });
        }
    }
    v
}

/// Row-major cells for an h x w cylinder (periodic in the width direction), w >= 3.
/// The last column's right neighbour is column 0 of the same row.
pub fn cyl_cells(h: usize, w: usize) -> Vec<Cell> {
    assert!(w >= 3, "cylinder needs width >= 3");
    let mut v = rect_cells(h, w, &|_, _| Force::Free);
    for (i, cell) in v.iter_mut().enumerate() {
        let c = i % w;
        if c == 0 {
            cell.wrap_bit = WrapBit::SetFromUp;
        } else if c + 1 < w {
            cell.wrap_bit = WrapBit::Keep;
        } else {
            cell.right = Some(0);
        }
    }
    v
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn normalize_is_canonical() {
        assert_eq!(normalize(&[3, 0, 7, 3]), normalize(&[1, 0, 2, 1]));
        assert_eq!(decode(normalize(&[9, 9, 0, 4]), 4)[..4], [1, 1, 0, 2]);
    }
}
