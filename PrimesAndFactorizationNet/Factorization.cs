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

			ulong currentLimit = upperLimit;
			foreach(var prime in PrimesCache.Primes())
			{
				currentLimit = IntegerExponentiation.ISqrt(number);

				if (prime > currentLimit)
					break;

				while (number % prime == 0)
				{
					yield return prime;
					number = number / prime;
				}				
			}
			if (number != 1)
				yield return number;
		}

		public Dictionary<ulong, int> FactorizeAsPowersOfPrimes(ulong number)
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

		public IEnumerable<ulong> GetAllFactors(ulong number) => GetAllFactorsFromPrimePoweresDictionary(FactorizeAsPowersOfPrimes(number));		

		private IEnumerable<ulong> GetAllFactorsFromPrimePoweresDictionary(Dictionary<ulong, int> powers)
		{
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

		public IEnumerable<ulong> GetCommonFactors(params ulong[] numbers)
		{
			var commonPrimesDictionary = GetCommonPrimesDictionary(numbers);
			return GetAllFactorsFromPrimePoweresDictionary(commonPrimesDictionary);
		}

		public bool AreCoPrime(params ulong[] numbers)
		{
			var commonFactors = GetCommonFactors(numbers).ToArray();
			return commonFactors.Length == 1 && commonFactors[0] == 1;
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

		public ulong ReconstructNumberByPrimePowers(Dictionary<ulong, int> powers)
		{
			ulong result = 1;
			foreach (KeyValuePair<ulong, int> pair in powers)
			{
				result *= IntegerExponentiation.IntPow(pair.Key, pair.Value);
			}
			return result;
		}		

		public IEnumerable<ulong> GetProperFactors(ulong number) => GetAllFactors(number).SkipLast(1);

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
		
		#region helpers

		private static (int, int) CalculateSegmentIndex(ulong index, int segmentLength)
		{
			int segmentNumber = (int)(index / (ulong)segmentLength);
			int indexInSegment = (int)(index - (ulong)segmentLength * (ulong)segmentNumber);
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
			var numbersAsPrimesDict = (from num in numbers select FactorizeAsPowersOfPrimes(num)).ToList();

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
