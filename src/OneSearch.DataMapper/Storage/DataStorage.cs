using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OneSearch.DataMapper.Exceptions;
using OneSearch.DataMapper.Extensions;
using OneSearch.DataMapper.Options;
using OneSearch.DataMapper.Storage.Compress;
using OneSearch.DataMapper.Storage.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace OneSearch.DataMapper.Storage
{
    internal class DataStorage : IDataStorage
    {
        private const int MaxPayloadSize = 1024 * 1024 - 2; // Lambda payload size limit 1MB under ALB
        private const int Tries = 3;
        private const int RetrySleepMilliseconds = 200;
        private const HttpStatusCode PreconditionRequired = HttpStatusCode.PreconditionRequired;

        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true
        };

        private readonly ILogger<DataStorage> logger;
        private readonly StorageOptions options;
        private readonly IRetryableStorage storage;
        private readonly IStorageCompressor storageCompressor;

        public DataStorage(ILogger<DataStorage> logger, StorageOptions options,
            IRetryableStorage storage, IStorageCompressor storageCompressor)
        {
            this.logger = logger;
            this.storage = storage;
            this.storageCompressor = storageCompressor;
            this.options = options;
        }

        public async Task Save(IList<StorageItem> items)
        {
            if (!(items?.Any() ?? false))
                return;

            var bytes = storageCompressor.Compress(JsonSerializer.SerializeToUtf8Bytes(items, jsonSerializerOptions));
            var multipartItems = bytes.ToMultipartItems(MaxPayloadSize).ToList();

            var tries = Tries;
            var statusCode = HttpStatusCode.OK;
            string response = null;
            while (tries-- > 0)
            {
                (statusCode, response) = await TrySave(multipartItems);
                if (statusCode != PreconditionRequired)
                    break;
                await Task.Delay(RetrySleepMilliseconds);
            }

            CheckResponse(statusCode, response);
            logger.LogInformation("Update {Ads} successfully", string.Join(" ", items.Select(x => x.Id)));
        }

        private async Task<(HttpStatusCode statusCode, string response)> TrySave(
            IEnumerable<MultipartItem> multipartItems)
        {
            var statusCode = HttpStatusCode.OK;
            string response = null;
            foreach (var multipartItem in multipartItems)
            {
                (statusCode, response) = await TrySave(multipartItem);
                if (statusCode != HttpStatusCode.OK)
                    break;
            }

            return (statusCode, response);
        }

        private async Task<(HttpStatusCode statusCode, string response)> TrySave(MultipartItem multipartItem)
        {
            try
            {
                return await storage.Save(multipartItem);
            }
            catch (Exception exception)
            {
                logger.LogError($"Failed to forward MultipartItem {exception}");
                return (HttpStatusCode.InternalServerError, null);
            }
        }

        private void CheckResponse(HttpStatusCode statusCode, string response)
        {
            if (statusCode != HttpStatusCode.OK)
            {
                OnFail((int)statusCode, response);
                return;
            }

            var responseItems = JsonConvert.DeserializeObject<IEnumerable<StorageResponseItem>>(response);
            foreach (var responseItem in responseItems)
            {
                if (responseItem.StatusCode.Equals((int)HttpStatusCode.OK) ||
                    options.IgnoredHttpStatusCodes.Contains(responseItem.StatusCode))
                    continue;

                logger.LogError($"Got {responseItem.StatusCode} {responseItem.Message} from web storage");
                OnFail(responseItem.StatusCode, responseItem.Message);
            }
        }

        private void OnFail(int statusCode, string response)
        {
            if (!options.SilentOnError)
                throw new StorageException(
                    $"Failed to forward items to web storage with {nameof(HttpStatusCode)}: {statusCode}. Response: {response}");
        }
    }
}