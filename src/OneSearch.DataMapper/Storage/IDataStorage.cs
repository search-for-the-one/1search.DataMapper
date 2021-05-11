using System.Collections.Generic;
using System.Threading.Tasks;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Storage
{
    public interface IDataStorage
    {
        Task Save(IList<StorageItem> items);
    }
}