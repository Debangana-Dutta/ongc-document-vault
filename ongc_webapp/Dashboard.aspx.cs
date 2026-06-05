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

                    string userStatsQuery =
                    @"
                    SELECT
                        SUM(CASE WHEN account_status='APPROVED' THEN 1 ELSE 0 END) AS approved,
                        SUM(CASE WHEN account_status='PENDING' THEN 1 ELSE 0 END) AS pending,
                        SUM(CASE WHEN account_status='REJECTED' THEN 1 ELSE 0 END) AS rejected
                    FROM users";

                    using (NpgsqlCommand cmdUsers =
                    new NpgsqlCommand(userStatsQuery, conn))
                    {
                        using (NpgsqlDataReader dr =
                            cmdUsers.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                lblApprovedUsers.Text =
                                    Convert.ToString(dr["approved"]);

                                lblPendingUsers.Text =
                                    Convert.ToString(dr["pending"]);

                                lblRejectedUsers.Text =
                                    Convert.ToString(dr["rejected"]);
                            }
                        }
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