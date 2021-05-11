using System.Threading.Tasks;

namespace OneSearch.DataMapper.Messaging
{
    public interface IChannel
    {
        Task<IMessages> GetMessages();
    }
}