//! Count types carried by the transfer-matrix engine.

use std::fmt;

/// Anything the engine can accumulate per frontier state.
pub trait Count: Copy + Send + Sync {
    fn zero() -> Self;
    fn one() -> Self;
    fn add_assign(&mut self, other: &Self);
}

/// Fixed-width 256-bit unsigned integer, little-endian limbs.
///
/// Large enough for n x n counts up to n = 18. Addition panics on overflow
/// instead of wrapping — a silent wrap is exactly how the original C# 7x7
/// result went wrong.
#[derive(Clone, Copy, Default, PartialEq, Eq, Debug)]
pub struct U256(pub [u64; 4]);

impl U256 {
    pub const ZERO: U256 = U256([0; 4]);

    pub fn from_u64(x: u64) -> Self {
        U256([x, 0, 0, 0])
    }

    pub fn checked_add(&self, o: &U256) -> Option<U256> {
        let mut r = [0u64; 4];
        let mut carry = 0u128;
        for i in 0..4 {
            let s = self.0[i] as u128 + o.0[i] as u128 + carry;
            r[i] = s as u64;
            carry = s >> 64;
        }
        (carry == 0).then_some(U256(r))
    }

    pub fn checked_sub(&self, o: &U256) -> Option<U256> {
        let mut r = [0u64; 4];
        let mut borrow = 0u64;
        for i in 0..4 {
            let (d1, b1) = self.0[i].overflowing_sub(o.0[i]);
            let (d2, b2) = d1.overflowing_sub(borrow);
            r[i] = d2;
            borrow = (b1 | b2) as u64;
        }
        (borrow == 0).then_some(U256(r))
    }

    /// Divide by a small divisor, returning (quotient, remainder).
    pub fn div_rem_small(&self, d: u64) -> (U256, u64) {
        let mut q = [0u64; 4];
        let mut rem = 0u128;
        for i in (0..4).rev() {
            let cur = (rem << 64) | self.0[i] as u128;
            q[i] = (cur / d as u128) as u64;
            rem = cur % d as u128;
        }
        (U256(q), rem as u64)
    }

    pub fn is_zero(&self) -> bool {
        self.0.iter().all(|&x| x == 0)
    }

    pub fn to_f64(&self) -> f64 {
        self.0.iter().rev().fold(0.0, |f, &l| f * 18446744073709551616.0 + l as f64)
    }
}

impl Count for U256 {
    fn zero() -> Self {
        U256::ZERO
    }
    fn one() -> Self {
        U256::from_u64(1)
    }
    #[inline]
    fn add_assign(&mut self, o: &Self) {
        *self = self.checked_add(o).expect("count overflowed 256 bits");
    }
}

impl fmt::Display for U256 {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        const CHUNK: u64 = 1_000_000_000_000_000_000;
        if self.is_zero() {
            return f.pad("0");
        }
        let mut parts = vec![];
        let mut x = *self;
        while !x.is_zero() {
            let (q, r) = x.div_rem_small(CHUNK);
            parts.push(r);
            x = q;
        }
        let mut s = parts.pop().unwrap().to_string();
        while let Some(p) = parts.pop() {
            s += &format!("{:018}", p);
        }
        f.pad(&s)
    }
}

/// Floating-point weights, used for growth-rate (eigenvalue) estimates.
impl Count for f64 {
    fn zero() -> Self {
        0.0
    }
    fn one() -> Self {
        1.0
    }
    #[inline]
    fn add_assign(&mut self, o: &Self) {
        *self += *o;
    }
}

/// The 16 largest primes below 2^61, for modular counting.
pub const PRIMES: [u64; 16] = [
    2305843009213693951, 2305843009213693921, 2305843009213693907, 2305843009213693723,
    2305843009213693693, 2305843009213693669, 2305843009213693613, 2305843009213693561,
    2305843009213693549, 2305843009213693487, 2305843009213693421, 2305843009213693373,
    2305843009213693277, 2305843009213693193, 2305843009213693153, 2305843009213693133,
];

/// Residues modulo `PRIMES[OFF]` and `PRIMES[OFF + 1]`.
///
/// One pass of the engine with this count costs 32 B per state (vs 48 B for
/// `U256`) and has no size limit; several passes are combined with `crt`.
#[derive(Clone, Copy, Debug, PartialEq)]
pub struct ModPair<const OFF: usize>(pub [u64; 2]);

impl<const OFF: usize> Count for ModPair<OFF> {
    fn zero() -> Self {
        ModPair([0; 2])
    }
    fn one() -> Self {
        ModPair([1; 2])
    }
    #[inline]
    fn add_assign(&mut self, o: &Self) {
        for i in 0..2 {
            let p = PRIMES[OFF + i];
            let s = self.0[i] + o.0[i]; // < 2^62, no overflow
            self.0[i] = if s >= p { s - p } else { s };
        }
    }
}

