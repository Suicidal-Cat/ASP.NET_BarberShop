using System.ComponentModel.DataAnnotations;

namespace BarberShopWeb.DTOs
{
	public class ResetPasswordDto
	{
		[Required]
		public string Token { get; set; }
		[Required]
		[RegularExpression("^[a-zA-Z0-9._%+\\-]+@[a-zA-Z0-9.\\-]+\\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address")]
		public string Email { get; set; }
		[Required]
		[StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
		public string NewPassword { get; set; }
    }
}
