using BarberShop.Domain;
using BarberShop.Services.JWT;
using BarberShopWeb.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BarberShopWeb.MobileControllers
{
	public class AccountController : Controller
	{
		private readonly JWTService jwtService;
		//prijava user-a
		private readonly SignInManager<IdentityUser> signInManager;
		//creating user
		private readonly UserManager<IdentityUser> userManager;

		public AccountController(JWTService jwtService,SignInManager<IdentityUser> signInManager,UserManager<IdentityUser> userManager)
        {
			this.jwtService = jwtService;
			this.signInManager = signInManager;
			this.userManager = userManager;
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpGet]
		public async Task<ActionResult<UserDto>> RefreshToken()
		{
            var user = await userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
			return CreateApplicationUserDto((ApplicationUser)user);
		}

		[HttpPost]
        public async Task<ActionResult<UserDto>> Login([FromBody]LoginDto model)
		{
			var user = await userManager.FindByEmailAsync(model.Email);
			if (user == null) return Unauthorized("Invalid email or password");

			if (user.EmailConfirmed == false) return Unauthorized("Please confirm your email");

			var result=await signInManager.CheckPasswordSignInAsync(user, model.Password,false);

			if (!result.Succeeded) return Unauthorized("Invalid email or password");

			return CreateApplicationUserDto((ApplicationUser)user);
		}
		[HttpPost]
		public async Task<IActionResult> Register([FromBody]RegisterDto model)
		{
			if (await CheckEmailExistsAsync(model.Email)) return BadRequest("Email is already taken!");

			ApplicationUser userToAdd = new ApplicationUser
			{
				FirstName = model.FirstName.ToLower(),
				LastName = model.LastName.ToLower(),
				UserName= model.Email.ToLower(),
				Email = model.Email.ToLower(),
				PhoneNumber = model.PhoneNumber.ToLower(),
				//!!!!!
				EmailConfirmed = true,
			};

			var result=await userManager.CreateAsync(userToAdd,model.Password);
			if (!result.Succeeded) return BadRequest(result.Errors);

			return Ok("Your account has been created, you can login");
		}

		private UserDto CreateApplicationUserDto(ApplicationUser user)
		{
			return new UserDto
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				JWT = jwtService.CreateJWT(user)
			};
		}
		//check if email already exists
		private async Task<bool> CheckEmailExistsAsync(string email)
		{
			return await userManager.Users.AnyAsync(u => u.Email == email.ToLower());
		}

	}
}
