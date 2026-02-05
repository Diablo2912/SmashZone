using System;
using System.Data;
using System.Globalization;

namespace SmashZone.Pages.User
{
    public partial class Cart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindCart();
        }

        private void BindCart()
        {
            DataTable cart = Session["Cart"] as DataTable;

            if (cart == null || cart.Rows.Count == 0)
            {
                pnlEmpty.Visible = true;
                pnlCart.Visible = false;
                lblGrandTotal.Text = "0.00";
                return;
            }

            pnlEmpty.Visible = false;
            pnlCart.Visible = true;

            rptCart.DataSource = cart;
            rptCart.DataBind();

            decimal total = 0m;
            foreach (DataRow r in cart.Rows)
            {
                decimal price = Convert.ToDecimal(r["Price"]);
                int qty = Convert.ToInt32(r["Qty"]);
                total += price * qty;
            }

            lblGrandTotal.Text = total.ToString("0.00", CultureInfo.InvariantCulture);
        }

        protected void rptCart_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            DataTable cart = Session["Cart"] as DataTable;
            if (cart == null) return;

            int id = Convert.ToInt32(e.CommandArgument);

            DataRow row = null;
            foreach (DataRow r in cart.Rows)
            {
                if (Convert.ToInt32(r["Id"]) == id)
                {
                    row = r;
                    break;
                }
            }
            if (row == null) return;

            if (e.CommandName == "remove")
            {
                cart.Rows.Remove(row);
            }
            else if (e.CommandName == "inc")
            {
                int qty = Convert.ToInt32(row["Qty"]);
                row["Qty"] = qty + 1;
            }
            else if (e.CommandName == "dec")
            {
                int qty = Convert.ToInt32(row["Qty"]);
                qty--;

                // if qty becomes 0, remove it
                if (qty <= 0)
                    cart.Rows.Remove(row);
                else
                    row["Qty"] = qty;
            }

            Session["Cart"] = cart;
            BindCart();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Session["Cart"] = null;
            BindCart();
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            DataTable cart = Session["Cart"] as DataTable;

            if (cart == null || cart.Rows.Count == 0)
            {
                pnlEmpty.Visible = true;
                pnlCart.Visible = false;
                lblGrandTotal.Text = "0.00";
                return;
            }

            // For now: just redirect to a checkout page (create later)
            // OR you can show a message / do DB insert for order here.
            Response.Redirect("~/Pages/User/Checkout.aspx");
        }
    }
}
