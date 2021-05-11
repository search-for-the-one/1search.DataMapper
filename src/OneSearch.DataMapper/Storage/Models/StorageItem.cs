namespace OneSearch.DataMapper.Storage.Models
{
    public class StorageItem
    {
        public StorageItem()
        {
        }

        public StorageItem(string id, string data)
        {
            Id = id;
            Data = data;
        }

        public string Id { get; set; }
        public string Data { get; set; }
    }
}