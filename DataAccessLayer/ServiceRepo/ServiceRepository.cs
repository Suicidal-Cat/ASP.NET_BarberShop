using BarberShop.Domain;
using BarberShop.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ServiceRepo
{
	internal class ServiceRepository : IServiceRepository
	{
		private readonly BarberShopDbContext context;

		public ServiceRepository(BarberShopDbContext context)
        {
			this.context = context;
		}
        public void Add(Service t)
		{
			context.Add(t);
		}

		public void Delete(Service t)
		{
			context.Remove(t);
			try
			{
				context.SaveChanges();
			}catch (Exception ex) { }
			
		}

		public List<Service> GetAll()
		{
			return context.Services.Include(s=>s.ServiceCategory).ToList();
		}

		public IQueryable<Service> GetByCondition(Func<Service, bool> predicate)
		{
            return context.Services.Include(s=>s.ServiceCategory).Where(predicate).AsQueryable();
        }

		public Service GetById(int id)
		{
			return context.Services.Include(s => s.ServiceCategory).Single(b => b.ServiceId == id);
		}

		public void Update(Service t)
		{
			context.Update(t);
			context.SaveChanges();
		}
	}
}
