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
            return table == "Badminton_Products"
                || table == "Tennis_Products"
                || table == "Squash_Products";
        }

        private string GetSportFromTable(string table)
        {
            if (table == "Badminton_Products") return "Badminton";
            if (table == "Tennis_Products") return "Tennis";
            if (table == "Squash_Products") return "Squash";
            return "Unknown";
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
                        txtPrice.Text = Convert.ToDecimal(dr["ProductPrice"])
                            .ToString("0.00", CultureInfo.InvariantCulture);
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

            if (!decimal.TryParse(txtPrice.Text.Trim(), NumberStyles.Any,
                CultureInfo.InvariantCulture, out decimal price) || price < 0)
            {
                ShowMsg("Invalid price (use 49.90).", true);
                return;
            }

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
                Directory.CreateDirectory(folder);

                newImageFile = Guid.NewGuid().ToString("N") + ext;
                fuImage.SaveAs(Path.Combine(folder, newImageFile));

                string old = hfOldImage.Value;
                string oldPath = Server.MapPath("~/Images/Product_Img/" + old);
                if (!string.IsNullOrWhiteSpace(old) && File.Exists(oldPath))
                    File.Delete(oldPath);
            }

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlSource = $@"
UPDATE dbo.{table}
SET ProductTitle=@Title, ProductImage=@Image, ProductPrice=@Price,
    ProductDescription=@Desc, ProductStock=@Stock, ProductCategory=@Category
WHERE Id=@Id";

                        using (SqlCommand cmd = new SqlCommand(sqlSource, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@Title", title);
                            cmd.Parameters.AddWithValue("@Image", newImageFile);
                            cmd.Parameters.AddWithValue("@Price", price);
                            cmd.Parameters.AddWithValue("@Desc", desc);
                            cmd.Parameters.AddWithValue("@Stock", stock);
                            cmd.Parameters.AddWithValue("@Category", category);
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }

                        string sqlAll = @"
UPDATE dbo.All_Products
SET Sport=@Sport, ProductTitle=@Title, ProductImage=@Image,
    ProductPrice=@Price, ProductDescription=@Desc,
    ProductStock=@Stock, ProductCategory=@Category
WHERE SourceTable=@SourceTable AND SourceProductId=@SourceProductId";

                        int rows;
                        using (SqlCommand cmd = new SqlCommand(sqlAll, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@Sport", GetSportFromTable(table));
                            cmd.Parameters.AddWithValue("@Title", title);
                            cmd.Parameters.AddWithValue("@Image", newImageFile);
                            cmd.Parameters.AddWithValue("@Price", price);
                            cmd.Parameters.AddWithValue("@Desc", desc);
                            cmd.Parameters.AddWithValue("@Stock", stock);
                            cmd.Parameters.AddWithValue("@Category", category);
                            cmd.Parameters.AddWithValue("@SourceTable", table);
                            cmd.Parameters.AddWithValue("@SourceProductId", id);
                            rows = cmd.ExecuteNonQuery();
                        }

                        if (rows == 0)
                        {
                            string insertAll = @"
INSERT INTO dbo.All_Products
(Sport, ProductTitle, ProductImage, ProductPrice,
 ProductDescription, ProductStock, ProductCategory,
 SourceTable, SourceProductId)
VALUES
(@Sport,@Title,@Image,@Price,@Desc,@Stock,@Category,
 @SourceTable,@SourceProductId)";

                            using (SqlCommand cmd = new SqlCommand(insertAll, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@Sport", GetSportFromTable(table));
                                cmd.Parameters.AddWithValue("@Title", title);
                                cmd.Parameters.AddWithValue("@Image", newImageFile);
                                cmd.Parameters.AddWithValue("@Price", price);
                                cmd.Parameters.AddWithValue("@Desc", desc);
                                cmd.Parameters.AddWithValue("@Stock", stock);
                                cmd.Parameters.AddWithValue("@Category", category);
                                cmd.Parameters.AddWithValue("@SourceTable", table);
                                cmd.Parameters.AddWithValue("@SourceProductId", id);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        ShowMsg("Update failed: " + Server.HtmlEncode(ex.Message), true);
                        return;
                    }
                }
            }

            ShowMsg("Product updated successfully.", false);
            imgPreview.ImageUrl = "~/Images/Product_Img/" + newImageFile;
            hfOldImage.Value = newImageFile;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Admin/manageProducts.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Admin/manageProducts.aspx");
        }

        private void ShowMsg(string msg, bool isError)
        {
            lblMsg.ForeColor = isError
                ? System.Drawing.Color.Red
                : System.Drawing.Color.Green;
            lblMsg.Text = msg;
        }
    }
}
