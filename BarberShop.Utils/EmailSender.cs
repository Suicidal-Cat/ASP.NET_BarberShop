using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Utils
{
	public class EmailSender : IEmailSender
	{
		private readonly IConfiguration configuration;

		public EmailSender(IConfiguration configuration)
		{
			this.configuration = configuration;
		}
		public Task SendEmailAsync(string email, string subject, string message)
		{

			var client = new SmtpClient(configuration["MailSettings:Server"],int.Parse(configuration["MailSettings:Port"]))
			{
				EnableSsl = true,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(configuration["MailSettings:UserName"], configuration["MailSettings:Password"])
			};

			return client.SendMailAsync(
			new MailMessage(from: configuration["MailSettings:SenderEmail"],
							to: email,
							subject,
							message
							));
		}
	}
}
