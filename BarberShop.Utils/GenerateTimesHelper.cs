using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Utils
{
	public static class GenerateTimesReservationHelper
	{
		public static List<string> GenereteTimes(string startTime,string endTime)
		{
			DateTime start = DateTime.ParseExact(startTime, "HH:mm", null);
			DateTime end = DateTime.ParseExact(endTime, "HH:mm", null);
			List<string> result = new List<string>();
			while(start != end)
			{
				result.Add(start.ToString("HH:mm"));
				start=start.AddMinutes(30);
			}
			return result;
		}
		public static DateTime Duration(string startTime,int duration)
		{
			DateTime sTime= DateTime.ParseExact(startTime, "HH:mm", null);
			int h = 0;
			int min=0;
			while (duration > 60)
			{
				h++;
				duration -= 60;
			}
			min = duration;
			sTime=sTime.AddHours(h);
			sTime=sTime.AddMinutes(min);
			return sTime;
		}
	}
}
