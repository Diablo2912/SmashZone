using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace SmashZone.App_Code
{
    public static class Email
    {
        // ✅ Central send method used by all emails
        private static void Send(MailMessage mail)
        {
            string smtpHost = ConfigurationManager.AppSettings["SMTP_Host"];
            int smtpPort = int.Parse(ConfigurationManager.AppSettings["SMTP_Port"]);
            string smtpUser = ConfigurationManager.AppSettings["SMTP_User"];
            string smtpPass = ConfigurationManager.AppSettings["SMTP_Password"];
            bool enableSSL = bool.Parse(ConfigurationManager.AppSettings["SMTP_EnableSSL"]);

            using (SmtpClient smtp = new SmtpClient(smtpHost, smtpPort))
            {
                smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                smtp.EnableSsl = enableSSL;
                smtp.Send(mail);
            }
        }

        public static void SendEmailOTP(string toEmail, string otp)
        {
            try
            {
                string smtpUser = ConfigurationManager.AppSettings["SMTP_User"];

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(smtpUser, "SmashZone"),
                    Subject = "Your SmashZone OTP Code",
                    IsBodyHtml = true,
                    Body = $@"
<div style='font-family: Arial; text-align: center;'>
  <h2>SmashZone Login Verification</h2>
  <p>Your One-Time Password (OTP) is:</p>
  <h1 style='letter-spacing: 5px;'>{otp}</h1>
  <p>This code expires in 3 minutes.</p>
</div>"
                };

                mail.To.Add(toEmail);
                Send(mail);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Email OTP Error: " + ex);
            }
        }

        public static void SendReceiptEmail(string toEmail, string orderId, string totalSgd, string receiptUrl, string itemsHtml)
        {
            try
            {
                string smtpUser = ConfigurationManager.AppSettings["SMTP_User"];

                string body = $@"
<div style='font-family: Arial;'>
  <h2>SmashZone Receipt ✅</h2>
  <p><b>Order ID:</b> {System.Web.HttpUtility.HtmlEncode(orderId)}</p>
  <p><b>Total Paid:</b> ${System.Web.HttpUtility.HtmlEncode(totalSgd)} SGD</p>
  {(string.IsNullOrWhiteSpace(receiptUrl) ? "" : $"<p><b>Stripe Receipt:</b> <a href='{receiptUrl}' target='_blank'>Open Receipt</a></p>")}
  <hr/>
  <h3>Items</h3>
  {itemsHtml}
  <hr/>
  <p>Thank you for shopping with SmashZone Sports.</p>
</div>";

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(smtpUser, "SmashZone"),
                    Subject = "SmashZone Receipt - " + orderId,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(toEmail);
                Send(mail);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Email Receipt Error: " + ex);
            }
        }

        public static void SendActivationEmail(string toEmail, string activationLink)
        {
            try
            {
                string smtpUser = ConfigurationManager.AppSettings["SMTP_User"];

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(smtpUser, "SmashZone"),
                    Subject = "Activate your SmashZone account",
                    IsBodyHtml = true,
                    Body = $@"
<div style='font-family:Arial'>
  <h2>Activate your account</h2>
  <p>This link expires in <b>10 minutes</b>.</p>
  <p>
    <a href='{activationLink}'
       style='display:inline-block;padding:10px 14px;background:#0d6efd;color:#fff;text-decoration:none;border-radius:8px;'>
      Verify my account
    </a>
  </p>
  <p>If the button doesn't work, copy & paste this link:</p>
  <p>{activationLink}</p>
</div>"
                };

                mail.To.Add(toEmail);
                Send(mail);
            }
            catch
            {
                throw;
            }
        }

        // ✅ NEW: Password reset email
        public static void SendPasswordResetEmail(string toEmail, string link)
        {
            try
            {
                string smtpUser = ConfigurationManager.AppSettings["SMTP_User"];

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(smtpUser, "SmashZone"),
                    Subject = "Reset your SmashZone password",
                    IsBodyHtml = true,
                    Body = $@"
<div style='font-family:Arial'>
  <h2>Reset your password</h2>
  <p>This link expires in <b>10 minutes</b>.</p>
  <p>
    <a href='{link}'
       style='display:inline-block;padding:10px 14px;background:#111;color:#fff;text-decoration:none;border-radius:8px;'>
      Reset Password
    </a>
  </p>
  <p>If you did not request this, you can ignore this email.</p>
</div>"
                };

                mail.To.Add(toEmail);
                Send(mail);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Email Reset Error: " + ex);
            }
        }
    }
}
