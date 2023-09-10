using System.Runtime.Serialization;

namespace SensorDataApi.Exceptions
{
    [Serializable]
    public class DuringSimulationException : CustomException
    {
        public DuringSimulationException() : base()
        {

        }

        public DuringSimulationException(string message) : base(message)
        {

        }
        protected DuringSimulationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

    }
}
