using BarberShop.Domain;
using BarberShop.Infrastructure;
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
			context.SaveChanges();
		}

		public List<Service> GetAll()
		{
			return context.Services.ToList();
		}

		public Service GetById(int id)
		{
			return context.Services.Single(b => b.ServiceId == id);
		}

		public void Update(Service t)
		{
			context.Update(t);
			context.SaveChanges();
		}
	}
}
