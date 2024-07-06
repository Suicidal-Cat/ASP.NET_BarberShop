using BarberShop.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BarberShopWeb.MobileControllers
{
	[Route("mobile/[controller]")]
	[ApiController]
	public class AppointmentMobileController : ControllerBase
	{
		private readonly IAppointmentService appointmentService;

		public AppointmentMobileController(IAppointmentService appointmentService)
        {
			this.appointmentService = appointmentService;
		}

		[HttpGet("getAvailableAppointments/{barberId}/{startDate}/{endDate}")]
		public IActionResult GetAvailableAppointments(int barberId,DateTime startDate,DateTime endDate) {

			return Ok(appointmentService.GetAvaiableAppointments(barberId, startDate, endDate).ToList());
		}

	}
}
