using System.Runtime.Serialization;

namespace SensorDataApi.Exceptions
{
    [Serializable]
    public class DataSendException : CustomException
    {
        public DataSendException() : base()
        {

        }

        public DataSendException(string message) : base(message)
        {

        }
        protected DataSendException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

    }
}
