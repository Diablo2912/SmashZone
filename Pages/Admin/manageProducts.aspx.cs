using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace SmashZone.Pages.Admin
{
    public partial class manageProducts : System.Web.UI.Page
    {
        private string ConnStr =>
            ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCategoryDropdown();
                BindGrid();
            }
        }

        // ================= CATEGORY DROPDOWN =================
        private void LoadCategoryDropdown()
        {
            string sql = @"
SELECT DISTINCT ProductCategory FROM (
    SELECT ProductCategory FROM Badminton_Products
    UNION ALL
    SELECT ProductCategory FROM Tennis_Products
    UNION ALL
    SELECT ProductCategory FROM Squash_Products
) x
WHERE ProductCategory IS NOT NULL AND ProductCategory <> ''
ORDER BY ProductCategory";

            ddlCategory.Items.Clear();
            ddlCategory.Items.Add(new ListItem("All", ""));

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        ddlCategory.Items.Add(
                            new ListItem(r["ProductCategory"].ToString()));
                    }
                }
            }
        }

        // ================= GRID LOAD =================
        private void BindGrid()
        {
            DataTable dt = new DataTable();

            string sport = ddlSport.SelectedValue;
            string category = ddlCategory.SelectedValue;
            string search = txtSearch.Text.Trim();

            decimal? minPrice = TryDecimal(txtMinPrice.Text);
            decimal? maxPrice = TryDecimal(txtMaxPrice.Text);

            string sql = @"
SELECT *
FROM (
    SELECT Id,'Badminton' Sport,ProductImage,ProductTitle,ProductPrice,
           ProductStock,ProductCategory,IsFeatured,'Badminton_Products' SourceTable
    FROM Badminton_Products

    UNION ALL

    SELECT Id,'Tennis',ProductImage,ProductTitle,ProductPrice,
           ProductStock,ProductCategory,IsFeatured,'Tennis_Products'
    FROM Tennis_Products

    UNION ALL

    SELECT Id,'Squash',ProductImage,ProductTitle,ProductPrice,
           ProductStock,ProductCategory,IsFeatured,'Squash_Products'
    FROM Squash_Products
) p
WHERE 1=1";

            if (!string.IsNullOrEmpty(sport))
                sql += " AND p.Sport=@Sport";

            if (!string.IsNullOrEmpty(category))
                sql += " AND p.ProductCategory=@Category";

            if (!string.IsNullOrEmpty(search))
                sql += " AND (p.ProductTitle LIKE @Search OR p.ProductCategory LIKE @Search)";

            if (minPrice.HasValue)
                sql += " AND p.ProductPrice >= @MinPrice";

            if (maxPrice.HasValue)
                sql += " AND p.ProductPrice <= @MaxPrice";

            sql += " ORDER BY p.Sport, p.ProductTitle";

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (!string.IsNullOrEmpty(sport))
                    cmd.Parameters.AddWithValue("@Sport", sport);

                if (!string.IsNullOrEmpty(category))
                    cmd.Parameters.AddWithValue("@Category", category);

                if (!string.IsNullOrEmpty(search))
                    cmd.Parameters.AddWithValue("@Search", "%" + search + "%");

                if (minPrice.HasValue)
                    cmd.Parameters.AddWithValue("@MinPrice", minPrice.Value);

                if (maxPrice.HasValue)
                    cmd.Parameters.AddWithValue("@MaxPrice", maxPrice.Value);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }

            gvProducts.DataSource = dt;
            gvProducts.DataBind();
        }

        private decimal? TryDecimal(string v)
        {
            if (decimal.TryParse(v, out decimal d))
                return d;
            return null;
        }

        // ================= FILTER EVENTS =================
        protected void btnApply_Click(object sender, EventArgs e)
        {
            gvProducts.PageIndex = 0;
            BindGrid();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvProducts.PageIndex = 0;
            BindGrid();
        }

        protected void Filters_Changed(object sender, EventArgs e)
        {
            gvProducts.PageIndex = 0;
            BindGrid();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ddlSport.SelectedIndex = 0;
            ddlCategory.SelectedIndex = 0;
            txtMinPrice.Text = "";
            txtMaxPrice.Text = "";
            txtSearch.Text = "";
            BindGrid();
        }

        // ================= PAGING =================
        protected void gvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProducts.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        // ================= EDIT / DELETE =================
        protected void gvProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // 🚫 Ignore commands without arguments
            if (string.IsNullOrWhiteSpace(e.CommandArgument?.ToString()))
                return;

            string[] args = e.CommandArgument.ToString().Split('|');

            // 🚫 Must have at least Id + Table
            if (args.Length < 2)
                return;

            if (!int.TryParse(args[0], out int id))
                return;

            string table = args[1];

            if (e.CommandName == "EditProduct")
            {
                EnsureSafeTable(table);

                string url = ResolveUrl("~/Pages/Admin/editProducts.aspx")
                    + "?id=" + id
                    + "&tbl=" + HttpUtility.UrlEncode(table);

                Response.Redirect(url, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            else if (e.CommandName == "DeleteProduct")
            {
                string image = args.Length >= 3 ? args[2] : null;

                EnsureSafeTable(table);
                DeleteProduct(table, id, image);
                BindGrid();
            }
        }


        private void DeleteProduct(string table, int id, string image)
        {
            EnsureSafeTable(table);

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(
                $"DELETE FROM dbo.{table} WHERE Id=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            try
            {
                string cleaned = (image ?? "").TrimStart('~', '/');
                string path = Server.MapPath("~/" + cleaned);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }
            catch { }
        }

        // ================= FEATURED =================
        protected void chkFeatured_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            GridViewRow row = (GridViewRow)cb.NamingContainer;

            int id = int.Parse(((HiddenField)row.FindControl("hfPid")).Value);
            string table = ((HiddenField)row.FindControl("hfTbl")).Value;

            if (cb.Checked && GetFeaturedCount() >= 3)
            {
                cb.Checked = false;
                ClientScript.RegisterStartupScript(
                    GetType(), "limit",
                    "alert('Only 3 featured products allowed.');", true);
                return;
            }

            EnsureSafeTable(table);

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(
                $"UPDATE dbo.{table} SET IsFeatured=@F WHERE Id=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@F", cb.Checked ? 1 : 0);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            BindGrid();
        }

        private int GetFeaturedCount()
        {
            string sql = @"
SELECT
    (SELECT COUNT(*) FROM Badminton_Products WHERE IsFeatured=1) +
    (SELECT COUNT(*) FROM Tennis_Products WHERE IsFeatured=1) +
    (SELECT COUNT(*) FROM Squash_Products WHERE IsFeatured=1)";

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void EnsureSafeTable(string t)
        {
            if (t != "Badminton_Products" &&
                t != "Tennis_Products" &&
                t != "Squash_Products")
                throw new Exception("Invalid table");
        }
    }
}
