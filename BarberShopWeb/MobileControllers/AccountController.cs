using BarberShop.Domain;
using BarberShop.Services.JWT;
using BarberShopWeb.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Text;

namespace BarberShopWeb.MobileControllers
{
	[Route("mobile/account")]
	[ApiController]
	public class AccountController : Controller
	{
		private readonly JWTService jwtService;
		//sign-in user
		private readonly SignInManager<IdentityUser> signInManager;
		//creating user
		private readonly UserManager<IdentityUser> userManager;
		//email sender
		private readonly IEmailSender emailSender;
		private readonly IConfiguration configuration;

		public AccountController(JWTService jwtService,SignInManager<IdentityUser> signInManager,UserManager<IdentityUser> userManager,IEmailSender emailSender,IConfiguration configuration)
        {
			this.jwtService = jwtService;
			this.signInManager = signInManager;
			this.userManager = userManager;
			this.emailSender = emailSender;
			this.configuration = configuration;
		}

		[HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody]LoginDto model)
		{
			var user = await userManager.FindByEmailAsync(model.Email);
			if (user == null) return BadRequest("Invalid email or password");

			if (user.EmailConfirmed == false) return Unauthorized("Please confirm your email");

			var result=await signInManager.CheckPasswordSignInAsync(user, model.Password,false);

			if (!result.Succeeded) return BadRequest("Invalid email or password");

			return CreateApplicationUserDto((ApplicationUser)user);
		}

		[HttpPost("register")]
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
			};

			var result=await userManager.CreateAsync(userToAdd,model.Password);
			if (!result.Succeeded) return BadRequest(result.Errors);


			try
			{
				if(await SendConfirmationEmail(userToAdd)) 
					return Ok("Your account has been created, please confirm your email.");
				return BadRequest("Failed to send confirmation email. Contact support.");
			}
			catch
			{
				return BadRequest("Failed to send confirmation email. Contact support.");
			}

		}


		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpGet("refreshToken")]
		public async Task<ActionResult<UserDto>> RefreshToken()
		{
			var user = await userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
			return CreateApplicationUserDto((ApplicationUser)user);
		}

		[HttpPut("confirmEmail")]
		public async Task<IActionResult> ConfirmEmail(EmailConfirmationDto model)
		{
			IdentityUser user = await userManager.FindByEmailAsync(model.Email);
			if (user == null) return Unauthorized("Email is not registered yet.");

			if (user.EmailConfirmed == true) return BadRequest("Your email is already confirmed. You can login.");

			try
			{
				var decodeToken = WebEncoders.Base64UrlDecode(model.Token);
				string decodedToken=Encoding.UTF8.GetString(decodeToken);

				var result=await userManager.ConfirmEmailAsync(user,decodedToken);
				if (result.Succeeded == true) return Ok(new JsonResult(new
				{
					message = "Your email is confirmed. You can login now."
				}));

				return BadRequest("Invalid request. Try again later.");

			}
			catch (Exception)
			{
				return BadRequest("Invalid request. Try again later."); ;
			}
		}

		[HttpPost("resend-email-confirmation/{email}")]
		public async Task<IActionResult> ResendEmailConfirmation(string email)
		{
			if (string.IsNullOrEmpty(email)) return BadRequest("Invalid email.");
			
			IdentityUser user=await userManager.FindByEmailAsync(email);

			if (user == null) return Unauthorized("Email is not registered yet.");
			if (user.EmailConfirmed == true) return BadRequest("Your email is already confirmed. You can login.");

			try
			{
				if (await SendConfirmationEmail((ApplicationUser)user))
					return Ok(new JsonResult(new
					{
						message = "Confirmation link is sent. Please confirm you email."
					}));
				return BadRequest("Failed to send confirmation email. Contact support.");
			}
			catch
			{
				return BadRequest("Failed to send confirmation email. Contact support.");
			}

		}


		private async Task<bool> SendConfirmationEmail(ApplicationUser user)
		{
			string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
			token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
			string url = $"{configuration["JWT:Issuer"]}/{configuration["Email:ConfirmEmailPath"]}?token={token}&email={user.Email}";

			string body = $"<p>Welcome {user.FirstName}<p>" +
				"<p>Please confirm your email by following this link: <p>" +
				$"<a href=\"{url}\">Click here</a>" +
				"<br><p>Welcome to our community.<p>";

			await emailSender.SendEmailAsync(user.Email, "[BARBESHOP] Confirm your email", body);

			return true;
		}

		private UserDto CreateApplicationUserDto(ApplicationUser user)
		{
			return new UserDto
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email= user.Email,
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
