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
	public interface IUnitOfWork
	{
        public IBarberRepository BarberRepository { get; set; }
		public IServiceRepository ServiceRepository { get; set; }
        public IServiceCategoryRepository ServiceCategoryRepository { get; set; }
		public IAppointmentRepository AppointmentRepository { get; set; }
        public void SaveChanges();
	}
}
