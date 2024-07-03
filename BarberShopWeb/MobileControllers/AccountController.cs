using BarberShop.Domain;
using BarberShop.Services.JWT;
using BarberShop.Utils;
using BarberShopWeb.DTOs;
using BarberShopWeb.DTOs.Account;
using BarberShopWeb.Hateoas;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace BarberShopWeb.MobileControllers
{
    [Route("mobile/[controller]")]
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
        private readonly LinkGenerator linkGenerator;

        public AccountController(JWTService jwtService,SignInManager<IdentityUser> signInManager,UserManager<IdentityUser> userManager,IEmailSender emailSender,IConfiguration configuration,LinkGenerator linkGenerator)
        {
			this.jwtService = jwtService;
			this.signInManager = signInManager;
			this.userManager = userManager;
			this.emailSender = emailSender;
			this.configuration = configuration;
            this.linkGenerator = linkGenerator;
        }

		[HttpGet("/mobile")]
		public IActionResult AccountRotes() {

			List<Link> links = new List<Link>
			{
				new Link()
				{
					Method = "POST",
					Rel = "login",
					Href = linkGenerator.GetUriByAction(HttpContext, nameof(Login))
				},
				new Link()
				{
					Method = "POST",
					Rel = "register",
					Href = linkGenerator.GetUriByAction(HttpContext, nameof(Register))
				},
				new Link()
				{
					Method = "GET",
					Rel = "refreshToken",
					Href = linkGenerator.GetUriByAction(HttpContext, nameof(RefreshToken))
				},
                new Link()
                {
                    Method = "POST",
                    Rel = "resendEmail",
                    Href = linkGenerator.GetUriByAction(httpContext: HttpContext,
					action: nameof(ResendEmailConfirmation),
					controller: "Account",
					values: new { email = "email" })
                },
                new Link()
				{
					Method = "POST",
					Rel = "forgotPassword",
                    Href = linkGenerator.GetUriByAction(httpContext: HttpContext,
                    action: nameof(ForgotPassword),
                    controller: "Account",
                    values: new { email = "email" })
                }
            };

            LinkCollectionWrapper<string> result=new LinkCollectionWrapper<string>("navigation",links);

            return Ok(result);
		}

		[HttpPost("login")]
        public async Task<ActionResult<LinkCollectionWrapper<UserDto>>> Login([FromBody]LoginDto model)
		{
			var user = await userManager.FindByEmailAsync(model.Email);
			if (user == null) return BadRequest("Invalid email or password");

			if (user.EmailConfirmed == false) return Unauthorized("Please confirm your email");

			var result=await signInManager.CheckPasswordSignInAsync(user, model.Password,false);

			if (!result.Succeeded) return BadRequest("Invalid email or password");

            return await CreateApplicationUserDto((ApplicationUser)user);
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

            var addRoleResult=await userManager.AddToRoleAsync(userToAdd, UserRoles.Role_User);
            if (!addRoleResult.Succeeded) return BadRequest(result.Errors);

            try
			{
				if(await SendConfirmationEmail(userToAdd)) 
					return Ok(new { message="Your account has been created, please confirm your email." });
				return BadRequest("Failed to send confirmation email. Contact support.");
			}
			catch
			{
				return BadRequest("Failed to send confirmation email. Contact support.");
			}

		}


		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpGet("refreshToken")]
		public async Task<ActionResult<LinkCollectionWrapper<UserDto>>> RefreshToken()
		{
			var user = await userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
			return await CreateApplicationUserDto((ApplicationUser)user);
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
				if (result.Succeeded == true) return Ok(new
				{
					message = "Your email is confirmed. You can login now."
				});

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
					
					return Ok(new
					{
						message = "Confirmation link is sent. Please confirm your email.",
					});
				return BadRequest("Failed to send confirmation email. Contact support.");
			}
			catch
			{
				return BadRequest("Failed to send confirmation email. Contact support.");
            }
        }

		[HttpPost("forgotPassword/{email}")]
		public async Task<IActionResult> ForgotPassword(string email)
		{
			if (string.IsNullOrEmpty(email)) return BadRequest("Invalid email.");
			IdentityUser user = await userManager.FindByEmailAsync(email);

			if (user == null) return Unauthorized("Email is not registered yet.");
			if (user.EmailConfirmed == false) return BadRequest("Please confirm your email first.");

			try
			{
				if(await SendForgotPasswordEmail((ApplicationUser)user))
				{
					return Ok(new {message="Please check your email to reset your password."});
				}
				return BadRequest("Failed to send confirmation email. Contact support.");
			}
			catch (Exception)
			{
				return BadRequest("Failed to send confirmation email. Contact support.");
			}
		}

		[HttpPut("resetPassword")]
		public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
		{
			IdentityUser user = await userManager.FindByEmailAsync(model.Email);
			if (user == null) return Unauthorized("Email is not registered yet.");

			if (user.EmailConfirmed == false) return BadRequest("Please confirm your email first.");

			try
			{
				var decodeToken = WebEncoders.Base64UrlDecode(model.Token);
				string decodedToken = Encoding.UTF8.GetString(decodeToken);

				var result = await userManager.ResetPasswordAsync(user, decodedToken,model.NewPassword);
				if (result.Succeeded == true) return Ok(new
				{
					message = "Your password has been reset."
				});

				return BadRequest("Invalid request. Try again later.");

			}
			catch (Exception)
			{
				return BadRequest("Invalid request. Try again later."); ;
			}
		}

        //helper methods

        private async Task<bool> SendForgotPasswordEmail(ApplicationUser user)
		{
			string token = await userManager.GeneratePasswordResetTokenAsync(user);
			token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
			string url = $"{configuration["JWT:Issuer"]}/{configuration["MailSettings:ResetPasswordPath"]}?token={token}&email={user.Email}";

			string body = $"<p>Hi, {user.FirstName} {user.LastName}<p>" +
				"<p>Please reset your password by following this link: <p>" +
				$"<a href=\"{url}\">Click here</a>";

			await emailSender.SendEmailAsync(user.Email, "[BARBESHOP] Confirm your password", body);

			return true;
		}

		private async Task<bool> SendConfirmationEmail(ApplicationUser user)
		{
			string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
			token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
			string url = $"{configuration["JWT:Issuer"]}/{configuration["MailSettings:ConfirmEmailPath"]}?token={token}&email={user.Email}";

			string body = $"<p>Welcome {user.FirstName} {user.LastName}<p>" +
				"<p>Please confirm your email by following this link: <p>" +
				$"<a href=\"{url}\">Click here</a>" +
				"<br><p>Thanks for joining our community.<p>";

			await emailSender.SendEmailAsync(user.Email, "[BARBESHOP] Confirm your email", body);

			return true;
		}

		private async Task<LinkCollectionWrapper<UserDto>> CreateApplicationUserDto(ApplicationUser user)
		{
			var roles=await userManager.GetRolesAsync(user);
			
			var userDTO= new UserDto
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email= user.Email,
				Role = roles[0],
				JWT = await jwtService.CreateJWT(user)
			};

			List<Link> links = new List<Link>();
            links.Add(new Link()
            {
                Method = "GET",
                Rel = "allBarbers",
                Href = linkGenerator.GetUriByAction(httpContext: HttpContext,
                                action: "GetBarbers",
                                controller: "BarberMobile")
            });

            links.Add(new Link()
            {
                Method = "GET",
                Rel = "allServices",
                Href = linkGenerator.GetUriByAction(httpContext: HttpContext,
                                action: "GetServices",
                                controller: "ServiceMobile")
            });

            links.Add(new Link()
            {
                Method = "GET",
                Rel = "serviceCategories",
                Href = linkGenerator.GetUriByAction(HttpContext, action: "GetServiceCategories", controller: "ServiceMobile")
            });

            if (userDTO.Role == "Admin")
			{
                
                links.Add(new Link()
				{
					Method = "GET",
					Rel = "barberPagination",
                    Href = linkGenerator.GetUriByAction(httpContext: HttpContext,
                                action: "GetBarbersPagination",
                                controller: "BarberMobile",
                                values: new { pageNumber = 1 })
                });
                links.Add(new Link()
                {
                    Method = "POST",
                    Rel = "createBarber",
                    Href = linkGenerator.GetUriByAction(HttpContext, action: "Create", controller: "BarberMobile")
                });
                
                links.Add(new Link()
				{
					Method = "GET",
					Rel = "servicePagination",
					Href = linkGenerator.GetUriByAction(httpContext: HttpContext,
								action: "GetServicesPagination",
								controller: "ServiceMobile",
								values: new { pageNumber = 1 })
				});

				links.Add(new Link()
				{
					Method = "POST",
					Rel = "createService",
					Href = linkGenerator.GetUriByAction(HttpContext, action:"Create", controller:"ServiceMobile")
				});
            }

			return new LinkCollectionWrapper<UserDto>(userDTO,links);

		}
		//check if email already exists
		private async Task<bool> CheckEmailExistsAsync(string email)
		{
			return await userManager.Users.AnyAsync(u => u.Email == email.ToLower());
		}

	}
}
