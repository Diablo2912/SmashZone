using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmashZone
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // attach once
            this.Page.PreRender -= SiteMaster_PreRender;
            this.Page.PreRender += SiteMaster_PreRender;
        }

        private void SiteMaster_PreRender(object sender, EventArgs e)
        {
            UpdateCartBadge();
        }

        private void UpdateCartBadge()
        {
            // ✅ Find the badge only if it exists in THIS master
            Label badge = FindControl("lblCartCount") as Label;
            if (badge == null) return;

            DataTable cart = Session["Cart"] as DataTable;

            if (cart == null || cart.Rows.Count == 0)
            {
                badge.Text = "";
                badge.Style["display"] = "none";
                return;
            }

            int totalQty = 0;

            foreach (DataRow r in cart.Rows)
            {
                int q;
                if (int.TryParse(Convert.ToString(r["Qty"]), out q))
                    totalQty += q;
            }

            if (totalQty <= 0)
            {
                badge.Text = "";
                badge.Style["display"] = "none";
            }
            else
            {
                badge.Text = totalQty.ToString();
                badge.Style["display"] = "inline-block";
            }
        }

        protected void btnNavSearch_Click(object sender, EventArgs e)
        {
            // ✅ Find textbox only if it exists in THIS master
            TextBox tb = FindControl("txtNavSearch") as TextBox;

            string q = (tb?.Text ?? "").Trim();
            Response.Redirect("~/Pages/User/Search.aspx?q=" + Server.UrlEncode(q));
        }
    }
}
