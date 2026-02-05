using System;
using System.Web.UI;

namespace SmashZone
{
    public class BasePage : Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            // Not logged in
            if (Session["AccountId"] == null)
            {
                MasterPageFile = "~/Master_Pages/Site.Master";
            }
            else
            {
                // Logged in: check role
                string role = Session["Role"]?.ToString();

                if (role == "Admin")
                    MasterPageFile = "~/Master_Pages/AdminLogin.Master";
                else
                    MasterPageFile = "~/Master_Pages/UserLogin.Master";
            }

            base.OnPreInit(e);
        }
    }
}
