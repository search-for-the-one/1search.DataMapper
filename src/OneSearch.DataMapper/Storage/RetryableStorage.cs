using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OneSearch.DataMapper.Options;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Storage
{
    internal class RetryableStorage : IRetryableStorage
    {
        private readonly ILogger<RetryableStorage> logger;
        private readonly StorageOptions options;
        private readonly IStorage storage;

        public RetryableStorage(StorageOptions options, ILogger<RetryableStorage> logger,
            IStorage storage)
        {
            this.logger = logger;
            this.storage = storage;
            this.options = options;
        }

        public bool IsRetryable(HttpStatusCode statusCode)
        {
            return options.RetryHttpStatusCodes.Contains((int) statusCode);
        }

        public async Task<(HttpStatusCode statusCode, string response)> Save(MultipartItem multipartItem)
        {
            var response = await storage.Save(multipartItem);
            logger.LogInformation($"Response status code: {response.statusCode}");
            if (!IsRetryable(response.statusCode))
                return response;

            for (var i = 1; i <= options.Retries; i++)
            {
                await Task.Delay(options.WaitBeforeRetryMilliseconds);

                logger.LogInformation($"Forwarding batch items to storage with {i} retry");
                response = await storage.Save(multipartItem);
                logger.LogInformation($"Response status code: {response.statusCode}");
                if (!IsRetryable(response.statusCode))
                    break;
            }

            return response;
        }
    }
}