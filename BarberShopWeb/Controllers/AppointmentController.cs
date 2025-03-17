using BarberShop.Domain;
using BarberShop.Services.Interfaces;
using BarberShop.Utils;
using BarberShopWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;

namespace BarberShopWeb.Controllers
{
	public class AppointmentController : Controller
	{
		private readonly IServiceService serviceService;
		private readonly IAppointmentService appointmentService;
		private readonly IBarberService barberService;
		private readonly UserManager<IdentityUser> userManager;
		private readonly IEmailSender emailSender;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public AppointmentController(IServiceService serviceService, IAppointmentService appointmentService,IBarberService barberService,UserManager<IdentityUser> userManager,IEmailSender emailSender, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
			this.serviceService = serviceService;
			this.appointmentService = appointmentService;
			this.barberService = barberService;
			this.userManager = userManager;
			this.emailSender = emailSender;
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }
		//services
        public IActionResult Index()
		{	
            if (!User.Identity.IsAuthenticated)
			{
				return Redirect("/Identity/Account/Login");
			}
			ListAddServicesReservationVM list=new ListAddServicesReservationVM();
			foreach (Service service in serviceService.Services)
			{
				if (service.ServiceCategory.Name == "Haircuts") list.Haircuts.Add(new AddServiceReservationVM() { Service=service}
					);
				else if (service.ServiceCategory.Name == "Beard") list.Beard.Add(new AddServiceReservationVM() { Service = service }
					);
				else if (service.ServiceCategory.Name == "Other") list.Other.Add(new AddServiceReservationVM() { Service = service }
					);
			}
			return View(list);
		}
        //barbers
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NextChooseBarber(ListAddServicesReservationVM list) {

			AddServiceReservationVM? hair = list.Haircuts.FirstOrDefault(h => h.IsChecked == true);
			AddServiceReservationVM? beard = list.Beard.FirstOrDefault(h => h.IsChecked == true);
			List<AddServiceReservationVM>? other = list.Other.Where(h => h.IsChecked == true).ToList();

			if (hair == null && beard == null && other.Count <= 0)
			{
				TempData["error"] = "Please select a service.";
				return RedirectToAction("Index");
			}

			List<Service> services = new List<Service>();
			if (hair != null) services.Add(hair.Service);
			if(beard!=null) services.Add(beard.Service);
			foreach(var ot in other)services.Add(ot.Service);

			AddBarberReservationVM addBarberReservationVM = new AddBarberReservationVM();
			addBarberReservationVM.Barbers = barberService.Barbers.Where(b=>b.Status==Status.Active).Select(b=>new BarberCheckBox { Barber=b}).ToList();

			foreach(BarberCheckBox b in addBarberReservationVM.Barbers)
			{
                var client = httpClientFactory.CreateClient();
                string url = $"{configuration["JWT:Issuer"]}/Drive/downloadLink/{b.Barber.ImageUrl}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var imagePath = await response.Content.ReadAsStringAsync();
                    b.Barber.ImageUrl = imagePath;
                }
                else
                {
                    b.Barber.ImageUrl = "";
                }
            }


			addBarberReservationVM.Services = services;

