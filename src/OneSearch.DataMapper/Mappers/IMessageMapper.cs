using System.Collections.Generic;
using System.Threading.Tasks;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Mappers
{
    public interface IMessageMapper
    {
        Task<IEnumerable<StorageItem>> Map(string message);
    }
}