using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neo.ConsoleApp.DependencyInjection;
using OneSearch.DataMapper.Processor;

namespace OneSearch.DataMapper
{
    public class DataMapperApp : IConsoleApp<int>
    {
        private readonly ILogger<DataMapperApp> logger;
        private readonly IDataProcessor processor;

        public DataMapperApp(
            IDataProcessor processor,
            ILogger<DataMapperApp> logger)
        {
            this.processor = processor;
            this.logger = logger;
        }

        public async Task<int> Run()
        {
            try
            {
                await processor.Process();
                return 0;
            }
            catch (Exception exception)
            {
                logger.LogCritical($"Unexpected error: {exception}");
                return 1;
            }
        }
    }
}