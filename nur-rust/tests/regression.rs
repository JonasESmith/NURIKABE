use nur_rust::count::U256;
use nur_rust::{brute, counts};

fn count(h: usize, w: usize) -> String {
    counts::count_rect(h, w).0.to_string()
}

/// Every rectangle up to 5x5 against direct enumeration.
#[test]
fn rectangles_match_brute_force() {
    for h in 1..=5 {
        for w in 1..=5 {
            assert_eq!(count(h, w), brute::count_rect(h, w).to_string(), "{h}x{w}");
        }
    }
}

/// Every symmetry-fixed count up to 5x5 against direct enumeration.
#[test]
fn fixed_counts_match_brute_force() {
    for n in 1..=5 {
        let b = brute::fixed_counts(n);
        let k = counts::classes(n, counts::count_rect(n, n).0);
        let tm = [k.identity, k.rot90, k.rot180, k.reflect, k.diag].map(|x| x.to_string());
        assert_eq!(tm, b.map(|x| x.to_string()), "n={n}");
    }
}

/// Values from the original C# runs (README), n = 2..6.
#[test]
fn matches_original_csharp_results() {
    let expected = ["13", "140", "3756", "318890", "84900755"];
    for (n, e) in (2..=6).zip(expected) {
        assert_eq!(count(n, n), e, "n={n}");
    }
    let classes = ["4", "31", "497"];
    for (n, e) in (2..=4).zip(classes) {
        let k = counts::classes(n, counts::count_rect(n, n).0);
        assert_eq!(k.classes.to_string(), e, "classes n={n}");
    }
}

/// The README's 7x7 value is the true count wrapped to 32 bits.
#[test]
fn readme_7x7_was_32bit_overflow() {
    let (c, _) = counts::count_rect(7, 7);
    assert_eq!(c.to_string(), "70526802085");
    assert_eq!(c.0[0] % (1 << 32), 1_807_325_349);
}

/// Scanning h x w and w x h use different frontier widths, so agreement is a
/// non-trivial consistency check beyond the brute-force range.
#[test]
fn transposed_rectangles_agree() {
    for h in 1..=9 {
        for w in h + 1..=9 {
            assert_eq!(count(h, w), count(w, h), "{h}x{w}");
        }
    }
}

/// Closed forms for narrow strips: width 1 is 1 + h(h+1)/2, width 2 is 6*2^h - 3h - 5.
#[test]
fn narrow_strip_closed_forms() {
    for h in 1..=40u64 {
        assert_eq!(count(h as usize, 1), (1 + h * (h + 1) / 2).to_string(), "1x{h}");
    }
    for h in 1..=40u64 {
        let v = 6u128 * (1u128 << h) - 3 * h as u128 - 5;
        assert_eq!(count(h as usize, 2), v.to_string(), "2x{h}");
    }
}

/// Regression values for larger squares and classes.
#[test]
fn known_squares_and_classes() {
    let squares = [
        (7, "70526802085"),
        (8, "181801067635290"),
        (9, "1453021880446219833"),
        (10, "35908697917492590578915"),
        (11, "2740931314807077983757411602"),
        (12, "645356336991577834064108970458897"),
    ];
    for (n, e) in squares {
        assert_eq!(count(n, n), e, "n={n}");
    }
    let classes = [(5, "40572"), (6, "10615496"), (7, "8816234223"), (8, "22725136631195")];
    for (n, e) in classes {
        let k = counts::classes(n, counts::count_rect(n, n).0);
        assert_eq!(k.classes.to_string(), e, "classes n={n}");
    }
}

#[test]
fn u256_display_roundtrip() {
    assert_eq!(U256([0, 1, 0, 0]).to_string(), "18446744073709551616");
}

#[test]
fn cylinder_matches_brute_force() {
    use nur_rust::engine::{run, single_component, Dp};
    use nur_rust::frontier::cyl_cells;
    for w in 3..=6 {
        for h in 1..=24 / w {
            let dp: Dp<U256> = run(&cyl_cells(h, w), w, true);
            let tm = single_component(&dp, w).to_string();
            assert_eq!(tm, brute::count_cylinder(h, w).to_string(), "cylinder {h}x{w}");
        }
    }
}

#[test]
fn modular_matches_u256() {
    for n in 1..=13 {
        assert_eq!(counts::count_rect_mod(n, n).0.to_string(), count(n, n), "n={n}");
    }
    for (h, w) in [(3, 11), (11, 3), (1, 20), (7, 9)] {
        assert_eq!(counts::count_rect_mod(h, w).0.to_string(), count(h, w), "{h}x{w}");
    }
}

/// Exact a(18) (U256 and modular runs agree) and a(19) (modular, 6 primes).
#[test]
#[ignore = "a(18) takes ~3 min and ~10 GB; run with --ignored"]
fn a18_modular() {
    assert_eq!(
        counts::count_rect_mod(18, 18).0.to_string(),
        "1988110212420663583149643542851236792048983465756040531985371494758311961136"
    );
}

/// The asymptotic formula (fitted on n = 17..19) against smaller exact counts.
#[test]
fn prediction_tracks_exact_counts() {
    use nur_rust::growth::predicted_ln;
    for n in 10..=14 {
        let exact = counts::count_rect(n, n).0.to_f64().ln();
        let rel = (predicted_ln(n) - exact).exp() - 1.0;
        assert!(rel.abs() < 1e-3, "n={n}: relative error {rel:e}");
    }
}
