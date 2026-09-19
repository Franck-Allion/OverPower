using System;

namespace OverPower.Domain.Save
{
    /// <summary>
    /// Extensible, immutable save DTO containing persistent statistical metrics across runs.
    /// </summary>
    public sealed class StatisticsSaveDto
    {
        public int RunsStarted { get; }
        public int RunsCompleted { get; }
        public int Victories { get; }
        public int HeroDeaths { get; }

        /// <summary>
        /// Creates a new StatisticsSaveDto with explicit non-negative and logical relationship checks.
        /// </summary>
        public StatisticsSaveDto(int runsStarted, int runsCompleted, int victories, int heroDeaths)
        {
            if (runsStarted < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(runsStarted), "Runs started cannot be negative.");
            }
            if (runsCompleted < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(runsCompleted), "Runs completed cannot be negative.");
            }
            if (victories < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(victories), "Victories cannot be negative.");
            }
            if (heroDeaths < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(heroDeaths), "Hero deaths cannot be negative.");
            }

            if (runsCompleted > runsStarted)
            {
                throw new ArgumentException("Runs completed cannot exceed runs started.", nameof(runsCompleted));
            }
            if (victories + heroDeaths > runsCompleted)
            {
                throw new ArgumentException("Sum of victories and hero deaths cannot exceed total runs completed.", nameof(victories));
            }

            RunsStarted = runsStarted;
            RunsCompleted = runsCompleted;
            Victories = victories;
            HeroDeaths = heroDeaths;
        }
    }
}
