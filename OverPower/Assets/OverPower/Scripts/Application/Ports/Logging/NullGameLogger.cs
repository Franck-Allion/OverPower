namespace OverPower.Application.Ports.Logging
{
    /// <summary>
    /// A stateless, allocation-free no-op logging implementation of <see cref="IGameLogger"/>.
    /// </summary>
    public sealed class NullGameLogger : IGameLogger
    {
        private static readonly NullGameLogger _instance = new NullGameLogger();

        public static IGameLogger Instance => _instance;

        private NullGameLogger() { }

        public void Debug(string category, string message) { }
        public void Info(string category, string message) { }
        public void Warning(string category, string message) { }
        public void Error(string category, string message) { }
    }
}
