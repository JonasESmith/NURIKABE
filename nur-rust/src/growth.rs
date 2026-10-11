//! Growth-rate estimates and the asymptotic prediction for n x n counts.

use crate::engine::run_from;
use crate::frontier::{cyl_cells, rect_cells, Cell, Force};

/// Power iteration on a one-row transfer step; returns the dominant
/// eigenvalue estimate after each of `rows` rows.
fn row_eigen(row: &[Cell], nslots: usize, rows: usize) -> Vec<f64> {
    let mut cur = vec![(0, 1.0f64)];
    let mut out = Vec::with_capacity(rows);
    for _ in 0..rows {
        let mut dp = run_from(row, nslots, true, cur);
        let lambda: f64 = dp.states.iter().map(|e| e.1).sum();
        for e in dp.states.iter_mut() {
            e.1 /= lambda;
        }
        cur = dp.states;
        out.push(lambda);
    }
    out
}

/// Dominant eigenvalue of the row transfer matrix for free-boundary strips of
/// width `w`. ln(Λ_{w+1}) - ln(Λ_w) tends to the per-cell growth constant.
pub fn strip_eigenvalue(w: usize, rows: usize) -> Vec<f64> {
    row_eigen(&rect_cells(1, w, &|_, _| Force::Free), w, rows)
}

/// Same for cylinders (periodic width). ln(Λ_w)/w tends to the per-cell
/// growth constant with no surface term: corrections fall like 1/w² rather
/// than the strips' 1/w.
pub fn cylinder_eigenvalue(w: usize, rows: usize) -> Vec<f64> {
    row_eigen(&cyl_cells(1, w), w, rows)
}

/// Fitted asymptotic form ln a(n) = F n^2 + B n + C + K ln n.
///
/// F comes from cylinder eigenvalues (widths 3..18, extrapolated); B, C, K
/// were fitted to exact a(17..19). Fitting on n <= 14 predicted a(15..19)
/// within 2e-5, and a(19) was predicted from n <= 17 within 2.1e-5 before it
/// was computed. Relative error grows roughly like 2.4e-5 (n - 19)^2, mostly
/// from the uncertainty in F (±0.00002).
pub const F: f64 = 0.56050;
pub const B: f64 = -0.4649993221;
pub const C: f64 = 2.046178068;
pub const K: f64 = -0.6563597724;

pub fn predicted_ln(n: usize) -> f64 {
    let n = n as f64;
    F * n * n + B * n + C + K * n.ln()
}

/// Predicted a(n) as (mantissa, decimal exponent).
pub fn predicted(n: usize) -> (f64, i64) {
    let log10 = predicted_ln(n) / std::f64::consts::LN_10;
    let e = log10.floor();
    (10f64.powf(log10 - e), e as i64)
}
