using System;
using System.Configuration;
using System.Data.SqlClient;
using SmashZone.App_Code;

namespace SmashZone.Pages.User
{
    public partial class activate : System.Web.UI.Page
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            string token = Request.QueryString["t"];

            if (string.IsNullOrWhiteSpace(token))
            {
                Show("Invalid activation link.", true);
                return;
            }

            string tokenHash = ActivationHelper.Sha256Hex(token);

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT Id, EmailVerified, ActivationExpiry
FROM dbo.Accounts
WHERE ActivationTokenHash = @H;", conn))
            {
                cmd.Parameters.AddWithValue("@H", tokenHash);

                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        Show("Invalid or already used activation link.", true);
                        return;
                    }

                    bool verified = Convert.ToBoolean(dr["EmailVerified"]);
                    DateTime? exp = dr["ActivationExpiry"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["ActivationExpiry"]);

                    if (verified)
                    {
                        Show("Your account is already verified ✅", false);
                        return;
                    }

                    if (!exp.HasValue || DateTime.Now > exp.Value)
                    {
                        Show("Activation link expired. Please sign up again or request a new link.", true);
                        return;
                    }

                    int userId = Convert.ToInt32(dr["Id"]);

                    // Need to close reader before running update on same connection
                    dr.Close();

                    using (SqlCommand upd = new SqlCommand(@"
UPDATE dbo.Accounts
SET EmailVerified = 1,
    ActivationTokenHash = NULL,
    ActivationExpiry = NULL
WHERE Id = @Id;", conn))
                    {
                        upd.Parameters.AddWithValue("@Id", userId);
                        upd.ExecuteNonQuery();
                    }

                    Show("Your account has been verified ✅ You can now log in.", false);
                }
            }
        }

        private void Show(string msg, bool isError)
        {
            lblMsg.Text = isError
                ? "<div class='alert alert-danger mt-3'>" + Server.HtmlEncode(msg) + "</div>"
                : "<div class='alert alert-success mt-3'>" + Server.HtmlEncode(msg) + "</div>";
        }
    }
}
