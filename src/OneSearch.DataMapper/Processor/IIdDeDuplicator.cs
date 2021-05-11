using System.Collections.Generic;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Processor
{
    internal interface IIdDeDuplicator
    {
        IEnumerable<StorageItem> DeDuplicate(IList<StorageItem> items);
    }
}