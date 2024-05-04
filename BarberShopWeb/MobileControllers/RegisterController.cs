using BarberShop.Domain;
using BarberShop.Services.JWT;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BarberShopWeb.MobileControllers
{
	public class RegisterController : Controller
	{
		private readonly JWTService jwtService;
		//prijava user-a
		private readonly SignInManager<ApplicationUser> signInManager;
		//creating user
		private readonly UserManager<ApplicationUser> userManager;

		public RegisterController(JWTService jwtService,SignInManager<ApplicationUser> signInManager,UserManager<ApplicationUser> userManager)
        {
			this.jwtService = jwtService;
			this.signInManager = signInManager;
			this.userManager = userManager;
		}
        public IActionResult Index()
		{
			return Ok("radi");
		}
	}
}
