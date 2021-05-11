using System;
using System.Runtime.Serialization;

namespace OneSearch.DataMapper.Exceptions
{
    public class StorageException : DataMapperException
    {
        public StorageException()
        {
        }

        protected StorageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public StorageException(string message) : base(message)
        {
        }

        public StorageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}