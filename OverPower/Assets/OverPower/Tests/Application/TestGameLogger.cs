using System.Collections.Generic;
using OverPower.Application.Ports.Logging;

namespace OverPower.Tests.Application
{
    /// <summary>
    /// A lightweight, in-memory log capturer intended for automated unit/integration tests.
    /// </summary>
    public sealed class TestGameLogger : IGameLogger
    {
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error
        }

        public readonly struct LogEntry
        {
            public LogLevel Level { get; }
            public string Category { get; }
            public string Message { get; }

            public LogEntry(LogLevel level, string category, string message)
            {
                Level = level;
                Category = category;
                Message = message;
            }
        }

        private readonly List<LogEntry> _entries = new List<LogEntry>();

        public IReadOnlyList<LogEntry> Entries => _entries;

        public void Debug(string category, string message) => _entries.Add(new LogEntry(LogLevel.Debug, category, message));
        public void Info(string category, string message) => _entries.Add(new LogEntry(LogLevel.Info, category, message));
        public void Warning(string category, string message) => _entries.Add(new LogEntry(LogLevel.Warning, category, message));
        public void Error(string category, string message) => _entries.Add(new LogEntry(LogLevel.Error, category, message));

        public void Clear() => _entries.Clear();
    }
}
