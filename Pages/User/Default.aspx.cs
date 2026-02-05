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
            string connStr = ConfigurationManager
                .ConnectionStrings["SmashZoneCS"].ConnectionString;

            string sql = @"
SELECT TOP 3 *
FROM
(
    SELECT 
        Id,
        ProductTitle,
        ProductImage,
        ProductPrice,
        ProductDescription,
        'Badminton' AS ProductType
    FROM dbo.Badminton_Products
    WHERE IsFeatured = 1

    UNION ALL

    SELECT 
        Id,
        ProductTitle,
        ProductImage,
        ProductPrice,
        ProductDescription,
        'Tennis' AS ProductType
    FROM dbo.Tennis_Products
    WHERE IsFeatured = 1

    UNION ALL

    SELECT 
        Id,
        ProductTitle,
        ProductImage,
        ProductPrice,
        ProductDescription,
        'Squash' AS ProductType
    FROM dbo.Squash_Products
    WHERE IsFeatured = 1
) x
ORDER BY ProductType, ProductTitle;
";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    rptFeaturedProducts.DataSource = dr;
                    rptFeaturedProducts.DataBind();
                }
            }
        }
    }
}
