using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OneSearch.DataMapper.Mappers;
using OneSearch.DataMapper.Options;
using OneSearch.DataMapper.Storage.Models;
using OneSearch.DataMapper.Validators;

namespace OneSearch.DataMapper.Processor
{
    internal class MessageProcessor : IMessageProcessor
    {
        private readonly IIdDeDuplicator idDeDuplicator;
        private readonly IMessageMapper mapper;
        private readonly MessageProcessorOptions options;
        private readonly IMessageValidator validator;

        public MessageProcessor(IMessageValidator validator, IMessageMapper mapper, IIdDeDuplicator idDeDuplicator,
            MessageProcessorOptions options)
        {
            this.mapper = mapper;
            this.idDeDuplicator = idDeDuplicator;
            this.options = options;
            this.validator = validator;
        }

        private static int ThreadCount => Environment.ProcessorCount;

        public async Task<List<StorageItem>> Process(IEnumerable<string> messages)
        {
            return idDeDuplicator
                .DeDuplicate(options.ProcessInParallel
                    ? await GenerateStorageItemsInParallel(messages)
                    : await GenerateStorageItemsSynchronously(messages))
                .ToList();
        }

        private async Task<List<StorageItem>> GenerateStorageItemsInParallel(IEnumerable<string> messages)
        {
            var semaphore = new SemaphoreSlim(ThreadCount, ThreadCount);

            var tasks = messages.Select(message => Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    return await GenerateStorageItems(message);
                }
                finally
                {
                    semaphore.Release(1);
                }
            })).ToList();
            return (await Task.WhenAll(tasks)).SelectMany(x => x).ToList();
        }

        private async Task<List<StorageItem>> GenerateStorageItemsSynchronously(IEnumerable<string> messages)
        {
            var items = new List<StorageItem>();
            foreach (var message in messages)
            {
                items.AddRange(await GenerateStorageItems(message));
            }

            return items;
        }

        private async Task<IEnumerable<StorageItem>> GenerateStorageItems(string message)
        {
            if (!await validator.IsValid(message))
                return Enumerable.Empty<StorageItem>();

            return (await mapper.Map(message))?.Where(x => x != null) ?? Enumerable.Empty<StorageItem>();
        }
    }
}