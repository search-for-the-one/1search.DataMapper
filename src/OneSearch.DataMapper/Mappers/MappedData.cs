namespace OneSearch.DataMapper.Mappers
{
    public class MappedData<TData>
    {
        public MappedData(ActionType actionType, string id, TData data)
        {
            Id = id;
            Data = data;
            ActionType = actionType;
        }

        public string Id { get; set; }
        public TData Data { get; set; }
        public ActionType ActionType { get; set; }
    }
}