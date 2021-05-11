# Data mapper

This is a C# client library to help you bridge data sources and 1search cluster.

<pre>

                                  ┌────────────────────────────────────────────────────────┐                                 
                                  │  Custom data mapper                                    │                                 
┌──────────────────────┐          │    ┌───────────┐                                       │                                 
│ DataSource           │          │    │ Mapper    │ ─ ─ ┐                                 │                                 
│                      │          │    └───────────┘           ┌────────────────────────┐  │           ┌────────────────────┐
│    -- RabbitMq       │          │    ┌───────────┐     │     │                        │  │           │                    │
│                      │─────────▶│    │ Validator │─ ─ ─ ─ ─ ─│  1search data mapper   │  │──────────▶│  1search service   │
│    -- LocalFiles     │          │    └───────────┘     │     │                        │  │           │                    │
│                      │          │    ┌───────────┐           └────────────────────────┘  │           └────────────────────┘
└──────────────────────┘          │    │ Filter    │ ─ ─ ┘                                 │                                 
                                  │    └───────────┘                                       │                                 
                                  └────────────────────────────────────────────────────────┘                                 
</pre>


The brief process is:

1. Fetch a configured number of messages from message channel

2. Transform message to fit your model

3. Upsert the transformed data to 1search cluster

In step 2 you expend the transformation by adding validations, filtering, and mapping in the process by implementing corresponding interfaces in your code.


---

## Quick start
This is a quick start example for a flower shop.


### 1. Install latest OneSearch.DataMapper 

`Install-Package OneSearch.DataMapper`


It will implicitly reference `Neo.ConsoleApp.DependencyInjection` which is a customized open-source dependecy injection framework.

This affects how to bootstrap the console application but original dotnet core dependency injections should remains same in most ways.

### 2. Implement interface IMessageMapper
This is the interface for mapping message from any message channel, like RabbitMQ or even local files, to your domain models.


Please implement the method `public Task<IEnumerable<StorageItem>> Map(string message)`
```
using System.Collections.Generic;
using System.Threading.Tasks;
using OneSearch.DataMapper.Mappers;
using OneSearch.DataMapper.Storage.Models;

namespace FlowerShopDataMapper
{
    public class FlowerMessageMapper : IMessageMapper
    {
        public Task<IEnumerable<StorageItem>> Map(string message)
        {
            // Here is your implementation
            throw new System.NotImplementedException();
        }
    }
}

```
### 2. Create Startup.cs
Instead of directly using the normal dotnet core `Startup` class, you need to inherit class `ConsoleAppStartup<int>`  and use the overrided `ConfigureServices` method from `Neo.ConsoleApp.DependencyInjection`. 


The `AddDataMapper` configure the service to add data mapper service and designate your implementation `FlowerMessageMapper` of `IMessageMapper`

```
using Microsoft.Extensions.DependencyInjection;
using Neo.ConsoleApp.DependencyInjection;
using OneSearch.DataMapper.Extensions;

namespace FlowerShopDataMapper
{
    public class Startup : ConsoleAppStartup<int>
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddDataMapper<FlowerMessageMapper>(Configuration);
        }
    }
}
```

### 3. Create Program.cs

```
internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        // DataMapperApp is one built in console app class to run the data mapping process
        return await new Startup().RunAsync<DataMapperApp>();
    }
}
```


### Settings
```
{
    "StorageOptions": {
        "Storage": "LocalFileStorage",
        "StorageCompressor": "BrotliCompressor",
        "Url": "http://your.1search.storage.endpoint/Storage/multipart",
        "LocalFileStorageFolder": "/path/to/your/storage"
    },
    "MessageProcessorOptions": {
        "ProcessInParallel": true
    },
    "DataProcessorOptions": {
        "ProducerConsumerQueueSize": 2,
        "ProduceMessageWaitMilliseconds": 50,
        "FetchMessagesWaitMilliseconds": 1000
    }
}
```