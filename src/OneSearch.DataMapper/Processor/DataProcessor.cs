using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using OneSearch.DataMapper.Messaging;
using OneSearch.DataMapper.Options;
using OneSearch.DataMapper.Storage;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Processor
{
    internal class DataProcessor : IDataProcessor
    {
        private readonly IMessageProcessor messageProcessor;
        private readonly DataProcessorOptions options;

        private readonly BlockingCollection<(IMessages QueueMessages, List<StorageItem> MappedItems)>
            producerConsumerQueue;

        private readonly IQueue queue;
        private readonly IDataStorage storage;

        public DataProcessor(IQueue queue, IMessageProcessor messageProcessor, IDataStorage storage,
            DataProcessorOptions options)
        {
            this.queue = queue;
            this.messageProcessor = messageProcessor;
            this.storage = storage;
            this.options = options;
            producerConsumerQueue =
                new BlockingCollection<(IMessages QueueMessages, List<StorageItem> MappedItems)>(
                    options.ProducerConsumerQueueSize);
        }

        public async Task Process()
        {
            var res = await Task.WhenAny(Task.Run(async () => await ProduceMessages()),
                Task.Run(async () => await ConsumeMessages()));
            throw res.Exception?.InnerException ?? new Exception("Data mapper stopped unexpectedly");
        }

        private async Task ProduceMessages()
        {
            while (true)
            {
                var queueMessages = await queue.GetMessages();
                var mappedItems = await messageProcessor.Process(queueMessages.Messages);
                while (!producerConsumerQueue.TryAdd((queueMessages, mappedItems)))
                    await Task.Delay(options.ProduceMessageWaitMilliseconds);
            }

            // ReSharper disable once FunctionNeverReturns
        }

        private async Task ConsumeMessages()
        {
            while (true)
            {
                if (producerConsumerQueue.TryTake(out var item))
                {
                    await storage.Save(item.MappedItems);
                    item.QueueMessages.Ack();
                }
                else
                {
                    await Task.Delay(options.FetchMessagesWaitMilliseconds);
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }
}