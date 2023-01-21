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
		public static Factorizator _factorizator = new Factorizator(1_000_000);
		static readonly Random _rnd = new Random();
			
		[Test]
		[Repeat(1000)]
		public void FactorizationTest()
		{
			NumberAndPrimeFactors expectedResult = PrimeTestHelper.ComposeRandomNumber(_rnd.Next(1, 10), 10);
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
			NumberAndPrimeFactors mockNumber1 = PrimeTestHelper.ComposeRandomNumber(5,8);
			NumberAndPrimeFactors mockNumber2 = PrimeTestHelper.ComposeRandomNumber(5,8);
			List<ulong> commonFactors = new();

		}
	}
}