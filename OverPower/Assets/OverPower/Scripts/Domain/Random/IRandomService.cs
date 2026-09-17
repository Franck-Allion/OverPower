using System.Collections.Generic;

namespace OverPower.Domain.Random
{
    public interface IRandomService
    {
        /// <summary>
        /// Generates a deterministic, unbiased integer in the range [0, maxExclusive).
        /// </summary>
        /// <param name="maxExclusive">The exclusive upper bound (must be greater than 0).</param>
        /// <returns>A value greater than or equal to 0, and strictly less than maxExclusive.</returns>
        int NextInt(int maxExclusive);

        /// <summary>
        /// Generates a deterministic, unbiased integer in the range [minInclusive, maxExclusive).
        /// </summary>
        /// <param name="minInclusive">The inclusive lower bound.</param>
        /// <param name="maxExclusive">The exclusive upper bound (must be strictly greater than minInclusive).</param>
        /// <returns>A value greater than or equal to minInclusive, and strictly less than maxExclusive.</returns>
        int NextInt(int minInclusive, int maxExclusive);

        /// <summary>
        /// Shuffles the specified list in-place using a deterministic Fisher-Yates algorithm.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="items">The list to shuffle (must not be null).</param>
        void Shuffle<T>(IList<T> items);

        /// <summary>
        /// Derives a completely independent, deterministic named substream from the current random service
        /// without consuming any random states of the parent.
        /// </summary>
        /// <param name="streamId">The stable, semantic identifier of the substream (must not be null or blank).</param>
        /// <returns>A new deterministic random service initialized with the derived seed.</returns>
        IRandomService CreateSubstream(string streamId);
    }
}
