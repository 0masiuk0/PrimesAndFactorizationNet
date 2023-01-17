using PrimesAndFactorizationNet;
using Combinatorics;
using NUnit.Framework.Constraints;

namespace PrimesAndFactorizationTests
{
	[TestFixture]
	public class FactorizationTests
	{
		public static Factorizator _factorizator = new Factorizator(1_000_000);
		
		[Test]
		[Repeat(10)]
		public void FactorizationTest()
		{
			Random rnd = new Random();
			int factorsCount = rnd.Next(25);
			List<ulong> factors = new();
			List<ulong> usedFactors = new();
			for (int i = 0; i < factorsCount; i++)
			{
				factors.Add(PrimeCacheTest.primesArray[rnd.Next(15)]);
			}			
			ulong product = 1UL;
			foreach(ulong factor in factors)
			{
				var newProduct = product * factor;
				if (newProduct > _factorizator.UpperLimitOfPrimeFactors)
					break;
				product = newProduct;
				usedFactors.Add(factor);
			}

			usedFactors.Sort();
			var expectedResult = _factorizator.GetPrimeFactors((ulong)product).ToList();

			Assert.AreEqual(expectedResult, usedFactors);
		}
	}
}