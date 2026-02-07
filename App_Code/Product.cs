using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SmashZone.App_Code
{
    public class Product
    {
        public int Id { get; set; }                 // All_Products.Id
        public string Sport { get; set; }
        public string ProductTitle { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public int ProductStock { get; set; }
        public string ProductCategory { get; set; }

        // ✅ mapping to original sport table row
        public string SourceTable { get; set; }     // e.g. Badminton_Products
        public int SourceProductId { get; set; }    // e.g. 12

        public static Product GetById(int id)
        {
            string cs = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            const string sql = @"
SELECT TOP 1
    Id, Sport, ProductTitle, ProductImage, ProductPrice,
    ProductDescription, ProductStock, ProductCategory,
    SourceTable, SourceProductId
FROM dbo.All_Products
WHERE Id = @Id;
";

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;

                    return new Product
                    {
                        Id = Convert.ToInt32(r["Id"]),
                        Sport = r["Sport"]?.ToString(),
                        ProductTitle = r["ProductTitle"]?.ToString(),
                        ProductImage = r["ProductImage"]?.ToString(),
                        ProductPrice = Convert.ToDecimal(r["ProductPrice"]),
                        ProductDescription = r["ProductDescription"]?.ToString(),
                        ProductStock = Convert.ToInt32(r["ProductStock"]),
                        ProductCategory = r["ProductCategory"]?.ToString(),
                        SourceTable = r["SourceTable"]?.ToString(),
                        SourceProductId = r["SourceProductId"] == DBNull.Value ? 0 : Convert.ToInt32(r["SourceProductId"])
                    };
                }
            }
        }
    }
}
