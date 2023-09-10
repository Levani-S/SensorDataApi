using System.Runtime.Serialization;

namespace SensorDataApi.Exceptions
{
    [Serializable]
    public class NotFoundException : CustomException
    {
        public NotFoundException() : base()
        {

        }

        public NotFoundException(string message) : base(message)
        {

        }
        protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

    }
}
