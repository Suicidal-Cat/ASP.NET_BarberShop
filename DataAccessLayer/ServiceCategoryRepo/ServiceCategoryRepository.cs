using BarberShop.Domain;
using BarberShop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ServiceCategoryRepo
{
    public class ServiceCategoryRepository : IServiceCategoryRepository
    {
        private readonly BarberShopDbContext context;

        public ServiceCategoryRepository(BarberShopDbContext context)
        {
            this.context = context;
        }
        public void Add(ServiceCategory t)
        {
            context.Add(t);
        }

        public void Delete(ServiceCategory t)
        {
            context.Remove(t);
            context.SaveChanges();
        }

        public List<ServiceCategory> GetAll()
        {
            return context.ServiceCategories.ToList();
        }

        public ServiceCategory GetById(int id)
        {
            return context.ServiceCategories.Single(b => b.Id == id);
        }

        public void Update(ServiceCategory t)
        {
            context.Update(t);
            context.SaveChanges();
        }
    }
}
