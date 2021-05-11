using System;
using System.Runtime.Serialization;

namespace OneSearch.DataMapper.Exceptions
{
    public class FilterException : DataMapperException
    {
        public FilterException()
        {
        }

        protected FilterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public FilterException(string message) : base(message)
        {
        }

        public FilterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}