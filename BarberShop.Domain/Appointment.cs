using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Domain
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string StartTime { get; set; } = null!;
        public int AppDuration { get; set; }
        public int Price { get; set; }
        public Barber Barber { get; set; } = null!;
        [Required]
        public IdentityUser IdentityUser { get; set; } = null!;
        public string IdentityUserId { get; set; } = null!;
        public List<Service> Services { get; set; } = null!;
    }
}
