using System;
using System.Collections.Generic;
using OverPower.Domain.Random;

namespace OverPower.Infrastructure.Random
{
    /// <summary>
    /// A deterministic, seeded pseudo-random number generator implementing <see cref="IRandomService"/> 
    /// using the non-cryptographic PCG32 (XSH-RR 64/32) algorithm.
    /// </summary>
    /// <remarks>
    /// Changing this algorithm or its parameters will alter deterministic replay/run sequences. 
    /// Known-seed tests protect this behavior against unintended changes.
    /// </remarks>
    public sealed class SeededRandomService : IRandomService
    {
        private ulong _state;

        public SeededRandomService(ulong seed)
        {
            _state = unchecked(seed + 1442695040888963407UL);
            NextUInt32();
        }

        private uint NextUInt32()
        {
            ulong oldState = _state;
            _state = unchecked(oldState * 6364136223846793005UL + 1442695040888963407UL);
            uint xorshifted = unchecked((uint)(((oldState >> 18) ^ oldState) >> 27));
            int rot = (int)(oldState >> 59);
            return unchecked((xorshifted >> rot) | (xorshifted << ((-rot) & 31)));
        }

        private uint NextUInt32(uint bound)
        {
            uint threshold = unchecked((uint)(-bound) % bound);
            while (true)
            {
                uint r = NextUInt32();
                if (r >= threshold)
                {
                    return r % bound;
                }
            }
        }

        public int NextInt(int maxExclusive)
        {
            if (maxExclusive <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxExclusive), "Upper bound must be greater than 0.");
            }

            return (int)NextUInt32((uint)maxExclusive);
        }

        public int NextInt(int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive)
            {
                throw new ArgumentOutOfRangeException(nameof(maxExclusive), "Upper bound must be strictly greater than lower bound.");
            }

            long range = (long)maxExclusive - minInclusive;
            uint bound = (uint)range;
            return minInclusive + (int)NextUInt32(bound);
        }

        public void Shuffle<T>(IList<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items), "Collection to shuffle cannot be null.");
            }

            int count = items.Count;
            for (int i = count - 1; i >= 1; i--)
            {
                int j = NextInt(i + 1);
                T temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }
    }
}
