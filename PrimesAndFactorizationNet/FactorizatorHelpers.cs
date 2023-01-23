using System.Collections;
using System.Collections.Generic;

namespace PrimesAndFactorizationNet
{
	public static class IntegerExponentiation
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

	internal static class FactorizationHelper
	{
		public static List<T> Intersect<T>(IEnumerable<T> a, IEnumerable<T> b)
		{
			List<T> result = new();
			List<T> bList = b.ToList();
			foreach(T item in a)
			{
				if (bList.Contains(item))
				{
					result.Add(item);
					bList.Remove(item);
				}
			}
			return result;
		}

		public static IEnumerable<IEnumerable<T>> Cartesian<T>(IEnumerable<IEnumerable<T>> items)
		{
			var slots = items
			// initialize enumerators
			.Select(x => x.GetResetableEnumerator())
			// get only those that could start in case there is an empty collection
			.Where(x => x.MoveNext())
			.ToArray();

			if (slots.Length == 0)
				yield break;

			while (true)
			{
				// yield current values
				yield return slots.Select(x => x.Current);

				// increase enumerators
				foreach (var slot in slots)
				{
					// reset the slot if it couldn't move next
					if (!slot.MoveNext())
					{
						// stop when the last enumerator resets
						if (slot == slots.Last()) { yield break; }
						slot.Reset();
						slot.MoveNext();
						// move to the next enumerator if this reseted
						continue;
					}
					// we could increase the current enumerator without reset so stop here
					break;
				}
			}
		}
	}

	static class ResetabeIEnumerableExtentionMethod
		{
			public static ResetableEnumerator<T> GetResetableEnumerator<T>(this IEnumerable<T> collection)
			{
				return new ResetableEnumerator<T>(collection);
			}
			
			public class ResetableEnumerator<U> : IEnumerator<U>
			{
				readonly IEnumerable<U> _collection;
				IEnumerator<U> _originalEnumerator;
				public ResetableEnumerator(IEnumerable<U> collection)
				{
					_collection = collection;
					_originalEnumerator = _collection.GetEnumerator();
				}

				public U Current => _originalEnumerator.Current;
				object IEnumerator.Current => _originalEnumerator.Current;
				public bool MoveNext() => _originalEnumerator.MoveNext();
				public void Reset() 
				{
					_originalEnumerator = _collection.GetEnumerator();
				}
				
				public bool ResetAndMoveNext()
				{
					Reset();
					return MoveNext();
				}

				public void Dispose()
				{}
			}
		}
}