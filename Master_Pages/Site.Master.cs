using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmashZone
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Only show cart badge if user is logged in
                bool loggedIn = Session["UserId"] != null;

                var hf = FindControl("hfIsLoggedIn") as HiddenField;
                if (hf != null) hf.Value = loggedIn ? "1" : "0";

                UpdateCartBadge();
            }
        }

        private void UpdateCartBadge()
        {
            // Cart stored in Session as List<CartItem>
            var cart = Session["Cart"] as List<CartItem>;
            int count = 0;

            if (cart != null)
            {
                foreach (var item in cart)
                {
                    count += item.Quantity;
                }
            }

            var lbl = FindControl("lblCartCount") as Label;
            if (lbl != null)
            {
                if (count > 0)
                {
                    lbl.Text = count.ToString();
                    lbl.Visible = true;
                }
                else
                {
                    lbl.Text = "";
                    lbl.Visible = false;
                }
            }
        }
    }

    // If your project already has this class elsewhere, remove this duplicate.
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public string ProductType { get; set; }
        public string ProductImage { get; set; }
    }
}
