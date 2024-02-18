using BarberShop.Domain;

namespace BarberShopWeb.ViewModels
{
    public class IndexServicesPaginationVM
    {
        public IEnumerable<Service> Services { get; set; }
        public bool Next { get; set; } = true;
        public bool Prev { get; set; } = false;
        public int CurrentPage { get; set; }
        public string Search { get; set; }
    }
}
