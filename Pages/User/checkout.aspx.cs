using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using Stripe;
using Stripe.Checkout;

namespace SmashZone.Pages.User
{
    public partial class checkout : SmashZone.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindSummary();
        }

        private void BindSummary()
        {
            var cart = Session["Cart"] as DataTable;
            if (cart == null || cart.Rows.Count == 0)
            {
                Response.Redirect("~/Pages/User/Cart.aspx");
                return;
            }

            rptSummary.DataSource = cart;
            rptSummary.DataBind();

            decimal total = 0m;
            foreach (DataRow r in cart.Rows)
                total += Convert.ToDecimal(r["Price"]) * Convert.ToInt32(r["Qty"]);

            lblTotal.Text = total.ToString("0.00", CultureInfo.InvariantCulture);
        }

        protected void btnPay_Click(object sender, EventArgs e)
        {
            try
            {
                pnlErr.Visible = false;
                lblErr.Text = "";

                var cart = Session["Cart"] as DataTable;
                if (cart == null || cart.Rows.Count == 0)
                {
                    pnlErr.Visible = true;
                    lblErr.Text = "Cart is empty.";
                    return;
                }

                // Stripe key
                string secretKey = ConfigurationManager.AppSettings["StripeSecretKey"];
                if (string.IsNullOrWhiteSpace(secretKey))
                    throw new Exception("StripeSecretKey missing in Web.config AppSettings.");

                StripeConfiguration.ApiKey = secretKey;

                // Build absolute domain (works for localhost too)
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                // Build line items
                var lineItems = new List<SessionLineItemOptions>();

                foreach (DataRow r in cart.Rows)
                {
                    string title = r["Title"]?.ToString() ?? "Item";
                    int qty = Convert.ToInt32(r["Qty"]);
                    decimal price = Convert.ToDecimal(r["Price"]); // in SGD dollars

                    // mapping
                    string sport = r.Table.Columns.Contains("Sport") ? (r["Sport"]?.ToString() ?? "") : "";
                    string sourceTable = r.Table.Columns.Contains("SourceTable") ? (r["SourceTable"]?.ToString() ?? "") : "";
                    int sourceProductId = r.Table.Columns.Contains("SourceProductId") ? Convert.ToInt32(r["SourceProductId"]) : 0;

                    if (string.IsNullOrWhiteSpace(sourceTable) || sourceProductId <= 0)
                        throw new Exception($"Missing mapping for '{title}' (SourceTable/SourceProductId).");

                    long unitAmountCents = (long)Math.Round(price * 100m, 0);

                    lineItems.Add(new SessionLineItemOptions
                    {
                        Quantity = qty,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "sgd",
                            UnitAmount = unitAmountCents,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = title,
                                Metadata = new Dictionary<string, string>
                                {
                                    { "sport", sport ?? "" },
                                    { "sourceTable", sourceTable },
                                    { "sourceProductId", sourceProductId.ToString() }
                                }
                            }
                        }
                    });
                }

                var options = new SessionCreateOptions
                {
                    Mode = "payment",
                    PaymentMethodTypes = new List<string> { "card" },

                    LineItems = lineItems,

                    SuccessUrl = baseUrl + "/Pages/User/checkoutSuccess.aspx?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = baseUrl + "/Pages/User/Cart.aspx",

                    // Optional: helps you link to your user
                    ClientReferenceId = (Session["AccountId"] != null) ? Session["AccountId"].ToString() : null
                };

                var service = new SessionService();
                Session stripeSession = service.Create(options);

                // Redirect user to Stripe Checkout
                Response.Redirect(stripeSession.Url, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                pnlErr.Visible = true;
                lblErr.Text = "❌ " + Server.HtmlEncode(ex.Message);
            }
        }
    }
}
