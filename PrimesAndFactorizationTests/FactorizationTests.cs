using PrimesAndFactorizationNet;
using Combinatorics;
using NUnit.Framework.Constraints;

namespace PrimesAndFactorizationTests
{
	[TestFixture]
	public class FactorizationTests
	{
		public static Factorizator _factorizator;
		static readonly Random _rnd = new Random();
		
		[SetUp]
		public void Setup()
		{
			_factorizator = new Factorizator(1_000_000);
		}
		
		[Test]
		[Repeat(10)]
		public void FactorizationTest()
		{
			NumberAndPrimeFactors expectedResult = PrimeTestHelper.ComposeRandomNumber(_rnd.Next(1, 7), 10);
			var result = _factorizator.GetPrimeFactors().ToList();

			Assert.AreEqual(expectedResult.PrimeFactors, result);
		}

		[Test]
		[Repeat(10)]
		public void FatorizationToPowerOfPrimesTest()
		{
			Dictionary<ulong,int> expectedResult = new();
			int numberOfFactors = _rnd(1, 6);
			ulong testNumber = 1;
			for(int i= 0; i < numberOfFactors; i++)
			{
				ulong prime = PrimeTestHelper.GetRandomPrime(150);
				int power = _rnd.Next(1,4);
				expectedResult[prime] = power;
				testNumber *= PrimesAndFactorizationNet.IntegerExponentiation(prime, power);
			}
			Dictionary<ulong,int> result = _factorizator.FactorizeAsPowersOfPrimes(testNumber);

			Assert.That(expectedResult, Is.EqualTo(result).AsCollection);
		}


		static class GetAllfactorsTestSource:IEnumerable<(ulong, ulong[])>
		{
			public static GetEnumerator<(ulong, ulong[])>()
			{
				yield return new {3, new int[] {1, 3}};
				yield return new {28, new int[] {1, 2, 4, 7, 14, 28}};
				yield return new {36, new int[] {1, 2, 3, 4, 36, 6, 9, 12, 18}};
				yield return new {128, new int[] {32, 64, 2, 128, 4, 1, 8, 16}};
				yield return new {129, new int[] {43, 129, 3, 1}};
				yield return new {455, new int[] {65, 1, 35, 5, 7, 455, 13, 91}};
			}
		}


		[TsestSource(typeof(GetAllfactorsTestSource))]
		public void GetAllFactorsTest((ulong, ulong[]) testCase)
		{
			var result = _factorizator.GetAllFactors(testCase.Item1).ToArray();			
			Assert.That(result, Is.EqualTo(testCase.Item2).AsCollection);
		}
	}
}