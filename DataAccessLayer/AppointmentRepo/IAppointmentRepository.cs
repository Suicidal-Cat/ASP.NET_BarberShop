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
		public IQueryable<Result> GetByConditionGroupBy<GroupAttribute, Result>(Func<Appointment, bool> where, Func<Appointment, GroupAttribute> groupBy, Func<IGrouping<GroupAttribute, Appointment>, Result> select);
	}
}
