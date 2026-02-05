using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using Stripe;
using Stripe.Checkout;
using SmashZone.App_Code;

namespace SmashZone.Pages.User
{
    public partial class checkoutSuccess : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadAndSaveTransaction();
        }

        private void LoadAndSaveTransaction()
        {
            try
            {
                // ================= SESSION ID =================
                string sessionId = Request.QueryString["session_id"];
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    ShowError("Missing session_id.");
                    return;
                }

                // ================= STRIPE =================
                string secretKey = ConfigurationManager.AppSettings["StripeSecretKey"];
                if (string.IsNullOrWhiteSpace(secretKey))
                {
                    ShowError("StripeSecretKey missing.");
                    return;
                }

                StripeConfiguration.ApiKey = secretKey;

                var service = new SessionService();
                var session = service.Get(sessionId, new SessionGetOptions
                {
                    Expand = new List<string> { "payment_intent", "line_items" }
                });

                // ================= PAYMENT CHECK =================
                if (!string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase))
                {
                    ShowError("Payment not completed. Status: " + session.PaymentStatus);
                    return;
                }

                // ================= TOTAL =================
                decimal total = (session.AmountTotal ?? 0) / 100m;

                lblOrderId.Text = session.Id;
                lblAmount.Text = "$" + total.ToString("0.00", CultureInfo.InvariantCulture) + " SGD";

                // ================= RECEIPT =================
                string receiptUrl = "";

                if (!string.IsNullOrWhiteSpace(session.PaymentIntentId))
                {
                    var chargeService = new ChargeService();
                    var charges = chargeService.List(new ChargeListOptions
                    {
                        PaymentIntent = session.PaymentIntentId,
                        Limit = 1
                    });

                    if (charges.Data.Count > 0)
                        receiptUrl = charges.Data[0].ReceiptUrl;
                }

                lnkReceipt.NavigateUrl = string.IsNullOrWhiteSpace(receiptUrl) ? "#" : receiptUrl;
                lnkReceipt.Text = string.IsNullOrWhiteSpace(receiptUrl)
                    ? "Receipt unavailable"
                    : "Open Stripe receipt";

                // ================= SAVE TO DB =================
                SaveTransaction(session, total, receiptUrl);

                // ================= EMAIL =================
                string email = session.CustomerDetails?.Email;

                if (!string.IsNullOrWhiteSpace(email))
                {
                    Email.SendReceiptEmail(
                        email,
                        session.Id,
                        total.ToString("0.00", CultureInfo.InvariantCulture),
                        receiptUrl,
                        BuildItemsHtml(session)
                    );

                    ShowEmailStatus("✅ Receipt emailed to " + Server.HtmlEncode(email), false);
                }
                else
                {
                    ShowEmailStatus("⚠️ No email received from Stripe.", true);
                }

                pnlOk.Visible = true;
                pnlErr.Visible = false;

                // ================= CLEAR CART =================
                Session["Cart"] = null;
            }
            catch (StripeException se)
            {
                ShowError("Stripe error: " + Server.HtmlEncode(se.Message));
            }
            catch (Exception ex)
            {
                ShowError("Server error: " + Server.HtmlEncode(ex.Message));
            }
        }

        // ================= SAVE TRANSACTION =================
        private void SaveTransaction(Session session, decimal total, string receiptUrl)
        {
            if (Session["AccountId"] == null)
                return;

            int accountId = Convert.ToInt32(Session["AccountId"]);

            string connStr = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
IF NOT EXISTS (SELECT 1 FROM dbo.Transactions WHERE StripeSessionId = @StripeSessionId)
BEGIN
    INSERT INTO dbo.Transactions
    (AccountId, StripeSessionId, PaymentIntentId, ReceiptUrl, Currency, AmountTotal, Status)
    VALUES
    (@AccountId, @StripeSessionId, @PaymentIntentId, @ReceiptUrl, @Currency, @AmountTotal, @Status)
END";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountId", accountId);
                    cmd.Parameters.AddWithValue("@StripeSessionId", session.Id);
                    cmd.Parameters.AddWithValue("@PaymentIntentId",
                        (object)session.PaymentIntentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReceiptUrl",
                        (object)receiptUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Currency", session.Currency ?? "sgd");
                    cmd.Parameters.AddWithValue("@AmountTotal", total);
                    cmd.Parameters.AddWithValue("@Status", session.PaymentStatus ?? "unknown");

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ================= ITEMS HTML =================
        private string BuildItemsHtml(Session session)
        {
            var html = "<ul>";

            if (session.LineItems?.Data != null)
            {
                foreach (var li in session.LineItems.Data)
                {
                    string name = li.Description ?? "Item";
                    int qty = (int)(li.Quantity ?? 1);
                    decimal unit = li.Price?.UnitAmount != null
                        ? li.Price.UnitAmount.Value / 100m
                        : 0m;

                    html += "<li>" +
                        Server.HtmlEncode(name) +
                        " — Qty " + qty +
                        " — $" + unit.ToString("0.00", CultureInfo.InvariantCulture) +
                        " SGD</li>";
                }
            }

            html += "</ul>";
            return html;
        }

        // ================= UI HELPERS =================
        private void ShowError(string msg)
        {
            pnlOk.Visible = false;
            pnlErr.Visible = true;
            lblErr.Text = "❌ " + msg;
        }

        private void ShowEmailStatus(string msg, bool warn)
        {
            pnlEmailStatus.Visible = true;
            pnlEmailStatus.CssClass = warn
                ? "alert alert-warning mt-3"
                : "alert alert-success mt-3";
            lblEmailStatus.Text = msg;
        }
    }
}
