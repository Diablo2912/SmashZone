using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Configuration;

namespace SmashZone.Pages.User
{
    public partial class Search : BasePage
    {
        string cs = ConfigurationManager.ConnectionStrings["SmashZoneCS"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hfMaxPrice.Value = "500";
                hfTypes.Value = "All";
                BindResults();
            }
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindResults();
        }

        protected void btnApplyFilters_Click(object sender, EventArgs e)
        {
            BindResults();
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindResults();
        }

        private void BindResults()
        {
            string keyword = txtQ.Text.Trim();
            string typesCsv = hfTypes.Value;
            int maxPrice = int.Parse(hfMaxPrice.Value);

            var types = (typesCsv ?? "")
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            bool isAll = types.Contains("All");

            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = @"
SELECT * FROM All_Products
WHERE ProductPrice <= @MaxPrice
AND (@Keyword = '' OR ProductTitle LIKE '%' + @Keyword + '%')
";

                if (!isAll && types.Any())
                {
                    sql += " AND ProductCategory IN (" +
                           string.Join(",", types.Select((t, i) => "@cat" + i)) + ")";
                }

                switch (ddlSort.SelectedValue)
                {
                    case "price_asc": sql += " ORDER BY ProductPrice ASC"; break;
                    case "price_desc": sql += " ORDER BY ProductPrice DESC"; break;
                    case "name_asc": sql += " ORDER BY ProductTitle ASC"; break;
                    case "name_desc": sql += " ORDER BY ProductTitle DESC"; break;
                }

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Keyword", keyword);
                cmd.Parameters.AddWithValue("@MaxPrice", maxPrice);

                if (!isAll && types.Any())
                {
                    for (int i = 0; i < types.Count; i++)
                        cmd.Parameters.AddWithValue("@cat" + i, types[i]);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptResults.DataSource = dt;
                rptResults.DataBind();

                lblCount.Text = dt.Rows.Count.ToString();
                pnlEmpty.Visible = dt.Rows.Count == 0;
            }
        }

        protected string GetImgUrl(object productImageObj)
        {
            string img = (productImageObj == null) ? "" : productImageObj.ToString().Trim();

            if (string.IsNullOrWhiteSpace(img))
                return ResolveUrl("~/Images/no-image.png");

            // If DB already stores a full/relative path, use it as-is
            // e.g. "Images/Product_Img/a.png" or "/Images/Product_Img/a.png"
            if (img.StartsWith("~") || img.StartsWith("/") || img.Contains("/"))
                return ResolveUrl(img.StartsWith("~") ? img : "~/" + img.TrimStart('/'));

            // Otherwise assume it's just a filename
            return ResolveUrl("~/Images/Product_Img/" + img);
        }

    }
}
