using System.IO;

namespace OneSearch.DataMapper.Storage.Compress
{
    public interface IStorageCompressor
    {
        string ContentEncodingName { get; }
        byte[] Compress(byte[] bytes);
        Stream GetDecompressStream(byte[] bytes);
    }
}