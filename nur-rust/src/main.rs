use std::time::Instant;

use nur_rust::{brute, counts, growth};

const USAGE: &str = "\
usage: nur-rust <command> [args]

  count    [LO] [HI]    n x n pattern counts, 256-bit    (default 1 10)
  modcount [LO] [HI]    n x n counts via primes + CRT; no size limit, less memory
  classes  [LO] [HI]    counts + equivalence classes      (default 1 10)
  rect     MAX          table T(h, w) for 1 <= w <= h <= MAX
  check    [MAX]        compare against brute force for n <= MAX (default 5)
  growth   W [ROWS]     strip eigenvalues for widths 1..=W (default ROWS 60)
  cylinder W1 W2 [ROWS] cylinder eigenvalues, ln(lambda)/w -> growth constant
  predict  [LO] [HI]    asymptotic prediction of a(n)      (default 18 30)";

fn arg(args: &[String], i: usize, default: usize) -> usize {
    args.get(i).map_or(default, |s| s.parse().unwrap_or_else(|_| panic!("bad number: {s}")))
}

fn main() {
    let args: Vec<String> = std::env::args().collect();
    match args.get(1).map(String::as_str) {
        Some(cmd @ ("count" | "classes")) => {
            let (lo, hi) = (arg(&args, 2, 1), arg(&args, 3, 10));
            for n in lo..=hi {
                let t = Instant::now();
                let (c, states) = counts::count_rect(n, n);
                println!("n={n:<3} count={c}  states={states}  time={:.3}s", t.elapsed().as_secs_f64());
                if cmd == "classes" {
                    let t = Instant::now();
                    let k = counts::classes(n, c);
                    println!(
                        "      classes={}  fixed: rot90={} rot180={} reflect={} diag={}  time={:.3}s",
                        k.classes, k.rot90, k.rot180, k.reflect, k.diag,
                        t.elapsed().as_secs_f64()
                    );
                }
            }
        }
        Some("modcount") => {
            let (lo, hi) = (arg(&args, 2, 1), arg(&args, 3, 10));
            for n in lo..=hi {
                let t = Instant::now();
                let (c, states) = counts::count_rect_mod(n, n);
                println!(
                    "n={n:<3} count={c}  bits={}  states={states}  time={:.3}s",
                    c.bits(),
                    t.elapsed().as_secs_f64()
                );
            }
        }
        Some("rect") => {
            let max = arg(&args, 2, 7);
            for h in 1..=max {
                let row: Vec<String> = (1..=h).map(|w| counts::count_rect(h, w).0.to_string()).collect();
                println!("h={h}: {}", row.join(", "));
            }
        }
        Some("check") => {
            let max = arg(&args, 2, 5);
            let mut all_ok = true;
            for n in 1..=max {
                let b = brute::fixed_counts(n);
                let id = counts::count_rect(n, n).0;
                let k = counts::classes(n, id);
                let tm = [k.identity, k.rot90, k.rot180, k.reflect, k.diag].map(|x| x.to_string());
                let ok = tm.iter().zip(b).all(|(t, b)| *t == b.to_string());
                all_ok &= ok;
                println!(
                    "n={n} brute={b:?} transfer={tm:?} classes={} {}",
                    k.classes,
                    if ok { "OK" } else { "MISMATCH" }
                );
            }
            if !all_ok {
                std::process::exit(1);
            }
        }
        Some("growth") => {
            let (w_max, rows) = (arg(&args, 2, 12), arg(&args, 3, 60));
            let mut prev = 0.0;
            for w in 1..=w_max {
                let l = growth::strip_eigenvalue(w, rows).last().unwrap().ln();
                println!("w={w:<3} ln(lambda)={l:.10}  diff={:.10}", l - prev);
                prev = l;
            }
        }
        Some("cylinder") => {
            let (w1, w2, rows) = (arg(&args, 2, 3).max(3), arg(&args, 3, 12), arg(&args, 4, 80));
            for w in w1..=w2 {
                let t = Instant::now();
                let seq = growth::cylinder_eigenvalue(w, rows);
                let l = seq.last().unwrap().ln();
                // Change over the last row shows whether power iteration has converged.
                let delta = (l - seq[seq.len() - 2].ln()).abs();
                println!(
                    "w={w:<3} ln(lambda)={l:.12}  ln(lambda)/w={:.10}  delta={delta:.1e}  time={:.1}s",
                    l / w as f64,
                    t.elapsed().as_secs_f64()
                );
            }
        }
        Some("predict") => {
            let (lo, hi) = (arg(&args, 2, 18), arg(&args, 3, 30));
            println!(
                "ln a(n) ~ {} n^2 + ({}) n + {} + ({}) ln n",
                growth::F, growth::B, growth::C, growth::K
            );
            for n in lo..=hi {
                let (m, e) = growth::predicted(n);
                println!("n={n:<3} a(n) ~ {m:.5}e{e}");
            }
        }
        _ => {
            eprintln!("{USAGE}");
            std::process::exit(2);
        }
    }
}
