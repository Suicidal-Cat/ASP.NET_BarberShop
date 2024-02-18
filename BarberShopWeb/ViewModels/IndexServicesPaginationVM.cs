using BarberShop.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BarberShopWeb.ViewModels
{
    public class IndexServicesPaginationVM
    {
        public IEnumerable<Service> Services { get; set; }
        public bool Next { get; set; } = true;
        public bool Prev { get; set; } = false;
        public int CurrentPage { get; set; }
        public string Search { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ServiceCategory { get; set; }
        [ValidateNever]
        public int category { get; set; } = -1;
    }
}
