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
        private readonly ulong _seed;
        private ulong _state;

        public SeededRandomService(ulong seed)
        {
            _seed = seed;
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

            // Using wider long arithmetic to safely calculate range size without overflow (e.g. max - min).
            // range size fits perfectly inside uint.MaxValue when max = int.MaxValue and min = int.MinValue.
            long range = (long)maxExclusive - minInclusive;
            uint bound = (uint)range;

            // Unchecked addition and signed casting are intentional here.
            // Under standard C# unchecked behavior, if (int)NextUInt32(bound) overflows, the subsequent
            // addition minInclusive + value correctly wraps around, producing an unbiased, mathematically
            // correct signed integer in the exact requested range [minInclusive, maxExclusive).
            return unchecked(minInclusive + (int)NextUInt32(bound));
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

        public IRandomService CreateSubstream(string streamId)
        {
            if (streamId == null)
            {
                throw new ArgumentNullException(nameof(streamId), "Substream identifier cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(streamId))
            {
                throw new ArgumentException("Substream identifier cannot be empty or whitespace.", nameof(streamId));
            }

            // Derive seed using FNV-1a 64-bit and SplitMix64 finalization
            ulong hash = 14695981039346656037UL;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(streamId);
            for (int i = 0; i < bytes.Length; i++)
            {
                hash = unchecked((hash ^ bytes[i]) * 1099511628211UL);
            }

            ulong combined = unchecked(hash ^ _seed);

            // SplitMix64 finalization step
            combined = unchecked((combined ^ (combined >> 30)) * 0xbf58476d1ce4e5b9UL);
            combined = unchecked((combined ^ (combined >> 27)) * 0x94d049bb133111ebUL);
            ulong childSeed = unchecked(combined ^ (combined >> 31));

            return new SeededRandomService(childSeed);
        }
    }
}
