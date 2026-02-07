using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace SmashZone.App_Code
{
    public static class ProductReview
    {
        // ================= ELIGIBILITY =================
        // Returns a Transaction PK value (Id or TransactionId) that:
        // ✅ belongs to account
        // ✅ is paid
        // ✅ ItemsJson contains {sourceTable, sourceProductId}
        // ✅ NOT yet reviewed for (TransactionId + ProductId + AccountId)
        public static int GetNextEligibleTransactionId(
            string sourceTable,
            int sourceProductId,
            int accountId,
            int allProductsId
        )
        {
            if (string.IsNullOrWhiteSpace(sourceTable) || sourceProductId <= 0 || accountId <= 0 || allProductsId <= 0)
                return 0;

            // Try with Transactions.Id first; if not exists, retry with Transactions.TransactionId
            try
            {
                return GetNextEligibleTransactionId_Internal(sourceTable, sourceProductId, accountId, allProductsId, "Id");
            }
            catch (SqlException ex)
            {
                // your screenshot: "Invalid column name 'Id'"
                if (ex.Message != null && ex.Message.IndexOf("Invalid column name 'Id'", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return GetNextEligibleTransactionId_Internal(sourceTable, sourceProductId, accountId, allProductsId, "TransactionId");
                }

                throw; // something else wrong
            }
        }

        private static int GetNextEligibleTransactionId_Internal(
            string sourceTable,
            int sourceProductId,
            int accountId,
            int allProductsId,
            string txPkColumn   // "Id" OR "TransactionId"
        )
        {
            string cs = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            // ✅ txPkColumn is hardcoded by us (NOT user input), so safe to inject here
            string sql = $@"
SELECT TOP 1 t.[{txPkColumn}]
FROM dbo.Transactions t
WHERE t.AccountId = @aid
  AND LOWER(ISNULL(t.Status,'')) = 'paid'
  AND t.ItemsJson IS NOT NULL
  AND EXISTS (
      SELECT 1
      FROM OPENJSON(t.ItemsJson)
      WITH (
          sourceTable NVARCHAR(100) '$.sourceTable',
          sourceProductId INT '$.sourceProductId'
      ) j
      WHERE j.sourceTable = @tbl
        AND j.sourceProductId = @pid
  )
  AND NOT EXISTS (
      SELECT 1
      FROM dbo.ProductReviews r
      WHERE r.TransactionId = t.[{txPkColumn}]
        AND r.ProductId = @prodId
        AND r.AccountId = @aid
  )
ORDER BY t.[{txPkColumn}] DESC;";

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@aid", accountId);
                cmd.Parameters.AddWithValue("@tbl", sourceTable);
                cmd.Parameters.AddWithValue("@pid", sourceProductId);
                cmd.Parameters.AddWithValue("@prodId", allProductsId);

                conn.Open();
                object o = cmd.ExecuteScalar();
                return (o == null) ? 0 : Convert.ToInt32(o);
            }
        }

        // ================= INSERT REVIEW =================
        // Uses YOUR table columns: ReviewId (identity), ProductId, AccountId, Rating, Comment, CreatedAt, UpdatedAt, TransactionId
        public static void InsertReview(int allProductsId, int accountId, int transactionId, int rating, string comment)
        {
            string cs = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            const string sql = @"
INSERT INTO dbo.ProductReviews
(ProductId, AccountId, Rating, Comment, CreatedAt, UpdatedAt, TransactionId)
VALUES
(@pid, @aid, @rating, @comment, GETDATE(), NULL, @txid);
";

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@pid", allProductsId);
                cmd.Parameters.AddWithValue("@aid", accountId);
                cmd.Parameters.AddWithValue("@rating", rating);
                cmd.Parameters.AddWithValue("@comment", (object)(comment ?? "") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@txid", transactionId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ================= SUMMARY =================
        public static (double avg, int count) GetSummary(int allProductsId)
        {
            string cs = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            const string sql = @"
SELECT 
    AVG(CAST(Rating AS FLOAT)) AS AvgRating,
    COUNT(*) AS Cnt
FROM dbo.ProductReviews
WHERE ProductId = @pid;
";

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@pid", allProductsId);
                conn.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                        return (0.0, 0);

                    double avg = (r["AvgRating"] == DBNull.Value) ? 0.0 : Convert.ToDouble(r["AvgRating"]);
                    int cnt = (r["Cnt"] == DBNull.Value) ? 0 : Convert.ToInt32(r["Cnt"]);
                    return (avg, cnt);
                }
            }
        }

        // ================= REVIEWS LIST =================
        public static DataTable GetReviewsForProduct(int allProductsId, int currentAccountId)
        {
            string cs = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            const string sql = @"
SELECT 
    Rating,
    Comment,
    CreatedAt,
    AccountId
FROM dbo.ProductReviews
WHERE ProductId = @pid
ORDER BY CreatedAt DESC;
";

            DataTable dt = new DataTable();
            dt.Columns.Add("Stars", typeof(string));
            dt.Columns.Add("CommentSafe", typeof(string));
            dt.Columns.Add("CreatedAt", typeof(DateTime));
            dt.Columns.Add("UserLabel", typeof(string));

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@pid", allProductsId);
                conn.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        int rating = Convert.ToInt32(r["Rating"]);
                        string comment = r["Comment"] == DBNull.Value ? "" : r["Comment"].ToString();
                        DateTime created = Convert.ToDateTime(r["CreatedAt"]);
                        int accId = Convert.ToInt32(r["AccountId"]);

                        string who = (currentAccountId > 0 && accId == currentAccountId) ? "You" : "Customer";

                        DataRow row = dt.NewRow();
                        row["Stars"] = MakeStars(rating);
                        row["CommentSafe"] = WebUtility.HtmlEncode(comment)
                            .Replace("\r\n", "<br/>")
                            .Replace("\n", "<br/>");
                        row["CreatedAt"] = created;
                        row["UserLabel"] = who;

                        dt.Rows.Add(row);
                    }
                }
            }

            return dt;
        }

        // ================= STARS HELPERS =================
        public static string MakeStars(int rating)
        {
            rating = Math.Max(0, Math.Min(5, rating));
            return new string('★', rating) + new string('☆', 5 - rating);
        }

        public static string MakeStarsFromAverage(double avg)
        {
            int rounded = (int)Math.Round(avg, MidpointRounding.AwayFromZero);
            return MakeStars(rounded);
        }
    }
}
