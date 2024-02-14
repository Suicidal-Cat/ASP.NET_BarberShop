
using BarberShop.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BarberShopWeb.ViewModels
{
	public class AddServiceVM
	{
        public Service Service { get; set; }

		[ValidateNever]
		public IEnumerable<SelectListItem>ServiceCategory { get; set; }

    }
}
