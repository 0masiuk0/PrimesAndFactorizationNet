using primes.net;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace PrimesAndFactorizationNet
{
	public static class Helpers
	{
        public ulong ReconstructNumberByPrimePowers(Dictionary<ulong, int> powers)
        {
            ulong result = 1;
            foreach(KeyValuePair<ulong, int> pair in poweres)
            {
                result *= IntPow(pair.Key, pair.Value);
            }
            return result;
        }    

        public IEnumerable<ulong> GetAllFactors(ulong number)
        {
            Dictionary<ulong, int> powers = GetPowers(number);
            var possibleRanges = from primePower in poweres select 
                                    from p in Enumerable.Range(0, primePower.Value) 
                                    group p by primePower.Value;
            
            
        }

        public IEnumerable<ulong> GetProperFactors(ulong number) => GetAllFactors(number).SkipLast(1);

        static ulong IntPow(ulong x, int pow)
        {
            if (pow < 0)
            {
                throw new ArgumentOurOfRangeException("Cannot raise to negative power in integers");
            }

            ulong ret = 1;
            while ( pow != 0 )
            {
                if ( (pow & 1) == 1 )
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }

        public static IEnumerable Cartesian(this IEnumerable<IEnumerable> items)
        {
            var slots = items
            // initialize enumerators
            .Select(x => x.GetEnumerator())
            // get only those that could start in case there is an empty collection
            .Where(x => x.MoveNext())
            .ToArray();

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
}
