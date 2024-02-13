using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Domain
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        [Required]
        public DateTime MyProperty { get; set; }
        [Required]
        public string StartTime { get; set; } = null!;
        public string AppDuration { get; set; } = null!;
        public int Price { get; set; }
        public Barber? Barber { get; set; }
        [Required]
        public IdentityUser IdentityUser { get; set; } = null!;
        public List<Service>? Services { get; set; }
    }
}