			return View(addBarberReservationVM);
		}
        //date and time
        [Authorize]
        [HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult NextChooseDateTime(AddBarberReservationVM list)
		{

			NextChooseDateTimeVM vm=new NextChooseDateTimeVM();
			Appointment app = new Appointment();
			foreach(Service service in list.Services)
			{
				Service ser = serviceService.Get(service.ServiceId);
				app.AppDuration += ser.Duration;
				app.Price += ser.Price;
			}
			app.Services = list.Services;
			app.Barber = list.Barbers.FirstOrDefault(b => b.IsChecked == true).Barber;

			vm.Appointment = app;
			return View(vm);
		}
        //partial view for available times
        [HttpGet]
		public IActionResult GenerateTimes(string reservationDate,int id,int duration)
		{
            List<Appointment> reservations= appointmentService.SearchByDateBarber(reservationDate, id).OrderBy(ap=>ap.StartTime).ToList();

			Barber b = barberService.Get(id);

			List<string> reservationTimes = GenerateTimesReservationHelper.GenereteTimes(b.StartWorkingHours, b.EndWorkingHours);

			GenerateTimesVM vm = new GenerateTimesVM();

			if (reservationTimes.Count == 0)
			{
				vm.Times = reservationTimes;
				return PartialView("GenerateTimes", vm);
			}

			if (reservations.Count > 0)
			{
				for(int i=0;i<reservationTimes.Count;i++) { 
					DateTime start= DateTime.ParseExact(reservationTimes[i], "HH:mm", null);
					DateTime end = start.AddMinutes(duration);
					foreach(Appointment appointment in reservations)
					{
						DateTime startRES = DateTime.ParseExact(appointment.StartTime, "HH:mm", null);
						DateTime endRES = startRES.AddMinutes(appointment.AppDuration);
						if (start <= startRES && end > startRES && appointment.IsCanceled==false)
						{
							reservationTimes[i] = "0";
							break;
						}
						if(start>=startRES && start<endRES && appointment.IsCanceled == false)
						{
							reservationTimes[i] = "0";
							break;
						}
					}
				}
			}
			vm.Times = reservationTimes;
            return PartialView("GenerateTimes", vm);
		}
        //create appointment
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment(NextChooseDateTimeVM vm)
		{
			vm.Appointment.IdentityUser = new IdentityUser();
			vm.Appointment.IdentityUser.Id = userManager.GetUserId(this.User);

			vm.Appointment.Barber = barberService.Get(vm.Appointment.Barber.BarberId);

			appointmentService.Add(vm.Appointment);


			string emailBody = "<p>Starts: " + vm.Appointment.Date.ToString("dd/MM/yyyy") +" "+vm.Appointment.StartTime + (string.Compare(vm.Appointment.StartTime, "12:00") < 0 ? " AM" : " PM") + "</p>" +
				"<p>Barber: "+vm.Appointment.Barber.FirstName+" "+vm.Appointment.Barber.LastName+"</p>"+"<p>Duration: "+vm.Appointment.AppDuration+"</p>"+"<p>Price: "+vm.Appointment.Price+"</p>";

			string[] splitTime = vm.Appointment.StartTime.Split(':');
			DateTime startDate = new DateTime(vm.Appointment.Date.Year, vm.Appointment.Date.Month, vm.Appointment.Date.Day, int.Parse(splitTime[0]), int.Parse(splitTime[1]), 0, DateTimeKind.Unspecified);
			DateTime endDate = startDate.AddMinutes(vm.Appointment.AppDuration);

			ICSGenerator icsGenerator = new ICSGenerator();
			string ics = icsGenerator.GenerateIcs(startDate, endDate);


			await emailSender.SendEmailAsync(User?.Identity?.Name, "[BARBERSHOP] Potvrda rezervacije broj: " + vm.Appointment.AppointmentId, emailBody + $"ics:{ics}");


			return RedirectToAction("Index", "Home");
		}
        [HttpGet]
		public IActionResult ShowNextAppointmentPV()
		{
			string now = DateTime.Now.Date.ToString("yyyy-MM-dd");
			string idUser = userManager.GetUserId(this.User) ?? "";
			if (idUser == "") return Ok(204);
			Appointment appointment = appointmentService.SearchByDateFirst(now,idUser);
			if (appointment != null && appointment.IsCanceled==false)
			{
				return PartialView("ShowNextAppointmentPV", appointment);
			}
			else return Ok(204);	
		}
		[HttpGet]
		public IActionResult ShowAppointments()
		{
			return View();
		}
		[HttpGet]
		public IActionResult GetAllAppointmentsDatePV(string reservationDate)
		{
			List<Appointment> appointments=appointmentService.SearchByDate(reservationDate);
			return PartialView(appointments);
		}
		[HttpPost]
		public IActionResult DeleteAppointment(int appId)
		{
			appointmentService.Delete(appId);
			return Ok(200);
		}
		[HttpPost]
		public IActionResult CancelAppointment(int appId)
		{
			Appointment app=appointmentService.Get(appId);
			app.IsCanceled = true;
			appointmentService.Update(app);
			return Ok(200);
		}
	}
}
