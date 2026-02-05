using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Script.Serialization;

namespace SmashZone.Pages.Admin
{
    public partial class dashboard : System.Web.UI.Page
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsAdmin())
            {
                ShowError("Access denied. Admins only.");
                return;
            }

            if (!IsPostBack)
            {
                var to = DateTime.Today;
                var from = to.AddDays(-13);

                txtFrom.Text = from.ToString("yyyy-MM-dd");
                txtTo.Text = to.ToString("yyyy-MM-dd");

                LoadDashboard(from, to);
            }
        }

        protected void btnApply_Click(object sender, EventArgs e)
        {
            if (!TryReadRange(out DateTime from, out DateTime to))
                return;

            LoadDashboard(from, to);
        }

        private void LoadDashboard(DateTime from, DateTime to)
        {
            lblRange.Text = $"{from:dd MMM yyyy} → {to:dd MMM yyyy}";

            LoadKPIs();
            LoadTrend(from, to);
            LoadRecentTransactions();
        }

        private void LoadKPIs()
        {
            var today = DateTime.Today;
            var d7 = today.AddDays(-6);
            var d30 = today.AddDays(-29);

            (decimal sum, int orders) Today = SumOrders(today, today);
            (decimal sum, int orders) Seven = SumOrders(d7, today);
            (decimal sum, int orders) Thirty = SumOrders(d30, today);
            (decimal sum, int orders) All = SumOrders(null, null);

            lblToday.Text = Today.sum.ToString("0.00", CultureInfo.InvariantCulture);
            lblTodayOrders.Text = Today.orders.ToString();

            lbl7.Text = Seven.sum.ToString("0.00", CultureInfo.InvariantCulture);
            lbl7Orders.Text = Seven.orders.ToString();

            lbl30.Text = Thirty.sum.ToString("0.00", CultureInfo.InvariantCulture);
            lbl30Orders.Text = Thirty.orders.ToString();

            lblAll.Text = All.sum.ToString("0.00", CultureInfo.InvariantCulture);
            lblAllOrders.Text = All.orders.ToString();
        }

        private (decimal sum, int orders) SumOrders(DateTime? from, DateTime? to)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                // Change CreatedAt if your column name differs
                cmd.CommandText = @"
SELECT
    ISNULL(SUM(AmountTotal), 0) AS TotalSales,
    COUNT(*) AS TotalOrders
FROM dbo.Transactions
WHERE Status = 'paid'
  AND (@from IS NULL OR CAST(CreatedAt AS date) >= @from)
  AND (@to IS NULL OR CAST(CreatedAt AS date) <= @to);";

                cmd.Parameters.AddWithValue("@from", (object)from?.Date ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@to", (object)to?.Date ?? DBNull.Value);

                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        decimal sum = Convert.ToDecimal(rdr["TotalSales"]);
                        int orders = Convert.ToInt32(rdr["TotalOrders"]);
                        return (sum, orders);
                    }
                }
            }
            return (0m, 0);
        }

        private void LoadTrend(DateTime from, DateTime to)
        {
            var labels = new List<string>();
            var data = new List<decimal>();

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = @"
SELECT
    CAST(CreatedAt AS date) AS [Day],
    ISNULL(SUM(AmountTotal), 0) AS Sales
FROM dbo.Transactions
WHERE Status = 'paid'
  AND CAST(CreatedAt AS date) BETWEEN @from AND @to
GROUP BY CAST(CreatedAt AS date)
ORDER BY [Day];";

                cmd.Parameters.AddWithValue("@from", from.Date);
                cmd.Parameters.AddWithValue("@to", to.Date);

                var map = new Dictionary<DateTime, decimal>();

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        DateTime day = Convert.ToDateTime(rdr["Day"]);
                        decimal sales = Convert.ToDecimal(rdr["Sales"]);
                        map[day] = sales;
                    }
                }

                for (var d = from.Date; d <= to.Date; d = d.AddDays(1))
                {
                    labels.Add(d.ToString("dd MMM"));
                    data.Add(map.ContainsKey(d) ? map[d] : 0m);
                }
            }

            var js = new JavaScriptSerializer();
            hfLabels.Value = js.Serialize(labels);
            hfData.Value = js.Serialize(data);
        }

        private void LoadRecentTransactions()
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = @"
SELECT TOP 15
    CONVERT(varchar(19), CreatedAt, 120) AS CreatedAt,
    AccountId,
    StripeSessionId,
    AmountTotal,
    Status,
    ReceiptUrl
FROM dbo.Transactions
ORDER BY CreatedAt DESC;";

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow r in dt.Rows)
                    {
                        decimal amt = 0m;
                        decimal.TryParse(r["AmountTotal"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out amt);
                        r["AmountTotal"] = amt.ToString("0.00", CultureInfo.InvariantCulture);
                    }

                    gvRecent.DataSource = dt;
                    gvRecent.DataBind();
                }
            }
        }

        private bool IsAdmin()
        {
            return Session["Role"] != null &&
                   Session["Role"].ToString().Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        private bool TryReadRange(out DateTime from, out DateTime to)
        {
            from = DateTime.Today.AddDays(-13);
            to = DateTime.Today;

            if (!DateTime.TryParse(txtFrom.Text, out from) || !DateTime.TryParse(txtTo.Text, out to))
            {
                ShowError("Invalid date range.");
                return false;
            }

            if (from.Date > to.Date)
            {
                ShowError("From date cannot be after To date.");
                return false;
            }

            return true;
        }

        private void ShowError(string msg)
        {
            pnlErr.Visible = true;
            lblErr.Text = "❌ " + Server.HtmlEncode(msg);
        }
    }
}
