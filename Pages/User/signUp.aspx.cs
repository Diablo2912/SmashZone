using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using Salt_Password_Sample;   // ✅ your Hash.cs namespace

namespace SmashZone.Pages.User
{
    public partial class signUp : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Text;

            // ✅ basic validation
            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(password))
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "missing",
                    "alert('Please fill in all fields.');",
                    true
                );
                return;
            }

            // ✅ hash password (SHA512 + auto salt)
            string hashedPassword = Hash.ComputeHash(password, "SHA512", null);

            string connStr = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // ✅ Check duplicate email
                    using (SqlCommand checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Accounts WHERE Email = @Email", conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (exists > 0)
                        {
                            ClientScript.RegisterStartupScript(
                                this.GetType(),
                                "dup",
                                "alert('Email already registered.');",
                                true
                            );
                            return;
                        }
                    }

                    // ✅ Insert (store HASHED password)
                    using (SqlCommand insertCmd = new SqlCommand(@"
                        INSERT INTO Accounts (First_Name, Last_Name, Email, Phone_Number, Password)
                        VALUES (@FirstName, @LastName, @Email, @Phone, @Password);", conn))
                    {
                        insertCmd.Parameters.AddWithValue("@FirstName", firstName);
                        insertCmd.Parameters.AddWithValue("@LastName", lastName);
                        insertCmd.Parameters.AddWithValue("@Email", email);
                        insertCmd.Parameters.AddWithValue("@Phone", phone);
                        insertCmd.Parameters.AddWithValue("@Password", hashedPassword);

                        insertCmd.ExecuteNonQuery();
                    }

                    // ✅ redirect after successful signup
                    Response.Redirect("Login.aspx");
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
