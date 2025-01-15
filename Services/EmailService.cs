using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly string _senderEmail;
    private readonly string _appPassword;

    public EmailService()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) 
            .AddJsonFile("appsettings.json") 
            .Build();

        _senderEmail = configuration["EmailSettings:SenderEmail"];
        _appPassword = configuration["EmailSettings:AppPassword"];
    }

    public void SendEmail(string recipientEmail, string subject, string body)
    {
        try
        {
            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.Credentials = new NetworkCredential(_senderEmail, _appPassword);
                smtpClient.EnableSsl = true;
                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true 
                };

                mailMessage.To.Add(recipientEmail);

                smtpClient.Send(mailMessage);
                Console.WriteLine($"Email successfully sent to {recipientEmail}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
    
    public void SendEmailWithAttachment(string recipientEmail, string subject, string body, string attachmentPath)
    {
        try
        {
            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.Credentials = new NetworkCredential(_senderEmail, _appPassword);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(recipientEmail);

                if (!string.IsNullOrEmpty(attachmentPath) && System.IO.File.Exists(attachmentPath))
                {
                    mailMessage.Attachments.Add(new Attachment(attachmentPath));
                }

                smtpClient.Send(mailMessage);
                Console.WriteLine($"Email with attachment sent to {recipientEmail}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }

}