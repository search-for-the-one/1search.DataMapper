using System.IO;
using System.IO.Compression;

namespace OneSearch.DataMapper.Storage.Compress
{
    public class GZipCompressor : IStorageCompressor
    {
        public string ContentEncodingName => "gzip";

        public byte[] Compress(byte[] bytes)
        {
            using var input = new MemoryStream(bytes);
            using var output = new MemoryStream();
            using (var zip = new GZipStream(output, CompressionLevel.Fastest))
            {
                input.CopyTo(zip);
            }

            return output.ToArray();
        }

        public Stream GetDecompressStream(byte[] bytes)
        {
            return new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress);
        }
    }
}