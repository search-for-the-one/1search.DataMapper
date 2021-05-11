using System.Collections.Generic;

namespace OneSearch.DataMapper.Messaging
{
    public interface IMessages
    {
        IEnumerable<string> Messages { get; }
        void Ack();
    }
}