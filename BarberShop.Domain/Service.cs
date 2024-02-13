using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Domain
{
	public class Service
	{
        public int ServiceId { get; set; }
		[Required]
        public string Name { get; set; } = null!;
		public int Price { get; set; }
		public int Duration { get; set; }
        public ServiceCategory ServiceCategories { get; set; } = null!;
        public List<Appointment>? Appointments { get; set; }

    }
}
