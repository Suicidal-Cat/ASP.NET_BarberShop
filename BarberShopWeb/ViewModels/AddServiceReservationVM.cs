using BarberShop.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Reflection.Metadata.Ecma335;

namespace BarberShopWeb.ViewModels
{
	public class AddServiceReservationVM
	{
		[ValidateNever]
        public Service Service { get; set; }
		[ValidateNever]
		public bool IsChecked { get; set; } = false;
    }
    public class ListAddServicesReservationVM{
		[ValidateNever]
		public List<AddServiceReservationVM> Haircuts { get; set; }=new List<AddServiceReservationVM>();
		[ValidateNever]
		public List<AddServiceReservationVM> Beard { get; set; }=new List<AddServiceReservationVM>();
		[ValidateNever]
		public List<AddServiceReservationVM> Other { get; set; } = new List<AddServiceReservationVM>();
	}

}
