using BarberShop.Domain;
using BarberShop.Services.ImplementationDatabase;
using BarberShop.Services.Interfaces;
using BarberShop.Utils;
using BarberShopWeb.DTOs;
using BarberShopWeb.Hateoas;
using BarberShopWeb.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
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
		private readonly IServiceService serviceService;
		private readonly IEmailSender emailSender;
		private readonly UserManager<IdentityUser> userManager;
		private readonly LinkGenerator linkGenerator;

		public AppointmentMobileController(IAppointmentService appointmentService,IBarberService barberService,IServiceService serviceService, IEmailSender emailSender, UserManager<IdentityUser> userManager, LinkGenerator linkGenerator)
        {
			this.appointmentService = appointmentService;
			this.barberService = barberService;
			this.serviceService = serviceService;
			this.emailSender = emailSender;
			this.userManager = userManager;
			this.linkGenerator = linkGenerator;
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
		public async Task<IActionResult> CreateAppointmnet(AppointmentDTO appointment)
		{
            Appointment ap = new Appointment
			{
				AppDuration = appointment.AppDuration,
				Barber= appointment.Barber,
				Date= appointment.Date,
				IsCanceled= appointment.IsCanceled,
				Price= appointment.Price,
				Services= appointment.Services,
				StartTime= appointment.StartTime,
			};

			foreach(Service service in appointment.Services)
			{
				service.ServiceCategory = null;
			}


			var user = await userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
			ap.IdentityUser = user;
			appointmentService.Add(ap);

			string emailBody = "<p>Starts: " + ap.Date.ToString("dd/MM/yyyy") + " " + ap.StartTime + (string.Compare(ap.StartTime, "12:00") < 0 ? " AM" : " PM") + "</p>" + "<p>Barber: " + ap.Barber.FirstName + " " + ap.Barber.LastName + "</p>" + "<p>Duration: " + ap.AppDuration + "</p>" + "<p>Price: " + ap.Price + "</p>";

			string[] splitTime=ap.StartTime.Split(':');
			DateTime startDate=new DateTime(ap.Date.Year, ap.Date.Month, ap.Date.Day, int.Parse(splitTime[0]), int.Parse(splitTime[1]),0,DateTimeKind.Unspecified);
			DateTime endDate = startDate.AddMinutes(ap.AppDuration);

			ICSGenerator icsGenerator = new ICSGenerator();
			string ics=icsGenerator.GenerateIcs(startDate, endDate);

			await emailSender.SendEmailAsync(user.Email, "[BARBERSHOP] Potvrda rezervacije broj: " + ap.AppointmentId, emailBody + $"ics:{ics}");

			return Ok(new { message = "Appointment is successfully created" });
		}

		[HttpGet("latestAppointment")]
		public async Task<IActionResult> LatestAppointment()
		{
			string now = DateTime.Now.Date.ToString("yyyy-MM-dd");

			var user = await userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);

            Appointment? appointment = appointmentService.SearchByDateFirst(now, user.Id);
			if (appointment != null && appointment.IsCanceled == false)
			{
				appointment.IdentityUser = null;

                LinkCollectionWrapper<Appointment> result = new LinkCollectionWrapper<Appointment>();
				result.Value = appointment;
				result.Links = GenerateLinksForAppointment(appointment);
				return Ok(result);
			}
			else return StatusCode(204);
		}

		[HttpGet("appointments/{date}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Role_Admin)]
		public IActionResult GetAllAppointments(string date)
		{
            List<Appointment> appointments = appointmentService.SearchByDate(date);

            List<LinkCollectionWrapper<Appointment>> apps = new List<LinkCollectionWrapper<Appointment>>();

			foreach (Appointment appointment in appointments)
			{
				apps.Add(new LinkCollectionWrapper<Appointment>(appointment, GenerateLinksForAppointment(appointment)));
			}

            LinkCollectionWrapper<List<LinkCollectionWrapper<Appointment>>> result = new LinkCollectionWrapper<List<LinkCollectionWrapper<Appointment>>>(apps, null);

			return Ok(result);

		}

		[HttpPost("appointments/cancel/{appId}")]
		public IActionResult CancelAppointment(int appId)
		{
            Appointment app = appointmentService.Get(appId);
			if (app == null) return BadRequest();
			app.IsCanceled = true;
			appointmentService.Update(app);
			return StatusCode(200);
		}

		[HttpPut("appointments/update/{appId}")]
		public async Task<IActionResult> UpdateAppointment([FromRoute] int appId, [FromBody] AppointmentDTO appointment)
		{
            Appointment app = appointmentService.Get(appId);
			app.AppDuration = appointment.AppDuration;
			app.Barber = appointment.Barber;
			app.Date = appointment.Date;
			app.IsCanceled = appointment.IsCanceled;
			app.Price = appointment.Price;
			app.StartTime = appointment.StartTime;

			//delete existing
			for(int i=0;i<app.Services.Count();i++)
			{
				bool found = false;
				foreach (Service serviceToUpdate in appointment.Services)
					if (app.Services[i].ServiceId == serviceToUpdate.ServiceId)
					{
						found = true;
						break;
					}
				if (found == false) app.Services.Remove(app.Services[i]);
					//delete appointmentService
			}

			//add new
			List<Service> servicesToUpdate = appointment.Services.Select((service) => new Service { ServiceId = service.ServiceId }).ToList();
			//List<Service> serviceToAdd = new List<Service>();
			for (int i = 0; i < servicesToUpdate.Count(); i++)
			{
				bool found = false;
				foreach (Service s in app.Services)
					if (s.ServiceId == servicesToUpdate[i].ServiceId)
					{
						found = true;
						break;
					}
				if (found == false) app.Services.Add(serviceService.Get(servicesToUpdate[i].ServiceId));
			}
			//app.Services = servicesToUpdate;


			var user = await userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
			app.IdentityUser = user;

			app.AppointmentId = appId;
			appointmentService.Update(app);
			return StatusCode(200);
		}


		//////////////////////////////////
		private List<Link> GenerateLinksForAppointment(Appointment appointment)
		{
			List<Link> links = new List<Link>()
			{
				new Link(){
					Method = "POST",
					Rel = "cancelAppointment",
					Href = linkGenerator.GetUriByAction(HttpContext,action: nameof(CancelAppointment),
							controller: "AppointmentMobile",
							values: new { appId = appointment.AppointmentId })
				},
				new Link(){
					Method = "PUT",
					Rel = "updateAppointment",
					Href = linkGenerator.GetUriByAction(HttpContext,action: nameof(UpdateAppointment),
							controller: "AppointmentMobile",
							values: new { appId = appointment.AppointmentId })
				},
			};
			return links;
		}

	}
}
