using BarberShop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Services.Interfaces
{
	public interface IServiceService
	{
		public void Add(Service service);
		public void Update(Service service);

		public void Delete(int id);
		public IEnumerable<Service> Services { get; }
		public Service Get(int id);
	}
}
