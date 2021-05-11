using System.Threading.Tasks;

namespace OneSearch.DataMapper.Validators
{
    public interface IMessageValidator
    {
        Task<bool> IsValid(string message);
    }
}