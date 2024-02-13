using DataAccessLayer.BarberRepo;
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
		public void SaveChanges();
	}
}
