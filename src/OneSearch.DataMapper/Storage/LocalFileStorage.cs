using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OneSearch.DataMapper.Options;
using OneSearch.DataMapper.Storage.Compress;
using OneSearch.DataMapper.Storage.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace OneSearch.DataMapper.Storage
{
    internal class LocalFileStorage : IStorage
    {
        private readonly List<byte> data = new List<byte>();
        private readonly IStorageCompressor storageCompressor;
        private readonly LocalFileStorageOptions options;

        public LocalFileStorage(IStorageCompressor storageCompressor, LocalFileStorageOptions options)
        {
            this.storageCompressor = storageCompressor;
            this.options = options;
        }

        public async Task<(HttpStatusCode statusCode, string response)> Save(MultipartItem multipartItem)
        {
            data.AddRange(multipartItem.Data);
            if (multipartItem.Index >= 0)
                return (HttpStatusCode.OK, null);

            var items = await Deserialize();
            data.Clear();
            Output(items);

            return (HttpStatusCode.OK, JsonConvert.SerializeObject(System.Array.Empty<object>()));
        }

        private async Task<IEnumerable<StorageItem>> Deserialize()
        {
            await using var gunzipStream = storageCompressor.GetDecompressStream(data.ToArray());
            return await JsonSerializer.DeserializeAsync<IEnumerable<StorageItem>>(gunzipStream);
        }

        private void Output(IEnumerable<StorageItem> items)
        {
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.Id))
                {
                    var file = Path.Combine(options.StorageFolder, $"{item.Id}.json");
                    if (string.IsNullOrEmpty(item.Data))
                    {
                        if (File.Exists(file))
                            File.Delete(file);
                    }
                    else
                    {
                        File.WriteAllText(file, item.Data);
                    }
                }
            }
        }
    }
}