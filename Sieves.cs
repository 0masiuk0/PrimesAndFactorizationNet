//https://habr.com/ru/post/526924/
//BY https://habr.com/ru/users/lightln2/

using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace primes.net
{
	public class SieveOfEratosthenes : ISieve
	{
		private BitArray Data;
		public int SieveSize => Data.Length;

		public SieveOfEratosthenes(int sieveSize)
		{
			Data = new BitArray(sieveSize);
			Data.SetAll(true);

			for (int p = 2; p * p < sieveSize; p++)
			{
				if (Data[p])
				{
					for (int i = p * p; i < SieveSize; i += p)
					{
						Data[i] = false;
					}
				}
			}
		}

		public void ListPrimes(Action<ulong> callback)
		{
			for (int i = 2; i < SieveSize; i++)
			{
				if (Data[i]) callback.Invoke((ulong)i);
			}
		}
	}

	public class PrimesSieve : ISieve
	{
		const int BUFFER_LENGTH = 200 * 1024;
		const int WHEEL = 30;
		const int WHEEL_PRIMES_COUNT = 3;

		private static ulong[] WheelRemainders = { 1, 7, 11, 13, 17, 19, 23, 29 };
		private static ulong[] SkipPrimes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };
		private static byte[] Masks = { 1, 2, 4, 8, 16, 32, 64, 128 };
		private static uint[][] OffsetsPerByte;

		private ulong SieveSize;
		private ulong[] FirstPrimes;
		private ulong[][] PrimeMultiples;

		static PrimesSieve()
		{
			OffsetsPerByte = new uint[256][];
			List<uint> offsets = new List<uint>();
			for (int b = 0; b < 256; b++)
			{
				offsets.Clear();
				for (int i = 0; i < WheelRemainders.Length; i++)
				{
					if ((b & Masks[i]) != 0) offsets.Add((uint)WheelRemainders[i]);
				}
				OffsetsPerByte[b] = offsets.ToArray();
			}
		}

		public PrimesSieve(ulong sieveSize)
		{			
			this.SieveSize = sieveSize;
			int firstChunkLength = (int)Math.Sqrt(sieveSize) + 1;
			SieveOfEratosthenes sieve = new SieveOfEratosthenes(firstChunkLength);
			List<ulong> firstPrimes = new List<ulong>();
			sieve.ListPrimes(firstPrimes.Add);
			FirstPrimes = firstPrimes.Skip(WHEEL_PRIMES_COUNT).ToArray();
			PrimeMultiples = new ulong[WheelRemainders.Length][];
			for (int i = 0; i < WheelRemainders.Length; i++)
			{
				PrimeMultiples[i] = new ulong[FirstPrimes.Length];
				for (int j = 0; j < FirstPrimes.Length; j++)
				{
					ulong prime = FirstPrimes[j];
					ulong val = prime * prime;
					while (val % WHEEL != WheelRemainders[i]) val += 2 * prime;
					PrimeMultiples[i][j] = (val - WheelRemainders[i]) / WHEEL;
				}
			}
		}

		private void SieveSegment(byte[] segmentData, ulong segmentStart, ulong segmentEnd)
		{
			for (int i = 0; i < segmentData.Length; i++) segmentData[i] = 255;
			ulong segmentLength = segmentEnd - segmentStart;

			for (int i = 0; i < WheelRemainders.Length; i++)
			{
				byte mask = (byte)~Masks[i];
				for (int j = 0; j < PrimeMultiples[i].Length; j++)
				{
					ulong current = PrimeMultiples[i][j] - segmentStart;
					if (current >= segmentLength) continue;
					ulong prime = FirstPrimes[j];

					while (current < segmentLength)
					{
						segmentData[current] &= mask;
						current += prime;
					}

					PrimeMultiples[i][j] = segmentStart + current;
				}
			}
		}

		public void ListPrimes(Action<ulong> callback)
		{
			foreach(var prime in Primes())
			{
				callback.Invoke(prime);
			}
		} 

		public IEnumerable<ulong> Primes()
		{
			foreach (ulong prime in SkipPrimes) if (prime < SieveSize) yield return prime;

			ulong max = (SieveSize + WHEEL - 1) / WHEEL;
			byte[] segmentData = new byte[BUFFER_LENGTH];
			ulong segmentStart = 1;
			ulong segmentEnd = Math.Min(segmentStart + BUFFER_LENGTH, max);
			while (segmentStart < max)
			{
				SieveSegment(segmentData, segmentStart, segmentEnd);
				for (uint i = 0; i < segmentData.Length; i++)
				{
					ulong offset = (segmentStart + i) * WHEEL;
					byte data = segmentData[i];
					uint[] offsets = OffsetsPerByte[data];
					for (int j = 0; j < offsets.Length; j++)
					{
						ulong p = offset + offsets[j];
						if (p >= SieveSize) break;
						yield return p;
					}
				}
				segmentStart = segmentEnd;
				segmentEnd = Math.Min(segmentStart + BUFFER_LENGTH, max);
			}
		}
	}

	public interface ISieve
	{
		void ListPrimes(Action<ulong> callback);
	}
}