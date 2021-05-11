namespace OneSearch.DataMapper.Extensions.ServiceCollections
{
    public interface IMapServiceCollection
    {
        IMapWithServiceCollection Map<TItem, TMappedItem>();
    }
}