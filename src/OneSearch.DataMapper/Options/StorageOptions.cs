using System.Collections.Generic;
using Neo.Extensions.DependencyInjection;

namespace OneSearch.DataMapper.Options
{
    public class StorageOptions : IConfig
    {
        public string Url { get; set; }
        public int Retries { get; set; } = 5;
        public int WaitBeforeRetryMilliseconds { get; set; } = 1000;
        public IList<int> RetryHttpStatusCodes { get; set; } = new List<int>();
        public IList<int> IgnoredHttpStatusCodes { get; set; } = new List<int>();
        public bool SilentOnError { get; set; }
    }
}