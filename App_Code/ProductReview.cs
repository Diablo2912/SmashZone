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

        public static void UpsertReview(int productId, int accountId, int rating, string comment)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM ProductReviews WHERE ProductId=@ProductId AND AccountId=@AccountId)
BEGIN
    UPDATE ProductReviews
    SET Rating=@Rating,
        Comment=@Comment,
        UpdatedAt=GETDATE()
    WHERE ProductId=@ProductId AND AccountId=@AccountId
END
ELSE
BEGIN
    INSERT INTO ProductReviews(ProductId, AccountId, Rating, Comment)
    VALUES(@ProductId, @AccountId, @Rating, @Comment)
END
", con))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@AccountId", accountId);
                cmd.Parameters.AddWithValue("@Rating", rating);
                cmd.Parameters.AddWithValue("@Comment", (object)comment ?? DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static DataTable GetReviewsForProduct(int productId, int currentAccountId)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
SELECT 
    r.Rating,
    r.Comment,
    r.CreatedAt,
    r.AccountId
FROM ProductReviews r
WHERE r.ProductId = @ProductId
ORDER BY r.CreatedAt DESC
", con))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    // Add UI helper columns (Stars, UserLabel, CommentSafe)
                    dt.Columns.Add("Stars", typeof(string));
                    dt.Columns.Add("UserLabel", typeof(string));
                    dt.Columns.Add("CommentSafe", typeof(string));

                    foreach (DataRow row in dt.Rows)
                    {
                        int rating = Convert.ToInt32(row["Rating"]);
                        row["Stars"] = MakeStars(rating);

                        int accId = Convert.ToInt32(row["AccountId"]);
                        row["UserLabel"] = (accId == currentAccountId) ? "You" : ("User #" + accId);

                        string c = row["Comment"] == DBNull.Value ? "" : row["Comment"].ToString();
                        row["CommentSafe"] = System.Web.HttpUtility.HtmlEncode(c)
                            .Replace("\r\n", "<br/>")
                            .Replace("\n", "<br/>");
                    }

                    return dt;
                }
            }
        }

        public static (decimal avg, int count) GetSummary(int productId)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
SELECT 
    AVG(CAST(Rating AS DECIMAL(10,2))) AS AvgRating,
    COUNT(*) AS Cnt
FROM ProductReviews
WHERE ProductId=@ProductId
", con))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);

                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        decimal avg = r["AvgRating"] == DBNull.Value ? 0m : Convert.ToDecimal(r["AvgRating"]);
                        int cnt = r["Cnt"] == DBNull.Value ? 0 : Convert.ToInt32(r["Cnt"]);
                        return (avg, cnt);
                    }
                }
            }
            return (0m, 0);
        }

        private static string MakeStars(int rating)
        {
            rating = Math.Max(1, Math.Min(5, rating));
            return new string('★', rating) + new string('☆', 5 - rating);
        }

        public static string MakeStarsFromAverage(decimal avg)
        {
            // Simple rounding to nearest whole star for display
            int rounded = (int)Math.Round(avg, MidpointRounding.AwayFromZero);
            rounded = Math.Max(0, Math.Min(5, rounded));
            return new string('★', rounded) + new string('☆', 5 - rounded);
        }
    }
}
