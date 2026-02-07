using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmashZone.Master_Pages
{
    public partial class UserLogin : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Use the same session key as your pages
            bool loggedIn = Session["AccountId"] != null;

            var hf = FindControl("hfIsLoggedIn") as HiddenField;
            if (hf != null) hf.Value = loggedIn ? "1" : "0";

            // ✅ MUST run every request (including postbacks)
            UpdateCartBadge();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            // ✅ runs AFTER all click events (ItemCommand, btn clicks)
            UpdateCartBadge();
        }

        // ✅ Call this from content pages after you modify Session["Cart"]
        public void RefreshCartBadge()
        {
            UpdateCartBadge();

            // Force UpdatePanel refresh during async postback
            if (upCartBadge != null)
                upCartBadge.Update();
        }

        private void UpdateCartBadge()
        {
            int count = 0;

            DataTable cart = Session["Cart"] as DataTable;

            if (cart != null)
            {
                // ✅ number of unique products in cart (rows)
                count = cart.Rows.Count;
            }

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
