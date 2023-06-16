using Microsoft.Extensions.Logging;
using System;
using Cqrs.Common.Interfaces;

namespace Cqrs.Common.Logging
{
    public class LoggingService<TLoggerName> : ILoggingService<TLoggerName>
    {
        private readonly ILogger<TLoggerName> _logger;
        private readonly ICurrentStateService _currentState;

        public LoggingService(ILogger<TLoggerName> logger,
            ICurrentStateService currentState)
        {
            _logger = logger;
            _currentState = currentState;
        }

        public void LogCritical(Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Critical, exception, message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            Log(LogLevel.Debug, null, message, args);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Error, exception, message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            Log(LogLevel.Information, null, message, args);
        }

        public void LogTrace(string message, params object[] args)
        {
            Log(LogLevel.Trace, null, message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            Log(LogLevel.Warning, null, message, args);
        }

        private void Log(LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            _logger.Log(logLevel, exception, $"[{_currentState.TransactionId}] ({_currentState.User}) {message}", args);
        }
    }
}
