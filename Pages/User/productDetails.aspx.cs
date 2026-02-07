using System;
using System.Data;
using System.Globalization;
using System.Drawing;
using SmashZone.App_Code;
using SmashZone.Master_Pages;

namespace SmashZone.Pages.User
{
    public partial class productDetails : SmashZone.BasePage
    {
        // ================= PAGE LOAD =================
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string idStr = Request.QueryString["id"];
                int id;

                if (!int.TryParse(idStr, out id) || id <= 0)
                {
                    ShowError("Invalid product link.");
                    return;
                }

                hfId.Value = id.ToString();

                LoadProduct(id);
                LoadRatings(id);
                ApplyReviewAccessUI();
            }
        }

        // ================= LOGIN CHECK =================
        private bool IsUserLoggedIn()
        {
            return Session["AccountId"] != null;
        }

        // ================= ERROR =================
        private void ShowError(string msg)
        {
            lblMsg.ForeColor = Color.Red;
            lblMsg.Text = "❌ " + msg;

            btnAddToCart.Enabled = false;
            btnSubmitReview.Enabled = false;
        }

        // ================= LOAD PRODUCT =================
        private void LoadProduct(int id)
        {
            Product p = Product.GetById(id);
            if (p == null)
            {
                ShowError("Product not found.");
                return;
            }

            // visible info
            lblTitle.Text = p.ProductTitle;
            lblCrumbTitle.Text = p.ProductTitle;
            lblPrice.Text = "$" + p.ProductPrice.ToString("0.00", CultureInfo.InvariantCulture);

            lblDesc.Text = Server.HtmlEncode(p.ProductDescription ?? "")
                .Replace("\r\n", "<br/>")
                .Replace("\n", "<br/>");

            lblCategory.Text = Server.HtmlEncode(p.ProductCategory ?? "");
            lblSport.Text = Server.HtmlEncode(p.Sport ?? "");

            // image
            hfImage.Value = p.ProductImage ?? "";
            imgProduct.ImageUrl = string.IsNullOrWhiteSpace(p.ProductImage)
                ? ResolveUrl("~/Images/no-image.png")
                : ResolveUrl("~/" + p.ProductImage);

            // ✅ store correct mapping (IMPORTANT for review eligibility + checkout)
            hfSourceTable.Value = p.SourceTable ?? "";
            hfSourceProductId.Value = p.SourceProductId.ToString();

            // stock UI
            if (p.ProductStock > 0)
            {
                lblStockText.Text = "In stock, ready to ship";
                stockDot.Attributes["class"] = "dot dot-green";

                btnAddToCart.Enabled = true;
                btnAddToCart.CssClass = "btn btn-cart btn-wide";
            }
            else
            {
                lblStockText.Text = "Out of stock";
                stockDot.Attributes["class"] = "dot dot-red";

                btnAddToCart.Enabled = false;
                btnAddToCart.CssClass = "btn btn-cart btn-wide disabled";
            }
        }

        // ================= CART HELPERS =================
        private DataTable GetOrCreateCart()
        {
            DataTable cart = Session["Cart"] as DataTable;

            if (cart == null)
            {
                cart = new DataTable();
                cart.Columns.Add("SourceTable", typeof(string));
                cart.Columns.Add("SourceProductId", typeof(int));
                cart.Columns.Add("Title", typeof(string));
                cart.Columns.Add("Image", typeof(string));
                cart.Columns.Add("Price", typeof(decimal));
                cart.Columns.Add("Qty", typeof(int));

                Session["Cart"] = cart;
            }

            return cart;
        }

        // ================= ADD TO CART =================
        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (!IsUserLoggedIn())
            {
                lblMsg.ForeColor = Color.Red;
                lblMsg.Text = "❌ Please login to add to cart.";
                return;
            }

            int allProductsId = int.Parse(hfId.Value);

            Product p = Product.GetById(allProductsId);
            if (p == null)
            {
                lblMsg.ForeColor = Color.Red;
                lblMsg.Text = "❌ Product not found.";
                return;
            }

            if (p.ProductStock <= 0)
            {
                lblMsg.ForeColor = Color.Red;
                lblMsg.Text = "❌ Out of stock.";
                return;
            }

            // ✅ USE REAL MAPPING FROM All_Products
            string sourceTable = p.SourceTable;
            int sourceProductId = p.SourceProductId;

            if (string.IsNullOrWhiteSpace(sourceTable) || sourceProductId <= 0)
            {
                lblMsg.ForeColor = Color.Red;
                lblMsg.Text = "❌ Unable to add to cart (missing product mapping).";
                return;
            }

            DataTable cart = GetOrCreateCart();

            // find existing row using (SourceTable + SourceProductId)
            DataRow existing = null;
            foreach (DataRow r in cart.Rows)
            {
                string rTbl = r["SourceTable"]?.ToString() ?? "";
                int rPid = Convert.ToInt32(r["SourceProductId"]);

                if (string.Equals(rTbl, sourceTable, StringComparison.OrdinalIgnoreCase)
                    && rPid == sourceProductId)
                {
                    existing = r;
                    break;
                }
            }

            if (existing == null)
            {
                DataRow nr = cart.NewRow();
                nr["SourceTable"] = sourceTable;
                nr["SourceProductId"] = sourceProductId;
                nr["Title"] = p.ProductTitle;
                nr["Image"] = p.ProductImage ?? "Images/no-image.png";
                nr["Price"] = p.ProductPrice;
                nr["Qty"] = 1;
                cart.Rows.Add(nr);
            }
            else
            {
                int qty = Convert.ToInt32(existing["Qty"]);
                existing["Qty"] = qty + 1;
            }

            Session["Cart"] = cart;

            // refresh badge instantly
            var m = this.Master as UserLogin;
            if (m != null) m.RefreshCartBadge();

            lblMsg.ForeColor = Color.Green;
            lblMsg.Text = "✅ Added to cart!";
        }

        // ================= REVIEW ACCESS CONTROL =================
        private void ApplyReviewAccessUI()
        {
            ddlRating.Enabled = false;
            txtReview.Enabled = false;
            btnSubmitReview.Enabled = false;
            hfEligibleTransactionId.Value = "";

            if (!IsUserLoggedIn())
            {
                lblRatingMsg.ForeColor = Color.Red;
                lblRatingMsg.Text = "❌ Please login to leave a review.";
                return;
            }

            int accountId = int.Parse(Session["AccountId"].ToString());

            // ✅ IMPORTANT: check eligibility using source mapping
            string srcTbl = hfSourceTable.Value;
            int srcPid = 0;
            int.TryParse(hfSourceProductId.Value, out srcPid);

            if (string.IsNullOrWhiteSpace(srcTbl) || srcPid <= 0)
            {
                lblRatingMsg.ForeColor = Color.Red;
                lblRatingMsg.Text = "❌ Product mapping missing (cannot verify purchase).";
                return;
            }

            // ✅ ALSO include ProductId (All_Products.Id) because your unique constraint includes ProductId
            int allProductsId = int.Parse(hfId.Value);

            int txId = ProductReview.GetNextEligibleTransactionId(srcTbl, srcPid, accountId, allProductsId);

            if (txId <= 0)
            {
                lblRatingMsg.ForeColor = Color.Red;
                lblRatingMsg.Text =
                    "❌ You can only review after purchasing this product " +
                    "(or you've already reviewed your purchases).";
                return;
            }

            hfEligibleTransactionId.Value = txId.ToString();
            ddlRating.Enabled = true;
            txtReview.Enabled = true;
            btnSubmitReview.Enabled = true;
            lblRatingMsg.Text = "";
        }

        // ================= SUBMIT REVIEW =================
        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            if (!IsUserLoggedIn())
            {
                ApplyReviewAccessUI();
                return;
            }

            int allProductsId = int.Parse(hfId.Value);
            int accountId = int.Parse(Session["AccountId"].ToString());

            // read tx id granted by ApplyReviewAccessUI()
            int txId;
            if (!int.TryParse(hfEligibleTransactionId.Value, out txId) || txId <= 0)
            {
                lblRatingMsg.ForeColor = Color.Red;
                lblRatingMsg.Text = "❌ No eligible purchase found.";
                ApplyReviewAccessUI();
                return;
            }

            int rating;
            if (!int.TryParse(ddlRating.SelectedValue, out rating))
                rating = 5;

            rating = Math.Max(1, Math.Min(5, rating));

            string comment = (txtReview.Text ?? "").Trim();
            if (comment.Length > 1000)
                comment = comment.Substring(0, 1000);

            ProductReview.InsertReview(
                allProductsId,
                accountId,
                txId,
                rating,
                comment
            );

            lblRatingMsg.ForeColor = Color.Green;
            lblRatingMsg.Text = "✅ Review submitted!";
            txtReview.Text = "";

            LoadRatings(allProductsId);
            ApplyReviewAccessUI();
        }

        // ================= LOAD RATINGS =================
        private void LoadRatings(int allProductsId)
        {
            int currentId = 0;
            if (Session["AccountId"] != null)
                int.TryParse(Session["AccountId"].ToString(), out currentId);

            var summary = ProductReview.GetSummary(allProductsId);
            lblAvgRatingBottom.Text = summary.avg.ToString("0.0");
            lblRatingCountBottom.Text = summary.count.ToString();
            lblStarsBottom.Text = ProductReview.MakeStarsFromAverage(summary.avg);

            DataTable dt = ProductReview.GetReviewsForProduct(allProductsId, currentId);
            rptReviews.DataSource = dt;
            rptReviews.DataBind();

            pnlNoReviews.Visible = dt.Rows.Count == 0;
        }
    }
}
