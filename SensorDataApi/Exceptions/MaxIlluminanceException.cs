using System.Runtime.Serialization;

namespace SensorDataApi.Exceptions
{
    [Serializable]
    public class MaxIlluminanceException : CustomException
    {
        public MaxIlluminanceException() : base()
        {

        }

        public MaxIlluminanceException(string message) : base(message)
        {

        }
        protected MaxIlluminanceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

    }
}
