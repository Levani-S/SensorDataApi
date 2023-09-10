using Microsoft.EntityFrameworkCore;
using SensorDataApi.Models;


namespace SensorDataApi.Data
{
    public class SensorDataDbContext : DbContext
    {
        public SensorDataDbContext(DbContextOptions<SensorDataDbContext> options) : base(options)
        {
        }
        public DbSet<LightSensor> LightSensor { get; set; }
        public DbSet<TempSensor> TempSensor { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }

}
