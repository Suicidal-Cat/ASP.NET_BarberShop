using Microsoft.AspNetCore.Mvc;

namespace BarberShopWeb.Controllers
{
	public class Appointment : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
