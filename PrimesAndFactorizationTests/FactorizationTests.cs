

using PrimesAndFactorizationNet;

namespace PrimesAndFactorizationTests
{
	[TestFixture]
	public class FactorizationTests
	{
		[SetUp]
		public void Setup()
		{
			PrimesCache.MemorizePrimesBelow(1_000_000);
		}

		[Test]
		public void Test1()
		{
			Assert.Pass();
		}
	}
}