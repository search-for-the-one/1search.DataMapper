using System.Collections.Generic;
using System.Threading.Tasks;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Processor
{
    internal interface IMessageProcessor
    {
        Task<List<StorageItem>> Process(IEnumerable<string> messages);
    }
}