using System;
using System.Runtime.Serialization;

namespace OneSearch.DataMapper.Exceptions
{
    public class MapperException : DataMapperException
    {
        public MapperException()
        {
        }

        protected MapperException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MapperException(string message) : base(message)
        {
        }

        public MapperException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}