using System;
using System.Security.Cryptography;
using System.Web.UI;
using SmashZone.App_Code;

namespace SmashZone.Pages.User
{
    public partial class _2fa : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Must have email in session (set during Login)
            if (Session["Email"] == null)
            {
                Response.Redirect("~/Pages/User/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Generate OTP once
                string otp = GenerateOtp();

                // Store OTP + expiry
                Session["OTP"] = otp;
                Session["OTP_Expiry"] = DateTime.Now.AddMinutes(3);

                // Send to user's email
                Email.SendEmailOTP(Session["Email"].ToString(), otp);
            }
        }

        protected void btnOtp_Click(object sender, EventArgs e)
        {
            string enteredOtp = (hfOtp.Value ?? "").Trim();

            // Basic input check
            if (enteredOtp.Length != 6)
            {
                Alert("Please enter the 6-digit OTP.");
                return;
            }

            // Read stored OTP
            string storedOtp = Session["OTP"] as string;

            // Read expiry safely
            DateTime expiry;
            if (Session["OTP_Expiry"] == null || !DateTime.TryParse(Session["OTP_Expiry"].ToString(), out expiry))
            {
                Alert("OTP session expired. Please request a new code.");
                return;
            }

            // Expired?
            if (DateTime.Now > expiry)
            {
                // clear old OTP
                Session.Remove("OTP");
                Session.Remove("OTP_Expiry");

                Alert("OTP expired. Please request a new code.");
                return;
            }

            // Match?
            if (!string.Equals(enteredOtp, storedOtp, StringComparison.Ordinal))
            {
                Alert("Invalid OTP. Please try again.");
                return;
            }

            // ✅ Success: clear OTP + continue login
            Session.Remove("OTP");
            Session.Remove("OTP_Expiry");

            // Optional: mark 2FA verified in session
            Session["TwoFA_Verified"] = "True";

            Response.Redirect("~/Pages/User/Default.aspx");
        }

        private static string GenerateOtp()
        {
            // .NET Framework compatible secure OTP
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[4];
                rng.GetBytes(bytes);

                int value = Math.Abs(BitConverter.ToInt32(bytes, 0));
                return (value % 1_000_000).ToString("D6");
            }
        }

        private void Alert(string msg)
        {
            string safe = (msg ?? "").Replace("'", "\\'");
            ClientScript.RegisterStartupScript(
                this.GetType(),
                "otpmsg",
                "alert('" + safe + "');",
                true
            );
        }
    }
}
