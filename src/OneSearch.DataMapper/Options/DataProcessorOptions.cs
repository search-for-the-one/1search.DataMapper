using Neo.Extensions.DependencyInjection;

namespace OneSearch.DataMapper.Options
{
    public class DataProcessorOptions : IConfig
    {
        public int ProducerConsumerQueueSize { get; set; } = 1;
        public int ProduceMessageWaitMilliseconds { get; set; } = 50;
        public int FetchMessagesWaitMilliseconds { get; set; } = 200;
    }
}