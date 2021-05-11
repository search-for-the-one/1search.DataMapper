using Microsoft.Extensions.DependencyInjection;

namespace OneSearch.DataMapper.Extensions.ServiceCollections
{
    public interface IMapWithServiceCollection
    {
        IServiceCollection With<TFilter, TMapper>();
    }
}