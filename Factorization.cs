using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PrimesAndFactorizationNet
{
	public class Factorizator
	{
		public ulong UpperLimitOfPrimeFactors => PrimesCache.CoveredRangeOfIntegers;

		public Factorizator(ulong upperLimitOfPrimeFactors)
		{
			if (PrimesCache.CoveredRangeOfIntegers < upperLimitOfPrimeFactors)
			{
				PrimesCache.MemorizePrimesBelow(upperLimitOfPrimeFactors);
			}
		}

		public ulong[] GetPrimeFactors(ulong number)
		{
			List<ulong> factors = new();
			ulong upperLimit = ISqrt(number);

			if (upperLimit > UpperLimitOfPrimeFactors)
				throw new ArgumentException($"Cached range of integers (<{UpperLimitOfPrimeFactors}) " +
					$"may not be sufficient to factorize {number}");

			ulong currentLimit = upperLimit;
			foreach(var prime in PrimesCache.PrimesBelowLimit(upperLimit))
			{
				if (prime > currentLimit)
					break;

				while (number % prime == 0)
				{
					number = number / prime;

					factors.Add(prime);

					currentLimit = ISqrt(number) + 1;
				}
			}
			if (number != 1)
				factors.Add(number);

			return factors.ToArray();
		}

		public Dictionary<ulong, int> FactorizeAsPowersOfPrimes(ulong number)
		{
			Dictionary<ulong, int> powersOfFactors = new Dictionary<ulong, int>();			
			foreach(ulong primeFactor in GetPrimeFactors(number))
			{
				if (powersOfFactors.ContainsKey(primeFactor))
					powersOfFactors[primeFactor] = 1;
				else
					powersOfFactors[primeFactor]++;
			}
			return powersOfFactors;
		}

		public bool IsCoPrime(ulong a, ulong b)
		{
			return EulcidGreatestCommonDenominator(a, b);		
		}

		public List<ulong> GetCoPrimes(ulong N, ulong upperLimit)
		{
			List<ulong> coPrimes = new();
			var distinctFactorsOfN = GetPrimeFactors(N).Distinct().ToArray();
			int bitArrayCount = (int)(upperLimit / (ulong)int.MaxValue) + 1;
			BitArray[] sieveSegments = new BitArray[bitArrayCount];

			for(int i = 0; i < bitArrayCount-1; i++)
			{
				sieveSegments[i] = new(int.MaxValue);
				sieveSegments[i].SetAll(true);
			}

			int lastBitArrayBitCount = (int)(upperLimit - (ulong)int.MaxValue * (ulong)(bitArrayCount - 1));
			sieveSegments[bitArrayCount - 1] = new(lastBitArrayBitCount);

			foreach(var factor in distinctFactorsOfN)
			{
				var index = CalculateSegmentIndex(factor - 1, int.MaxValue);
				sieveSegments[index.Item1][index.Item2] = false;
				checked
				{
					try
					{
						for (ulong j = factor * factor - 1; j < upperLimit; j += factor)
						{
							index = CalculateSegmentIndex(factor - 1, int.MaxValue);
							sieveSegments[index.Item1][index.Item2] = false;
						}
					}
					catch(OverflowException)
					{ }
				}
				for(int i = 0; i < bitArrayCount; i++)
				{
					int bitCountHere = i == bitArrayCount - 1 ? lastBitArrayBitCount : int.MaxValue;
					for(int j = 0; j < bitCountHere; j++)
					{
						if (sieveSegments[i][j])
							coPrimes.Add((ulong)i * (ulong)int.MaxValue + (ulong)j);
					}
				}				
			}

			return coPrimes;
		}

		private bool EulcidGreatestCommonDenominator(ulong a, ulong b)
		{
			throw new NotImplementedException();
		}

		public ulong ReconstructNumberByPrimePowers(Dictionary<ulong, int> powers)
		{
			ulong result = 1;
			foreach (KeyValuePair<ulong, int> pair in powers)
			{
				result *= IntPow(pair.Key, pair.Value);
			}
			return result;
		}

		public IEnumerable<ulong> GetAllFactors(ulong number)
		{
			Dictionary<ulong, int> powers = FactorizeAsPowersOfPrimes(number);
			var possibleRanges = from primePower in powers
								 select
									from p in Enumerable.Range(0, primePower.Value)
									select new { primePower.Value, p };

			foreach(var combination in Cartesian(possibleRanges))
			{
				IEnumerable<(ulong, int)> combo = combination as IEnumerable<(ulong, int)>;
				ulong result = 1;
				foreach(var pair in combo)
				{
					result *= IntPow(pair.Item1, pair.Item2);
				}
				yield return result;
			}

		}

		public IEnumerable<ulong> GetProperFactors(ulong number) => GetAllFactors(number).SkipLast(1);

		static ulong IntPow(ulong x, int pow)
		{
			if (pow < 0)
			{
				throw new ArgumentOutOfRangeException("Cannot raise to negative power in integers");
			}

			ulong ret = 1;
			while (pow != 0)
			{
				if ((pow & 1) == 1)
					ret *= x;
				x *= x;
				pow >>= 1;
			}
			return ret;
		}		

		#region helpers
		private static ulong ISqrt(ulong n)
		{
			if (n == 0) return 0;
			if (n > 0)
			{
				int bitLength = Convert.ToInt32(Math.Ceiling(Math.Log(n, 2)));
				ulong root = 1UL << (bitLength / 2);

				while (!isSqrt(n, root))
				{
					root += n / root;
					root /= 2;
				}

				return root;
			}

			throw new ArithmeticException("NaN");
		}

		private static Boolean isSqrt(ulong n, ulong root)
		{
			ulong lowerBound = root * root;
			ulong upperBound = (root + 1) * (root + 1);

			return (n >= lowerBound && n < upperBound);
		}

		private static (int, int) CalculateSegmentIndex(ulong index, int segmentLength)
		{
			int segmentNumber = (int)(index / (ulong)segmentLength);
			int indexInSegment = (int)(index - (ulong)segmentLength * (ulong)segmentNumber);
			return new(segmentNumber, indexInSegment);
		}

		private static IEnumerable Cartesian(IEnumerable<IEnumerable> items)
		{
			var slots = items
			// initialize enumerators
			.Select(x => x.GetEnumerator())
			// get only those that could start in case there is an empty collection
			.Where(x => x.MoveNext())
			.ToArray();

			while (true)
			{
				// yield current values
				yield return slots.Select(x => x.Current);

				// increase enumerators
				foreach (var slot in slots)
				{
					// reset the slot if it couldn't move next
					if (!slot.MoveNext())
					{
						// stop when the last enumerator resets
						if (slot == slots.Last()) { yield break; }
						slot.Reset();
						slot.MoveNext();
						// move to the next enumerator if this reseted
						continue;
					}
					// we could increase the current enumerator without reset so stop here
					break;
				}
			}
		}
		#endregion
	}
}
