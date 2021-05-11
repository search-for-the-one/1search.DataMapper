using System.Threading.Tasks;

namespace OneSearch.DataMapper.Validators
{
    internal class NullValidator : IMessageValidator
    {
        public Task<bool> IsValid(string message)
        {
            return Task.FromResult(true);
        }
    }
}