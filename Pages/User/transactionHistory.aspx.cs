using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmashZone.Pages.User
{
    public partial class transactionHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadUserTransactions();
        }

        private void LoadUserTransactions()
        {
            try
            {
                if (Session["AccountId"] == null)
                {
                    ShowError("You must be logged in to view transaction history.");
                    return;
                }

                int accountId = Convert.ToInt32(Session["AccountId"]);
                string connStr = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(@"
SELECT
    -- Change CreatedAt column name here if yours is different
    CONVERT(varchar(19), CreatedAt, 120) AS CreatedAt,
    StripeSessionId,
    AmountTotal,
    Status,
    ReceiptUrl
FROM dbo.Transactions
WHERE AccountId = @AccountId
ORDER BY CreatedAt DESC;", conn))
                {
                    cmd.Parameters.Add("@AccountId", SqlDbType.Int).Value = accountId;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                        {
                            pnlEmpty.Visible = true;
                            gvTx.DataSource = null;
                            gvTx.DataBind();
                            return;
                        }

                        pnlEmpty.Visible = false;

                        // Format amount nicely (SGD)
                        foreach (DataRow r in dt.Rows)
                        {
                            decimal amt = 0m;
                            decimal.TryParse(r["AmountTotal"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out amt);
                            r["AmountTotal"] = amt.ToString("0.00", CultureInfo.InvariantCulture);
                        }

                        gvTx.DataSource = dt;
                        gvTx.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Server error: " + Server.HtmlEncode(ex.Message));
            }
        }

        protected void gvTx_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            // Receipt hyperlink binding
            string receiptUrl = DataBinder.Eval(e.Row.DataItem, "ReceiptUrl")?.ToString();

            var lnk = (HyperLink)e.Row.FindControl("lnkReceipt");
            if (lnk != null)
            {
                if (string.IsNullOrWhiteSpace(receiptUrl))
                {
                    lnk.Text = "Unavailable";
                    lnk.NavigateUrl = "#";
                    lnk.Enabled = false;
                }
                else
                {
                    lnk.Text = "Open";
                    lnk.NavigateUrl = receiptUrl;
                }
            }
        }

        private void ShowError(string msg)
        {
            pnlErr.Visible = true;
            lblErr.Text = "❌ " + msg;
        }
    }
}
