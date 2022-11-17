using System.ComponentModel;

namespace CarOrder.Models
{
    public class Car
    {
        public int Id { get; set; }

        public string Model { get; set; } = null!;

        public string LicensePlate { get; set; } = null!;
    }
}
