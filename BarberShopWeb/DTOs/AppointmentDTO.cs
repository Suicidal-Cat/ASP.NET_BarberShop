using BarberShop.Domain;
using Microsoft.AspNetCore.Identity;

namespace BarberShopWeb.DTOs
{
	public class AppointmentDTO
	{
		public int AppointmentId { get; set; }
		public DateTime Date { get; set; }
		public string StartTime { get; set; } = null!;
		public int AppDuration { get; set; }
		public int Price { get; set; }
		public bool IsCanceled { get; set; } = false;
		public Barber Barber { get; set; } = null!;
		public List<Service> Services { get; set; } = null!;
	}
}
