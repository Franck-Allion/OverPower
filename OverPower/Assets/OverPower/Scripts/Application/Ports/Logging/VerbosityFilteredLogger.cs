using System;

namespace OverPower.Application.Ports.Logging
{
    /// <summary>
    /// A decorator that filters log messages based on a specified <see cref="GameLogVerbosity"/> level.
    /// </summary>
    public sealed class VerbosityFilteredLogger : IGameLogger
    {
        private readonly IGameLogger _inner;
        private readonly GameLogVerbosity _verbosity;

        public VerbosityFilteredLogger(IGameLogger inner, GameLogVerbosity verbosity)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _verbosity = verbosity;
        }

        public void Debug(string category, string message)
        {
            if (_verbosity == GameLogVerbosity.Verbose)
            {
                _inner.Debug(category, message);
            }
        }

        public void Info(string category, string message)
        {
            if (_verbosity == GameLogVerbosity.Normal || _verbosity == GameLogVerbosity.Verbose)
            {
                _inner.Info(category, message);
            }
        }

        public void Warning(string category, string message)
        {
            if (_verbosity == GameLogVerbosity.Normal || _verbosity == GameLogVerbosity.Verbose)
            {
                _inner.Warning(category, message);
            }
        }

        public void Error(string category, string message)
        {
            // Error is always logged regardless of the verbosity setting.
            _inner.Error(category, message);
        }
    }
}
