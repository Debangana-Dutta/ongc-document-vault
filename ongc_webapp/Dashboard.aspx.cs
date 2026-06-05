using System;
using System.Configuration;
using System.Data;
using Npgsql;

namespace ongc_webapp
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private string connString = ConfigurationManager.ConnectionStrings["PostgresConn"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadLiveSystemSummary();
            }
        }

        private void LoadLiveSystemSummary()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string countQuery = "SELECT COUNT(*) AS total_files FROM indexed_documents";
                using (NpgsqlCommand cmd = new NpgsqlCommand(countQuery, conn))
                {
                    object result = cmd.ExecuteScalar();
                    long totalFiles = (result != null) ? Convert.ToInt64(result) : 0;

                    lblTotalFiles.Text = string.Format("{0:N0}", totalFiles);
                    lblIndexedSuccess.Text = string.Format("{0:N0}", totalFiles);
                    lblPendingIndexing.Text = "0";
                }

                string logQuery = @"SELECT id, file_name, file_path, dynamic_metadata 
                                    FROM indexed_documents 
                                    ORDER BY id DESC LIMIT 5";

                using (NpgsqlCommand cmd2 = new NpgsqlCommand(logQuery, conn))
                {
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd2))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        rptRecentLogs.DataSource = dt;
                        rptRecentLogs.DataBind();
                    }
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}