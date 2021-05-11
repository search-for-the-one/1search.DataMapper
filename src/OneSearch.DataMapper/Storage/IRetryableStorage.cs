using System.Net;

namespace OneSearch.DataMapper.Storage
{
    internal interface IRetryableStorage : IStorage
    {
        bool IsRetryable(HttpStatusCode statusCode);
    }
}