// ======================= checkoutSuccess.aspx.cs (FULL CODE) =======================
// This version:
// ✅ saves transaction (with ItemsJson + TotalQty)
// ✅ deducts stock from the correct sport table + All_Products
// ✅ prevents double-deduction using Transactions.StockDeducted (idempotent)
// ✅ uses Stripe Product Metadata: sport, sourceTable, sourceProductId

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Script.Serialization;
using Stripe;
using Stripe.Checkout;
using SmashZone.App_Code;
using StripeProduct = Stripe.Product;

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
                    Expand = new List<string>
                    {
                        "payment_intent",
                        "line_items",
                        "line_items.data.price",
                        "line_items.data.price.product" // ✅ get Product + Metadata(sport/sourceTable/sourceProductId)
                    }
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

                // ================= BUILD ITEMS (JSON + LIST) =================
                var built = BuildItemsJson(session);
                string itemsJson = built.itemsJson;
                int totalQty = built.totalQty;
                List<TxItem> items = built.items;

                // ================= SAVE TO DB =================
                SaveTransaction(session, total, receiptUrl, itemsJson, totalQty);

                // ================= STOCK DEDUCTION (ONCE) =================
                DeductStockOnce(session.Id, items);

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

        // ========= item structure stored into Transactions.ItemsJson =========
        private class TxItem
        {
            public string name { get; set; }
            public string sport { get; set; }
            public int qty { get; set; }

            // ✅ NEW: exact mapping so we deduct correctly
            public string sourceTable { get; set; }     // e.g. "Badminton_Products"
            public int sourceProductId { get; set; }    // e.g. 12
        }

        // ========= build items json + items list from Stripe line items =========
        private (string itemsJson, int totalQty, List<TxItem> items) BuildItemsJson(Session session)
        {
            var items = new List<TxItem>();
            int totalQty = 0;

            if (session.LineItems?.Data != null)
            {
                foreach (var li in session.LineItems.Data)
                {
                    int qty = (int)(li.Quantity ?? 1);
                    totalQty += qty;

                    string name = li.Description ?? "Item";
                    string sport = "";
                    string sourceTable = "";
                    int sourceProductId = 0;

                    var product = li.Price?.Product as StripeProduct;
                    if (product != null)
                    {
                        if (!string.IsNullOrWhiteSpace(product.Name))
                            name = product.Name;

                        if (product.Metadata != null)
                        {
                            if (product.Metadata.ContainsKey("sport"))
                                sport = product.Metadata["sport"];

                            // ✅ product mapping metadata
                            if (product.Metadata.ContainsKey("sourceTable"))
                                sourceTable = product.Metadata["sourceTable"];

                            if (product.Metadata.ContainsKey("sourceProductId"))
                                int.TryParse(product.Metadata["sourceProductId"], out sourceProductId);
                        }
                    }

                    // fallback guess if no sport metadata
                    if (string.IsNullOrWhiteSpace(sport))
                    {
                        string lower = (name ?? "").ToLowerInvariant();
                        if (lower.Contains("badminton")) sport = "Badminton";
                        else if (lower.Contains("tennis")) sport = "Tennis";
                        else if (lower.Contains("squash")) sport = "Squash";
                        else sport = "Unknown";
                    }

                    items.Add(new TxItem
                    {
                        name = name,
                        sport = sport,
                        qty = qty,
                        sourceTable = sourceTable,
                        sourceProductId = sourceProductId
                    });
                }
            }

            var js = new JavaScriptSerializer();
            return (js.Serialize(items), totalQty, items);
        }

        // ================= SAVE TRANSACTION =================
        // Requires DB columns:
        //   ItemsJson NVARCHAR(MAX) NULL
        //   TotalQty  INT NOT NULL DEFAULT(0)
        //   StockDeducted BIT NOT NULL DEFAULT(0)
        private void SaveTransaction(Session session, decimal total, string receiptUrl, string itemsJson, int totalQty)
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
    (AccountId, StripeSessionId, PaymentIntentId, ReceiptUrl, Currency, AmountTotal, Status, ItemsJson, TotalQty, StockDeducted)
    VALUES
    (@AccountId, @StripeSessionId, @PaymentIntentId, @ReceiptUrl, @Currency, @AmountTotal, @Status, @ItemsJson, @TotalQty, 0)
