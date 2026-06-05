using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI.WebControls;
using Npgsql;

namespace ongc_webapp
{
    public partial class ManageUserAccess : System.Web.UI.Page
    {
        private string connString =
            ConfigurationManager
                .ConnectionStrings["PostgresConn"]
                .ConnectionString;

        private int UserId
        {
            get
            {
                return Convert.ToInt32(
                    Request.QueryString["userid"]);
            }
        }

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (Session["Role"] == null ||
                Session["Role"].ToString().ToUpper() != "ADMIN")
            {
                Response.Redirect("Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadUserInfo();
                LoadDatasets();
                LoadUserDatasets();
                LoadMetadataForSelectedDatasets();
                LoadUserMetadata();
            }
        }

        private void LoadUserInfo()
        {
            using (NpgsqlConnection conn =
                new NpgsqlConnection(connString))
            {
                conn.Open();

                string query =
                @"SELECT
                    username,
                    role,
                    account_status
                  FROM users
                  WHERE id=@id";

                using (NpgsqlCommand cmd =
                    new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "id",
                        UserId);

                    using (NpgsqlDataReader dr =
                        cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            lblUserInfo.Text =
                                "User: " +
                                dr["username"] +
                                "<br/>Role: " +
                                dr["role"] +
                                "<br/>Status: " +
                                dr["account_status"];
                        }
                    }
                }
            }
        }

        private void LoadDatasets()
        {
            using (NpgsqlConnection conn =
                new NpgsqlConnection(connString))
            {
                conn.Open();

                string query =
                @"SELECT dataset_name
                  FROM datasets
                  ORDER BY dataset_name";

                using (NpgsqlCommand cmd =
                    new NpgsqlCommand(query, conn))
                using (NpgsqlDataReader dr =
                    cmd.ExecuteReader())
                {
                    cblDatasets.Items.Clear();

                    while (dr.Read())
                    {
                        string ds =
                            dr["dataset_name"]
                                .ToString();

                        cblDatasets.Items.Add(
                            new ListItem(ds, ds));
                    }
                }
            }
        }

        private void LoadUserDatasets()
        {
            using (NpgsqlConnection conn =
                new NpgsqlConnection(connString))
            {
                conn.Open();

                string query =
                @"SELECT dataset_name
                  FROM user_dataset_access
                  WHERE userid=@userid";

                using (NpgsqlCommand cmd =
                    new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "userid",
                        UserId);

                    using (NpgsqlDataReader dr =
                        cmd.ExecuteReader())
                    {
                        HashSet<string> datasets =
                            new HashSet<string>();

                        while (dr.Read())
                        {
                            datasets.Add(
                                dr["dataset_name"]
                                    .ToString());
                        }

                        foreach (ListItem item in cblDatasets.Items)
                        {
                            item.Selected =
                                datasets.Contains(
                                    item.Value);
                        }
                    }
                }
            }
        }

        protected void btnLoadMetadata_Click(
            object sender,
            EventArgs e)
        {
            LoadMetadataForSelectedDatasets();
            LoadUserMetadata();
        }

        private void LoadMetadataForSelectedDatasets()
        {
            List<string> datasets =
                new List<string>();

            foreach (ListItem item in cblDatasets.Items)
            {
                if (item.Selected)
                    datasets.Add(item.Value);
            }

            cblMetadata.Items.Clear();

            if (datasets.Count == 0)
                return;

            using (NpgsqlConnection conn =
                new NpgsqlConnection(connString))
            {
                conn.Open();

                string query =
                @"SELECT DISTINCT
                    jsonb_object_keys(dynamic_metadata)
                  FROM indexed_documents
                  WHERE dataset_name = ANY(@datasets)
                  ORDER BY 1";

                using (NpgsqlCommand cmd =
                    new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "datasets",
                        datasets.ToArray());

                    using (NpgsqlDataReader dr =
                        cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string col =
                                dr.GetString(0);

                            cblMetadata.Items.Add(
                                new ListItem(col, col));
                        }
                    }
                }
            }
        }

        private void LoadUserMetadata()
        {
            using (NpgsqlConnection conn =
                new NpgsqlConnection(connString))
            {
                conn.Open();

                string query =
                @"SELECT metadata_name
                  FROM user_metadata_access
                  WHERE userid=@userid";

                using (NpgsqlCommand cmd =
                    new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "userid",
                        UserId);

                    using (NpgsqlDataReader dr =
                        cmd.ExecuteReader())
                    {
                        HashSet<string> metadata =
                            new HashSet<string>();

                        while (dr.Read())
                        {
                            metadata.Add(
                                dr["metadata_name"]
                                    .ToString());
                        }

                        foreach (ListItem item in cblMetadata.Items)
                        {
                            item.Selected =
                                metadata.Contains(
                                    item.Value);
                        }
                    }
                }
            }
        }

        protected void chkSelectAllDatasets_CheckedChanged(
        object sender,
        EventArgs e)
        {
            foreach (ListItem item in cblDatasets.Items)
            {
                item.Selected = chkSelectAllDatasets.Checked;
            }

            LoadMetadataForSelectedDatasets();
        }

        protected void chkSelectAllMetadata_CheckedChanged(
            object sender,
            EventArgs e)
        {
            foreach (ListItem item in cblMetadata.Items)
            {
                item.Selected = chkSelectAllMetadata.Checked;
            }
        }

        protected void btnSave_Click(
            object sender,
            EventArgs e)
        {
            using (NpgsqlConnection conn =
                new NpgsqlConnection(connString))
            {
                conn.Open();

                using (NpgsqlTransaction tx =
                    conn.BeginTransaction())
                {
                    try
                    {
                        new NpgsqlCommand(
                            @"DELETE FROM user_dataset_access
                              WHERE userid=@userid",
                            conn,
                            tx)
                        {
                            Parameters =
                            {
                                new NpgsqlParameter(
                                    "userid",
                                    UserId)
                            }
                        }.ExecuteNonQuery();

                        new NpgsqlCommand(
                            @"DELETE FROM user_metadata_access
                              WHERE userid=@userid",
                            conn,
                            tx)
                        {
                            Parameters =
                            {
                                new NpgsqlParameter(
                                    "userid",
                                    UserId)
                            }
                        }.ExecuteNonQuery();

                        foreach (ListItem item in cblDatasets.Items)
                        {
                            if (!item.Selected)
                                continue;

                            using (NpgsqlCommand cmd =
                                new NpgsqlCommand(
                                    @"INSERT INTO user_dataset_access
                                      (userid,dataset_name)
                                      VALUES
                                      (@userid,@dataset)",
                                    conn,
                                    tx))
                            {
                                cmd.Parameters.AddWithValue(
                                    "userid",
                                    UserId);

                                cmd.Parameters.AddWithValue(
                                    "dataset",
                                    item.Value);

                                cmd.ExecuteNonQuery();
                            }
                        }

                        foreach (ListItem item in cblMetadata.Items)
                        {
                            if (!item.Selected)
                                continue;

                            using (NpgsqlCommand cmd =
                                new NpgsqlCommand(
                                    @"INSERT INTO user_metadata_access
                                      (userid,metadata_name)
                                      VALUES
                                      (@userid,@metadata)",
                                    conn,
                                    tx))
                            {
                                cmd.Parameters.AddWithValue(
                                    "userid",
                                    UserId);

                                cmd.Parameters.AddWithValue(
                                    "metadata",
                                    item.Value);

                                cmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }

            Response.Redirect(
                "AdminPanel.aspx");
        }
    }
}
