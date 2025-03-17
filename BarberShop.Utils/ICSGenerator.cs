using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Utils
{
	public class ICSGenerator
	{
		public string GenerateIcs(DateTime startDate, DateTime endDate)
		{
			var sb = new StringBuilder();
			sb.AppendLine("BEGIN:VCALENDAR");
			sb.AppendLine("VERSION:2.0");
			sb.AppendLine("PRODID:dk20200125@student.fon.bg.ac.rs");
			sb.AppendLine("CALSCALE:GREGORIAN");
			sb.AppendLine("METHOD:PUBLISH");
			sb.AppendLine("BEGIN:VEVENT");
			sb.AppendLine("UID:" + Guid.NewGuid());
			sb.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMddTHHmmss"));
			sb.AppendLine("DTSTART:" + startDate.ToString("yyyyMMddTHHmmss"));
			sb.AppendLine("DTEND:" + endDate.ToString("yyyyMMddTHHmmss"));
			sb.AppendLine("SUMMARY:BARBERSHOP APPOINTMENT");
			sb.AppendLine("DESCRIPTION:You successfully made your appointment. Please add it to your calendar.");
			sb.AppendLine("ORGANIZER;CN=Barbershop:dk20200125@student.fon.bg.ac.rs");
			sb.AppendLine("PRIORITY:3");
			sb.AppendLine("END:VEVENT");
			sb.AppendLine("END:VCALENDAR");

			return sb.ToString();
		}
	}
}
