using System;
using System.Runtime.Serialization;

namespace OneSearch.DataMapper.Exceptions
{
    public abstract class DataMapperException : Exception
    {
        protected DataMapperException()
        {
        }

        protected DataMapperException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected DataMapperException(string message) : base(message)
        {
        }

        protected DataMapperException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}