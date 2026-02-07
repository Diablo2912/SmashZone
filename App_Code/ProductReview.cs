using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace SmashZone.App_Code
{
    public class ProductReview
    {
        private static string ConnStr =>
            ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

        // ======================================================
        // FIND NEXT PURCHASE THAT HAS NOT BEEN REVIEWED
        // ======================================================
        public static int GetNextEligibleTransactionId(int productId, int accountId)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
SELECT TOP 1 t.TransactionId
FROM Transactions t
CROSS APPLY OPENJSON(t.ItemsJson)
WITH (AllProductsId INT '$.AllProductsId') j
LEFT JOIN ProductReviews r
    ON r.TransactionId = t.TransactionId
   AND r.ProductId = @ProductId
   AND r.AccountId = @AccountId
WHERE t.AccountId = @AccountId
  AND t.Status IN ('paid','succeeded','completed','success')
  AND j.AllProductsId = @ProductId
  AND r.ReviewId IS NULL
ORDER BY t.CreatedAt DESC;
", con))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@AccountId", accountId);

                con.Open();
                object val = cmd.ExecuteScalar();
                return (val == null || val == DBNull.Value) ? 0 : Convert.ToInt32(val);
            }
        }

        // ======================================================
        // INSERT REVIEW (1 PER TRANSACTION)
        // ======================================================
        public static void InsertReview(
            int productId,
            int accountId,
            int transactionId,
            int rating,
            string comment)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
INSERT INTO ProductReviews
(ProductId, AccountId, TransactionId, Rating, Comment)
VALUES
(@ProductId, @AccountId, @TransactionId, @Rating, @Comment);
", con))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@AccountId", accountId);
                cmd.Parameters.AddWithValue("@TransactionId", transactionId);
                cmd.Parameters.AddWithValue("@Rating", rating);
                cmd.Parameters.AddWithValue("@Comment", (object)comment ?? DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ======================================================
        // LOAD REVIEWS FOR DISPLAY
        // ======================================================
        public static DataTable GetReviewsForProduct(int productId, int currentAccountId)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
SELECT 
    Rating,
    Comment,
    CreatedAt,
    AccountId
FROM ProductReviews
WHERE ProductId = @ProductId
ORDER BY CreatedAt DESC
", con))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    dt.Columns.Add("Stars", typeof(string));
                    dt.Columns.Add("UserLabel", typeof(string));
                    dt.Columns.Add("CommentSafe", typeof(string));

                    foreach (DataRow row in dt.Rows)
                    {
                        int rating = Convert.ToInt32(row["Rating"]);
                        row["Stars"] = MakeStars(rating);

                        int accId = Convert.ToInt32(row["AccountId"]);
                        row["UserLabel"] = (accId == currentAccountId)
                            ? "You"
                            : "User #" + accId;

                        string c = row["Comment"] == DBNull.Value ? "" : row["Comment"].ToString();
                        row["CommentSafe"] = System.Web.HttpUtility.HtmlEncode(c)
                            .Replace("\r\n", "<br/>")
                            .Replace("\n", "<br/>");
                    }

                    return dt;
                }
            }
        }

        // ======================================================
        // SUMMARY
        // ======================================================
        public static (decimal avg, int count) GetSummary(int productId)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
SELECT 
    AVG(CAST(Rating AS DECIMAL(10,2))) AS AvgRating,
    COUNT(*) AS Cnt
FROM ProductReviews
WHERE ProductId = @ProductId
", con))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);

                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        decimal avg = r["AvgRating"] == DBNull.Value ? 0m : Convert.ToDecimal(r["AvgRating"]);
                        int cnt = Convert.ToInt32(r["Cnt"]);
                        return (avg, cnt);
                    }
                }
            }
            return (0m, 0);
        }

        // ======================================================
        // STAR HELPERS
        // ======================================================
        private static string MakeStars(int rating)
        {
            rating = Math.Max(1, Math.Min(5, rating));
            return new string('★', rating) + new string('☆', 5 - rating);
        }

        public static string MakeStarsFromAverage(decimal avg)
        {
            int rounded = (int)Math.Round(avg, MidpointRounding.AwayFromZero);
            rounded = Math.Max(0, Math.Min(5, rounded));
            return new string('★', rounded) + new string('☆', 5 - rounded);
        }
    }
}
