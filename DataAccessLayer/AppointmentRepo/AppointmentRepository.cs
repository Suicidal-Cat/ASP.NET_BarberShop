using BarberShop.Domain;
using BarberShop.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.AppointmentRepo
{
	public class AppointmentRepository : IAppointmentRepository
	{
		private readonly BarberShopDbContext context;

		public AppointmentRepository(BarberShopDbContext barberShopDbContext)
        {
			this.context = barberShopDbContext;
		}
        public void Add(Appointment t)
		{
			
			context.Add(t);
			context.Entry(t.IdentityUser).State = EntityState.Unchanged;
			context.Entry(t.Barber).State = EntityState.Unchanged;
			foreach(Service service in t.Services) context.Entry(service).State = EntityState.Unchanged;
		}

		public void Delete(Appointment t)
		{
			context.Remove(t);
			context.SaveChanges();
		}

		public List<Appointment> GetAll()
		{
			return context.Appointments.ToList();
		}

		public IQueryable<Appointment> GetByCondition(Func<Appointment, bool> predicate)
		{
			return context.Appointments.Include(ap=>ap.Barber).Where(predicate).AsQueryable();
		}

		public Appointment GetById(int id)
		{
			return context.Appointments.Single(ap=>ap.AppointmentId == id);

		}

		public void Update(Appointment t)
		{
			context.Update(t);
			context.SaveChanges();
		}
	}
}
