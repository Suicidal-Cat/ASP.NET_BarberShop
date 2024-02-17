	using BarberShop.Domain;
using BarberShop.Services.Interfaces;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Services.ImplementationDatabase
{
	public class AppointmentService : IAppointmentService
	{
		private readonly IUnitOfWork uow;

		public AppointmentService(IUnitOfWork uow)
        {
			this.uow = uow;
		}
        public IEnumerable<Appointment> Appointments => uow.AppointmentRepository.GetAll();

		public void Add(Appointment appointment)
		{
			uow.AppointmentRepository.Add(appointment);
			uow.SaveChanges();
		}

		public void Delete(int id)
		{
			uow.AppointmentRepository.Delete(new Appointment { AppointmentId = id });
		}

		public Appointment Get(int id)
		{
			return uow.AppointmentRepository.GetById(id);
		}

		public List<Appointment> SearchByDate(string date,int id)
		{
			Func<Appointment, bool> func = (ap => ap.Date.ToString("yyyy-MM-dd") == date && ap.Barber?.BarberId == id);
			return uow.AppointmentRepository.GetByCondition(ap=>ap.Date.ToString("yyyy-MM-dd")==date).ToList();
		}

		public void Update(Appointment appointment)
		{
			uow.AppointmentRepository.Update(appointment);
		}
	}
}
