using PrimesAndFactorizationNet;
using Combinatorics;
using NUnit.Framework.Constraints;

namespace PrimesAndFactorizationTests
{
	[TestFixture]
	public class FactorizationTests
	{
		public static Factorizator _factorizator = new Factorizator(1_000_000);
		static readonly Random _rnd = new Random();
		
		[Test]
		[Repeat(10)]
		public void FactorizationTest()
		{
			NumberAndPrimeFactors expectedResult = PrimeTestHelper.ComposeRandomNumber(_rnd.Next(1, 7), 10);
			var result = _factorizator.GetPrimeFactors().ToList();

			Assert.AreEqual(expectedResult.PrimeFactors, result);
		}
	}
}