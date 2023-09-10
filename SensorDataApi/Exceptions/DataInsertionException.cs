using System.Runtime.Serialization;

namespace SensorDataApi.Exceptions
{
    [Serializable]
    public class DataInsertionException : CustomException
    {
        public DataInsertionException() : base()
        {

        }

        public DataInsertionException(string message) : base(message)
        {

        }
        protected DataInsertionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

    }
}
