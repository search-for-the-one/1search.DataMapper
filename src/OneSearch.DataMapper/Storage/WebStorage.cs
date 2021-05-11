using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using OneSearch.DataMapper.Options;
using OneSearch.DataMapper.Storage.Compress;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Storage
{
    internal class WebStorage : IStorage
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly StorageOptions options;
        private readonly IStorageCompressor storageCompressor;

        public WebStorage(StorageOptions options, IHttpClientFactory httpClientFactory,
            IStorageCompressor storageCompressor)
        {
            this.options = options;
            this.httpClientFactory = httpClientFactory;
            this.storageCompressor = storageCompressor;
        }

        public async Task<(HttpStatusCode statusCode, string response)> Save(MultipartItem multipartItem)
        {
            var httpClient = httpClientFactory.CreateClient();
            var uri = new Uri(options.Url);
            var httpResponseMessage = await httpClient.PutAsync(uri, ToByteArrayContent(multipartItem));

            return (httpResponseMessage.StatusCode,
                httpResponseMessage.IsSuccessStatusCode ? await httpResponseMessage.Content.ReadAsStringAsync() : null);
        }

        private ByteArrayContent ToByteArrayContent(MultipartItem item)
        {
            var content = new ByteArrayContent(item.ToBytes());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Headers.ContentEncoding.Add(storageCompressor.ContentEncodingName);
            return content;
        }
    }
}