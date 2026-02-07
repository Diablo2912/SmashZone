using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SmashZone.Pages.User
{
    public partial class tennisProducts : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hfMaxPrice.Value = "500";
                hfTypes.Value = "All";

                if (!string.IsNullOrEmpty(Request.QueryString["maxPrice"]))
                    hfMaxPrice.Value = Request.QueryString["maxPrice"];

                if (!string.IsNullOrEmpty(Request.QueryString["types"]))
                    hfTypes.Value = Request.QueryString["types"];

                LoadProducts();
            }
        }

        protected void btnApplyFilters_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        protected string GetStockBadge(object stockObj)
        {
            int stock = 0;
            if (stockObj != null && stockObj != DBNull.Value)
                int.TryParse(stockObj.ToString(), out stock);

            if (stock <= 0)
                return "<span class='p-badge p-badge-soldout'>SOLD OUT</span>";

            if (stock < 50)
                return "<span class='p-badge p-badge-low'>LOW STOCK</span>";

            return "";
        }

        protected string GetViewBtnClass(object stockObj)
        {
            int stock = 0;
            if (stockObj != null && stockObj != DBNull.Value)
                int.TryParse(stockObj.ToString(), out stock);

            return stock <= 0 ? "btn-disabled" : "";
        }

        public string GetSortUrl(string sort)
        {
            string maxPrice = hfMaxPrice.Value;
            string types = hfTypes.Value;

            return ResolveUrl("~/Pages/User/tennisProducts.aspx"
                + "?sort=" + Uri.EscapeDataString(sort)
                + "&maxPrice=" + Uri.EscapeDataString(maxPrice ?? "500")
                + "&types=" + Uri.EscapeDataString(types ?? "All"));
        }

        private void LoadProducts()
        {
            string cs = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

            string typesCsv = hfTypes.Value;
            int maxPrice = 500;
            int.TryParse(hfMaxPrice.Value, out maxPrice);

            var types = (typesCsv ?? "")
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            bool isAll = types.Contains("All");

            string sort = (Request.QueryString["sort"] ?? "").ToLower();

            string orderBy;
            switch (sort)
            {
                case "price_asc": orderBy = "ProductPrice ASC"; break;
                case "price_desc": orderBy = "ProductPrice DESC"; break;
                case "name_desc": orderBy = "ProductTitle DESC"; break;
                case "name_asc":
                default: orderBy = "ProductTitle ASC"; break;
            }

            string sql = @"
SELECT Id, ProductTitle, ProductImage, ProductPrice, ProductStock, ProductCategory
FROM dbo.Tennis_Products
WHERE ProductPrice <= @MaxPrice
";

            if (!isAll && types.Any())
            {
                sql += " AND ProductCategory IN (" +
                       string.Join(",", types.Select((t, i) => "@cat" + i)) + ")";
            }

            sql += " ORDER BY " + orderBy;

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaxPrice", maxPrice);

                if (!isAll && types.Any())
                {
                    for (int i = 0; i < types.Count; i++)
                        cmd.Parameters.AddWithValue("@cat" + i, types[i]);
                }

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }

            rptProducts.DataSource = dt;
            rptProducts.DataBind();
            lblProductCount.Text = dt.Rows.Count.ToString();
        }
    }
}
