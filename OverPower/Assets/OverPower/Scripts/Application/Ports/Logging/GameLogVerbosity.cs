namespace OverPower.Application.Ports.Logging
{
    /// <summary>
    /// Defines the filtering level for the game logging infrastructure.
    /// </summary>
    public enum GameLogVerbosity
    {
        /// <summary>
        /// Only write logs of level Error.
        /// </summary>
        ErrorsOnly,

        /// <summary>
        /// Write logs of level Info, Warning, and Error. Drops Debug.
        /// </summary>
        Normal,

        /// <summary>
        /// Write all logs including Debug, Info, Warning, and Error.
        /// </summary>
        Verbose
    }
}
