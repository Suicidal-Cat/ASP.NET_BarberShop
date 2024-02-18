using BarberShop.Domain;
using BarberShop.Services.Interfaces;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BarberShop.Services.ImplementationDatabase
{
	public class ServiceService : IServiceService
	{
		private readonly IUnitOfWork uow;
		public ServiceService(IUnitOfWork uow)
		{
			this.uow = uow;
		}
		public IEnumerable<Service> Services => uow.ServiceRepository.GetAll();

		public void Add(Service service)
		{
			uow.ServiceRepository.Add(service);
			uow.SaveChanges();
		}

		public void Delete(int id)
		{
			uow.ServiceRepository.Delete(new Service { ServiceId = id });
		}

		public Service Get(int id)
		{
			return uow.ServiceRepository.GetById(id);
		}

        public IEnumerable<Service> SearchByName(string name)
        {
            return uow.ServiceRepository.GetByCondition(s=>s.Name.Contains(name));
        }

        public void Update(Service service)
		{
			uow.ServiceRepository.Update(service);
		}
	}
}
