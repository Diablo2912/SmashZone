using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using Salt_Password_Sample; // your Hash.cs namespace

namespace SmashZone.Pages.User
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "missing",
                    "alert('Please enter your email and password.');",
                    true
                );
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = @"
                        SELECT TOP 1 Id, First_Name, Last_Name, Email, Role, Two_Factor_Status, Password
                        FROM Accounts
                        WHERE Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (!rdr.Read())
                            {
                                ClientScript.RegisterStartupScript(
                                    this.GetType(),
                                    "bad",
                                    "alert('Invalid email or password.');",
                                    true
                                );
                                return;
                            }

                            string storedHash = rdr["Password"].ToString();

                            // Verify password using your Hash.cs
                            bool ok = Hash.VerifyHash(password, "SHA512", storedHash);

                            if (!ok)
                            {
                                ClientScript.RegisterStartupScript(
                                    this.GetType(),
                                    "bad2",
                                    "alert('Invalid email or password.');",
                                    true
                                );
                                return;
                            }

                            // ✅ Read Two_Factor_Status safely (bit/bool/string)
                            bool twoFAEnabled = false;
                            object twoFAObj = rdr["Two_Factor_Status"];

                            if (twoFAObj != DBNull.Value)
                            {
                                // Works for BIT (0/1), bool, or "True"/"False"
                                if (twoFAObj is bool b) twoFAEnabled = b;
                                else if (twoFAObj is int i) twoFAEnabled = (i == 1);
                                else twoFAEnabled = twoFAObj.ToString().Equals("true", StringComparison.OrdinalIgnoreCase)
                                                 || twoFAObj.ToString() == "1";
                            }

                            // ✅ Set session first (important!)
                            Session["AccountId"] = rdr["Id"].ToString();
                            Session["FirstName"] = rdr["First_Name"].ToString();
                            Session["LastName"] = rdr["Last_Name"].ToString();
                            Session["Email"] = rdr["Email"].ToString();
                            Session["Role"] = rdr["Role"].ToString();
                            Session["TwoFA"] = twoFAEnabled ? "True" : "False";

                            // Optional: masterpage switching logic (your BasePage already does this though)
                            Session["CHANGE_MASTERPAGE"] = "~/Master_Pages/UserLogin.Master";

                            // ✅ If 2FA enabled -> go to OTP page
                            if (twoFAEnabled)
                            {
                                Response.Redirect("~/Pages/User/2fa.aspx");
                                return;
                            }

                            // ✅ Otherwise -> normal login success
                            Response.Redirect("~/Pages/User/Default.aspx");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string safe = ex.Message.Replace("'", "").Replace("\r", " ").Replace("\n", " ");
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "err",
                    "alert('ERROR: " + safe + "');",
                    true
                );
            }
        }
    }
}
