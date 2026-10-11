# nur-rust

A counter for **Nurikabe patterns**: n×n (or h×w) black/white grids where
- the black cells (water) are 4-connected, and
- no 2×2 block is entirely black (no pools).

Islands are unconstrained, and the empty grid counts (2×2 gives 13).

This crate replaces the brute-force approach of `nur-brute`, `nur-prune` and `rust-port` with a transfer-matrix counter:
- 7×7 takes **2 ms**, where the C# prune version took about **22 hours**.
- Exact counts now reach **19×19** (about 10⁸⁴).
- Counts for larger boards are predicted by an asymptotic formula, validated against held-out exact values.

The rest of this document covers what was built, why, what each piece was expected to gain and what it actually gained, and how the results were verified.

---

## Contents

1. [Quick start](#1-quick-start)
2. [Starting point: the previous implementations](#2-starting-point-the-previous-implementations)
3. [Errors found in the previous results](#3-errors-found-in-the-previous-results)
4. [The core idea: count, don't enumerate](#4-the-core-idea-count-dont-enumerate)
5. [What was included, why, and the gains](#5-what-was-included-why-and-the-gains)
6. [Options considered and rejected](#6-options-considered-and-rejected)
7. [Verification](#7-verification)
8. [Results](#8-results)
9. [Predicting a(n) for any n](#9-predicting-an-for-any-n)
10. [Exact formulas for fixed-width strips](#10-exact-formulas-for-fixed-width-strips)
11. [Limits and future work](#11-limits-and-future-work)
12. [Code layout](#12-code-layout)

---

## 1. Quick start

```
cargo build --release
./target/release/nur-rust count 1 14      # n x n counts (256-bit, fastest)
./target/release/nur-rust modcount 18 19  # n x n counts via primes + CRT (any size, less memory)
./target/release/nur-rust classes 1 14    # + equivalence classes under rotation/reflection
./target/release/nur-rust rect 9          # T(h, w) table for all rectangles
./target/release/nur-rust check           # compare against brute force, n <= 5
./target/release/nur-rust growth 13       # strip eigenvalues
./target/release/nur-rust cylinder 3 14   # cylinder eigenvalues -> growth constant
./target/release/nur-rust predict 18 30   # asymptotic prediction of a(n)
cargo test --release                      # regression + brute-force cross-checks (~6 s)
cargo test --release -- --ignored         # also recompute a(18) (~3 min, ~10 GB)
```

All timings below are wall-clock times on an Apple M4 Max (14 cores, 36 GB).

---

## 2. Starting point: the previous implementations

| implementation | approach | 5×5 | 6×6 | 7×7 |
|---|---|---|---|---|
| `nur-brute` (C#) | tries all 2^(n²) grids, 8 threads | 1.2 s | 2,226 s | infeasible |
| `nur-prune` (C#) | row-by-row backtracking with pool pruning, 1 thread | 1.2 s | 67 s | 78,964 s (~22 h), wrong result |
| `rust-port` | row bitmasks, precomputed row-pair tables, rayon | 62 ms | 3.4 s | estimated hours |

Every one of them **enumerates valid patterns one at a time**, so its cost is at least proportional to the answer. The answer grows like about 1.75^(n²):

| n | valid patterns |
|---|---|
| 6 | 8.5×10⁷ |
| 7 | 7.1×10¹⁰ |
| 8 | 1.8×10¹⁴ |
| 9 | 1.5×10¹⁸ |

The best possible enumerator, even on a GPU, therefore stops around n = 8. Section 6 has the GPU estimates.

Further inefficiencies found in the audit:
- **`nur-prune`:**
  - builds rows as strings and parses them back with `Int32.Parse`;
  - allocates a fresh matrix per search node;
  - stores every valid pattern in a list, about 14 GB at 6×6;
  - has no parallelism.
- **`rust-port`:**
  - re-checks connectivity with a full flood fill of every row so far at each step, plus a second flood at each leaf;
  - its readme claims an incremental O(n) check that does not exist;
  - for 6×6 it splits the work into only 64 parallel tasks.
- **`nur-brute`:** resets its visited array for every grid, even ones already rejected by the pool check, and computes 2^(n²) as a `double`, which breaks at n ≥ 8.

---

## 3. Errors found in the previous results

| item | previously | corrected | cause |
|---|---|---|---|
| 7×7 count | 1,807,325,349 | **70,526,802,085** | `nur-prune` counts in a 32-bit `int` (`ref int patternCount`). 70,526,802,085 mod 2³² = 1,807,325,349 exactly, after 16 wraps. |
| 5×5 equivalence classes | 38,858 | **40,572** | No code in the repo computed this. Two independent brute-force methods (canonical forms and Burnside) give 40,572. |
| `rust-port` expected 7×7 | 1,807,325,349 | 70,526,802,085 | It copied the README value, so a correct run would have reported a mismatch. Fixed in `rust-port/src/main.rs`. |
| `rust-port` timings in the README | 6×6 in 32.5 s, 5×5 in 0.6 ms | 3.4 s wall, 62 ms | 32.5 s was CPU time summed over all cores. |

These errors are why every count type here is checked: `U256` panics on overflow rather than wrapping, and modular counting verifies that it had enough headroom (section 5.6).

---

## 4. The core idea: count, don't enumerate

Scan the grid **one cell at a time** (row-major). At any moment, the only thing that matters about the cells already placed is the **frontier**: the last cell in each column, and how its black cells are connected through the area already scanned.

```
      already scanned
  ┌───┬───┬───┬───┬───┐
  │   │   │   │   │   │
  ├───┼───┼───┼───┼───┤
  │ A │   │ A │ B │   │   ← frontier: one slot per column,
  ├───┼───┼───┼───┼───┘     each white (0) or a component label
  │ A │ ? │                ← next cell
  └───┴───┘
```

- **State:** one 4-bit label per column, plus one bit for the colour of the cell diagonally up-left. That bit is all the 2×2 rule needs.
  - 0 means white. Any other value is a component label, renumbered 1, 2, 3… in order of first appearance, so equivalent frontiers get the same key.
  - The whole state packs into a `u128`.
- **Step:** each state has two successors, white or black.
  - **Black** merges the labels of the black neighbours above and to the left. It is forbidden if it would complete a 2×2 black block.
  - **White** may remove a component's last frontier cell. That component is then sealed off for good, which is valid only if it was the only black component; in that case every remaining cell must be white, and the grid is counted as finished.
- **Merging:** states with the same key are merged and their counts added. This is where the speedup comes from: about 10⁸⁴ patterns at 19×19 collapse into at most about 10⁸ frontier states.
- **End:** the answer is the finished grids plus the final states with at most one component.

**Cost:** about n² steps, each linear in the number of frontier states. That number grows about **2.7× per unit of width** (654 states at n = 7, 13.2M at n = 17), instead of with the 1.75^(n²) patterns.

**Expected gain:** brute-force cost grows like 1.75^(n²) and this method like 2.7^n, so the advantage itself grows with n:
- about 10⁸ at n = 7 (22 h down to under a millisecond of real work);
- about 10¹⁴ at n = 8, where enumeration would take hours on a GPU.

---

## 5. What was included, why, and the gains

### 5.1 Transfer-matrix engine (`engine.rs`, `frontier.rs`)

**What:** the algorithm in section 4. One engine is driven by a list of `Cell` specs, which describe for each cell where its neighbours are, any forced colour, and any pinned or dropped slots.

**Why:** a single, well-tested transition function serves rectangles, all four symmetry counts, cylinders and eigenvalue estimates. A new mode is a new list of cells, not new counting logic.

**Gain (measured):**

| n | previous best | nur-rust | speedup |
|---|---|---|---|
| 5 | 62 ms (`rust-port`) | under 1 ms | over 60× |
| 6 | 3.4 s (`rust-port`) | under 1 ms | over 3,000× |
| 7 | about 22 h (`nur-prune`) | 2 ms | about 4×10⁷× |
| 8 | not feasible | 8 ms | – |
| 14 | – | 1.7 s | – |
| 17 | – | 58 s | – |

### 5.2 Parallel expand / sort / merge (rayon)

**What:** for each cell:
1. Every state writes its at most two successors into fixed slots of a preallocated buffer, in parallel, so there are no locks or shared hash map.
2. The buffer is sorted by key in parallel (`par_sort_unstable`).
3. Equal keys are merged in one linear pass.

**Why:** a global hash map would need locking or sharding to update in parallel. Sort-and-merge is simple, deterministic and scales well. Unused successor slots hold a sentinel key that sorts last.

**Expected gain:** 5–8× on 14 cores. Sorting limits how far it scales, and memory bandwidth is shared.

**Measured:** 14×14 takes 12.3 s on one thread and 1.73 s on 14, a **7.1×** speedup.

### 5.3 Buffer reuse and exact growth (memory)

**What:**
- Two state buffers are swapped every cell instead of allocating a new one per cell.
- Their capacity grows with `reserve_exact` instead of `Vec`'s doubling.

**Why:** the first version allocated a fresh multi-GB buffer for each of n² cells. macOS keeps freed large blocks counted against the process for a while, so peak memory reached about 2.3 KB per state when about 100 B was expected.

**Gain (measured peak memory):**

| n | before | after | reduction |
|---|---|---|---|
| 14 | 1.42 GB | 0.30 GB | 4.7× |
| 17 | 17.3 GB | 5.3 GB | 3.3× |

### 5.4 256-bit counts that never wrap (`count.rs`, `U256`)

**What:** a fixed four-limb 256-bit integer whose addition panics on overflow.

**Why:**
- Counts outgrow `u64` at 10×10 and `u128` at 13×13.
- During research, a `u128` run printed a wrong 13×13 value **with no error**. That is the same kind of silent wrap that broke the C# 7×7 result.

256 bits holds every count up to n = 18 (a(18) uses 251 bits). It is the fastest exact mode.

### 5.5 Symmetry classes via Burnside's lemma (`counts.rs`)

**What:** the number of patterns that are distinct up to rotation and reflection:

    classes = (F_id + F_rot180 + 2·F_rot90 + 2·F_reflect + 2·F_diag) / 8

where each F is the number of patterns that symmetry leaves unchanged. Each F is its own transfer-matrix run:
- **Reflect:** the right half of each row is forced to copy the left half.
- **Diagonal:** scan the upper triangle as a staircase. A symmetric pattern must touch the diagonal, so subtract the fills with an all-white diagonal and add back the empty grid.
- **Rotation by 180°:** scan the top half, then glue it to its rotated copy along the seam with a union-find, checking connectivity and the 2×2 blocks that straddle the seam.
- **Rotation by 90°:** scan one quarter, pinning the labels of its last column, then glue four rotated copies (plus the centre cell for odd n).

**Why:** the README tracked equivalence classes, but nothing computed them, and the 5×5 value was wrong.

**Gain:** classes cost less than the plain count. 16×16 classes take 8.3 s on top of 18 s for the count. The code asserts that the Burnside sum is divisible by 8 as a built-in consistency check.

### 5.6 Modular counting + Chinese remainder theorem (`modcount`)

**What:**
- Run the engine once per pair of 61-bit primes, keeping counts modulo those primes (`ModPair`, 32 B per state).
- Rebuild the exact count with Garner's form of the Chinese remainder theorem (`crt`, using the small `BigUint` type).
- The number of primes comes from the growth estimate (about 0.81 bits per cell) plus 64 bits of headroom.

**Why:**
- 19×19 needs 280 bits, too big for `U256`. Wider fixed integers would make every state bigger still.
- Residues have no size limit, and each state is a third smaller (32 B instead of 48 B).

**Safety:** a count too large for the chosen primes would rebuild to an essentially random value near the top of the prime product. The code requires at least 32 unused bits below that product, so a wrong answer is caught with probability about 1 − 2⁻³², not printed.

**Tradeoff (measured):** 17×17 takes 154 s with `modcount` (three passes) and 58 s with `count`. Use `count` up to n = 18 and `modcount` beyond.

**Gain:** made 19×19 possible: 102.6M states, 25 min, 22.8 GB.

### 5.7 Cylinder transfer matrix and growth constant (`growth.rs`)

**What:**
- **Strip eigenvalues:** the dominant eigenvalue Λ_w of the one-row transfer matrix for strips of width w, found by power iteration.
- **Cylinder eigenvalues:** the same for cylinders, where column w−1 touches column 0. This needs a second flag bit (`DIAG2`) for the 2×2 block that wraps around the seam, and a three-way label merge at the last column.

**Why:**
- The growth per cell, f = lim ln a(n)/n², is the leading term of any prediction.
- Strips have boundaries, so their estimates carry a surface term with a 1/w correction.
- Cylinders have no boundary, so ln Λ_w / w approaches f with a smaller correction of order 1/w².

**Gain:** f = **0.56050 ± 0.00002** (e^f ≈ 1.7516), from cylinders of width 3–18 extrapolated with four different methods that close in from both sides.

### 5.8 Brute-force oracles (`brute.rs`)

**What:** direct enumeration over bitmasks of:
- every rectangle up to 5×5 (and any h×w ≤ 30 cells);
- the four symmetry-fixed counts for n ≤ 5;
- cylinders up to 24 cells.

**Why:** an independent implementation with no shared logic is the strongest check that the transfer-matrix rules (closing, merging, the diagonal bit, seams) are right.

---

## 6. Options considered and rejected

| option | why rejected |
|---|---|
| **Optimizing the enumerator** (incremental connectivity labels, better parallel split, symmetry halving) | Realistic gains of 10–100× still leave the cost at least proportional to the number of patterns. Even a perfectly tuned CPU enumerator would take minutes for 7×7 and weeks for 8×8. |
| **GPU brute force** (CUDA on an H100, or Metal on the M4 Max) | Estimated 2–10 s for 7×7 and **1–5 hours** for 8×8 on an H100. The transfer matrix does 8×8 in 8 ms on the CPU. Search trees are very unbalanced, which hurts warp efficiency. |
| **GPU transfer matrix** | The algorithm maps well to a GPU (sort and reduce-by-key with CUB). But the CPU handles up to n = 19 here, and memory, not compute, is the limit. Worth revisiting only past about n = 20, using `cudarc` + CUB. The Mac's GPU (wgpu/Metal) is a poor fit: WGSL has no core 64-bit integers, and Metal has no 64-bit atomic add. |
| **Global concurrent hash map** for merging | Needs locking or sharding. Parallel sort-and-merge is simpler, deterministic and fast enough. |
| **Arbitrary-precision integers per state** | Heap allocation per state. `U256` covers n ≤ 18, and modular counting covers the rest more cheaply. |

---

## 7. Verification

The test suite (`cargo test --release`, about 6 s) checks:

| check | what it shows |
|---|---|
| Every h×w ≤ 5×5 against brute force | Engine rules are correct on rectangles. |
| All four symmetry-fixed counts, n ≤ 5, against brute force | Reflect, diagonal and rotation runs and seam gluing are correct. |
| Cylinders (w = 3–6, up to 24 cells) against brute force | Wrap-around seam handling is correct. |
| Original C# values for n = 2..6 and classes for n = 2..4 | Agrees with all previously trusted results. |
| a(7) = 70,526,802,085, and a(7) mod 2³² equals the old README value | Confirms the 32-bit overflow diagnosis. |
| T(h, w) = T(w, h) for all h, w ≤ 9 | A non-trivial check beyond brute-force range, since the two orientations use different frontier widths. |
| Width 1 = 1 + w(w+1)/2 and width 2 = 6·2^w − 3w − 5, for w ≤ 40 | Matches proven closed forms on long strips. |
| `modcount` = `count` for n ≤ 13 and several rectangles | The modular path and CRT are correct. |
| Regression values for n ≤ 12 and classes for n ≤ 8 | Guards against future regressions. |
| The prediction formula within 0.1% for n = 10–14 | The formula stays consistent with the exact counts. |

Checks made outside the test suite:
- **a(6):** the old enumerating `rust-port`, which shares no logic with this crate, gives 84,900,755.
- **a(18):** the `U256` run and a separate modular run agree digit for digit.
- **a(19):** two separate modular runs agree: the research prototype and this crate's `modcount`.
- **Rows of the T(h, w) table:** match an independent Python column transfer matrix up to 8×8.

---

## 8. Results

### n × n counts (empty grid included)

| n | valid patterns | time |
|---|---|---|
| 1 | 2 | |
| 2 | 13 | |
| 3 | 140 | |
| 4 | 3,756 | |
| 5 | 318,890 | < 1 ms |
| 6 | 84,900,755 | < 1 ms |
| 7 | 70,526,802,085 | 2 ms |
| 8 | 181,801,067,635,290 | 8 ms |
| 9 | 1,453,021,880,446,219,833 | 26 ms |
| 10 | 35,908,697,917,492,590,578,915 | 47 ms |
| 11 | 2,740,931,314,807,077,983,757,411,602 | 0.09 s |
| 12 | 645,356,336,991,577,834,064,108,970,458,897 | 0.22 s |
| 13 | 468336147464432442539277319957690282927 | 0.6 s |
| 14 | 1046790588647728747480461616766508110955020788 | 1.7 s |
| 15 | 7202366061923427388262225891780715160511014475442802 | 7.6 s |
| 16 | 152478931318211468615979883427328339803667992333742346367973 | 18 s |
| 17 | 9929063745031935247890777228988377062411999554383111298043882115866 | 58 s |
| 18 | 1988110212420663583149643542851236792048983465756040531985371494758311961136 | 190 s |
| 19 | 1223764590313000893662434448911665916137196775511215594353885983484883047025281502957 | 25 min (`modcount`) |

### Equivalence classes under rotation and reflection

| n | classes |
|---|---|
| 1–4 | 2, 4, 31, 497 |
| 5 | 40,572 |
| 6 | 10,615,496 |
| 7 | 8,816,234,223 |
| 8 | 22,725,136,631,195 |
| 9 | 181,627,737,152,352,539 |
| 10 | 4,488,587,239,720,096,790,990 |
| 11–16 | run `nur-rust classes 11 16` (the root README lists them) |

### Rectangles T(h, w), symmetric in h and w (`nur-rust rect 9`)

| h | T(h, 1) … T(h, h) |
|---|---|
| 3 | 7, 34, 140 |
| 4 | 11, 79, 545, 3756 |
| 5 | 16, 172, 2081, 25823, 318890 |
| 6 | 22, 361, 7888, 176790, 3903562, 84900755 |
| 7 | 29, 742, 29826, 1208992, 47682331, 1839494837, 70526802085 |

As of 2026-10-10, none of the following are in OEIS:
- the square counts;
- the equivalence classes;
- the width-3 and width-4 columns.

Width 2 is A101946. The same count without the 2×2 rule is A059525 (+1 for the empty grid).

---

## 9. Predicting a(n) for any n

**No closed form is expected.** The closely related count of connected sets in an n×n grid (A059525) is a stated open problem (A. Vince, 2017), and counting connected sets is #P-complete in general. What can be done is an asymptotic formula, validated honestly:

    ln a(n) ≈ F·n² + B·n + C + K·ln n

| constant | value | meaning | source |
|---|---|---|---|
| F | 0.56050 ± 0.00002 | bulk growth per cell (e^F ≈ 1.7516) | cylinder eigenvalues (5.7) |
| B | −0.4649993221 | boundary term (= 4 × −0.1162 per edge) | fit to exact a(17..19), F fixed |
| C | 2.046178068 | constant | same fit |
| K | −0.6563597724 | corner term in ln n | same fit |

**Why ln n:**
- Without it, every model extrapolates 10–100× worse.
- Its value is consistent with the corner correction that conformal field theory predicts for a critical model. The strip eigenvalues have a 1/w correction that corresponds to central charge c ≈ −2.6, which predicts K = c/4 ≈ −0.65.
- The global connectivity constraint is the likely reason the model is critical.

**Validation (fit on small n, predict larger n):**

| fitted on | predicted | worst relative error |
|---|---|---|
| n ≤ 14 | a(15) … a(19) | 2.1×10⁻⁵ |
| n ≤ 12 | a(13) … a(19) | 1.0×10⁻³ |
| n ≤ 17 | a(19), recorded **before** a(19) was computed | 2.1×10⁻⁵ |
| n ≤ 18 | a(19) | 4.4×10⁻⁶ |

**Predictions** (`nur-rust predict 20 30`; error bars from the spread over 12 model variants, which held-out tests showed to be somewhat conservative):

| n | predicted a(n) | relative error bar |
|---|---|---|
| 20 | 2.31521×10⁹³ | −3×10⁻⁵ / +1.1×10⁻⁴ |
| 21 | 1.34598×10¹⁰³ | −6×10⁻⁵ / +2.2×10⁻⁴ |
| 22 | 2.40426×10¹¹³ | −1.1×10⁻⁴ / +3.5×10⁻⁴ |
| 25 | 1.15136×10¹⁴⁷ | −3.4×10⁻⁴ / +9.5×10⁻⁴ |
| 30 | 8.72114×10²¹² | −1.2×10⁻³ / +2.6×10⁻³ |

The error grows roughly like 2.4×10⁻⁵·(n − 19)², almost entirely from the uncertainty in F. That is about ±2% at n = 50. Each extra reliable digit of F extends the useful range.

---

## 10. Exact formulas for fixed-width strips

For a fixed height h, the count is a linear recurrence in the length w with integer coefficients. This follows from the transfer matrix being finite. Each such count therefore has an exact formula. For h ≥ 2, its generating function has denominator (1−x)²·G_h(x), so

    T(h, w) = (a_h·w + b_h) + Σ cᵢ·rᵢ^w      (rᵢ = roots of G_h)

The linear part counts patterns whose water ends before the last column (the rest is then all white).

| h | T(h, w) | recurrence order | growth per column |
|---|---|---|---|
| 1 | 1 + w(w+1)/2 | 3 | – |
| 2 | 6·2^w − 3w − 5 | 3 | 2 |
| 3 | Σ cᵢrᵢ^w − 2w − 2, with rᵢ the roots of z⁶ − 5z⁵ + 3z⁴ + 11z³ − 24z² + 22z − 4 | 8 | 3.7772585921 |
| 4 | Σ cᵢrᵢ^w − 6w − 20, with G of degree 12 (dominant term 1.7326 · 6.8362^w) | 14 | 6.8361546929 |
| 5 | | 32 | 12.2038792878 |
| 6 | | 69 | 21.6205573186 |
| 7 | | 176 | 38.1812597774 |
| 8 | | 426 | 67.2814498993 |
| 9 | | 1114 | 118.405461201 |
| 10 | | 2863 | 208.176308059 |

- **h ≤ 8:** proven by exact checks over at least 2× the number of transfer states.
- **h = 9, 10:** verified modulo 20 primes.
- **h = 3–8:** G_h is irreducible.

The recurrence order grows about 2.5× per unit of height, so this route cannot give a closed form for squares. Still, the strip results confirm the growth constant from a second direction: ln Λ_{h+1} − ln Λ_h falls 0.5795, 0.5719, …, 0.5636 toward F.

---

## 11. Limits and future work

**Measured cost:**

| n | frontier states | `count` (48 B/state) | `modcount` (32 B/state) |
|---|---|---|---|
| 14 | 0.63M | 0.30 GB, 1.7 s | |
| 17 | 13.2M | 5.3 GB, 58 s | 154 s |
| 18 | 36.7M | 14.8 GB, 190 s | |
| 19 | 102.6M | – (needs more than 256 bits) | 22.8 GB, 25 min |

- **Buffers versus measured peak:** the buffers need about 4 × entry size × states, which is 13 GB at n = 19. The rest of the measured peak is macOS keeping freed blocks resident while the buffers grow.
- **n = 20:** about 280M states, which needs at least 36 GB and is not feasible on this 36 GB machine.

**Ways forward, roughly in order of payoff:**
1. **Compact state encoding.** Frontier labels cannot cross, because the frontier is a planar boundary, so a 5-symbol per-cell code fits in a `u64` up to width 27. That halves the key size, giving 16 B per state.
2. **Left-right symmetry of the scan:** about 2× fewer states.
3. **Disk-backed or distributed state sharding.** Each prime pass is already independent, so passes can run on separate machines.
4. **GPU transfer matrix** (`cudarc` + CUB radix sort and reduce-by-key on an H100) for n ≥ 20.
5. **More digits of F** from wider cylinders (19+), which directly tightens predictions for large n.
6. **OEIS submission:** the square sequence, the T(h, w) table and the classes.

---

## 12. Code layout

| file | purpose |
|---|---|
| `src/frontier.rs` | state key (`u128`: 4-bit label per slot + `DIAG`/`DIAG2` bits), canonical relabelling, `Cell` specs, rectangle and cylinder cell lists |
| `src/engine.rs` | parallel engine: expand into at most 2 successors per state, sort, merge; closing rules |
| `src/counts.rs` | rectangles (`U256` and modular), symmetry-fixed counts, Burnside classes |
| `src/count.rs` | `Count` trait; `U256` (panics on overflow); `ModPair` residues; `BigUint` + `crt`; `f64` for eigenvalues |
| `src/growth.rs` | strip and cylinder eigenvalues; prediction constants and `predicted_ln` |
| `src/brute.rs` | direct enumeration: rectangles, symmetries, cylinders |
| `src/main.rs` | command-line interface |
| `tests/regression.rs` | brute-force cross-checks, historical values, closed forms, regressions |