/// Arbitrary-size unsigned integer, little-endian limbs. Only what CRT
/// reconstruction and printing need.
#[derive(Clone, Debug, Default, PartialEq, Eq)]
pub struct BigUint(Vec<u64>);

impl BigUint {
    pub fn from_u64(x: u64) -> Self {
        BigUint(vec![x])
    }

    pub fn mul_add_small(&mut self, m: u64, a: u64) {
        let mut carry = a as u128;
        for l in self.0.iter_mut() {
            let v = *l as u128 * m as u128 + carry;
            *l = v as u64;
            carry = v >> 64;
        }
        if carry != 0 {
            self.0.push(carry as u64);
        }
    }

    pub fn div_rem_small(&self, d: u64) -> (BigUint, u64) {
        let mut q = vec![0u64; self.0.len()];
        let mut rem = 0u128;
        for i in (0..self.0.len()).rev() {
            let cur = (rem << 64) | self.0[i] as u128;
            q[i] = (cur / d as u128) as u64;
            rem = cur % d as u128;
        }
        while q.last() == Some(&0) {
            q.pop();
        }
        (BigUint(q), rem as u64)
    }

    pub fn is_zero(&self) -> bool {
        self.0.iter().all(|&x| x == 0)
    }

    pub fn bits(&self) -> u32 {
        match self.0.iter().rposition(|&x| x != 0) {
            None => 0,
            Some(i) => 64 * i as u32 + 64 - self.0[i].leading_zeros(),
        }
    }
}

impl From<U256> for BigUint {
    fn from(x: U256) -> Self {
        let mut v = x.0.to_vec();
        while v.last() == Some(&0) {
            v.pop();
        }
        BigUint(v)
    }
}

impl fmt::Display for BigUint {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        const CHUNK: u64 = 1_000_000_000_000_000_000;
        if self.is_zero() {
            return f.pad("0");
        }
        let mut parts = vec![];
        let mut x = self.clone();
        while !x.is_zero() {
            let (q, r) = x.div_rem_small(CHUNK);
            parts.push(r);
            x = q;
        }
        let mut s = parts.pop().unwrap().to_string();
        while let Some(p) = parts.pop() {
            s += &format!("{:018}", p);
        }
        f.pad(&s)
    }
}

fn mul_mod(a: u64, b: u64, p: u64) -> u64 {
    (a as u128 * b as u128 % p as u128) as u64
}

fn pow_mod(mut a: u64, mut e: u64, p: u64) -> u64 {
    let mut r = 1;
    while e > 0 {
        if e & 1 == 1 {
            r = mul_mod(r, a, p);
        }
        a = mul_mod(a, a, p);
        e >>= 1;
    }
    r
}

/// Chinese remainder reconstruction (Garner) of x from `residues[i] = x mod PRIMES[i]`.
/// Returns the unique x below the product of the primes used.
pub fn crt(residues: &[u64]) -> BigUint {
    let k = residues.len();
    // Mixed-radix digits: x = t0 + p0 (t1 + p1 (t2 + ...)).
    let mut t = vec![0u64; k];
    for i in 0..k {
        let p = PRIMES[i];
        // Value of the digits so far, and the product of earlier primes, mod p.
        let (mut v, mut m) = (0u64, 1u64);
        for j in 0..i {
            v = (v + mul_mod(t[j], m, p)) % p;
            m = mul_mod(m, PRIMES[j] % p, p);
        }
        let diff = (residues[i] % p + p - v) % p;
        t[i] = mul_mod(diff, pow_mod(m, p - 2, p), p);
    }
    let mut x = BigUint::from_u64(t[k - 1]);
    for i in (0..k - 1).rev() {
        x.mul_add_small(PRIMES[i], t[i]);
    }
    x
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn crt_roundtrip() {
        // x = 3^150, about 238 bits.
        let mut x = BigUint::from_u64(1);
        for _ in 0..150 {
            x.mul_add_small(3, 0);
        }
        let res: Vec<u64> = PRIMES[..5].iter().map(|&p| x.div_rem_small(p).1).collect();
        assert_eq!(crt(&res), x);
        assert_eq!(x.bits(), 238);
    }

    #[test]
    fn display_and_carry() {
        let a = U256([u64::MAX, 0, 0, 0]);
        let b = a.checked_add(&U256::from_u64(1)).unwrap();
        assert_eq!(b, U256([0, 1, 0, 0]));
        assert_eq!(b.to_string(), "18446744073709551616");
        assert_eq!(b.checked_sub(&U256::from_u64(1)).unwrap(), a);
        assert_eq!(U256::ZERO.to_string(), "0");
    }

    #[test]
    #[should_panic(expected = "overflowed")]
    fn overflow_panics() {
        let mut a = U256([u64::MAX; 4]);
        a.add_assign(&U256::one());
    }
}
