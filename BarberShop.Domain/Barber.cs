using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BarberShop.Domain
{
	public enum Status
	{
		Active,
		Retired,
		Left,
	}
	public class Barber
	{
        public int BarberId { get; set; }
		[Required]
		[MinLength(2, ErrorMessage = "Name must be at least 2 characters long.")]
		public string FirstName { get; set; } = null!;
		[Required]
		[MinLength(2, ErrorMessage = "Name must be at least 2 characters long.")]
		public string LastName { get; set; } = null!;
		[Required]
		[Phone]
		public string PhoneNumber { get; set; }=null!;
		[ValidateNever]
        public Status Status { get; set; }
		[ValidateNever]
		[DisplayName("Image")]
		public string ImageUrl { get; set; } = null!;
    }
}