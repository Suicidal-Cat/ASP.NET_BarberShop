using BarberShop.Domain;
using BarberShop.Services.Interfaces;
using BarberShopWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BarberShopWeb.Controllers
{
	public class AppointmentController : Controller
	{
		private readonly IServiceService serviceService;
		private readonly IAppointmentService appointmentService;

		public AppointmentController(IServiceService serviceService, IAppointmentService appointmentService)
        {
			this.serviceService = serviceService;
			this.appointmentService = appointmentService;
		}
        public IActionResult Index()
		{
			ListAddServicesReservation list=new ListAddServicesReservation();
			foreach (Service service in serviceService.Services)
			{
				if (service.ServiceCategory.Name == "Haircuts") list.Haircuts.Add(new AddServiceReservation() { Service=service}
					);
				else if (service.ServiceCategory.Name == "Beard cuts") list.Beard.Add(new AddServiceReservation() { Service = service }
					);
				else if (service.ServiceCategory.Name == "Other") list.Other.Add(new AddServiceReservation() { Service = service }
					);
			}
			return View(list);
		}
		[HttpPost]
		public IActionResult NextChoseBarber(ListAddServicesReservation list) {

			AddServiceReservation? hair = list.Haircuts.FirstOrDefault(h => h.IsChecked == true);
			AddServiceReservation? beard = list.Beard.FirstOrDefault(h => h.IsChecked == true);
			List<AddServiceReservation>? other = list.Other.Where(h => h.IsChecked == true).ToList();

			if (hair == null && beard == null && other.Count <= 0)
			{
				TempData["error"] = "Please select a service.";
				return RedirectToAction("Index");
			}
			return View();
		}
	}
}
