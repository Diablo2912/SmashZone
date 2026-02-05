using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using Stripe;
using Stripe.Checkout;

namespace SmashZone.Pages.User
{
    public partial class checkout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindSummary();
        }

        // ================= ORDER SUMMARY =================
        private void BindSummary()
        {
            var cart = Session["Cart"] as DataTable;

            if (cart == null || cart.Rows.Count == 0)
            {
                // No cart -> redirect
                Response.Redirect("~/Pages/User/Cart.aspx");
                return;
            }

            rptSummary.DataSource = cart;
            rptSummary.DataBind();

            decimal total = 0m;
            foreach (DataRow r in cart.Rows)
            {
                decimal price = Convert.ToDecimal(r["Price"], CultureInfo.InvariantCulture);
                int qty = Convert.ToInt32(r["Qty"]);
                total += price * qty;
            }

            lblTotal.Text = total.ToString("0.00", CultureInfo.InvariantCulture);
        }

        // ================= PAY BUTTON =================
        protected void btnPay_Click(object sender, EventArgs e)
        {
            pnlErr.Visible = false;
            lblErr.Text = "";

            try
            {
                var cart = Session["Cart"] as DataTable;
                if (cart == null || cart.Rows.Count == 0)
                {
                    ShowErr("Your cart is empty.");
                    return;
                }

                // Load Stripe secret key
                string secretKey = ConfigurationManager.AppSettings["StripeSecretKey"];
                if (string.IsNullOrWhiteSpace(secretKey))
                {
                    ShowErr("StripeSecretKey missing in web.config.");
                    return;
                }

                StripeConfiguration.ApiKey = secretKey;

                // ================= URLs =================
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                // ✅ REQUIRED: session_id token
                string successUrl = baseUrl +
                    ResolveUrl("~/Pages/User/checkoutSuccess.aspx") +
                    "?session_id={CHECKOUT_SESSION_ID}";

                string cancelUrl = baseUrl +
                    ResolveUrl("~/Pages/User/checkout_cancel.aspx");

                // ================= LINE ITEMS =================
                var lineItems = new System.Collections.Generic.List<SessionLineItemOptions>();

                foreach (DataRow r in cart.Rows)
                {
                    string title = Convert.ToString(r["Title"]);
                    string image = Convert.ToString(r["Image"]);
                    decimal price = Convert.ToDecimal(r["Price"], CultureInfo.InvariantCulture);
                    int qty = Convert.ToInt32(r["Qty"]);

                    long unitAmount = (long)Math.Round(
                        price * 100m,
                        0,
                        MidpointRounding.AwayFromZero
                    );

                    var item = new SessionLineItemOptions
                    {
                        Quantity = qty,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "sgd",
                            UnitAmount = unitAmount,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = title,
                                Images = string.IsNullOrWhiteSpace(image)
                                    ? null
                                    : new System.Collections.Generic.List<string>
                                    {
                                        baseUrl + ResolveUrl("~/" + image)
                                    }
                            }
                        }
                    };

                    lineItems.Add(item);
                }

                // ================= CREATE CHECKOUT SESSION =================
                var options = new SessionCreateOptions
                {
                    Mode = "payment",
                    PaymentMethodTypes = new System.Collections.Generic.List<string> { "card" },
                    LineItems = lineItems,
                    SuccessUrl = successUrl,
                    CancelUrl = cancelUrl,

                    // ✅ Enable promo code entry on Stripe Checkout page
                    AllowPromotionCodes = true,

                    // ✅ Pass logged-in email to Stripe (for receipt + success page)
                    CustomerEmail = Session["Email"]?.ToString()
                };

                var service = new SessionService();
                Session stripeSession = service.Create(options);

                // Redirect to Stripe Checkout
                Response.Redirect(stripeSession.Url, false);
            }
            catch (StripeException se)
            {
                ShowErr("Stripe error: " + Server.HtmlEncode(se.Message));
            }
            catch (Exception ex)
            {
                ShowErr("Server error: " + Server.HtmlEncode(ex.Message));
            }
        }

        // ================= ERROR UI =================
        private void ShowErr(string msg)
        {
            pnlErr.Visible = true;
            lblErr.Text = msg;
        }
    }
}
