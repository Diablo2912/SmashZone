using System;
using System.Configuration;
using System.Data.SqlClient;
using SmashZone.App_Code;

namespace SmashZone
{
    public partial class _Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadFeaturedProducts();
            }
        }

        private void LoadFeaturedProducts()
        {
            string connStr =
                ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT
                        ProductTitle,
                        ProductImage,
                        ProductPrice,
                        ProductDescription
                    FROM Products
                    WHERE Feature = 1";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    rptFeaturedProducts.DataSource = dr;
                    rptFeaturedProducts.DataBind();
                }
            }
        }
    }
}
