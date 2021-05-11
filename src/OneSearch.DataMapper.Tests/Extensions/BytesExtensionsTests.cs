using System;
using System.Linq;
using NUnit.Framework;
using OneSearch.DataMapper.Extensions;

namespace OneSearch.DataMapper.Tests.Extensions
{
    public class BytesExtensionsTests
    {
        [Test]
        public void ToMultipartItems()
        {
            const int maxLength = 10;
            var random = new Random(97);
            for (var len = 0; len < maxLength; len++)
            {
                var bytes = new byte[len];
                random.NextBytes(bytes);

                for (var size = 1; size <= len + 1; size++)
                {
                    var items = bytes.ToMultipartItems(size).ToList();
                    for (var i = 0; i < items.Count; i++)
                    {
                        Assert.AreEqual(i == items.Count - 1 ? ~i : i, items[i].Index);
                    }

                    Assert.AreEqual(bytes, items.SelectMany(x => x.Data));
                }
            }
        }
    }
}