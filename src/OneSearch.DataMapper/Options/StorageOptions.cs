using System.Collections.Generic;
using Neo.Extensions.DependencyInjection;
using OneSearch.DataMapper.Storage.Compress;

namespace OneSearch.DataMapper.Options
{
    public class StorageOptions : IConfig
    {
        public string Storage { get; set; }
        public string StorageCompressor { get; set; } = nameof(BrotliCompressor);
        public string Url { get; set; }
        public string LocalFileStorageFolder { get; set; }
        public int Retries { get; set; } = 5;
        public int WaitBeforeRetryMilliseconds { get; set; } = 1000;
        public IList<int> RetryHttpStatusCodes { get; set; } = new List<int>();
        public IList<int> IgnoredHttpStatusCodes { get; set; } = new List<int>();
        public bool SilentOnError { get; set; }
    }
}