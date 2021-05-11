using System;
using System.Runtime.InteropServices;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Storage
{
    public static class StorageExtensions
    {
        public static byte[] ToBytes(this MultipartItem item)
        {
            var multipart = new byte[sizeof(short) + item.Data.Length];
            SetMultipartIndex(multipart, (short) item.Index);
            item.Data.CopyTo(multipart, sizeof(short));
            return multipart;
        }

        private static void SetMultipartIndex(byte[] multipart, short multipartIndex)
        {
            ref var indexRef = ref MemoryMarshal.Cast<byte, short>(multipart.AsSpan().Slice(0, sizeof(short)))[0];
            indexRef = multipartIndex;
        }
    }
}