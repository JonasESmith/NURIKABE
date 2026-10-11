mod board;
mod connectivity;
mod parallel;
mod pool;
mod solver;

use std::env;
use std::time::Instant;

use colored::Colorize;

fn main() {
    let n: usize = env::args()
        .nth(1)
        .and_then(|s| s.parse().ok())
        .unwrap_or(5);

    if n < 2 || n > 16 {
        eprintln!("Grid size must be between 2 and 16");
        std::process::exit(1);
    }

    println!(
        "{}",
        format!("Solving {}x{} Nurikabe patterns...", n, n).bold()
    );

    let all_rows = board::all_rows(n);
    println!("  Row patterns: {} (2^{})", all_rows.len(), n);

    let compatible = pool::precompute_compatible(&all_rows);
    let sealed_2 = connectivity::precompute_sealed_2(&all_rows, n);
    let first_pairs = solver::precompute_valid_first_pairs(&all_rows, &compatible, &sealed_2);
    println!("  Precomputation done");

    let start = Instant::now();

    let count = if n <= 5 {
        let mut c = 0u64;
        solver::solve(n, &all_rows, &compatible, &first_pairs, &mut c);
        c
    } else {
        parallel::solve_parallel(n, &all_rows, &compatible, &first_pairs)
    };

    let elapsed = start.elapsed();

    println!();
    println!(
        "  {} {} valid patterns",
        "Result:".green().bold(),
        format_count(count)
    );
    println!("  {} {:.3?}", "Time:".cyan().bold(), elapsed);

    verify(n, count);
}

fn verify(n: usize, count: u64) {
    let expected: Option<u64> = match n {
        2 => Some(13),
        3 => Some(140),
        4 => Some(3_756),
        5 => Some(318_890),
        6 => Some(84_900_755),
        7 => Some(70_526_802_085),
        _ => None,
    };

    if let Some(exp) = expected {
        if count == exp {
            println!("  {} matches known result", "VERIFIED:".green().bold());
        } else {
            eprintln!(
                "  {} expected {}, got {}",
                "MISMATCH:".red().bold(),
                format_count(exp),
                format_count(count)
            );
        }
    } else {
        println!("  {} no known result for n={}", "Note:".yellow().bold(), n);
    }
}

fn format_count(n: u64) -> String {
    let s = n.to_string();
    let mut result = String::new();
    for (i, c) in s.chars().rev().enumerate() {
        if i > 0 && i % 3 == 0 {
            result.push(',');
        }
        result.push(c);
    }
    result.chars().rev().collect()
}
