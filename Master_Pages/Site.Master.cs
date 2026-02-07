using System;
using System.Data;
using System.Web.UI;

namespace SmashZone
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        // ✅ Runs AFTER all button clicks (no 1-click lag)
        protected void Page_PreRender(object sender, EventArgs e)
        {
        }


        protected void btnNavSearch_Click(object sender, EventArgs e)
        {
            var tb = FindControl("txtNavSearch") as System.Web.UI.WebControls.TextBox;
            if (tb == null) return;

            string q = (tb.Text ?? "").Trim();
            if (q.Length == 0) return;

            Response.Redirect("~/Pages/User/search.aspx?q=" + Server.UrlEncode(q));
        }

    }
}
