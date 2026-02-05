using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace SmashZone.Pages.Admin
{
    public partial class manageProducts : System.Web.UI.Page
    {
        private string ConnStr =>
            ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindGrid();
        }

        // ================= LOAD GRID =================
        private void BindGrid()
        {
            DataTable dt = new DataTable();

            string sql = @"
SELECT 
    Id,
    'Badminton' AS Sport,
    ProductImage,
    ProductTitle,
    ProductPrice,
    ProductStock,
    ProductCategory,
    IsFeatured,
    'Badminton_Products' AS SourceTable
FROM dbo.Badminton_Products

UNION ALL

SELECT 
    Id,
    'Tennis' AS Sport,
    ProductImage,
    ProductTitle,
    ProductPrice,
    ProductStock,
    ProductCategory,
    IsFeatured,
    'Tennis_Products' AS SourceTable
FROM dbo.Tennis_Products

UNION ALL

SELECT 
    Id,
    'Squash' AS Sport,
    ProductImage,
    ProductTitle,
    ProductPrice,
    ProductStock,
    ProductCategory,
    IsFeatured,
    'Squash_Products' AS SourceTable
FROM dbo.Squash_Products

ORDER BY Sport, ProductTitle;
";

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
            {
                da.Fill(dt);
            }

            gvProducts.DataSource = dt;
            gvProducts.DataBind();
        }

        // ================= PAGING =================
        protected void gvProducts_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            gvProducts.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        // ================= EDIT / DELETE =================
        protected void gvProducts_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.CommandArgument?.ToString()))
                return;

            string[] args = e.CommandArgument.ToString().Split('|');
            if (args.Length < 2) return;

            int id;
            if (!int.TryParse(args[0], out id)) return;

            string table = args[1];

            if (e.CommandName == "EditProduct")
            {
                EnsureSafeTable(table);

                // ✅ FIX: Use app-root absolute path (works no matter what folder you are in)
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
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand($"DELETE FROM dbo.{table} WHERE Id=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // Optional: delete image file
            try
            {
                string cleaned = (image ?? "").Trim().TrimStart('~', '/');
                if (!string.IsNullOrWhiteSpace(cleaned))
                {
                    string path = Server.MapPath("~/" + cleaned);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }
            }
            catch { }
        }

        // ================= FEATURED TOGGLE (ACROSS ALL 3 TABLES) =================
        protected void chkFeatured_CheckedChanged(object sender, EventArgs e)
        {
            var cb = (System.Web.UI.WebControls.CheckBox)sender;
            var row = (System.Web.UI.WebControls.GridViewRow)cb.NamingContainer;

            var hfPid = (System.Web.UI.WebControls.HiddenField)row.FindControl("hfPid");
            var hfTbl = (System.Web.UI.WebControls.HiddenField)row.FindControl("hfTbl");

            int id = int.Parse(hfPid.Value);
            string table = hfTbl.Value;
            bool wantFeatured = cb.Checked;

            // ✅ Enforce max 3 across all tables
            if (wantFeatured && GetFeaturedCountAcrossAllTables() >= 3)
            {
                cb.Checked = false;
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "limit",
                    "alert('Only 3 featured products are allowed across ALL sports.');",
                    true
                );
                return;
            }

            EnsureSafeTable(table);
            UpdateFeatured(table, id, wantFeatured);

            BindGrid();
        }

        private int GetFeaturedCountAcrossAllTables()
        {
            string sql = @"
SELECT
    (SELECT COUNT(*) FROM dbo.Badminton_Products WHERE IsFeatured = 1) +
    (SELECT COUNT(*) FROM dbo.Tennis_Products    WHERE IsFeatured = 1) +
    (SELECT COUNT(*) FROM dbo.Squash_Products    WHERE IsFeatured = 1);
";

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void UpdateFeatured(string table, int id, bool isFeatured)
        {
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(
                $"UPDATE dbo.{table} SET IsFeatured=@F WHERE Id=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@F", isFeatured ? 1 : 0);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void EnsureSafeTable(string table)
        {
            if (table != "Badminton_Products" &&
                table != "Tennis_Products" &&
                table != "Squash_Products")
            {
                throw new Exception("Invalid table name.");
            }
        }
    }
}
