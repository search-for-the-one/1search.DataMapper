using System;
using System.Collections.Generic;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Extensions
{
    public static class BytesExtensions
    {
        public static IEnumerable<MultipartItem> ToMultipartItems(this byte[] bytes, int partSizeLimit)
        {
            short index = 0;
            while (index * partSizeLimit < bytes.Length)
            {
                yield return new MultipartItem
                {
                    Index = (index + 1) * partSizeLimit >= bytes.Length ? (short) ~index : index,
                    Data = bytes.AsSpan().Slice(index * partSizeLimit,
                        Math.Min(bytes.Length - index * partSizeLimit, partSizeLimit)).ToArray()
                };
                index++;
            }
        }
    }
}