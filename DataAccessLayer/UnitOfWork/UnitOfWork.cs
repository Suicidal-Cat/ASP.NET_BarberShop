using BarberShop.Infrastructure;
using DataAccessLayer.BarberRepo;
using DataAccessLayer.ServiceRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.UnitOfWork
{
	internal class UnitOfWork : IUnitOfWork
	{
		private readonly BarberShopDbContext context;
		public IBarberRepository BarberRepository { get; set; }
		public IServiceRepository ServiceRepository { get; set; }
        public UnitOfWork(BarberShopDbContext context)
        {
            this.context = context;
			BarberRepository=new BarberRepository(context);
			ServiceRepository=new ServiceRepository(context);
        }

        public void SaveChanges()
		{
			context.SaveChanges();
		}
	}
}
