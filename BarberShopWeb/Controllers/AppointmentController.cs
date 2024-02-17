using BarberShop.Domain;
using BarberShop.Services.Interfaces;
using BarberShop.Utils;
using BarberShopWeb.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BarberShopWeb.Controllers
{
	public class AppointmentController : Controller
	{
		private readonly IServiceService serviceService;
		private readonly IAppointmentService appointmentService;
		private readonly IBarberService barberService;
		private readonly UserManager<IdentityUser> userManager;

		public AppointmentController(IServiceService serviceService, IAppointmentService appointmentService,IBarberService barberService,UserManager<IdentityUser> userManager)
        {
			this.serviceService = serviceService;
			this.appointmentService = appointmentService;
			this.barberService = barberService;
			this.userManager = userManager;
		}
		//services
        public IActionResult Index()
		{
			ListAddServicesReservationVM list=new ListAddServicesReservationVM();
			foreach (Service service in serviceService.Services)
			{
				if (service.ServiceCategory.Name == "Haircuts") list.Haircuts.Add(new AddServiceReservationVM() { Service=service}
					);
				else if (service.ServiceCategory.Name == "Beard cuts") list.Beard.Add(new AddServiceReservationVM() { Service = service }
					);
				else if (service.ServiceCategory.Name == "Other") list.Other.Add(new AddServiceReservationVM() { Service = service }
					);
			}
			return View(list);
		}
		//barbers
		[HttpPost]
		public IActionResult NextChoseBarber(ListAddServicesReservationVM list) {

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
			addBarberReservationVM.Services = services;

			return View(addBarberReservationVM);
		}
		//date and time
		[HttpPost]
		public IActionResult NextChoseDateTime(AddBarberReservationVM list)
		{

			NextChoseDateTimeVM vm=new NextChoseDateTimeVM();
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
            List<Appointment> reservations= appointmentService.SearchByDate(reservationDate, id).OrderBy(ap=>ap.StartTime).ToList();

			List<string> reservationTimes = GenerateTimesReservationHelper.GenereteTimes("10:00", "20:00");

			GenerateTimesVM vm = new GenerateTimesVM();
			if (reservations.Count > 0)
			{
				for(int i=0;i<reservationTimes.Count;i++) { 
					DateTime start= DateTime.ParseExact(reservationTimes[i], "HH:mm", null);
					DateTime end = start.AddMinutes(duration);
					foreach(Appointment appointment in reservations)
					{
						DateTime startRES = DateTime.ParseExact(appointment.StartTime, "HH:mm", null);
						DateTime endRES = startRES.AddMinutes(appointment.AppDuration);
						if (start <= startRES && end >= startRES)
						{
							reservationTimes[i] = "0";
							break;
						}
						else if(start>=startRES && end<=endRES)
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
		[HttpPost]
		public IActionResult CreateAppointment(NextChoseDateTimeVM vm)
		{
			vm.Appointment.IdentityUser = new IdentityUser();
			vm.Appointment.IdentityUser.Id = userManager.GetUserId(this.User);
			appointmentService.Add(vm.Appointment);
			return RedirectToAction("Index", "Home");
		}
	}
}
