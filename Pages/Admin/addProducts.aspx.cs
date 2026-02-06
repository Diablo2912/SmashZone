using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace SmashZone.Pages.Admin
{
    public partial class addProducts : SmashZone.BasePage
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                lblMsg.Text = "";
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            // sport table selected
            string table = ddlSport.SelectedValue;
            if (!IsAllowedTable(table))
            {
                ShowMsg("Invalid sport selected.", true);
                return;
            }

            // values
            string title = txtTitle.Text.Trim();
            string desc = txtDesc.Text.Trim();
            string category = txtCategory.Text.Trim();

            if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
            {
                ShowMsg("Invalid stock value.", true);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, NumberStyles.Any,
                CultureInfo.InvariantCulture, out decimal price))
            {
                ShowMsg("Invalid price format.", true);
                return;
            }

            // IsFeatured for sport tables
            bool feature = false;

            // ---------- IMAGE UPLOAD ----------
            if (!fuImage.HasFile)
            {
                ShowMsg("Please upload an image.", true);
                return;
            }

            string ext = Path.GetExtension(fuImage.FileName).ToLowerInvariant();
            string[] allowed = { ".jpg", ".jpeg", ".png", ".webp" };
            if (Array.IndexOf(allowed, ext) < 0)
            {
                ShowMsg("Only JPG, PNG, or WebP allowed.", true);
                return;
            }

            // Folder you want (relative to project root)
            string relativeFolder = "Images/Product_Img/";

            // Physical folder on server
            string physicalFolder = Server.MapPath("~/" + relativeFolder);
            if (!Directory.Exists(physicalFolder))
                Directory.CreateDirectory(physicalFolder);

            // Unique filename
            string fileName = Guid.NewGuid().ToString("N") + ext;

            // Full physical path to save file
            string physicalPath = Path.Combine(physicalFolder, fileName);

            // Save file
            fuImage.SaveAs(physicalPath);

            // Store relative path in DB
            string imagePathToStore = relativeFolder + fileName;

            // sport name for All_Products
            string sport = TableToSport(table);

            // ---------- INSERT BOTH TABLES IN 1 TRANSACTION ----------
            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();
                SqlTransaction tx = conn.BeginTransaction();

                try
                {
                    // 1) Insert into All_Products (include SourceTable + SourceProductId)
                    // IMPORTANT:
                    // SourceProductId will be the SAME as All_Products.Id (because we force sport table Id = newAllId)
                    string sqlAll = @"
INSERT INTO dbo.All_Products
(Sport, SourceTable, SourceProductId, ProductTitle, ProductImage, ProductPrice, ProductDescription, ProductStock, ProductCategory)
VALUES
(@Sport, @SourceTable, @SourceProductId, @Title, @Image, @Price, @Desc, @Stock, @Category);

SELECT SCOPE_IDENTITY();";

                    int newAllId;
                    using (SqlCommand cmdAll = new SqlCommand(sqlAll, conn, tx))
                    {
                        cmdAll.Parameters.AddWithValue("@Sport", sport);
                        cmdAll.Parameters.AddWithValue("@SourceTable", table);

                        // temporary placeholder; will set after we get newAllId
                        cmdAll.Parameters.AddWithValue("@SourceProductId", 0);

                        cmdAll.Parameters.AddWithValue("@Title", title);
                        cmdAll.Parameters.AddWithValue("@Image", imagePathToStore);
                        cmdAll.Parameters.AddWithValue("@Price", price);
                        cmdAll.Parameters.AddWithValue("@Desc", desc);
                        cmdAll.Parameters.AddWithValue("@Stock", stock);
                        cmdAll.Parameters.AddWithValue("@Category", category);

                        object result = cmdAll.ExecuteScalar();
                        newAllId = Convert.ToInt32(result);
                    }

                    // 1B) Update SourceProductId = newAllId (now that we know it)
                    using (SqlCommand cmdUpd = new SqlCommand(@"
UPDATE dbo.All_Products
SET SourceProductId = @Id
WHERE Id = @Id;", conn, tx))
                    {
                        cmdUpd.Parameters.AddWithValue("@Id", newAllId);
                        cmdUpd.ExecuteNonQuery();
                    }

                    // 2) Insert into sport table using same Id (IDENTITY_INSERT)
                    string sqlSport = $@"
SET IDENTITY_INSERT dbo.{table} ON;

INSERT INTO dbo.{table}
(Id, ProductTitle, ProductImage, ProductPrice, ProductDescription, ProductStock, ProductCategory, IsFeatured)
VALUES
(@Id, @Title, @Image, @Price, @Desc, @Stock, @Category, @Feature);

SET IDENTITY_INSERT dbo.{table} OFF;
";

                    using (SqlCommand cmdSport = new SqlCommand(sqlSport, conn, tx))
                    {
                        cmdSport.Parameters.AddWithValue("@Id", newAllId);
                        cmdSport.Parameters.AddWithValue("@Title", title);
                        cmdSport.Parameters.AddWithValue("@Image", imagePathToStore);
                        cmdSport.Parameters.AddWithValue("@Price", price);
                        cmdSport.Parameters.AddWithValue("@Desc", desc);
                        cmdSport.Parameters.AddWithValue("@Stock", stock);
                        cmdSport.Parameters.AddWithValue("@Category", category);
                        cmdSport.Parameters.AddWithValue("@Feature", feature);

                        cmdSport.ExecuteNonQuery();
                    }

                    tx.Commit();

                    // preview image
                    imgPreview.ImageUrl = "~/" + imagePathToStore;
                    imgPreview.Visible = true;

                    ShowMsg("Product added successfully (All_Products + " + sport + ").", false);
                    ClearForm();
                }
                catch (Exception ex)
                {
                    tx.Rollback();

                    // Optional cleanup if insert fails
                    // try { if (File.Exists(physicalPath)) File.Delete(physicalPath); } catch { }

                    ShowMsg("❌ Failed to add product: " + ex.Message, true);
                }
            }
        }

        private bool IsAllowedTable(string table)
        {
            return table == "Badminton_Products" ||
                   table == "Tennis_Products" ||
                   table == "Squash_Products";
        }

        private string TableToSport(string table)
        {
            if (table == "Badminton_Products") return "Badminton";
            if (table == "Tennis_Products") return "Tennis";
            return "Squash";
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            imgPreview.Visible = false;
            lblMsg.Text = "";
        }

        private void ClearForm()
        {
            ddlSport.SelectedIndex = 0;
            txtTitle.Text = "";
            txtPrice.Text = "";
            txtStock.Text = "";
            txtCategory.Text = "";
            txtDesc.Text = "";
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
