using System;
using System.Data;
using System.Web.UI;

namespace SmashZone
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // ✅ Use ONE login key everywhere
            bool loggedIn = Session["AccountId"] != null;

            if (hfIsLoggedIn != null)
                hfIsLoggedIn.Value = loggedIn ? "1" : "0";
        }

        // ✅ Runs AFTER all button clicks (no 1-click lag)
        protected void Page_PreRender(object sender, EventArgs e)
        {
            UpdateCartBadge();
        }

        // Optional: call manually after AddToCart
        public void RefreshCartBadge()
        {
            UpdateCartBadge();
            if (upCartBadge != null)
                upCartBadge.Update();
        }

        private void UpdateCartBadge()
        {
            // 🔒 SAFETY: never crash again
            if (lblCartCount == null)
                return;

            DataTable cart = Session["Cart"] as DataTable;

            // ✅ DISTINCT ITEMS ONLY
            int count = (cart == null) ? 0 : cart.Rows.Count;

            if (count > 0)
            {
                lblCartCount.Text = count.ToString();
                lblCartCount.Visible = true;
                lblCartCount.Style["display"] = "inline-block";
            }
            else
            {
                lblCartCount.Text = "";
                lblCartCount.Visible = false;
                lblCartCount.Style["display"] = "none";
            }
        }

        protected void btnNavSearch_Click(object sender, EventArgs e)
        {
            string q = (txtNavSearch.Text ?? "").Trim();
            if (q.Length == 0) return;

            Response.Redirect("~/Pages/User/search.aspx?q=" + Server.UrlEncode(q));
        }

    }
}
