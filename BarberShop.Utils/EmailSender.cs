using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
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

			string? ics=null;
			if (message.Contains("ics:"))
			{
				string[]splitMessage=message.Split("ics:");
				message=splitMessage[0];
				ics=splitMessage[1];
			}

			var mailMessage = new MailMessage
			{
				From = new MailAddress(configuration["MailSettings:SenderEmail"]),
				To = { new MailAddress(email) },
				Subject = subject,
				Body = message,
				IsBodyHtml = true // Set IsBodyHtml to true for HTML content
			};

			if (ics != null)
			{
				mailMessage.To.Add(new MailAddress(email));
				var icsAttachment = new Attachment(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(ics)), "event.ics", MediaTypeNames.Text.Plain);
				mailMessage.Attachments.Add(icsAttachment);
			}


			return client.SendMailAsync(mailMessage);
		}
	}
}

/*return client.SendMailAsync(
new MailMessage(from: configuration["MailSettings:SenderEmail"],
				to: email,
				subject,
				message
				));*/
