using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web.UI;
using SmashZone.App_Code;

namespace SmashZone.Pages.User
{
    public partial class _2fa : Page
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

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
                // default
                rblChannel.SelectedValue = "email";
                SendOtpBySelectedChannel();
            }
        }

        protected void rblChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            // switching channel => resend immediately
            SendOtpBySelectedChannel();
        }

        protected void btnResend_Click(object sender, EventArgs e)
        {
            // resend using currently selected channel
            SendOtpBySelectedChannel();
        }

        private void SendOtpBySelectedChannel()
        {
            string email = Session["Email"].ToString();

            // Generate OTP
            string otp = GenerateOtp();

            // Store OTP + expiry
            Session["OTP"] = otp;
            Session["OTP_Expiry"] = DateTime.Now.AddMinutes(3);

            string channel = (rblChannel.SelectedValue ?? "email").ToLowerInvariant();

            if (channel == "sms")
            {
                // get phone number from Accounts using the email
                string phone = GetPhoneByEmail(email);

                if (string.IsNullOrWhiteSpace(phone))
                {
                    // fallback to email if no phone
                    Email.SendEmailOTP(email, otp);
                    pChannelMsg.InnerText = "No phone number found. Code sent via email instead.";
                    return;
                }

                string phoneE164 = NormalizeSingaporePhone(phone);

                // send SMS
                SMS.SendOTP(phoneE164, otp);
                pChannelMsg.InnerText = "Your code was sent to you via SMS";
            }
            else
            {
                // send Email
                Email.SendEmailOTP(email, otp);
                pChannelMsg.InnerText = "Your code was sent to you via email";
            }
        }

        private string GetPhoneByEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT Phone_Number FROM Accounts WHERE Email=@Email", conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();

                object val = cmd.ExecuteScalar();
                return val == null ? "" : val.ToString();
            }
        }

        // "83323919" -> "+6583323919"
        private string NormalizeSingaporePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return phone;

            phone = phone.Trim().Replace(" ", "").Replace("-", "");

            if (phone.StartsWith("+")) return phone;

            if (phone.Length == 8) return "+65" + phone;

            return phone; // if stored already with country code but without '+', you can improve this
        }

        protected void btnOtp_Click(object sender, EventArgs e)
        {
            string enteredOtp = (hfOtp.Value ?? "").Trim();

            if (enteredOtp.Length != 6)
            {
                Alert("Please enter the 6-digit OTP.");
                return;
            }

            string storedOtp = Session["OTP"] as string;

            DateTime expiry;
            if (Session["OTP_Expiry"] == null || !DateTime.TryParse(Session["OTP_Expiry"].ToString(), out expiry))
            {
                Alert("OTP session expired. Please request a new code.");
                return;
            }

            if (DateTime.Now > expiry)
            {
                Session.Remove("OTP");
                Session.Remove("OTP_Expiry");
                Alert("OTP expired. Please request a new code.");
                return;
            }

            if (!string.Equals(enteredOtp, storedOtp, StringComparison.Ordinal))
            {
                Alert("Invalid OTP. Please try again.");
                return;
            }

            // ✅ Success
            Session.Remove("OTP");
            Session.Remove("OTP_Expiry");
            Session["TwoFA_Verified"] = "True";

            Response.Redirect("~/Pages/User/Default.aspx");
        }

        private static string GenerateOtp()
        {
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
