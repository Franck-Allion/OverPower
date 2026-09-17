namespace OverPower.Application.Ports.Logging
{
    /// <summary>
    /// A Unity-independent, application-level logging contract for expected outcomes, diagnostics, and metrics.
    /// </summary>
    public interface IGameLogger
    {
        void Debug(string category, string message);
        void Info(string category, string message);
        void Warning(string category, string message);
        void Error(string category, string message);
    }
}
