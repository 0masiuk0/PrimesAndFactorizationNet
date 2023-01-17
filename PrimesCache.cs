using primes.net;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace PrimesAndFactorizationNet
{
	public static class PrimesCache
	{
		const ulong FIRST_PRIME = 2;
		const ulong DFAULT_CACHE_UPPER_LIMIT = 1000;

		static ConcurrentDictionary<ulong, ulong> _primes;		
		static ulong _sievedRangeOfIntegers = 0;
		static object _cacheUpdateLock = new();

		public static ulong CoveredRangeOfIntegers { get => _sievedRangeOfIntegers; }

		
		static PrimesCache()
		{
			lock (_cacheUpdateLock)
			{
				ClearAndRecalculateCache(DFAULT_CACHE_UPPER_LIMIT);
			}
		}

		public static void MemorizePrimesBelow(ulong upperLimit)
		{
			if (upperLimit > _sievedRangeOfIntegers)
			{
				lock (_cacheUpdateLock)
				{
					ClearAndRecalculateCache(upperLimit);
				}
			}
		}

		public static void ClearAndRecalculateCache(ulong upper_limit)
		{
			_primes = new ConcurrentDictionary<ulong, ulong>();
			PrimesSieve sieve = new PrimesSieve(upper_limit);
			using (var primeEnumerator = sieve.Primes().GetEnumerator())
			{
				lock (_cacheUpdateLock)
				{
					primeEnumerator.MoveNext();
					ulong previous_prime = primeEnumerator.Current;
					while (primeEnumerator.MoveNext())
					{
						ulong nextPrime = primeEnumerator.Current;
						_primes[previous_prime] = nextPrime;
						previous_prime = nextPrime;
					}
					_primes[previous_prime] = UInt64.MaxValue;
					_sievedRangeOfIntegers = upper_limit;
				}
			}
		}

		public static IEnumerable<ulong> Primes()
		{
			ulong currentPrime = FIRST_PRIME;
			do
			{
				yield return currentPrime;
				currentPrime = _primes[currentPrime];
			} while (currentPrime != UInt64.MaxValue);
		}

		public static bool IsCachedPrime(ulong number)
		{
			if (number <= _sievedRangeOfIntegers)
				return _primes.ContainsKey(number);
			else
				throw new ArgumentOutOfRangeException("number", "Number is beyoned range of integers that has been checked for primes.");
		}

		public static IEnumerable<ulong> PrimesBelowLimit(ulong limit)
		{
			if (limit <= _sievedRangeOfIntegers)
				foreach (var prime in Primes())
				{
					if (prime >= limit)
						yield break;
					yield return prime;
				}
			else
				throw new ArgumentOutOfRangeException("limit", "Limit is beyoned range of integers that has been checked for primes.");
		}

		public static IEnumerable<ulong> GetNPrimes(int N)
		{		
			if (N > _primes.Count)
			{
				throw new ArgumentException($"Cache contains less then N ({N}) primes at the moment.");
			}
			using (var primesEnumerator = Primes().GetEnumerator())
			{
				for (int i = 1; i <= N; i++)
				{
					primesEnumerator.MoveNext();
					yield return primesEnumerator.Current;
				}
			}

		}

		public static bool PrimalityCheckNoCache(ulong number)
		{
			if (number == 1) return false;
			if (number == 2 || number == 3 || number == 5) return true;
			if (number % 2UL == 0 || number % 3UL == 0 || number % 5UL == 0) return false;

			var boundary = IntegerExponentiation.ISqrt(number);

			// You can do less work by observing that at this point, all primes 
			// other than 2 and 3 leave a remainder of either 1 or 5 when divided by 6. 
			// The other possible remainders have been taken care of.
			ulong i = 6; // start from 6, since others below have been handled.
			while (i <= boundary)
			{
				if (number % (i + 1) == 0 || number % (i + 5) == 0)
					return false;

				i += 6;
			}

			return true;
		}
	}
}