END
ELSE
BEGIN
    UPDATE dbo.Transactions
    SET ReceiptUrl = @ReceiptUrl,
        PaymentIntentId = @PaymentIntentId,
        Status = @Status,
        AmountTotal = @AmountTotal,
        ItemsJson = @ItemsJson,
        TotalQty = @TotalQty
    WHERE StripeSessionId = @StripeSessionId;
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
                    cmd.Parameters.AddWithValue("@ItemsJson", (object)itemsJson ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TotalQty", totalQty);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ================= STOCK DEDUCTION (ONCE) =================
        private void DeductStockOnce(string stripeSessionId, List<TxItem> items)
        {
            if (items == null || items.Count == 0) return;

            string connStr = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    // Lock transaction row so refresh/back won't double deduct
                    bool alreadyDeducted;
                    using (var lockCmd = new SqlCommand(@"
SELECT StockDeducted
FROM dbo.Transactions WITH (UPDLOCK, HOLDLOCK)
WHERE StripeSessionId = @sid;", conn, tx))
                    {
                        lockCmd.Parameters.AddWithValue("@sid", stripeSessionId);
                        object val = lockCmd.ExecuteScalar();

                        if (val == null)
                        {
                            tx.Rollback();
                            throw new Exception("Transaction record not found for stock deduction.");
                        }

                        alreadyDeducted = Convert.ToBoolean(val);
                    }

                    if (alreadyDeducted)
                    {
                        tx.Commit();
                        return;
                    }

                    // whitelist tables (avoid SQL injection)
                    var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        "Badminton_Products",
                        "Tennis_Products",
                        "Squash_Products"
                    };

                    foreach (var it in items)
                    {
                        if (it.qty <= 0) continue;

                        if (string.IsNullOrWhiteSpace(it.sourceTable) || it.sourceProductId <= 0)
                        {
                            tx.Rollback();
                            throw new Exception($"Missing product mapping for '{it.name}'. (sourceTable/sourceProductId not found)");
                        }

                        if (!allowed.Contains(it.sourceTable))
                        {
                            tx.Rollback();
                            throw new Exception("Invalid source table detected.");
                        }

                        // get current stock with lock
                        int currentStock;
                        using (var getStockCmd = new SqlCommand($@"
SELECT ProductStock
FROM dbo.[{it.sourceTable}] WITH (UPDLOCK, ROWLOCK)
WHERE Id = @pid;", conn, tx))
                        {
                            getStockCmd.Parameters.AddWithValue("@pid", it.sourceProductId);
                            object s = getStockCmd.ExecuteScalar();

                            if (s == null)
                            {
                                tx.Rollback();
                                throw new Exception($"Product not found in {it.sourceTable} (Id={it.sourceProductId}).");
                            }

                            currentStock = Convert.ToInt32(s);
                        }

                        if (currentStock < it.qty)
                        {
                            tx.Rollback();
                            throw new Exception($"Not enough stock for '{it.name}'. Remaining: {currentStock}, needed: {it.qty}");
                        }

                        // deduct from sport table
                        using (var updCmd = new SqlCommand($@"
UPDATE dbo.[{it.sourceTable}]
SET ProductStock = ProductStock - @qty
WHERE Id = @pid;", conn, tx))
                        {
                            updCmd.Parameters.AddWithValue("@qty", it.qty);
                            updCmd.Parameters.AddWithValue("@pid", it.sourceProductId);
                            updCmd.ExecuteNonQuery();
                        }

                        // also keep All_Products in sync
                        using (var updAll = new SqlCommand(@"
UPDATE dbo.All_Products
SET ProductStock = ProductStock - @qty
WHERE SourceTable = @tbl AND SourceProductId = @pid;", conn, tx))
                        {
                            updAll.Parameters.AddWithValue("@qty", it.qty);
                            updAll.Parameters.AddWithValue("@tbl", it.sourceTable);
                            updAll.Parameters.AddWithValue("@pid", it.sourceProductId);
                            updAll.ExecuteNonQuery();
                        }
                    }

                    // mark as deducted
                    using (var markCmd = new SqlCommand(@"
UPDATE dbo.Transactions
SET StockDeducted = 1
WHERE StripeSessionId = @sid;", conn, tx))
                    {
                        markCmd.Parameters.AddWithValue("@sid", stripeSessionId);
                        markCmd.ExecuteNonQuery();
                    }

                    tx.Commit();
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
