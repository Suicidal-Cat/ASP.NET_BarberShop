using BarberShop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Services.Interfaces
{
	public interface IAppointmentService
	{
		public void Add(Appointment appointment);
		public void Update(Appointment appointment);

		public void Delete(int id);
		public IEnumerable<Appointment> Appointments { get; }
		public Appointment Get(int id);

		public List<Appointment> SearchByDateBarber(string date,int idBarber);
		public Appointment? SearchByDateFirst(string date,string idUser);

		public List<Appointment> SearchByDate(string date);

		public IEnumerable<DateCountResult> GetAvaiableAppointments(int barberId, DateTime startDate, DateTime endDate);
	}
}
