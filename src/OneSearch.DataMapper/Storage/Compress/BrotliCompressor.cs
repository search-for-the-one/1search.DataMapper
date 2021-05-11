using System.IO;
using System.IO.Compression;

namespace OneSearch.DataMapper.Storage.Compress
{
    public class BrotliCompressor : IStorageCompressor
    {
        public string ContentEncodingName => "br";

        public byte[] Compress(byte[] bytes)
        {
            using var input = new MemoryStream(bytes);
            using var output = new MemoryStream();
            using (var zip = new BrotliStream(output, (CompressionLevel) 5))
            {
                input.CopyTo(zip);
            }

            return output.ToArray();
        }

        public Stream GetDecompressStream(byte[] bytes)
        {
            return new BrotliStream(new MemoryStream(bytes), CompressionMode.Decompress);
        }
    }
}