using System;
using System.Threading.Tasks;

namespace OneSearch.DataMapper.ErrorHandlers
{
    public interface IErrorHandlerCollection
    {
        Task Handle(string message, string errorMessage, Exception exception = null);
    }
}