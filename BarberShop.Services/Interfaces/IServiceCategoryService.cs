using BarberShop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Services.Interfaces
{
    public interface IServiceCategoryService
    {
        public IEnumerable<ServiceCategory> ServiceCategories { get; }
    }
}
