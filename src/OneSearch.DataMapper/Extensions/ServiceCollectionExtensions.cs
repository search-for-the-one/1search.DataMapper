using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neo.Extensions.DependencyInjection;
using OneSearch.DataMapper.ErrorHandlers;
using OneSearch.DataMapper.Mappers;
using OneSearch.DataMapper.Options;
using OneSearch.DataMapper.Processor;
using OneSearch.DataMapper.Storage;
using OneSearch.DataMapper.Storage.Compress;
using OneSearch.DataMapper.Validators;

namespace OneSearch.DataMapper.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataMapper<TMessageMapper>(this IServiceCollection services,
            IConfigurationRoot configuration) where TMessageMapper : class, IMessageMapper
        {
            services.AddHttpClient();

            services.AddSingleton<IDataProcessor, DataProcessor>();
            services.AddSingleton<IMessageProcessor, MessageProcessor>();
            services.AddSingleton<IErrorHandlerCollection, ErrorHandlerCollection>();
            services.AddSingleton<IErrorHandler, LoggingErrorHandler>();
            services.AddSingleton<IIdDeDuplicator, LastOneWinsIdDeDuplicator>();
            services.AddSingleton<IDataStorage, DataStorage>();
            services.AddSingleton<IRetryableStorage, RetryableStorage>();
            services.AddSingleton<IMessageValidator, NullValidator>();
            services.AddSingleton<IMessageMapper, TMessageMapper>();

            services.AddConfig<StorageOptions>(configuration);
            services.AddConfig<MessageProcessorOptions>(configuration);
            services.AddConfig<DataProcessorOptions>(configuration);
            services.AddConfig<FactoryOptions>(configuration);
            services.AddConfig<LocalFileStorageOptions>(configuration);

            services.AddSingletonFromFactory<IStorage>(factory =>
                factory.AddService<WebStorage>(nameof(WebStorage))
                    .AddService<LocalFileStorage>(nameof(LocalFileStorage))
                    .WithOption<FactoryOptions>(options => options.Storage));

            services.AddSingletonFromFactory<IStorageCompressor>(factory =>
                factory.AddService<GZipCompressor>()
                    .AddService<BrotliCompressor>()
                    .WithOption<FactoryOptions>(options => options.StorageCompressor));

            return services;
        }
    }
}