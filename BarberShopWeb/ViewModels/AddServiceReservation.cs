using BarberShop.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Reflection.Metadata.Ecma335;

namespace BarberShopWeb.ViewModels
{
	public class AddServiceReservation
	{
		[ValidateNever]
        public Service Service { get; set; }
		[ValidateNever]
		public bool IsChecked { get; set; } = false;
    }
    public class ListAddServicesReservation{
		[ValidateNever]
		public List<AddServiceReservation> Haircuts { get; set; }=new List<AddServiceReservation>();
		[ValidateNever]
		public List<AddServiceReservation> Beard { get; set; }=new List<AddServiceReservation>();
		[ValidateNever]
		public List<AddServiceReservation> Other { get; set; } = new List<AddServiceReservation>();
	}

}
