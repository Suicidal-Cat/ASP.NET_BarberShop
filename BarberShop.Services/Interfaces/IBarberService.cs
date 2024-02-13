using BarberShop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Services.Interfaces
{
	public interface IBarberService
	{
		public void Add(Barber barber);
		public void Update(Barber barber);

		public void Delete(int id);
		public IEnumerable<Barber> Barbers { get; }
		public Barber Get(int id);
	}
}
