using Neo.Extensions.DependencyInjection;
using OneSearch.DataMapper.Storage.Compress;

namespace OneSearch.DataMapper.Options
{
    public class FactoryOptions : IConfig
    {
        public string Storage { get; set; }
        public string StorageCompressor { get; set; } = nameof(BrotliCompressor);
    }
}