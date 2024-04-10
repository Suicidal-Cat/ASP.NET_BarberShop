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

		public List<Appointment> SearchByDate(string date)
		{
			Func<Appointment, bool> func = (ap => ap.Date.ToString("yyyy-MM-dd") == date);
			return uow.AppointmentRepository.GetByCondition(func).OrderBy(ap=>ap.StartTime).ToList();
		}

		public List<Appointment> SearchByDateBarber(string date,int idBarber)
		{
			Func<Appointment, bool> func = (ap => ap.Date.ToString("yyyy-MM-dd") == date && ap.Barber?.BarberId == idBarber);
			return uow.AppointmentRepository.GetByCondition(func).ToList();
		}

		public Appointment? SearchByDateFirst(string date,string idUser)
		{
			Func<Appointment, bool> func = (ap => string.Compare(ap.Date.ToString("yyyy-MM-dd"), date) >= 0 && ap.IdentityUserId == idUser);
			string time = DateTime.Now.ToString("HH:mm");
			return uow.AppointmentRepository.GetByCondition(func).OrderBy(ap=>ap.Date).FirstOrDefault(ap=> !(string.Compare(ap.StartTime, time) < 0 && ap.Date==DateTime.Now.Date) && ap.IsCanceled==false);
		}


		public void Update(Appointment appointment)
		{
			uow.AppointmentRepository.Update(appointment);
		}
	}
}
