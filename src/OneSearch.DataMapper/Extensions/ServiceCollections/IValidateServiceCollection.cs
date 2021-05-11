namespace OneSearch.DataMapper.Extensions.ServiceCollections
{
    public interface IValidateServiceCollection
    {
        IMapServiceCollection ValidateWith(string typeIndicator, string jsonSchema = "{}");
    }
}