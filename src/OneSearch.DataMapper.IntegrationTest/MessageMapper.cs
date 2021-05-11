using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OneSearch.DataMapper.Mappers;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.IntegrationTest
{
    public class MessageMapper : IMessageMapper
    {
        public Task<IEnumerable<StorageItem>> Map(string message)
        {
            var id = new Random().Next().ToString();
            return Task.FromResult((IEnumerable<StorageItem>)
                new List<StorageItem> {new() {Id = id, Data = $"New flower {id}: {message}"}});
        }
    }
}