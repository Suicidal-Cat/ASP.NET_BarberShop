using System.ComponentModel.DataAnnotations;

namespace BarberShopWeb.DTOs
{
	public class RegisterDto
	{
		[Required]
		[StringLength(20, MinimumLength = 3, ErrorMessage = "First name must have at least 3 characters")]
		public string FirstName { get; set; }
		[Required]
		[StringLength(20, MinimumLength = 3, ErrorMessage = "Last name must have at least 3 characters")]
		public string LastName { get; set; }
		[Required]
		[EmailAddress(ErrorMessage = "Enter valid email address")]
		public string Email { get; set; }
		[Required]
		[Phone(ErrorMessage ="Enter valid phone number")]
		public string PhoneNumber { get; set; }
		[Required]
		[StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
		public string Password { get; set; }

	}
}
