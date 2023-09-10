namespace SensorDataApi.Models
{
    public class LightSensor
    {
        public Guid Id { get; set; }
        public double Illuminance { get; set; }
        public long Time { get; set; }
        public long DeviceId { get; set; }
    }
}
