namespace OneSearch.DataMapper.Storage.Models
{
    public class MultipartItem
    {
        public int Index { get; set; } // Use one's complement to indicate end of multipart
        public byte[] Data { get; set; }
    }
}