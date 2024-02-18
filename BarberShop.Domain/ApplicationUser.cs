using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Domain
{
	public class ApplicationUser:IdentityUser
	{
		[Required]
		[MinLength(2, ErrorMessage = "Name must be at least 2 characters long.")]
		public string FirstName { get; set; } = null!;
		[Required]
		[MinLength(2, ErrorMessage = "Last name must be at least 2 characters long.")]
		public string LastName { get; set; }= null!;
    }
}
