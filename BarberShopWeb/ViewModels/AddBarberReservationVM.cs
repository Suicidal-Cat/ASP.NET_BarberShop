using BarberShop.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BarberShopWeb.ViewModels
{
	public class BarberCheckBox
	{
		[ValidateNever]
		public Barber Barber { get; set; }
		[ValidateNever]
		public bool IsChecked { get; set; } = false;

    }
	public class AddBarberReservationVM
	{
		[ValidateNever]
        public List<Service> Services { get; set; }

		[ValidateNever]
		public List<BarberCheckBox> Barbers { get; set; }
	}
}
