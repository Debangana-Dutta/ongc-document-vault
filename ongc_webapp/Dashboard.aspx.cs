using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using Npgsql;

namespace ongc_webapp
{
    public partial class Dashboard : System.Web.UI.Page
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
                LoadLiveSystemSummary();
            }
        }

        // -----------------------------------------------------------------------
        // METRICS AGGREGATION ENGINE
        // -----------------------------------------------------------------------
        private void LoadLiveSystemSummary()
        {
            // Fully updated to calculate system metrics directly from your dynamic_metadata JSONB column
            string countQuery = @"
                SELECT 
                    COUNT(*) as total,
                    COUNT(CASE WHEN dynamic_metadata IS NOT NULL THEN 1 END) as indexed,
                    COUNT(CASE WHEN dynamic_metadata IS NULL THEN 1 END) as pending
                FROM public.indexed_documents;";

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(countQuery, conn))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                long totalFiles = reader["total"] != DBNull.Value ? Convert.ToInt64(reader["total"]) : 0;
                                long indexedFiles = reader["indexed"] != DBNull.Value ? Convert.ToInt64(reader["indexed"]) : 0;
                                long pendingFiles = reader["pending"] != DBNull.Value ? Convert.ToInt64(reader["pending"]) : 0;

                                // Bind parameters safely to your friend's frontend UI labels
                                lblTotalFiles.Text = totalFiles.ToString("#,##0");
                                lblIndexedSuccess.Text = indexedFiles.ToString("#,##0");
                                lblPendingIndexing.Text = pendingFiles.ToString("#,##0");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblTotalFiles.Text = "0";
                lblIndexedSuccess.Text = "0";
                lblPendingIndexing.Text = "0";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx", true);
        }
    }
}