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

		[Test]
		public void GetAllFactorsTest()
		{
			
		}
	}
}