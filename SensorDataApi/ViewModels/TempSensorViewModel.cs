namespace SensorDataApi.ViewModels
{
    public class TempSensorViewModel
    {
        public Guid Id { get; set; }
        public double Temperature { get; set; }
        public long Time { get; set; }
        public long DeviceId { get; set; }
    }
}
