using System.Collections.Generic;
using System.Linq;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Processor
{
    internal class LastOneWinsIdDeDuplicator : IIdDeDuplicator
    {
        public IEnumerable<StorageItem> DeDuplicate(IList<StorageItem> items)
        {
            return items.GroupBy(x => x.Id).Select(x => x.Last());
        }
    }
}