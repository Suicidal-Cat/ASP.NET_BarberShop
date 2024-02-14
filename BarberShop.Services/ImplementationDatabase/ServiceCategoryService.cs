using BarberShop.Domain;
using BarberShop.Services.Interfaces;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Services.ImplementationDatabase
{
    public class ServiceCategoryService:IServiceCategoryService
    {
        private readonly IUnitOfWork uow;
        public ServiceCategoryService(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public IEnumerable<ServiceCategory> ServiceCategories => uow.ServiceCategoryRepository.GetAll();
    }
}
