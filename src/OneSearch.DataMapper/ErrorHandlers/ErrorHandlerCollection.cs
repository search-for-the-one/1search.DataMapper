using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneSearch.DataMapper.ErrorHandlers
{
    internal class ErrorHandlerCollection : IErrorHandlerCollection
    {
        private readonly IEnumerable<IErrorHandler> errorHandlers;

        public ErrorHandlerCollection(IEnumerable<IErrorHandler> errorHandlers)
        {
            this.errorHandlers = errorHandlers;
        }

        public async Task Handle(string message, string errorMessage, Exception exception = null)
        {
            foreach (var handler in errorHandlers)
            {
                await handler.Handle(message, errorMessage, exception);
            }
        }
    }
}