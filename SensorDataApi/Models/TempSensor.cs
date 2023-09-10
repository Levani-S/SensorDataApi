namespace SensorDataApi.Models
{
    public class TempSensor
    {
        public Guid Id { get; set; }
        public double Temperature { get; set; }
        public long Time { get; set; }
        public long DeviceId { get; set; }
    }
}
