using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SmashZone.App_Code
{
    public class Product
    {
        public int Id { get; set; }
        public string Sport { get; set; }
        public string ProductTitle { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public int ProductStock { get; set; }
        public string ProductCategory { get; set; }

        // ✅ mapping
        public string SourceTable { get; set; }
        public int SourceProductId { get; set; }

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
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read()) return null;

                    return new Product
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Sport = dr["Sport"].ToString(),
                        ProductTitle = dr["ProductTitle"].ToString(),
                        ProductImage = dr["ProductImage"] == DBNull.Value ? "" : dr["ProductImage"].ToString(),
                        ProductPrice = Convert.ToDecimal(dr["ProductPrice"]),
                        ProductDescription = dr["ProductDescription"] == DBNull.Value ? "" : dr["ProductDescription"].ToString(),
                        ProductStock = dr["ProductStock"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ProductStock"]),
                        ProductCategory = dr["ProductCategory"].ToString(),

                        // ✅ new
                        SourceTable = dr["SourceTable"] == DBNull.Value ? "" : dr["SourceTable"].ToString(),
                        SourceProductId = dr["SourceProductId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["SourceProductId"])
                    };
                }
            }
        }
    }
}
