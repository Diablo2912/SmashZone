using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace SmashZone.App_Code
{
    public static class Email
    {
        /// <summary>
        /// Sends a 6-digit OTP email
        /// </summary>
        public static void SendEmailOTP(string toEmail, string otp)
        {
            try
            {
                string smtpHost = ConfigurationManager.AppSettings["SMTP_Host"];
                int smtpPort = int.Parse(ConfigurationManager.AppSettings["SMTP_Port"]);
                string smtpUser = ConfigurationManager.AppSettings["SMTP_User"];
                string smtpPass = ConfigurationManager.AppSettings["SMTP_Password"];
                bool enableSSL = bool.Parse(ConfigurationManager.AppSettings["SMTP_EnableSSL"]);

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(smtpUser, "SmashZone"),
                    Subject = "Your SmashZone OTP Code",
                    Body = $@"
                        <div style='font-family: Arial; text-align: center;'>
                            <h2>SmashZone Login Verification</h2>
                            <p>Your One-Time Password (OTP) is:</p>
                            <h1 style='letter-spacing: 5px;'>{otp}</h1>
                            <p>This code expires in 3 minutes.</p>
                        </div>",
                    IsBodyHtml = true
                };

                mail.To.Add(toEmail);

                SmtpClient smtp = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = enableSSL
                };

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                // For WebForms, log instead of Console.WriteLine
                System.Diagnostics.Trace.WriteLine("Email OTP Error: " + ex);
            }
        }
    }
}
