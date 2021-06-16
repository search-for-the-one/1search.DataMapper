using Neo.Extensions.DependencyInjection;

namespace OneSearch.DataMapper.Options
{
    public class LocalFileStorageOptions : IConfig
    {
        public string StorageFolder { get; set; }
    }
}