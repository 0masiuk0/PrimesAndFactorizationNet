namespace PrimesAndFactorizationNet
{
	internal static class FactorizatorHelpers
	{

		public static ulong IntPow(ulong x, int pow)
		{
			if (pow < 0)
			{
				throw new ArgumentOutOfRangeException("Cannot raise to negative power in integers");
			}

			ulong ret = 1;
			while (pow != 0)
			{
				if ((pow & 1) == 1)
					ret *= x;
				x *= x;
				pow >>= 1;
			}
			return ret;
		}

		public static ulong ISqrt(ulong n)
		{
			if (n == 0) return 0;
			if (n > 0)
			{
				int bitLength = Convert.ToInt32(Math.Ceiling(Math.Log(n, 2)));
				ulong root = 1UL << (bitLength / 2);

				while (!isSqrt(n, root))
				{
					root += n / root;
					root /= 2;
				}

				return root;
			}

			throw new ArithmeticException("NaN");
		}

		public static Boolean isSqrt(ulong n, ulong root)
		{
			ulong lowerBound = root * root;
			ulong upperBound = (root + 1) * (root + 1);

			return (n >= lowerBound && n < upperBound);
		}
	}
}