using PrimesAndFactorizationNet;
using Combinatorics;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Globalization;
using System.Linq;

namespace PrimesAndFactorizationTests
{
	[TestFixture]
	public class FactorizationTests
	{
		const ulong PRIME_FACTOR_LIMIT = 1_000_000UL;
		const ulong COMPOSITE_NUMBER_LIMIT = 1_000_000_000UL;
		public static Factorizator _factorizator = new Factorizator(PRIME_FACTOR_LIMIT);
		static readonly Random _rnd = new Random();
			
		[Test]
		[Repeat(1000)]
		public void FactorizationTest()
		{
			NumberAndPrimeFactors expectedResult = PrimeTestHelper.ComposeRandomNumber(_rnd.Next(1, 10), 10, COMPOSITE_NUMBER_LIMIT);
			var result = _factorizator.GetPrimeFactors(expectedResult.Number).ToList();

			Assert.That(expectedResult.PrimeFactors, Is.EqualTo(result));
		}

		[Test]
		[Repeat(10)]
		public void FatorizationToPowerOfPrimesTest()
		{
			Dictionary<ulong,int> expectedResult = new();
			int numberOfFactors = _rnd.Next(1, 6);			
			List<ulong> factors = new();
			for(int i= 0; i < numberOfFactors; i++)
			{
				ulong prime = PrimeTestHelper.GetRandomPrime(150);
				int power = _rnd.Next(1,4);				
				for(int j = 0; j < power; j++)
					factors.Add(prime);
			}
			NumberAndPrimeFactors testNumber = new NumberAndPrimeFactors(factors, 
				_factorizator.UpperLimitOfPrimeFactors * _factorizator.UpperLimitOfPrimeFactors);

			factors = testNumber.PrimeFactors.ToList();
			expectedResult = factors.Distinct()
				.Select(x => (x, factors.Count(y => y==x)))
				.ToDictionary(z => z.Item1, u => u.Item2);

			Dictionary<ulong,int> result = _factorizator.FactorizeAsPowersOfPrimes(testNumber.Number);

			Assert.That(expectedResult, Is.EqualTo(result).AsCollection);
		}

		private class GetFactorsTestSource : IEnumerable<(ulong, ulong[])>
		{
			public IEnumerator<(ulong, ulong[])> GetEnumerator()
			{
				yield return (3, new ulong[] { 1, 3 });
				yield return (28, new ulong[] { 1, 2, 4, 7, 14, 28 });
				yield return (36, new ulong[] { 1, 2, 3, 4, 36, 6, 9, 12, 18 });
				yield return (128, new ulong[] { 32, 64, 2, 128, 4, 1, 8, 16 });
				yield return (129, new ulong[] { 43, 129, 3, 1 });
				yield return (455, new ulong[] { 65, 1, 35, 5, 7, 455, 13, 91 });
			}
			#region ienumerator_boring_stuff
			// Must also implement IEnumerable.GetEnumerator, but implement as a private method.
			private IEnumerator GetEnumerator1()
			{
				return this.GetEnumerator();
			}
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator1();
			}
			#endregion
		}

		[TestCaseSource(typeof(GetFactorsTestSource))]
		public void GetAllFactorsTest((ulong, ulong[]) testCase)
		{
			var result = _factorizator.GetAllFactors(testCase.Item1);			
			CollectionAssert.AreEquivalent(result, testCase.Item2);
		}

		[TestCaseSource(typeof(GetFactorsTestSource))]
		public void GetProperFactorsTest((ulong, ulong[]) testCase)
		{
			var result = _factorizator.GetProperFactors(testCase.Item1);			
			var expectedResult = testCase.Item2.Where(s => s != testCase.Item1);
			CollectionAssert.AreEquivalent(result, expectedResult);
		}

		[Test]
		[Repeat(10)]
		public void GetComonFactorsTest()
		{
			NumberAndPrimeFactors mockNumber1 = PrimeTestHelper.ComposeRandomNumber(7,10,COMPOSITE_NUMBER_LIMIT);
			NumberAndPrimeFactors mockNumber2 = PrimeTestHelper.ComposeRandomNumber(7,10,COMPOSITE_NUMBER_LIMIT);
			
			var n1factorsSet = _factorizator.GetAllFactors(mockNumber1.Number).ToHashSet();
			var n2FactorsSet = _factorizator.GetAllFactors(mockNumber2.Number).ToHashSet();
			var commonFactors = n1factorsSet.Intersect(n2FactorsSet);

			var result = _factorizator.GetCommonFactors(mockNumber1.Number, mockNumber2.Number);

			CollectionAssert.AreEquivalent(result, commonFactors);
		}

		[Test]
		[Repeat(10)]
		public void AreCoPrimePositiveTest()
		{
			var primeFactors = PrimeTestHelper.GetNDifferentRandomPrimes(10, 50).ToList();
			primeFactors.Shuffle();			

			NumberAndPrimeFactors n1 = new(primeFactors.Take(5), PRIME_FACTOR_LIMIT);
			NumberAndPrimeFactors n2 = new(primeFactors.TakeLast(5), PRIME_FACTOR_LIMIT);

			bool result = _factorizator.AreCoPrime(n1.Number, n2.Number);
			
			Assert.That(result, Is.True);
		}

		[Test]
		[Repeat(20)]
		public void AreCoPrimeNegativeTest()
		{
			var primeFactors = PrimeTestHelper.GetNDifferentRandomPrimes(10, 40).ToList();
			primeFactors.Shuffle();

			var n1FactorsList = primeFactors.Take(5).ToList();
			var n2FactorsList = primeFactors.TakeLast(5).ToList();

			var extraFactors = PrimeTestHelper.GetNDifferentRandomPrimes(3, 25).ToList();
			n1FactorsList.AddRange(extraFactors);
			n2FactorsList.AddRange(extraFactors);

			NumberAndPrimeFactors n1 = new(n1FactorsList, PRIME_FACTOR_LIMIT);
			NumberAndPrimeFactors n2 = new(n2FactorsList, PRIME_FACTOR_LIMIT);

			var n1FactorSet = n1.PrimeFactors.ToHashSet();
			var n2FactorSet = n2.PrimeFactors.ToHashSet();
			bool expectedResult = n1FactorSet.Intersect(n2FactorSet).Count() == 0;

			bool result = _factorizator.AreCoPrime(n1.Number, n2.Number);

			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[TestCase(10UL, 30)]
		[TestCase(2UL, 8)]
		[TestCase(11UL, 11)]
		[TestCase(36UL, 12)]
		[TestCase(120UL, 12)]
		[TestCase((ulong)Int32.MaxValue * 2UL + 1, 20)]
		public void GetCoPrimesTest(ulong toWhat, int coPrimeUpperLimit)
		{
			var expectedResult = Enumerable.Range(1, coPrimeUpperLimit)
				.Where(x => _factorizator.AreCoPrime((ulong)x, toWhat))
				.ToArray();

			var result = _factorizator.GetCoPrimes(toWhat, (ulong)coPrimeUpperLimit);

			CollectionAssert.AreEquivalent(expectedResult, result);
		}
	}
}