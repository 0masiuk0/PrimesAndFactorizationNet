using System;

namespace PrimesAndFactorizationTests
{	
    internal static class PrimeTestHelper
    {
        Random _rnd = new()

        public static ulong GetRandomPrime(ulong upperInclusiveLimit)
        {
            var primesBelowLimit = primesArray.Where(s => s <= upperInclusiveLimit).ToArray()
            return primesArray[_rnd(primesBelowLimit.Length)];
        }

        public static Enumerable<ulong> GetNRandomPrimes(int N, ulong upperInclusiveLimit)
        {
            var primesBelowLimit = primesArray.Where(s => s <= upperInclusiveLimit).ToArray()
            for(int i = 0; i < N; i++)
            {
                    yield return primesArray[_rnd(primesBelowLimit.Length)];
            }
        }

        public ulong LimitedProduct(IEnumerable<ulong> factors, ulong upperInclusiveLimit = UInt64.MaxValue, out List<ulong> usedFactors)
        {
            checked
            {                
                usedFactors = new()
                ulong p = 1;
                foreach(var factor in factors)
                {                    
                    try
                    {
                        ulong productTemp = p * factor;
                        if (productTemp <= upperInclusiveLimit)
                        {
                            p = productTemp;
                            usedFactors.Add(factor);
                        }
                        else
                            return p;                        
                    }
                    catch(ArithmeticException)
                    {
                        return p;
                    }
                }
            }
        }

        public NumberAndPrimeFactors ComposeRandomNumber(int attemptedNumberOfPrimeFactors, int primeFactorsPoolSize)
        {
            var topPrimeFromPool = primesArray[primeFactorsPoolSize];
            List<ulong> factors = new()
            for(int i=0; i<attemptedNumberOfPrimeFactors; i++)
            {
                factors.Add(GetRandomPrime(topPrimeFromPool));
            }
            
            return NumberAndPrimeFactors(factors);
        }

        // 168 primes below 1000
		public static readonly ulong[] primesArray = new ulong[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
			31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107,
			109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181,
			191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263,
			269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353,
			359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443,
			449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541, 547,
			557, 563, 569, 571, 577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641,
			643, 647, 653, 659, 661, 673, 677, 683, 691, 701, 709, 719, 727, 733, 739,
			743, 751, 757, 761, 769, 773, 787, 797, 809, 811, 821, 823, 827, 829, 839,
			853, 857, 859, 863, 877, 881, 883, 887, 907, 911, 919, 929, 937, 941, 947,
			953, 967, 971, 977, 983, 991, 997 }; 
    }

    public struct NumberAndPrimeFactors
    {
        public readonly ulong Number;
        public readonly ulong[] PrimeFactors;

        public NumberAndPrimeFactors(IEnumerable<ulong> primeFactors)
        {
            List<ulong> usedFactors;
            Number = PrimeTestHelper.LimitedProduct(primeFactors, out usedFactors);
            PrimeFactors = primeFactors.ToList().Sort().ToArray();
        }

        public NumberAndPrimeFactors(ulong number, IEnumerable<ulong> primeFactors)
        {
            Number = number;
            PrimeFactors = primeFactors.ToList().Sort().ToArray();
        }
    }
}