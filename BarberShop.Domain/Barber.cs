using System.ComponentModel.DataAnnotations;

namespace BarberShop.Domain
{
	public enum Status
	{
		Active,
		Retired,
		Prestao_sa_radom,
	}
	public class Barber
	{
        public int BarberId { get; set; }
		[Required]
		public string FirstName { get; set; } = null!;
		[Required]
		public string LastName { get; set; } = null!;
		[Required]
		public string PhoneNumber { get; set; }=null!;
		[Required]
        public Status Status { get; set; }
    }
}