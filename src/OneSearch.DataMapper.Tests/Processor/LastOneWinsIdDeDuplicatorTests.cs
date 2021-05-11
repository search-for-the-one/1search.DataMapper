using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OneSearch.DataMapper.Processor;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Tests.Processor
{
    internal class LastOneWinsIdDeDuplicatorTests
    {
        [Test]
        public void DeDuplicate()
        {
            var item2 = new StorageItem("2", "data2");
            var item3 = new StorageItem("3", "data3");
            var item1 = new StorageItem("1", "data1");

            var list = new List<StorageItem>
            {
                new StorageItem("2", ""),
                item2,
                new StorageItem("1", ""),
                new StorageItem("1", ""),
                item3,
                new StorageItem("1", ""),
                new StorageItem("1", ""),
                item1
            };

            var distinctList = new LastOneWinsIdDeDuplicator().DeDuplicate(list).ToList();

            Assert.AreEqual(3, distinctList.Count);
            Assert.AreEqual(item2.Data, distinctList.FirstOrDefault(x => x.Id == item2.Id)?.Data);
            Assert.AreEqual(item3.Data, distinctList.FirstOrDefault(x => x.Id == item3.Id)?.Data);
            Assert.AreEqual(item1.Data, distinctList.FirstOrDefault(x => x.Id == item1.Id)?.Data);
        }
    }
}