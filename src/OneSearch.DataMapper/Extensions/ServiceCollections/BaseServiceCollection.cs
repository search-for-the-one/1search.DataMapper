using Microsoft.Extensions.DependencyInjection;

namespace OneSearch.DataMapper.Extensions.ServiceCollections
{
    internal abstract class BaseServiceCollection
    {
        protected BaseServiceCollection(IServiceCollection services)
        {
            Services = services;
        }

        protected IServiceCollection Services { get; }
    }
}