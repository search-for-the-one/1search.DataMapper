using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OneSearch.DataMapper.ErrorHandlers
{
    public class LoggingErrorHandler : IErrorHandler
    {
        private readonly ILogger<LoggingErrorHandler> logger;

        public LoggingErrorHandler(ILogger<LoggingErrorHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(string message, string errorMessage, Exception exception = null)
        {
            logger.LogError(exception, "{ErrorMessage} {Message}", errorMessage, message);

            return Task.CompletedTask;
        }
    }
}