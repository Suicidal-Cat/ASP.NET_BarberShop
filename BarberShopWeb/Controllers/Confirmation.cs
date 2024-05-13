using BarberShopWeb.DTOs;
using BarberShopWeb.DTOs.Account;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace BarberShopWeb.Controllers
{
	public class Confirmation : Controller
	{
        private readonly IConfiguration configuration;

        public Confirmation(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<IActionResult> EmailConfirmation(string token,string email)
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri($"{configuration["JWT:Issuer"]}/mobile/account/confirmEmail");
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var model = new EmailConfirmationDto { 
					Email = email,
					Token=token,
				};

				var response = await client.PutAsJsonAsync("confirmEmail", model);
				if (response.IsSuccessStatusCode)
				{
					return View("EmailConfirmation", "Your email is confirmed. You can login now.");
				}
				else
				{
					return View("EmailConfirmation", "Error: Invalid request. Try again later.");
				}
			}
		}

		[HttpGet]
		public IActionResult ResetPassword(string token, string email)
		{
			ResetPasswordDto ResetPasswordDto = new ResetPasswordDto()
			{
				Email = email,
				Token = token,
			};
			return View(ResetPasswordDto);
		}
		[HttpPost]
        public async Task<IActionResult> ConfirmPassowrd(ResetPasswordDto model)
		{
            if (model.NewPassword !=model.ConfirmPassword)
            {
                TempData["error"] = "Password must match.";
                return View("ResetPassword", model);
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{configuration["JWT:Issuer"]}/mobile/account/resetPassword");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.PutAsJsonAsync("resetPassword", model);
                if (response.IsSuccessStatusCode)
                {
                    return View("PasswordConfirmation", "Your password has been reset. Try to login.");
                }
                else
                {
                    return View("PasswordConfirmation", "Error: Invalid request. Try again later.");
                }
            }
        }

    }
}
