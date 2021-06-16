using System.Threading.Tasks;

namespace OneSearch.DataMapper.Messaging
{
    public interface IQueue
    {
        Task<IMessages> GetMessages();
    }
}