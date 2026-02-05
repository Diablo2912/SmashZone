using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SmashZone.App_Code;

namespace SmashZone.Pages.User
{
    public partial class squashProducts : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProducts();
            }
        }

        private void LoadProducts()
        {
            string connStr = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            string category = Request.QueryString["category"];
            string sort = Request.QueryString["sort"];

            string orderBy = "ProductTitle ASC";
            switch (sort)
            {
                case "price_asc": orderBy = "ProductPrice ASC"; break;
                case "price_desc": orderBy = "ProductPrice DESC"; break;
                case "name_desc": orderBy = "ProductTitle DESC"; break;
                case "name_asc":
                default: orderBy = "ProductTitle ASC"; break;
            }

            string sql = $@"
SELECT
    Id,
    ProductTitle,
    ProductImage,
    ProductPrice
FROM dbo.Squash_Products
WHERE (@Category IS NULL OR ProductCategory = @Category)
ORDER BY {orderBy};
";

            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (string.IsNullOrEmpty(category))
                        cmd.Parameters.AddWithValue("@Category", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@Category", category);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                rptProducts.DataSource = dt;
                rptProducts.DataBind();
                lblProductCount.Text = dt.Rows.Count.ToString();
            }
            catch (Exception)
            {
                lblProductCount.Text = "0";
            }
        }
    }
}
