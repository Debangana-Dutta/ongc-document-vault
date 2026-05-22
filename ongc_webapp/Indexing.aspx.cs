using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;

namespace ongc_webapp
{
    public partial class Indexing : System.Web.UI.Page
    {
        private readonly string connString =
            ConfigurationManager.ConnectionStrings["PostgresConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx", true);
                return;
            }

            if (!IsPostBack)
            {
                RenderVaultMatrix();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            RenderVaultMatrix();
        }

        private void RenderVaultMatrix()
        {
            phResultsContainer.Controls.Clear();
            string keyword = txtSearch.Text.Trim().ToLower();
            bool hasFilter = !string.IsNullOrEmpty(keyword);

            // Fetch dynamic row collections from JSONB
            string sql = "SELECT id, file_name, file_path, dynamic_metadata FROM public.indexed_documents ORDER BY uploaded_at DESC";

            DataTable dt = new DataTable();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                litMatchCount.Text = "0";
                return;
            }

            if (dt.Rows.Count == 0)
            {
                pnlEmptyState.Visible = true;
                pnlResultsSummary.Visible = false;
                litMatchCount.Text = "0";
                return;
            }

            pnlEmptyState.Visible = false;
            pnlResultsSummary.Visible = true;

            JavaScriptSerializer js = new JavaScriptSerializer { MaxJsonLength = 20971520 };
            StringBuilder sb = new StringBuilder();

            // Setup true enterprise tabular layout structure matching your UI mockup
            sb.Append("<table class='table table-striped table-hover mb-0 align-middle' style='font-size: 11px;'>");

            bool isHeaderRendered = false;
            int matchCounter = 0;

            foreach (DataRow dr in dt.Rows)
            {
                string rawJson = dr["dynamic_metadata"]?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(rawJson)) continue;

                Dictionary<string, object> envelope;
                try { envelope = js.Deserialize<Dictionary<string, object>>(rawJson); }
                catch { continue; }

                if (envelope == null || !envelope.ContainsKey("headers") || !envelope.ContainsKey("rows")) continue;

                var headers = envelope["headers"] as System.Collections.IEnumerable;
                var rows = envelope["rows"] as System.Collections.IEnumerable;

                if (headers == null || rows == null) continue;

                // Lock down headers vector once dynamically from data rows
                if (!isHeaderRendered)
                {
                    sb.Append("<thead style='background-color: #800020; color: white;' class='text-nowrap'><tr>");
                    sb.Append("<th style='background-color: #7a0c1a; color:white;'>id</th>");
                    sb.Append("<th style='background-color: #7a0c1a; color:white;'>file_name</th>");
                    sb.Append("<th style='background-color: #7a0c1a; color:white;'>file_path</th>");

                    foreach (var h in headers)
                    {
                        sb.Append($"<th style='background-color: #7a0c1a; color:white;'>{Server.HtmlEncode(h?.ToString())}</th>");
                    }
                    sb.Append("</tr></thead><tbody>");
                    isHeaderRendered = true;
                }

                // Process data records matching column offsets
                foreach (var r in rows)
                {
                    var cells = r as System.Collections.IEnumerable;
                    if (cells == null) continue;

                    List<string> cellValues = new List<string>();
                    bool isRowMatched = false;

                    foreach (var c in cells)
                    {
                        string val = c?.ToString() ?? string.Empty;
                        cellValues.Add(val);
                        if (hasFilter && val.ToLower().Contains(keyword)) isRowMatched = true;
                    }

                    // Global filename/ID fallback match filter checks
                    if (hasFilter && !isRowMatched)
                    {
                        if (dr["id"].ToString().ToLower().Contains(keyword) ||
                            dr["file_name"].ToString().ToLower().Contains(keyword))
                        {
                            isRowMatched = true;
                        }
                    }

                    if (hasFilter && !isRowMatched) continue;
                    matchCounter++;

                    // Append data rows matching UI specification
                    sb.Append("<tr>");
                    sb.Append($"<td class='text-muted'>{dr["id"]}</td>");
                    sb.Append($"<td class='fw-semibold'>{Server.HtmlEncode(dr["file_name"].ToString())}</td>");
                    sb.Append($"<td class='text-secondary'>{Server.HtmlEncode(dr["file_path"].ToString())}</td>");

                    foreach (var val in cellValues)
                    {
                        sb.Append($"<td>{Server.HtmlEncode(val)}</td>");
                    }
                    sb.Append("</tr>");
                }
            }

            sb.Append("</tbody></table>");
            litMatchCount.Text = matchCounter.ToString();

            // Inject the complete dynamic table tree into the placeholder element
            phResultsContainer.Controls.Add(new LiteralControl(sb.ToString()));
        }
    }
}