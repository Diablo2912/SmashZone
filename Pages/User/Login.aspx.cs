using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using Salt_Password_Sample;
using SmashZone.App_Code;   // ✅ IMPORTANT

namespace SmashZone.Pages.User
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        private void ShowMsg(string msg, bool isError)
        {
            pnlMsg.Visible = true;
            pnlMsg.CssClass = isError
                ? "alert alert-danger text-start mt-3"
                : "alert alert-success text-start mt-3";

            lblMsg.Text = msg;
        }

        private void HideMsg()
        {
            pnlMsg.Visible = false;
            lblMsg.Text = "";
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            HideMsg();

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ShowMsg("Please enter your email and password.", true);
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = @"
SELECT TOP 1 Id, First_Name, Last_Name, Email, Role, Two_Factor_Status, Password, EmailVerified
FROM Accounts
WHERE Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (!rdr.Read())
                            {
                                ShowMsg("Invalid email or password.", true);
                                return;
                            }

                            // ✅ Block if email not verified
                            bool emailVerified = false;
                            object evObj = rdr["EmailVerified"];

                            if (evObj != DBNull.Value)
                            {
                                if (evObj is bool b) emailVerified = b;
                                else if (evObj is int i) emailVerified = (i == 1);
                                else emailVerified = evObj.ToString() == "1"
                                                  || evObj.ToString().Equals("true", StringComparison.OrdinalIgnoreCase);
                            }

                            if (!emailVerified)
                            {
                                ShowMsg(
                                    "Your email is not verified yet.<br/>" +
                                    "Please check your inbox (and Spam/Junk) and click the verification link before logging in.",
                                    true
                                );
                                return;
                            }

                            // ✅ Password check only after verified
                            string storedHash = rdr["Password"].ToString();
                            bool ok = Hash.VerifyHash(password, "SHA512", storedHash);

                            if (!ok)
                            {
                                ShowMsg("Invalid email or password.", true);
                                return;
                            }

                            // ✅ Read Two_Factor_Status safely
                            bool twoFAEnabled = false;
                            object twoFAObj = rdr["Two_Factor_Status"];

                            if (twoFAObj != DBNull.Value)
                            {
                                if (twoFAObj is bool bb) twoFAEnabled = bb;
                                else if (twoFAObj is int ii) twoFAEnabled = (ii == 1);
                                else twoFAEnabled = twoFAObj.ToString().Equals("true", StringComparison.OrdinalIgnoreCase)
                                                 || twoFAObj.ToString() == "1";
                            }

                            // ✅ Set session
                            Session["AccountId"] = rdr["Id"].ToString();
                            Session["FirstName"] = rdr["First_Name"].ToString();
                            Session["LastName"] = rdr["Last_Name"].ToString();
                            Session["Email"] = rdr["Email"].ToString();
                            Session["Role"] = rdr["Role"].ToString();
                            Session["TwoFA"] = twoFAEnabled ? "True" : "False";
                            Session["CHANGE_MASTERPAGE"] = "~/Master_Pages/UserLogin.Master";

                            if (twoFAEnabled)
                            {
                                Response.Redirect("~/Pages/User/2fa.aspx");
                                return;
                            }

                            Response.Redirect("~/Pages/User/Default.aspx");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string safe = ex.Message.Replace("'", "").Replace("\r", " ").Replace("\n", " ");
                ShowMsg("ERROR: " + safe, true);
            }
        }

        // ✅ Modal "Send Reset Link"
        protected void btnSendReset_Click(object sender, EventArgs e)
        {
            HideMsg();

            string email = txtResetEmail.Text.Trim();

            // always same message (no enumeration)
            string success =
                "If an account with that email exists, a password reset link has been sent.<br/>" +
                "Please check your inbox (and Spam/Junk).";

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowMsg(success, false);
                return;
            }

            string cs = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            try
            {
                string token = ActivationHelper.GenerateToken();
                string tokenHash = ActivationHelper.Sha256Hex(token);
                DateTime expiry = DateTime.Now.AddMinutes(10);

                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand(@"
UPDATE Accounts
SET ResetTokenHash = @H,
    ResetTokenExpiry = @Exp
WHERE Email = @Email;", conn))
                {
                    cmd.Parameters.AddWithValue("@H", tokenHash);
                    cmd.Parameters.AddWithValue("@Exp", expiry);
                    cmd.Parameters.AddWithValue("@Email", email);

                    conn.Open();
                    cmd.ExecuteNonQuery(); // ignore affected rows intentionally
                }

                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string link = baseUrl + ResolveUrl("~/Pages/User/passwordReset.aspx?t=" + token);

                Email.SendPasswordResetEmail(email, link);
            }
            catch
            {
                // swallow errors intentionally
            }

            ShowMsg(success, false);
        }
    }
}
