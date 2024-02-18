using BarberShop.Domain;
using BarberShop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BarberRepo
{
	internal class BarberRepository : IBarberRepository
	{
		private readonly BarberShopDbContext context;
        public BarberRepository(BarberShopDbContext context)
        {
            this.context = context;
        }
        public void Add(Barber t)
		{
			context.Add(t);
		}

		public void Delete(Barber t)
		{
			context.Remove(t);
			context.SaveChanges();
		}

		public List<Barber> GetAll()
		{
			return context.Barbers.ToList();
		}

		public IQueryable<Barber> GetByCondition(Func<Barber, bool> predicate)
		{
			return context.Barbers.Where(predicate).AsQueryable();
		}

		public Barber GetById(int id)
		{
			return context.Barbers.Single(b => b.BarberId == id);
		}

		public void Update(Barber t)
		{
			context.Update(t);
			context.SaveChanges();
		}
	}
}
