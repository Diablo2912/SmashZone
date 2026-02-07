using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SmashZone.Pages.User
{
    public partial class badmintonProducts : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadProducts();
        }

        protected string GetStockBadge(object stockObj)
        {
            int stock = Convert.ToInt32(stockObj);

            if (stock <= 0)
                return "<span class='p-badge p-badge-soldout'>SOLD OUT</span>";

            if (stock < 50)
                return "<span class='p-badge p-badge-low'>LOW STOCK</span>";

            return "";
        }


        private void LoadProducts()
        {
            string cs = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;
            string sort = Request.QueryString["sort"];

            string orderBy = "ProductTitle ASC";
            if (sort == "price_asc") orderBy = "ProductPrice ASC";
            else if (sort == "price_desc") orderBy = "ProductPrice DESC";
            else if (sort == "name_desc") orderBy = "ProductTitle DESC";

            string sql = $@"
SELECT
    Id,
    ProductTitle,
    ProductImage,
    ProductPrice,
    ProductStock
FROM dbo.Badminton_Products
ORDER BY {orderBy};";

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            rptProducts.DataSource = dt;
            rptProducts.DataBind();

            lblProductCount.Text = dt.Rows.Count.ToString();
        }
    }
}
