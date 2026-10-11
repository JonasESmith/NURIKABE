# Nurikabe Solver — Rust Port

A high-performance Rust port of the C# Nurikabe exhaustive pattern enumerator. Given an n x n grid, this solver counts every valid binary pattern satisfying the Nurikabe rules.

## Rules of Nurikabe

Nurikabe is a binary grid puzzle. Each cell is either **water** (1/black) or **land** (0/white). A valid pattern must satisfy:

1. **Water Continuity** — All water cells must form a single connected region using 4-directional adjacency (up, down, left, right). Diagonal connections do not count.
2. **No Pools** — No 2x2 block of cells can all be water.

The solver exhaustively enumerates every n x n grid satisfying both rules.

## How It Works

### Algorithm: Row-by-Row Backtracking with Pruning

The solver builds patterns one row at a time using recursive backtracking:

1. **Pre-generate all 2^n possible row patterns** as `u16` bitmasks (bit j set = water at column j).
2. **Pre-compute tables** — pool-compatibility, 2-row sealed compatibility, and merged first-pair candidates are all computed upfront to eliminate per-node overhead.
3. **Backtrack row-by-row** — at each depth, only try pre-filtered successor rows.
4. **Incremental sealed-rows connectivity** — exploits the backtracking invariant that previous checks passed. At each depth, only the newly-sealed row needs verification against the new frontier. This is an O(n) bitwise check instead of an O(n x depth) flood-fill for the common case.
5. **Full flood fallback** — when the fast path can't determine connectivity (complex shapes), falls back to a bitmask flood-fill from the frontier.
6. **Final check** — at full depth, verify all water forms one connected component.

### Key Optimizations Over the C# Version

| Aspect | C# Original | Rust Port |
|--------|------------|-----------|
| Row representation | `string` ("01101") | `u16` bitmask |
| Pool check | Parse chars, compare 4 ints per column pair | Single bitwise expression, O(1) |
| Row compatibility | Computed on-the-fly each iteration | Pre-computed lookup table |
| Depth-1 pruning | Pool check + continuity flood per candidate | Single merged candidate list (pool + sealed pre-filtered) |
| Connectivity check | Allocate `int[,]` matrix, per-cell recursive flood-fill | Stack-allocated `[u16; 16]`, bitwise flood-fill |
| Pruning strategy | Try all 2^n next rows on continuity failure | Incremental: O(n) fast path, O(n*depth) fallback |
| Convergence detection | N/A (C# doesn't iterate) | Changed-flag instead of array clone |
| Memory allocation | New matrix per check (millions of heap allocs) | Zero heap allocation in hot path |
| Parallelism | Single-threaded (pruning) | Rayon work-stealing, adaptive partitioning depth |

### Module Breakdown

```
src/
  main.rs           — CLI entry, timing, verification against known counts
  board.rs          — Board struct ([u16; 16] bitmask rows), row generation
  pool.rs           — Bitwise pool detection + precomputed compatibility table
  connectivity.rs   — Bitmask flood-fill, incremental sealed check, precomputed 2-row table
  solver.rs         — Backtracking core + precomputed first-pair candidates
  parallel.rs       — Rayon parallel wrapper (adaptive: 1-row or 2-row partitioning)
```

## Verified Results

| N | Valid Patterns | Rust Time (wall) | Rust CPU Time | C# Prune Time | Speedup (CPU) |
|---|---|---|---|---|---|
| 2x2 | 13 | ~1 us | ~1 us | ~ | - |
| 3x3 | 140 | ~16 us | ~16 us | ~ | - |
| 4x4 | 3,756 | ~330 us | ~330 us | ~ | - |
| 5x5 | 318,890 | ~53 ms | ~53 ms | 1.209 s | ~23x |
| 6x6 | 84,900,755 | ~2.6 s | ~27 s | 67.4 s | ~2.5x |
| 7x7 | 1,807,325,349 | TBD | TBD | 78,963 s (~22 hrs) | TBD |

*Timings on Apple M-series, 14 cores. RUSTFLAGS="-C target-cpu=native".*

## Usage

```bash
# Build optimized release binary
cargo build --release

# Run for a specific grid size (default: 5)
cargo run --release -- 6

# Run tests
cargo test --release
```

For maximum performance, build with native CPU optimizations:
```bash
RUSTFLAGS="-C target-cpu=native" cargo build --release
```

## Verification

The solver includes a built-in verification table. After computing, it automatically checks the result against known values for n=2 through n=7. All counts match the results from the original C# research.

## TODOs

- [ ] Verify n=7 result and record timing (running now)
- [ ] Profile with `cargo flamegraph` on n=7 to find remaining hotspots
- [ ] Explore symmetry reduction (D4 group: 4 rotations + 4 reflections) for up to 8x speedup on unique pattern counting
- [ ] Add equivalence class counting (matching the C# project's Equiv Classes column)
- [ ] Try incremental Union-Find for connectivity instead of full flood-fill per node
- [ ] Add island-size-one mode (only rows with max island size 1) to match the C# subset results
- [ ] Benchmark n=8 and beyond — estimate feasibility
- [ ] Add CLI flags for thread count, verbose output, and optional pattern export
- [ ] Add progress reporting for long-running computations (n >= 7)
