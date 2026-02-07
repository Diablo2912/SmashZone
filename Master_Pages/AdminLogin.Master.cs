using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmashZone.Master_Pages
{
    public partial class AdminLogin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
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