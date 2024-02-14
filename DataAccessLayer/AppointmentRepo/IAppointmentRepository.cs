using BarberShop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.AppointmentRepo
{
	public interface IAppointmentRepository:IRepository<Appointment>
	{
	}
}
