using System;
using System.Globalization;
using System.Data;
using SmashZone.App_Code;

namespace SmashZone.Pages.User
{
    public partial class productDetails : SmashZone.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1) Validate id
                string idStr = Request.QueryString["id"];
                if (!int.TryParse(idStr, out int id) || id <= 0)
                {
                    ShowError("Invalid product link (missing id).");
                    return;
                }

                hfId.Value = id.ToString();

                // 2) Load product
                LoadProduct(id);

                // 3) Load ratings
                LoadRatings(id);

                // 4) Lock review UI if not logged in
                ApplyReviewAccessUI();
            }
        }

        // ================= LOGIN CHECK =================
        private bool IsUserLoggedIn()
        {
            // Change this if your login uses different session key
            return Session["AccountId"] != null;
        }

        private bool EnsureLoggedIn(string actionText)
        {
            if (IsUserLoggedIn())
                return true;

            lblMsg.ForeColor = System.Drawing.Color.Red;
            lblMsg.Text = "❌ You need to login to " + actionText + ".";
            return false;
        }

        // ================= LOAD PRODUCT =================
        private void LoadProduct(int id)
        {
            Product p = null;

            try
            {
                p = Product.GetById(id);
            }
            catch (Exception ex)
            {
                ShowError("Database error loading product: " + Server.HtmlEncode(ex.Message));
                return;
            }

            if (p == null)
            {
                ShowError("Product not found (id=" + id + ").");
                return;
            }

            lblTitle.Text = p.ProductTitle;
            lblCrumbTitle.Text = p.ProductTitle;

            lblPrice.Text = "$" + p.ProductPrice.ToString("0.00", CultureInfo.InvariantCulture);

            string desc = p.ProductDescription ?? "";
            lblDesc.Text = Server.HtmlEncode(desc)
                .Replace("\r\n", "<br/>")
                .Replace("\n", "<br/>");

            lblCategory.Text = Server.HtmlEncode(p.ProductCategory ?? "");
            lblSport.Text = Server.HtmlEncode(p.Sport ?? "");

            lblType.Text = "product";

            // ================= IMAGE =================
            hfImage.Value = p.ProductImage ?? "";

            imgProduct.ImageUrl = string.IsNullOrWhiteSpace(p.ProductImage)
                ? ResolveUrl("~/Images/no-image.png")
                : ResolveUrl("~/" + p.ProductImage);

            // ================= STOCK =================
            if (p.ProductStock > 0)
            {
                lblStockText.Text = "In stock, ready to ship";
                stockDot.Attributes["class"] = "dot dot-green";
            }
            else
            {
                lblStockText.Text = "Out of stock";
                stockDot.Attributes["class"] = "dot dot-red";
            }

            lblMsg.Text = "";
        }

        private void ShowError(string msg)
        {
            lblMsg.ForeColor = System.Drawing.Color.Red;
            lblMsg.Text = "❌ " + msg;

            btnAddToCart.Enabled = false;
            btnWishlist.Enabled = false;
            btnSubmitReview.Enabled = false;
        }

        // ================= ADD TO CART =================
        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (!EnsureLoggedIn("add items to your cart"))
                return;

            int id = int.Parse(hfId.Value);
            string title = lblTitle.Text;
            string image = hfImage.Value;

            int qty = 1;
            int.TryParse(txtQty.Text.Trim(), out qty);
            if (qty <= 0) qty = 1;

            decimal price = 0m;
            string priceText = (lblPrice.Text ?? "").Replace("$", "").Trim();
            decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out price);

            DataTable cart = Session["Cart"] as DataTable;

            if (cart == null)
            {
                cart = new DataTable();
                cart.Columns.Add("Id", typeof(int));
                cart.Columns.Add("Title", typeof(string));
                cart.Columns.Add("Image", typeof(string));
                cart.Columns.Add("Qty", typeof(int));
                cart.Columns.Add("Price", typeof(decimal));
                Session["Cart"] = cart;
            }

            DataRow existing = null;
            foreach (DataRow r in cart.Rows)
            {
                if ((int)r["Id"] == id)
                {
                    existing = r;
                    break;
                }
            }

            if (existing != null)
            {
                existing["Qty"] = (int)existing["Qty"] + qty;
                existing["Price"] = price;
            }
            else
            {
                var row = cart.NewRow();
                row["Id"] = id;
                row["Title"] = title;
                row["Image"] = image;
                row["Qty"] = qty;
                row["Price"] = price;
                cart.Rows.Add(row);
            }

            lblMsg.ForeColor = System.Drawing.Color.Green;
            lblMsg.Text = "✅ Added to cart!";
        }

        // ================= WISHLIST =================
        protected void btnWishlist_Click(object sender, EventArgs e)
        {
            if (!EnsureLoggedIn("add items to your wishlist"))
                return;

            int id = int.Parse(hfId.Value);

            string key = "All_Products:" + id;
            var list = Session["Wishlist"] as System.Collections.Generic.HashSet<string>;
            if (list == null)
            {
                list = new System.Collections.Generic.HashSet<string>();
                Session["Wishlist"] = list;
            }

            list.Add(key);

            lblMsg.ForeColor = System.Drawing.Color.Green;
            lblMsg.Text = "✅ Added to wishlist!";
        }

        // ================= RATINGS UI ACCESS =================
        private void ApplyReviewAccessUI()
        {
            bool loggedIn = IsUserLoggedIn();

            ddlRating.Enabled = loggedIn;
            txtReview.Enabled = loggedIn;
            btnSubmitReview.Enabled = loggedIn;

            if (!loggedIn)
            {
                lblRatingMsg.ForeColor = System.Drawing.Color.Red;
                lblRatingMsg.Text = "❌ Please login to leave a review.";
            }
            else
            {
                lblRatingMsg.Text = "";
            }
        }

        // ================= LOAD RATINGS =================
        private void LoadRatings(int productId)
        {
            int currentId = 0;
            if (Session["AccountId"] != null)
                int.TryParse(Session["AccountId"].ToString(), out currentId);

            // Summary
            var summary = ProductReview.GetSummary(productId);
            lblAvgRatingBottom.Text = (summary.avg <= 0m) ? "0.0" : summary.avg.ToString("0.0");
            lblRatingCountBottom.Text = summary.count.ToString();
            lblStarsBottom.Text = ProductReview.MakeStarsFromAverage(summary.avg);

            // List
            var dt = ProductReview.GetReviewsForProduct(productId, currentId);
            rptReviews.DataSource = dt;
            rptReviews.DataBind();

            pnlNoReviews.Visible = (dt.Rows.Count == 0);
        }

        // ================= SUBMIT REVIEW =================
        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            if (!EnsureLoggedIn("leave a review"))
            {
                ApplyReviewAccessUI();
                return;
            }

            int productId = int.Parse(hfId.Value);
            int accountId = int.Parse(Session["AccountId"].ToString());

            int rating = 5;
            int.TryParse(ddlRating.SelectedValue, out rating);
            if (rating < 1 || rating > 5) rating = 5;

            string comment = (txtReview.Text ?? "").Trim();
            if (comment.Length > 1000) comment = comment.Substring(0, 1000);

            try
            {
                ProductReview.UpsertReview(productId, accountId, rating, comment);

                lblRatingMsg.ForeColor = System.Drawing.Color.Green;
                lblRatingMsg.Text = "✅ Review submitted!";
                txtReview.Text = "";
            }
            catch (Exception ex)
            {
                lblRatingMsg.ForeColor = System.Drawing.Color.Red;
                lblRatingMsg.Text = "❌ Error saving review: " + Server.HtmlEncode(ex.Message);
            }

            LoadRatings(productId);
            ApplyReviewAccessUI();
        }
    }
}
