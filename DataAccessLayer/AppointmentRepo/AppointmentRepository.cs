using BarberShop.Domain;
using BarberShop.Infrastructure;
using Microsoft.AspNetCore.Mvc;
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
			return context.Appointments.Include(ap=>ap.Barber).Include(ap=>ap.Services).Where(predicate).AsQueryable();
		}

		public IQueryable<Result> GetByConditionGroupBy<GroupAttribute,Result>(Func<Appointment, bool> where, Func<Appointment, GroupAttribute> groupBy, Func<IGrouping<GroupAttribute, Appointment>, Result> select)
		{
			return context.Appointments
				.Include(ap => ap.Barber)
				.Where(where)
				.GroupBy(groupBy)
				.Select(select)
				.AsQueryable();
		}

		public Appointment GetById(int id)
		{
			return context.Appointments.Include(ap=>ap.Services).Single(ap=>ap.AppointmentId == id);

		}

		public void Update(Appointment t)
		{
			context.Update(t);
			context.Entry(t.IdentityUser).State = EntityState.Unchanged;
			context.SaveChanges();
		}
	}
}
