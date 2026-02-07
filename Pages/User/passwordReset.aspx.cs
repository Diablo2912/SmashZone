using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using Salt_Password_Sample;
using SmashZone.App_Code; // ✅ for ActivationHelper

namespace SmashZone.Pages.User
{
    public partial class passwordReset : Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            // ✅ Validate token on first load
            int accountId = GetAccountIdFromResetToken();
            if (accountId <= 0)
            {
                pnlForm.Visible = false;
                ShowMsg("Invalid or expired reset link.", true);
                return;
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            HideMsg();

            // ✅ re-check token again on submit (important)
            int accountId = GetAccountIdFromResetToken();
            if (accountId <= 0)
            {
                pnlForm.Visible = false;
                ShowMsg("Invalid or expired reset link.", true);
                return;
            }

            string newPassword = txtPassword.Text;

            try
            {
                using (SqlConnection conn = new SqlConnection(cs))
                {
                    conn.Open();

                    // 0) Get current password hash too (so “previous 3” works even if history empty)
                    string currentHash = null;
                    using (SqlCommand getCur = new SqlCommand(
                        "SELECT Password FROM Accounts WHERE Id=@Id", conn))
                    {
                        getCur.Parameters.AddWithValue("@Id", accountId);
                        object obj = getCur.ExecuteScalar();
                        currentHash = obj == DBNull.Value ? null : (obj?.ToString());
                    }

                    // 1) Check against previous 3 passwords (history + current)
                    int matchesFound = 0;

                    if (!string.IsNullOrEmpty(currentHash) &&
                        Hash.VerifyHash(newPassword, "SHA512", currentHash))
                    {
                        ShowMsg("Password can’t be the same as your previous <b>3 passwords</b>.", true);
                        return;
                    }

                    using (SqlCommand checkCmd = new SqlCommand(@"
SELECT TOP 3 PasswordHash
FROM PasswordHistory
WHERE AccountId = @AccountId
ORDER BY ChangedAt DESC;", conn))
                    {
                        checkCmd.Parameters.AddWithValue("@AccountId", accountId);

                        using (SqlDataReader rdr = checkCmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                string oldHash = rdr["PasswordHash"].ToString();
                                if (Hash.VerifyHash(newPassword, "SHA512", oldHash))
                                {
                                    ShowMsg("Password can’t be the same as your previous <b>3 passwords</b>.", true);
                                    return;
                                }
                                matchesFound++;
                            }
                        }
                    }

                    // 2) Hash new password
                    string newHash = Hash.ComputeHash(newPassword, "SHA512", null);

                    // 3) Update password + clear reset token (one-time use)
                    using (SqlCommand upd = new SqlCommand(@"
UPDATE Accounts
SET Password = @Password,
    ResetTokenHash = NULL,
    ResetTokenExpiry = NULL
WHERE Id = @AccountId;", conn))
                    {
                        upd.Parameters.AddWithValue("@Password", newHash);
                        upd.Parameters.AddWithValue("@AccountId", accountId);
                        upd.ExecuteNonQuery();
                    }

                    // 4) Insert into password history
                    using (SqlCommand ins = new SqlCommand(@"
INSERT INTO PasswordHistory (AccountId, PasswordHash)
VALUES (@AccountId, @PasswordHash);", conn))
                    {
                        ins.Parameters.AddWithValue("@AccountId", accountId);
                        ins.Parameters.AddWithValue("@PasswordHash", newHash);
                        ins.ExecuteNonQuery();
                    }

                    // 5) (Optional) Keep only last 3 history entries
                    using (SqlCommand prune = new SqlCommand(@"
;WITH cte AS (
    SELECT Id, ROW_NUMBER() OVER (ORDER BY ChangedAt DESC) AS rn
    FROM PasswordHistory
    WHERE AccountId = @AccountId
)
DELETE FROM PasswordHistory
WHERE Id IN (SELECT Id FROM cte WHERE rn > 3);", conn))
                    {
                        prune.Parameters.AddWithValue("@AccountId", accountId);
                        prune.ExecuteNonQuery();
                    }
                }

                pnlForm.Visible = false;
                ShowMsg("Password updated successfully ✅ You can now log in.", false);
            }
            catch (Exception ex)
            {
                ShowMsg("Unable to reset password. Please try again later.", true);
            }
        }

        // ✅ REAL token validation logic using Accounts.ResetTokenHash/ResetTokenExpiry
        private int GetAccountIdFromResetToken()
        {
            string token = Request.QueryString["t"];
            if (string.IsNullOrWhiteSpace(token)) return -1;

            string tokenHash = ActivationHelper.Sha256Hex(token);

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 Id
FROM Accounts
WHERE ResetTokenHash = @H
  AND ResetTokenExpiry IS NOT NULL
  AND ResetTokenExpiry >= GETDATE();", conn))
            {
                cmd.Parameters.AddWithValue("@H", tokenHash);

                conn.Open();
                object obj = cmd.ExecuteScalar();
                if (obj == null || obj == DBNull.Value) return -1;

                return Convert.ToInt32(obj);
            }
        }

        private void ShowMsg(string msg, bool isError)
        {
            pnlMsg.Visible = true;
            pnlMsg.CssClass = isError
                ? "alert alert-danger text-start mt-3"
                : "alert alert-success text-start mt-3";

            if (!isError)
            {
                // ✅ add login link button
                msg += "<div class='mt-2'>" +
                       "<a class='btn btn-dark btn-sm' href='Login.aspx'>Go to Login</a>" +
                       "</div>";
            }

            lblMsg.Text = msg;
        }


        private void HideMsg()
        {
            pnlMsg.Visible = false;
            lblMsg.Text = "";
        }
    }
}
