using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI.WebControls;
using Npgsql;
using System.Linq;
using System.Web.UI;


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

        private void LoadPresets()
        {
            string adminUsername =
                Session["Username"]?.ToString();

            using (NpgsqlConnection conn =
                new NpgsqlConnection(connString))
            {
                conn.Open();

                string query =
                    @"SELECT id, preset_name
              FROM access_presets
              WHERE admin_username=@admin
              ORDER BY preset_name";

                using (NpgsqlCommand cmd =
                    new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@admin",
                        adminUsername);

                    ddlPresets.DataSource =
                        cmd.ExecuteReader();

                    ddlPresets.DataTextField =
                        "preset_name";

                    ddlPresets.DataValueField =
                        "id";

                    ddlPresets.DataBind();
                }
            }
        }

        protected void btnSavePreset_Click(
        object sender,
        EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPresetName.Text))
            {
                lblStatus.Text =
                    "Please enter a preset name.";

                return;
            }
            string adminUsername =
                Session["Username"]?.ToString();

            List<string> datasets =
                cblDatasets.Items.Cast<ListItem>()
                .Where(i => i.Selected)
                .Select(i => i.Value)
                .ToList();

            string datasetString =
                string.Join(",", datasets);

            if (datasets.Count == 0)
            {
                lblStatus.Text =
                    "Select at least one dataset before saving a preset.";

                return;
            }
            List<string> metadata =
            new List<string>();

            foreach (Control ctrl in phMetadataSections.Controls)
            {
                if (ctrl is CheckBoxList cbl)
                {
                    foreach (ListItem item in cbl.Items)
                    {
                        if (item.Selected)
                        {
                            metadata.Add(item.Value);
                        }
                    }
                }
            }

            string metadataString =
                string.Join(",", metadata);


            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string checkQuery =
                @"SELECT COUNT(*)
                  FROM access_presets
                  WHERE admin_username=@admin
                  AND preset_name=@name";

                using (NpgsqlCommand checkCmd =
                    new NpgsqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue(
                        "@admin",
                        adminUsername);

                    checkCmd.Parameters.AddWithValue(
                        "@name",
                        txtPresetName.Text.Trim());

                    long exists =
                        Convert.ToInt64(
                            checkCmd.ExecuteScalar());

                    if (exists > 0)
                    {
                        lblStatus.Text =
                            "A preset with that name already exists.";

                        return;
                    }
                }

                string query =
                    @"INSERT INTO access_presets
            (
                admin_username,
                preset_name,
                datasets,
                metadata_columns
            )
            VALUES
            (
                @admin,
                @name,
                @datasets,
                @metadata
            )";

                using (NpgsqlCommand cmd =
                    new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@admin",
                        adminUsername);

                    cmd.Parameters.AddWithValue(
                        "@name",
                        txtPresetName.Text.Trim());

                    cmd.Parameters.AddWithValue(
                        "@datasets",
                        datasetString);

                    cmd.Parameters.AddWithValue(
                        "@metadata",
                        metadataString);

                    cmd.ExecuteNonQuery();
                }
            }

            LoadPresets();

            lblStatus.Text =
                "Preset saved successfully.";
        }

        protected void btnLoadPreset_Click(
        object sender,
        EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(ddlPresets.SelectedValue))
            {
                lblStatus.Text =
                    "Please select a preset.";

                return;
            }
            using (NpgsqlConnection conn =
                new NpgsqlConnection(connString))
            {
                conn.Open();

                string query =
                    @"SELECT
                datasets,
                metadata_columns
              FROM access_presets
              WHERE id=@id";

                using (NpgsqlCommand cmd =
                    new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@id",
                        Convert.ToInt32(
                            ddlPresets.SelectedValue));

                    using (NpgsqlDataReader dr =
                        cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            string[] datasets =
                                dr["datasets"]
                                .ToString()
                                .Split(',');

                            string[] metadata =
                                dr["metadata_columns"]
                                .ToString()
                                .Split(',');

                            foreach (ListItem item
                                in cblDatasets.Items)
                            {
                                item.Selected =
                                    datasets.Contains(
                                        item.Value);
                            }
                        }
                    }
                }
            }

            lblStatus.Text =
                "Preset loaded successfully.";
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
                LoadPresets();

                LoadUserInfo();
                LoadDatasets();
                LoadUserDatasets();
            }

            BuildMetadataSections();
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
            BuildMetadataSections();
        }

        protected void chkSelectAllDatasets_CheckedChanged(
                        object sender,
                        EventArgs e)
        {
            foreach (ListItem item in cblDatasets.Items)
            {
                item.Selected = chkSelectAllDatasets.Checked;
            }

            BuildMetadataSections();
        }

        private void BuildMetadataSections()
        {
            phMetadataSections.Controls.Clear();

            foreach (ListItem datasetItem
                in cblDatasets.Items)
            {
                if (!datasetItem.Selected)
                    continue;

                string dataset =
                    datasetItem.Value;
                Literal startSection =
                new Literal();

                startSection.Text =
                    phMetadataSections.Controls.Count == 0
                    ? "<details open>"
                    : "<details>";

                phMetadataSections.Controls.Add(
                    startSection);

                phMetadataSections.Controls.Add(
                    new Literal
                    {
                        Text =
                        "<summary style='cursor:pointer;" +
                        "font-weight:bold;" +
                        "padding:8px;" +
                        "background:#f3f4f6;" +
                        "border-radius:4px;" +
                        "margin-bottom:8px;'>" +
                        dataset +
                        "</summary>"
                    });


                CheckBoxList cbl =
                    new CheckBoxList();

                cbl.ID =
                    "meta_" + dataset;

                HashSet<string> selectedMetadata =
                        new HashSet<string>();

                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    string query =
                    @"SELECT metadata_name
                      FROM user_metadata_access
                      WHERE userid=@userid
                      AND dataset_name=@dataset";

                    using (NpgsqlCommand cmd =
                        new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "userid",
                            UserId);

                        cmd.Parameters.AddWithValue(
                            "dataset",
                            dataset);

                        using (NpgsqlDataReader dr =
                            cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                selectedMetadata.Add(
                                    dr["metadata_name"]
                                        .ToString());
                            }
                        }
                    }
                }

                

                using (NpgsqlConnection conn =
                    new NpgsqlConnection(connString))
                {
                    conn.Open();

                    string query =
                    @"SELECT DISTINCT
                jsonb_object_keys(dynamic_metadata)
              FROM indexed_documents
              WHERE dataset_name=@dataset
              ORDER BY 1";

                    using (NpgsqlCommand cmd =
                        new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "dataset",
                            dataset);

                        using (NpgsqlDataReader dr =
                            cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                string col =
                                    dr.GetString(0);

                                ListItem li =
                                    new ListItem(col, col);

                                li.Selected =
                                    selectedMetadata.Contains(col);

                                cbl.Items.Add(li);
                            }
                        }
                    }
                }

                phMetadataSections.Controls.Add(cbl);

                phMetadataSections.Controls.Add(
                    new Literal
                    {
                        Text = "</details><br/>"
                    });
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
                                        (
                                            userid,
                                            dataset_name
                                        )
                                        VALUES
                                        (
                                            @userid,
                                            @dataset
                                        )",
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

                        foreach (ListItem datasetItem in cblDatasets.Items)
                        {
                            if (!datasetItem.Selected)
                                continue;

                            string dataset =
                                datasetItem.Value;

                            CheckBoxList cbl = null;

                            foreach (Control ctrl in phMetadataSections.Controls)
                            {
                                if (ctrl is CheckBoxList &&
                                    ctrl.ID == "meta_" + dataset)
                                {
                                    cbl = (CheckBoxList)ctrl;
                                    break;
                                }
                            }

                            if (cbl == null)
                                continue;

                            foreach (ListItem item in cbl.Items)
                            {
                                if (!item.Selected)
                                    continue;

                                using (NpgsqlCommand cmd =
                                    new NpgsqlCommand(
                                        @"INSERT INTO user_metadata_access
                                              (
                                                  userid,
                                                  dataset_name,
                                                  metadata_name
                                              )
                                              VALUES
                                              (
                                                  @userid,
                                                  @dataset,
                                                  @metadata
                                              )",
                                        conn,
                                        tx))
                                {
                                    cmd.Parameters.AddWithValue(
                                        "userid",
                                        UserId);

                                    cmd.Parameters.AddWithValue(
                                        "dataset",
                                        dataset);

                                    cmd.Parameters.AddWithValue(
                                        "metadata",
                                        item.Value);

                                    cmd.ExecuteNonQuery();
                                }
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
