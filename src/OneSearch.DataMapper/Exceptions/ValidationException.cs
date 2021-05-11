using System;
using System.Runtime.Serialization;

namespace OneSearch.DataMapper.Exceptions
{
    public class ValidationException : DataMapperException
    {
        public ValidationException()
        {
        }

        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}