using NUnit.Framework.Constraints;
using PrimesAndFactorizationNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimesAndFactorizationTests
{
	[TestFixture]
	internal partial class PrimeCacheTest
	{
		[SetUp]
		public void SetUp()
		{
			PrimesCache.ClearAndRecalculateCache(primesArray.Last() + 1);
		}

		[Test]
		public void PrimeGenerationTest()
		{
			ulong[] primes = PrimesCache.Primes().ToArray();
			Assert.That(primes, Is.EqualTo(primesArray));
		}

		[TestCase(10UL)]
		[TestCase(50UL)]
		[TestCase(100UL)]
		public void PrimesBelowNTest(ulong N)
		{
			var expectation = primesArray.Where(s => s < N).ToArray();
			var result = PrimesCache.PrimesBelowLimit(N).ToArray();

			Assert.That(result, Is.EqualTo(expectation));
		}

		[TestCase(168)]
		[TestCase(15)]
		public void GetNPrimesTest(int N)
		{
			var expectation = primesArray.Take(N).ToArray();
			var result = PrimesCache.GetNPrimes(N).ToArray();

			Assert.That(result, Is.EqualTo(expectation));
		}

		[Test]
		public void GetNPrimesThrowSmallCacheExceptionTest()
		{			
			ActualValueDelegate<object> testDelegate = () => PrimesCache.GetNPrimes(1000).ToArray();
			Assert.That(testDelegate, Throws.ArgumentException);
		}

		[TestCase(127UL)]
		[TestCase(911UL)]
		[TestCase(165948884653UL)]
		public void PrimalityNoCacheTestPositive(ulong N)
		{
			Assert.True(PrimesCache.PrimalityCheckNoCache(N));
		}

		[TestCase(481UL)]
		[TestCase(913UL)]
		[TestCase(165948884657UL)]
		public void PrimalityNoCacheTestNegative(ulong N)
		{
			Assert.False(PrimesCache.PrimalityCheckNoCache(N));
		}

		[TestCase(481UL)]
		[TestCase(913UL)]
		public void ExtendPrimesCacheTest(ulong upperTreshold)
		{
			PrimesCache.ClearAndRecalculateCache(200);
			var underTreshold = primesArray.Where(s => s <= upperTreshold).ToArray();			
			var indexOfLast = underTreshold.Length;
			var expectatedRersult = primesArray.Take(indexOfLast + 1).ToArray();

			PrimesCache.ExtendPrimesCacheBySearch(upperTreshold);
			var result = PrimesCache.Primes();

			Assert.That(result, Is.EqualTo(expectatedRersult));
		}
	}
}
