namespace OverPower.Application.Ports.Logging
{
    /// <summary>
    /// Authoritative stable semantic category constants for logging throughout the application.
    /// </summary>
    public static class GameLogCategories
    {
        public const string RunStart = "RUN.START";
        public const string RunEnd = "RUN.END";
        public const string CombatCardPlayed = "COMBAT.CARD.PLAYED";
        public const string CombatDamageResolved = "COMBAT.DAMAGE.RESOLVED";
        public const string SaveSuccess = "SAVE.SUCCESS";
        public const string SaveFailure = "SAVE.FAILURE";
    }
}
