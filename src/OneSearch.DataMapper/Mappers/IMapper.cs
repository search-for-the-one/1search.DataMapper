using System.Threading.Tasks;

namespace OneSearch.DataMapper.Mappers
{
    public interface IMapper<in TItem, TMappedItem>
    {
        Task<MappedData<TMappedItem>> Map(string networkId, TItem item, ActionType actionType);
    }
}