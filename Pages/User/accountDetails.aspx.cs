using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace SmashZone.Pages.User
{
    public partial class accountDetails : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Must be logged in
                if (Session["AccountId"] == null ||
                    !int.TryParse(Session["AccountId"].ToString(), out _))
                {
                    Response.Redirect("~/Pages/User/Login.aspx");
                    return;
                }

                LoadAccountDetails();
            }
        }

        private void LoadAccountDetails()
        {
            int accountId = Convert.ToInt32(Session["AccountId"]);

            string connStr =
                ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            string sql = @"
                SELECT TOP 1
                    Id,
                    First_Name,
                    Last_Name,
                    Email,
                    Phone_Number,
                    Role,
                    Two_Factor_Status
                FROM Accounts
                WHERE Id = @Id;
            ";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", accountId);
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                        {
                            lblMsg.Text = "Account not found.";
                            return;
                        }

                        // ===== BASIC INFO =====
                        lblAccountId.Text = dr["Id"].ToString();
                        lblFirstName.Text = dr["First_Name"].ToString();
                        lblLastName.Text = dr["Last_Name"].ToString();
                        lblEmail.Text = dr["Email"].ToString();
                        lblEmailTop.Text = dr["Email"].ToString();
                        lblPhone.Text = dr["Phone_Number"].ToString();
                        lblRole.Text = dr["Role"].ToString();

                        lblFullName.Text =
                            (lblFirstName.Text + " " + lblLastName.Text).Trim();

                        if (string.IsNullOrWhiteSpace(lblFullName.Text))
                            lblFullName.Text = "User";

                        // ===== 2FA STATUS =====
                        bool is2FAEnabled =
                            Convert.ToBoolean(dr["Two_Factor_Status"]);

                        lbl2FA.Text = is2FAEnabled ? "ON" : "OFF";

                        // Badge colour
                        badge2FA.Attributes["class"] =
                            is2FAEnabled
                                ? "badge bg-success text-white px-3 py-2"
                                : "badge bg-danger text-white px-3 py-2";

                        // Button visibility
                        btnEnable2FA.Visible = !is2FAEnabled;
                        btnDisable2FA.Visible = is2FAEnabled;

                        // Optional static fields
                        lblStatus.Text = "Active";
                    }
                }
            }
            catch (SqlException ex)
            {
                lblMsg.Text = "SQL Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
            }
        }

        // ==================== 2FA BUTTONS ====================

        protected void btnEnable2FA_Click(object sender, EventArgs e)
        {
            Update2FA(true);
        }

        protected void btnDisable2FA_Click(object sender, EventArgs e)
        {
            Update2FA(false);
        }

        private void Update2FA(bool enable)
        {
            int accountId = Convert.ToInt32(Session["AccountId"]);

            string connStr =
                ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            string sql = @"
                UPDATE Accounts
                SET Two_Factor_Status = @Status
                WHERE Id = @Id;
            ";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // BIT column → pass bool directly
                    cmd.Parameters.AddWithValue("@Status", enable);
                    cmd.Parameters.AddWithValue("@Id", accountId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                // Reload UI so badge & buttons update
                LoadAccountDetails();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Unable to update 2FA: " + ex.Message;
            }
        }

        // ==================== LOGOUT ====================

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Pages/User/Login.aspx");
        }
    }
}
