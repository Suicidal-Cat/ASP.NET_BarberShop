using BarberShop.Domain;
using BarberShop.Services.ImplementationDatabase;
using BarberShop.Services.Interfaces;
using BarberShop.Utils;
using BarberShopWeb.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BarberShopWeb.MobileControllers
{
	[Route("mobile/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class AppointmentMobileController : ControllerBase
	{
		private readonly IAppointmentService appointmentService;
		private readonly IBarberService barberService;
		private readonly IEmailSender emailSender;
		private readonly UserManager<IdentityUser> userManager;

		public AppointmentMobileController(IAppointmentService appointmentService,IBarberService barberService, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
			this.appointmentService = appointmentService;
			this.barberService = barberService;
			this.emailSender = emailSender;
			this.userManager = userManager;
		}

		[HttpGet("getAvailableAppointments/{barberId}/{startDate}/{endDate}")]
		public IActionResult GetAvailableAppointments(int barberId,DateTime startDate,DateTime endDate) {

			return Ok(appointmentService.GetAvaiableAppointments(barberId, startDate, endDate).ToList());
		}

		[HttpGet("getAvailableTimes/{barberId}/{date}/{duration}")]
		public IActionResult GetAvailableTimes(int barberId, string date ,int duration)
		{
			List<Appointment> reservations = appointmentService.SearchByDateBarber(date, barberId).OrderBy(ap => ap.StartTime).ToList();

			Barber b = barberService.Get(barberId);

			List<string> reservationTimes = GenerateTimesReservationHelper.GenereteTimes(b.StartWorkingHours, b.EndWorkingHours);


			if (reservations.Count > 0)
			{
				for (int i = 0; i < reservationTimes.Count; i++)
				{
					DateTime start = DateTime.ParseExact(reservationTimes[i], "HH:mm", null);
					DateTime end = start.AddMinutes(duration);
					foreach (Appointment appointment in reservations)
					{
						DateTime startRES = DateTime.ParseExact(appointment.StartTime, "HH:mm", null);
						DateTime endRES = startRES.AddMinutes(appointment.AppDuration);
						if (start <= startRES && end > startRES && appointment.IsCanceled == false)
						{
							reservationTimes[i] = "0";
							break;
						}
						if (start >= startRES && start <= endRES && appointment.IsCanceled == false)
						{
							reservationTimes[i] = "0";
							break;
						}
					}
				}
			}
			return Ok(reservationTimes);

		}

		[HttpPost("create")]
		public async Task<IActionResult> CreateAppointmnet(Appointment ap)
		{
			var user = await userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
			ap.IdentityUser = user;
			appointmentService.Add(ap);

			string emailBody = "<p>Starts: " + ap.Date.ToString("dd/MM/yyyy") + " " + ap.StartTime + (string.Compare(ap.StartTime, "12:00") < 0 ? " AM" : " PM") + "</p>" + "<p>Barber: " + ap.Barber.FirstName + " " + ap.Barber.LastName + "</p>" + "<p>Duration: " + ap.AppDuration + "</p>" + "<p>Price: " + ap.Price + "</p>";


			await emailSender.SendEmailAsync(user.Email, "[BARBERSHOP] Potvrda rezervacije broj: " + ap.AppointmentId, emailBody);

			return Ok(new { message = "Appointment is successfully created" });
		}

	}
}
