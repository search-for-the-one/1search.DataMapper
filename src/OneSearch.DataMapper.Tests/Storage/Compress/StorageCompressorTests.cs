using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using OneSearch.DataMapper.Storage.Compress;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Tests.Storage.Compress
{
    public class StorageCompressorTests
    {
        private readonly IStorageCompressor[] storageCompressors = {new BrotliCompressor(), new GZipCompressor()};

        [Test]
        public void TestCompressAndDecompress()
        {
            var items = new List<StorageItem>
            {
                new StorageItem("123", "data1"),
                new StorageItem("453", "data2")
            };

            foreach (var compressor in storageCompressors)
            {
                var bytes = compressor.Compress(JsonSerializer.SerializeToUtf8Bytes(items));
                using var stream = compressor.GetDecompressStream(bytes);
                var decompressedItems = JsonSerializer.DeserializeAsync<List<StorageItem>>(stream).Result;
                CollectionAssert.AreEqual(items.Select(x => x.Id), decompressedItems.Select(x => x.Id));
                CollectionAssert.AreEqual(items.Select(x => x.Data), decompressedItems.Select(x => x.Data));
            }
        }
    }
}