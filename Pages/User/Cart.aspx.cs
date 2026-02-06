using System;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;

namespace SmashZone.Pages.User
{
    public partial class Cart : SmashZone.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCart();
            }
        }

        // ================= BIND CART =================
        private void BindCart()
        {
            DataTable cart = Session["Cart"] as DataTable;

            if (cart == null || cart.Rows.Count == 0)
            {
                pnlCart.Visible = false;
                pnlEmpty.Visible = true;
                lblGrandTotal.Text = "0.00";
                return;
            }

            pnlEmpty.Visible = false;
            pnlCart.Visible = true;

            rptCart.DataSource = cart;
            rptCart.DataBind();

            // Calculate grand total
            decimal total = 0m;
            foreach (DataRow r in cart.Rows)
            {
                decimal price = Convert.ToDecimal(r["Price"]);
                int qty = Convert.ToInt32(r["Qty"]);
                total += price * qty;
            }

            lblGrandTotal.Text = total.ToString("0.00", CultureInfo.InvariantCulture);
        }

        // ================= REPEATER COMMANDS =================
        protected void rptCart_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            DataTable cart = Session["Cart"] as DataTable;
            if (cart == null) return;

            // CommandArgument format: "Badminton_Products|12"
            string arg = e.CommandArgument?.ToString() ?? "";
            string[] parts = arg.Split('|');
            if (parts.Length != 2) return;

            string sourceTable = parts[0];
            if (!int.TryParse(parts[1], out int sourceProductId)) return;

            DataRow row = null;
            foreach (DataRow r in cart.Rows)
            {
                string rTbl = r["SourceTable"]?.ToString() ?? "";
                int rPid = Convert.ToInt32(r["SourceProductId"]);

                if (string.Equals(rTbl, sourceTable, StringComparison.OrdinalIgnoreCase)
                    && rPid == sourceProductId)
                {
                    row = r;
                    break;
                }
            }

            if (row == null) return;

            int qty = Convert.ToInt32(row["Qty"]);

            switch (e.CommandName)
            {
                case "inc":
                    row["Qty"] = qty + 1;
                    break;

                case "dec":
                    qty--;
                    if (qty <= 0)
                        cart.Rows.Remove(row);
                    else
                        row["Qty"] = qty;
                    break;

                case "remove":
                    cart.Rows.Remove(row);
                    break;
            }

            Session["Cart"] = cart;
            BindCart();
        }

        // ================= CLEAR CART =================
        protected void btnClear_Click(object sender, EventArgs e)
        {
            Session.Remove("Cart");
            BindCart();
        }

        // ================= CHECKOUT =================
        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            if (Session["AccountId"] == null)
            {
                // redirect to login or show message
                Response.Redirect("~/Pages/User/Login.aspx?returnUrl=Cart");
                return;
            }

            DataTable cart = Session["Cart"] as DataTable;
            if (cart == null || cart.Rows.Count == 0)
            {
                BindCart();
                return;
            }

            // Final safety check: ensure all rows have mapping
            foreach (DataRow r in cart.Rows)
            {
                string sourceTable = r["SourceTable"]?.ToString();
                int sourceProductId = Convert.ToInt32(r["SourceProductId"]);

                if (string.IsNullOrWhiteSpace(sourceTable) || sourceProductId <= 0)
                {
                    // Mapping broken → block checkout
                    lblGrandTotal.Text = "0.00";
                    return;
                }
            }

            // Proceed to checkout page (Stripe / order summary)
            Response.Redirect("~/Pages/User/Checkout.aspx");
        }
    }
}
