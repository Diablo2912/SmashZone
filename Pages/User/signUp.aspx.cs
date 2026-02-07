using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using Salt_Password_Sample;
using SmashZone.App_Code;

namespace SmashZone.Pages.User
{
    public partial class signUp : Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        private void ShowMsg(string msg, bool isError)
        {
            pnlMsg.Visible = true;
            pnlMsg.CssClass = isError
                ? "alert alert-danger text-start mt-3"
                : "alert alert-success text-start mt-3";

            lblMsg.Text = msg;   // ✅ IMPORTANT: label has Text
        }

        private void HideMsg()
        {
            pnlMsg.Visible = false;
            lblMsg.Text = "";    // ✅ IMPORTANT
        }


        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            HideMsg();

            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(password))
            {
                ShowMsg("Please fill in all fields.", true);
                return;
            }

            string hashedPassword = Hash.ComputeHash(password, "SHA512", null);
            string connStr = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // check duplicate email
                    using (SqlCommand checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Accounts WHERE Email = @Email", conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        if ((int)checkCmd.ExecuteScalar() > 0)
                        {
                            ShowMsg("This email is already registered. Please log in instead.", true);
                            return;
                        }
                    }

                    // insert account
                    using (SqlCommand insertCmd = new SqlCommand(@"
INSERT INTO Accounts (First_Name, Last_Name, Email, Phone_Number, Password, EmailVerified)
VALUES (@FirstName, @LastName, @Email, @Phone, @Password, 0);", conn))
                    {
                        insertCmd.Parameters.AddWithValue("@FirstName", firstName);
                        insertCmd.Parameters.AddWithValue("@LastName", lastName);
                        insertCmd.Parameters.AddWithValue("@Email", email);
                        insertCmd.Parameters.AddWithValue("@Phone", phone);
                        insertCmd.Parameters.AddWithValue("@Password", hashedPassword);
                        insertCmd.ExecuteNonQuery();
                    }
                }

                // send activation email
                SendActivationForNewUser(email);

                // success UI
                pnlForm.Visible = false;
                ShowMsg(
                    "Account created successfully!<br/>" +
                    "We’ve sent a verification link to your email.<br/>" +
                    "Please verify your account within <b>10 minutes</b>.",
                    false
                );
            }
            catch (Exception ex)
            {
                ShowMsg("Signup failed. Please try again later.", true);
            }
        }


        private void SendActivationForNewUser(string email)
        {
            string cs = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            string token = ActivationHelper.GenerateToken();
            string tokenHash = ActivationHelper.Sha256Hex(token);
            DateTime expiry = DateTime.Now.AddMinutes(10);

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.Accounts
SET ActivationTokenHash=@H, ActivationExpiry=@Exp, EmailVerified=0
WHERE Email=@Email;", conn))
            {
                cmd.Parameters.AddWithValue("@H", tokenHash);
                cmd.Parameters.AddWithValue("@Exp", expiry);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string link = baseUrl + ResolveUrl("~/Pages/User/activate.aspx?t=" + token);

            Email.SendActivationEmail(email, link);
        }

        private void Alert(string msg)
        {
            string safe = (msg ?? "").Replace("'", "\\'").Replace("\r", " ").Replace("\n", " ");
            ClientScript.RegisterStartupScript(this.GetType(), "msg", $"alert('{safe}');", true);
        }
    }
}
