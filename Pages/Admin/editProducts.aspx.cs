using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace SmashZone.Pages.Admin
{
    public partial class editProducts : SmashZone.BasePage
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                Response.Redirect("~/Pages/User/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                string idStr = Request.QueryString["id"];
                string table = Request.QueryString["tbl"];

                if (!int.TryParse(idStr, out int id) || !IsAllowedTable(table))
                {
                    Response.Redirect("~/Pages/Admin/manageProducts.aspx");
                    return;
                }

                hfId.Value = id.ToString();
                hfTable.Value = table;

                LoadProduct(id, table);
            }
        }

        private bool IsAllowedTable(string table)
        {
            return table == "Badminton_Products" ||
                   table == "Tennis_Products" ||
                   table == "Squash_Products";
        }

        private void LoadProduct(int id, string table)
        {
            using (SqlConnection conn = new SqlConnection(cs))
            {
                string sql = $@"
SELECT ProductTitle, ProductImage, ProductPrice, ProductDescription, ProductStock, ProductCategory
FROM dbo.{table}
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                        {
                            Response.Redirect("~/Pages/Admin/manageProducts.aspx");
                            return;
                        }

                        txtTitle.Text = dr["ProductTitle"].ToString();
                        txtPrice.Text = Convert.ToDecimal(dr["ProductPrice"]).ToString("0.00", CultureInfo.InvariantCulture);
                        txtDesc.Text = dr["ProductDescription"].ToString();
                        txtStock.Text = dr["ProductStock"].ToString();
                        txtCategory.Text = dr["ProductCategory"].ToString();

                        string img = dr["ProductImage"].ToString();
                        hfOldImage.Value = img;
                        imgPreview.ImageUrl = "~/Images/Product_Img/" + img;
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(hfId.Value, out int id)) return;
            string table = hfTable.Value;

            if (!IsAllowedTable(table))
            {
                Response.Redirect("~/Pages/Admin/manageProducts.aspx");
                return;
            }

            string title = txtTitle.Text.Trim();
            string desc = txtDesc.Text.Trim();
            string category = txtCategory.Text.Trim();

            if (!int.TryParse(txtStock.Text.Trim(), out int stock) || stock < 0)
            {
                ShowMsg("Invalid stock.", true);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price < 0)
            {
                ShowMsg("Invalid price (use 49.90).", true);
                return;
            }

            // Image handling (optional replace)
            string newImageFile = hfOldImage.Value;

            if (fuImage.HasFile)
            {
                string ext = Path.GetExtension(fuImage.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".webp")
                {
                    ShowMsg("Only JPG, PNG, or WebP allowed.", true);
                    return;
                }

                string folder = Server.MapPath("~/Images/Product_Img/");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                newImageFile = Guid.NewGuid().ToString("N") + ext;
                fuImage.SaveAs(Path.Combine(folder, newImageFile));

                // delete old image file
                string old = hfOldImage.Value;
                string oldPath = Server.MapPath("~/Images/Product_Img/" + old);
                if (!string.IsNullOrWhiteSpace(old) && File.Exists(oldPath))
                    File.Delete(oldPath);
            }

            using (SqlConnection conn = new SqlConnection(cs))
            {
                string sql = $@"
UPDATE dbo.{table}
SET ProductTitle = @Title,
    ProductImage = @Image,
    ProductPrice = @Price,
    ProductDescription = @Desc,
    ProductStock = @Stock,
    ProductCategory = @Category
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Image", newImageFile);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Desc", desc);
                    cmd.Parameters.AddWithValue("@Stock", stock);
                    cmd.Parameters.AddWithValue("@Category", category);
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            ShowMsg("✅ Product updated successfully.", false);
            imgPreview.ImageUrl = "~/Images/Product_Img/" + newImageFile;
            hfOldImage.Value = newImageFile;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Admin/manageProducts.aspx");
        }

        private void ShowMsg(string msg, bool isError)
        {
            lblMsg.ForeColor = isError ? System.Drawing.Color.Red : System.Drawing.Color.Green;
            lblMsg.Text = msg;
        }
    }
}
