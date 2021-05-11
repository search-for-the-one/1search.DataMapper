using Neo.Extensions.DependencyInjection;

namespace OneSearch.DataMapper.Options
{
    public class MessageProcessorOptions : IConfig
    {
        public bool ProcessInParallel { get; set; }
    }
}