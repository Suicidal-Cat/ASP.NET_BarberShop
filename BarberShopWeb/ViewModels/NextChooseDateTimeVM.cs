using BarberShop.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;

namespace BarberShopWeb.ViewModels
{
	public class NextChooseDateTimeVM
	{
		[ValidateNever]
        public Appointment Appointment { get; set; }

    }
}
