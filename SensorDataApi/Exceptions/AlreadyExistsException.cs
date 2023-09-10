using System.Runtime.Serialization;

namespace SensorDataApi.Exceptions
{
    [Serializable]
    public class AlreadyExistsException : CustomException
    {
        public AlreadyExistsException() : base()
        {

        }

        public AlreadyExistsException(string message) : base(message)
        {

        }
        protected AlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

    }
}
