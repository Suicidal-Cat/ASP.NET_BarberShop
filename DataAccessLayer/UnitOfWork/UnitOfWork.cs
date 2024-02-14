using BarberShop.Infrastructure;
using DataAccessLayer.AppointmentRepo;
using DataAccessLayer.BarberRepo;
using DataAccessLayer.ServiceCategoryRepo;
using DataAccessLayer.ServiceRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly BarberShopDbContext context;
		public IBarberRepository BarberRepository { get; set; }
		public IServiceRepository ServiceRepository { get; set; }
        public IServiceCategoryRepository ServiceCategoryRepository { get; set; }
        public IAppointmentRepository AppointmentRepository { get; set; }
        public UnitOfWork(BarberShopDbContext context)
        {
            this.context = context;
			BarberRepository=new BarberRepository(context);
			ServiceRepository=new ServiceRepository(context);
            ServiceCategoryRepository=new ServiceCategoryRepository(context);
			AppointmentRepository=new AppointmentRepository(context);

		}

        public void SaveChanges()
		{
			context.SaveChanges();
		}
	}
}
