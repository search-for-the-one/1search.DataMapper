using NUnit.Framework;
using OneSearch.DataMapper.Storage;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Tests.Storage
{
    public class StorageExtensionsTests
    {
        [Test]
        public void ToBytes()
        {
            var item = new MultipartItem
            {
                Index = 258,
                Data = new byte[] {0, 1, 0}
            };

            var bytes = item.ToBytes();

            CollectionAssert.AreEqual(new byte[] {2, 1, 0, 1, 0}, bytes);
        }
    }
}