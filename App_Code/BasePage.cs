using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using SmashZone.App_Code;

namespace SmashZone.App_Code
{
    public class BasePage : Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            if (Session["AccountId"] != null)
                MasterPageFile = "~/Master_Pages/UserLogin.Master";
            else
                MasterPageFile = "~/Master_Pages/Site.Master";

            base.OnPreInit(e);
        }
    }
}