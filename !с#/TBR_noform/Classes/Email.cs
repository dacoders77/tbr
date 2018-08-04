using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using MailKit.Net.Smtp; // Smtp https://github.com/jstedfast/MailKit 
using MailKit;
using MimeKit;
using TBR_noform;

namespace TFR_noform
{
	class Email
	{
		public static void Send(string topic, string emailBody)
		{
			
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(Settings.emailFromName, Settings.emailFromEmail));
			message.To.Add(new MailboxAddress(Settings.emailTo, Settings.emailTo));
			message.To.Add(new MailboxAddress(Settings.emailCopyName, Settings.emailCopy));
			message.Subject = "TBR: " + topic;
			message.Body = new TextPart("plain")
			{
				Text = emailBody
			};

			using (var client = new SmtpClient())
			{
				// For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
				client.ServerCertificateValidationCallback = (s, c, h, e) => true;
				client.Connect("smtp.yandex.ru", 25, false);
				// Note: only needed if the SMTP server requires authentication
				client.Authenticate(Settings.smtpLogin, Settings.smtpPassword);
				client.Send(message);
				client.Disconnect(true);
			}
		}
	}
}
