using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BarberShop.Domain
{
	public class Service
	{
        public int ServiceId { get; set; }
		[Required]
		[MinLength(2, ErrorMessage = "Name must be at least 2 characters long.")]
		public string Name { get; set; } = null!;
		[Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0.")]
		public int Price { get; set; }
		[Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0.")]
		public int Duration { get; set; }
		[ValidateNever]
		public ServiceCategory ServiceCategory { get; set; } = null!;
		[JsonIgnore]
        public List<Appointment>? Appointments { get; set; }

    }
}
