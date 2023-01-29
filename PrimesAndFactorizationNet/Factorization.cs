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

		public IEnumerable<ulong> GetPrimeFactors(ulong number)
		{			
			ulong upperLimit = IntegerExponentiation.ISqrt(number);

			if (upperLimit > UpperLimitOfPrimeFactors)
				throw new ArgumentException($"Cached range of integers (<{UpperLimitOfPrimeFactors}) " +
					$"may not be sufficient to factorize {number}");

			if (number == 0)
				throw new ArgumentException($"Cannot factorize zero.");

			ulong currentLimit = upperLimit;
			foreach(var prime in PrimesCache.Primes())
			{
				if (prime > currentLimit)
					break;

				while (number % prime == 0)
				{
					yield return prime;
					number = number / prime;
				}

				currentLimit = IntegerExponentiation.ISqrt(number);
			}
			if (number != 1)
				yield return number;
		}

		public Dictionary<ulong, int> GetPrimeFactorsToPowersDictionary(ulong number)
		{
			Dictionary<ulong, int> powersOfFactors = new Dictionary<ulong, int>();			
			foreach(ulong primeFactor in GetPrimeFactors(number))
			{
				if (!powersOfFactors.ContainsKey(primeFactor))
					powersOfFactors[primeFactor] = 1;
				else
					powersOfFactors[primeFactor]++;
			}
			return powersOfFactors;
		}

		public IEnumerable<ulong> GetDistinctPrimeFactors(ulong number) => GetPrimeFactors(number).Distinct();		

		private IEnumerable<ulong> GetAllFactorsFromPrimePoweresDictionary(Dictionary<ulong, int> powers)
		{
			if (powers.Count == 0)
			{
				yield return 1;
				yield break;
			}

			var possibleRanges = from factorKeyPowerValuePair in powers	 select
									from possiblePow in Enumerable.Range(0, factorKeyPowerValuePair.Value + 1)
									select new {factor=factorKeyPowerValuePair.Key, possiblePow};

			foreach(var combination in FactorizationHelper.Cartesian(possibleRanges))
			{				
				ulong result = 1;
				foreach(var pair in combination)
				{
					result *= IntegerExponentiation.IntPow(pair.factor, pair.possiblePow);
				}
				yield return result;
			}
		}

		public IEnumerable<ulong> GetAllFactors(ulong number) => GetAllFactorsFromPrimePoweresDictionary(GetPrimeFactorsToPowersDictionary(number));		

		public IEnumerable<ulong> GetProperFactors(ulong number) => GetAllFactors(number).SkipLast(1);

		public IEnumerable<ulong> GetCommonFactors(params ulong[] numbers)
		{
			var commonPrimesDictionary = GetCommonPrimesDictionary(numbers);
			var result = GetAllFactorsFromPrimePoweresDictionary(commonPrimesDictionary);
			return result;
		}

		public bool AreCoPrime(params ulong[] numbers)
		{
			var commonFactors = GetCommonPrimesDictionary(numbers);
			return commonFactors.Count == 0;
		}

		public IEnumerable<ulong> GetCoPrimes(ulong N, ulong upperLimit)
		{			
			var distinctFactorsOfN = GetPrimeFactors(N).Distinct().ToArray();
			int bitArrayCount = (int)(upperLimit / (ulong)int.MaxValue) + 1;
			BitArray[] sieveSegments = new BitArray[bitArrayCount];

			for(int i = 0; i < bitArrayCount-1; i++)
			{
				sieveSegments[i] = new(int.MaxValue, true);
			}

			int lastBitArrayBitCount = (int)(upperLimit - (ulong)int.MaxValue * (ulong)(bitArrayCount - 1));
			BitArray lastSegment = new(lastBitArrayBitCount, true);
			sieveSegments[bitArrayCount - 1] = lastSegment;

			foreach(var factor in distinctFactorsOfN)
			{
				checked
				{
					try
					{
						for (ulong j = factor; j <= upperLimit; j += factor)
						{
							var index = CalculateSegmentIndex(j, int.MaxValue);
							sieveSegments[index.Item1][index.Item2] = false;
						}
					}
					catch(OverflowException)
					{ }
				}								
			}

			for (int i = 0; i < bitArrayCount; i++)
			{
				int bitCountHere = i == bitArrayCount - 1 ? lastBitArrayBitCount : int.MaxValue;
				for (int j = 0; j < bitCountHere; j++)
				{
					if (sieveSegments[i][j])
						yield return (ulong)i * (ulong)int.MaxValue + (ulong)j + 1;
				}
			}
		}		

		public ulong ReconstructNumberByPrimePowers(Dictionary<ulong, int> powers)
		{
			ulong result = 1;
			foreach (KeyValuePair<ulong, int> pair in powers)
			{
				result *= IntegerExponentiation.IntPow(pair.Key, pair.Value);
			}
			return result;
		}		

		

		public ulong GetGreatestCommonDenominator(params ulong[] numbers)
		{
			var len = numbers.Length;
			if (len < 2)
				throw new ArgumentException("Greatest common denominathor function needs at least two arguments");

			if (len == 2)
				return EulcidGreatestCommonDenominator(numbers[0], numbers[1]);

			List<ulong> commonPrimeFactors = GetPrimeFactors(numbers[0]).ToList();
			for(int i = 1; i < len; i++)
			{
				commonPrimeFactors = FactorizationHelper.Intersect(commonPrimeFactors, GetPrimeFactors(numbers[i]));				
			}			
			
			return commonPrimeFactors.Aggregate(1UL, (product, factor) => product * factor);
		}

		public ulong GetEulerTotient(ulong number)
		{			
			foreach(ulong primeFactor in GetDistinctPrimeFactors(number))
			{
				number = number - number / primeFactor;
			}
			return number;
		}

		public ulong GetNumberRadical(ulong number) => 
			GetDistinctPrimeFactors(number).Aggregate(1UL, (product, factor) => product * factor);

		#region helpers

		private static (int, int) CalculateSegmentIndex(ulong number, int segmentLength)
		{
			int segmentNumber = (int)(number / (ulong)segmentLength);
			int indexInSegment = (int)(number - (ulong)segmentLength * (ulong)segmentNumber) - 1;
			return new(segmentNumber, indexInSegment);
		}

		private ulong EulcidGreatestCommonDenominator(ulong a, ulong b)
		{
			ulong gcd;

            while(a!= 0 && b!=0)
            {
                if(a > b)
                {
                    a = a % b;
                }
                else
                {
                    b = b % a;
                }
            }

            gcd = a + b;
            return gcd;
		}

		private Dictionary<ulong, int> GetCommonPrimesDictionary(params ulong[] numbers)
		{
			var numbersAsPrimesDict = (from num in numbers select GetPrimeFactorsToPowersDictionary(num)).ToList();

			Dictionary<ulong, int> result = new();			
			foreach(var primePowerPair in numbersAsPrimesDict[0])
			{
				if (Enumerable.All(numbersAsPrimesDict, dict => dict.ContainsKey(primePowerPair.Key)))
				{
					result[primePowerPair.Key] = Enumerable.Min(numbersAsPrimesDict.Select(x => x[primePowerPair.Key]));
				}
			}			

			return result;
		}		
		#endregion
	}
}
