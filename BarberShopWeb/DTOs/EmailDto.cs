using System.ComponentModel.DataAnnotations;

namespace BarberShopWeb.DTOs
{
	public class EmailDto
	{
		[Required]
        public string Email { get; set; }
        public string Subject { get; set; }
		public string Message { get; set; }
    }
}
