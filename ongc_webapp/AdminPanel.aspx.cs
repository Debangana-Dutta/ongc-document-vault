// ============================================================
//  AdminPanel.aspx.cs
//  ONGC Document Portal – Admin Panel Code-Behind
//
//  FIXES IN THIS VERSION
//  ─────────────────────────────────────────────────────────
//  FIX 1  btnIngestData_Click: the datasets INSERT was commented
//         out / stubbed.  It is now fully implemented inside the
//         per-file loop so every uploaded filename is registered
//         in the datasets table before BindDatasetCheckBoxList()
//         refreshes the UI.
//
//  FIX 2  ddlSelectUser_SelectedIndexChanged: when no policy row
//         exists for the selected user all metadata checkboxes
//         now default to UNCHECKED (access must be explicitly
//         granted) instead of all-checked.
//
//  FIX 3  btnIngestData_Click: full ingestion logic is restored
//         (was stubbed with comments).  After the loop completes,
//         BindDatasetCheckBoxList() is called so the new dataset
//         appears in the policy panel immediately.
// ============================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using Npgsql;
using NpgsqlTypes;
using Newtonsoft.Json;

namespace ongc_webapp
{
    public partial class AdminPanel : System.Web.UI.Page
    {
        private string connString =
            System.Configuration.ConfigurationManager
                .ConnectionStrings["PostgresConn"]
                .ConnectionString;

        // ════════════════════════════════════════════════════════
        //  PAGE LOAD
        //  Bind* calls are inside !IsPostBack so that the
        //  AutoPostBack dropdown does not reset checkbox state.
        // ════════════════════════════════════════════════════════
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindUserGrid();
                BindUserDropDown();
                BindDatasetCheckBoxList();
                BindColumnCheckBoxList();
            }
        }

        // ════════════════════════════════════════════════════════
        //  1. USER MANAGEMENT
        // ════════════════════════════════════════════════════════
        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            string newCpf = txtCPF.Text.Trim();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    // Pre-check for duplicate CPF
                    using (NpgsqlCommand chk = new NpgsqlCommand(
                        "SELECT COUNT(1) FROM users WHERE cpf = @cpf", conn))
                    {
                        chk.Parameters.AddWithValue("cpf", newCpf);
                        long existing = (long)chk.ExecuteScalar();
                        if (existing > 0)
                        {
                            ShowFeedback(lblAdminFeedback,
                                "⚠ A user with CPF " + newCpf + " already exists.", false);
                            return;
                        }
                    }

                    string query =
                        "INSERT INTO users (username, cpf, department) " +
                        "VALUES (@username, @cpf, @dept) " +
                        "ON CONFLICT (cpf) DO NOTHING";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("username", txtUserName.Text.Trim());
                        cmd.Parameters.AddWithValue("cpf", newCpf);
                        cmd.Parameters.AddWithValue("dept", txtDept.Text.Trim());
                        cmd.ExecuteNonQuery();
                    }
                }

                txtUserName.Text = txtCPF.Text = txtDept.Text = "";
                BindUserGrid();
                BindUserDropDown();
                ShowFeedback(lblAdminFeedback, "✔ User added successfully.", true);
            }
            catch (Exception ex)
            {
                ShowFeedback(lblAdminFeedback, "Error adding user: " + ex.Message, false);
            }
        }

        private void BindUserGrid()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    string query =
                        "SELECT username AS \"Name\", cpf AS \"CPF\", " +
                        "department AS \"Department\" FROM users ORDER BY username";
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvUsers.DataSource = dt;
                    gvUsers.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BindUserGrid: " + ex.Message);
            }
        }

        private void BindUserDropDown()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(
                        "SELECT cpf, username FROM users ORDER BY username", conn))
                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        ddlSelectUser.Items.Clear();
                        ddlSelectUser.Items.Add(new ListItem("-- Select User --", ""));
                        while (dr.Read())
                            ddlSelectUser.Items.Add(
                                new ListItem(
                                    dr["username"] + " (" + dr["cpf"] + ")",
                                    dr["cpf"].ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BindUserDropDown: " + ex.Message);
            }
        }

        // Loads datasets using integer PK + display name.
        // Expects schema:  datasets (datasetid SERIAL PK, datasetname TEXT UNIQUE)
        private void BindDatasetCheckBoxList()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query =
                        "SELECT datasetid, datasetname FROM datasets ORDER BY datasetname";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        cblDatasets.Items.Clear();
                        while (dr.Read())
                            cblDatasets.Items.Add(
                                new ListItem(
                                    dr["datasetname"].ToString(),
                                    dr["datasetid"].ToString()));  // integer id as string value
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BindDatasetCheckBoxList: " + ex.Message);
            }
        }

        private void BindColumnCheckBoxList()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query =
                        "SELECT DISTINCT jsonb_object_keys(dynamic_metadata) " +
                        "FROM indexed_documents " +
                        "WHERE dynamic_metadata IS NOT NULL " +
                        "ORDER BY 1";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        cblMetadataColumns.Items.Clear();
                        while (dr.Read())
                            cblMetadataColumns.Items.Add(
                                new ListItem(dr.GetString(0), dr.GetString(0)));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BindColumnCheckBoxList: " + ex.Message);
            }
        }

        // ════════════════════════════════════════════════════════
        //  2. LOAD EXISTING POLICY
        // ════════════════════════════════════════════════════════
        protected void ddlSelectUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cpf = ddlSelectUser.SelectedValue;
            if (string.IsNullOrEmpty(cpf)) return;

            // Repopulate lists fresh before applying saved selections
            BindDatasetCheckBoxList();
            BindColumnCheckBoxList();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    // ── Dataset grants ──────────────────────────────────
                    HashSet<int> grantedIds = new HashSet<int>();
                    string dsQuery =
                        "SELECT datasetid FROM user_dataset_access " +
                        "WHERE userid = (SELECT id FROM users WHERE cpf = @cpf)";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(dsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("cpf", cpf);
                        using (NpgsqlDataReader dr = cmd.ExecuteReader())
                            while (dr.Read())
                                grantedIds.Add(dr.GetInt32(0));
                    }

                    foreach (ListItem item in cblDatasets.Items)
                        if (int.TryParse(item.Value, out int dsId))
                            item.Selected = grantedIds.Contains(dsId);

                    // ── Metadata column policy ──────────────────────────
                    string colQuery =
                        "SELECT visible_columns FROM user_metadata_policy " +
                        "WHERE user_cpf = @cpf";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(colQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("cpf", cpf);
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            // Policy row exists: restore saved selections
                            List<string> visibleCols =
                                JsonConvert.DeserializeObject<List<string>>(result.ToString());
                            HashSet<string> colSet = new HashSet<string>(visibleCols);
                            foreach (ListItem item in cblMetadataColumns.Items)
                                item.Selected = colSet.Contains(item.Value);
                        }
                        else
                        {
                            // FIX 2: No policy row yet → default to ALL UNCHECKED.
                            // Admin must explicitly grant columns rather than
                            // accidentally saving an all-access policy.
                            foreach (ListItem item in cblMetadataColumns.Items)
                                item.Selected = false;
                        }
                    }
                }

                ShowFeedback(lblPolicyFeedback, "Policy loaded for selected user.", true);
            }
            catch (Exception ex)
            {
                ShowFeedback(lblPolicyFeedback, "Error loading policy: " + ex.Message, false);
            }
        }

        // ════════════════════════════════════════════════════════
        //  3. SAVE ACCESS POLICY
        // ════════════════════════════════════════════════════════
        protected void btnSaveAccessPolicy_Click(object sender, EventArgs e)
        {
            string cpf = ddlSelectUser.SelectedValue;
            if (string.IsNullOrEmpty(cpf))
            {
                ShowFeedback(lblPolicyFeedback, "⚠ Please select a user first.", false);
                return;
            }

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    using (NpgsqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            // ── a) Dataset access ────────────────────────────
                            string deleteQuery =
                                "DELETE FROM user_dataset_access " +
                                "WHERE userid = (SELECT id FROM users WHERE cpf = @cpf)";

                            using (NpgsqlCommand cmd =
                                new NpgsqlCommand(deleteQuery, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("cpf", cpf);
                                cmd.ExecuteNonQuery();
                            }

                            foreach (ListItem item in cblDatasets.Items)
                            {
                                if (!item.Selected ||
                                    !int.TryParse(item.Value, out int datasetIntId))
                                    continue;

                                string insertQuery =
                                    "INSERT INTO user_dataset_access (userid, datasetid) " +
                                    "VALUES ((SELECT id FROM users WHERE cpf = @cpf), @datasetid)";

                                using (NpgsqlCommand cmd =
                                    new NpgsqlCommand(insertQuery, conn, tx))
                                {
                                    cmd.Parameters.AddWithValue("cpf", cpf);
                                    cmd.Parameters.Add(
                                        new NpgsqlParameter("datasetid", NpgsqlDbType.Integer)
                                        { Value = datasetIntId });
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // ── b) Metadata column policy ────────────────────
                            List<string> selectedCols = new List<string>();
                            foreach (ListItem item in cblMetadataColumns.Items)
                                if (item.Selected) selectedCols.Add(item.Value);

                            // If nothing is selected store an empty JSON array
                            // (not NULL) so the search layer correctly returns
                            // no metadata columns rather than all of them.
                            bool allSelected =
                                selectedCols.Count == cblMetadataColumns.Items.Count;

                            string colJson = allSelected
                                ? null
                                : JsonConvert.SerializeObject(selectedCols);

                            string upsertColQuery = (colJson == null)
                                ? @"INSERT INTO user_metadata_policy
                                        (user_cpf, visible_columns, updated_at)
                                    VALUES (@cpf, NULL, NOW())
                                    ON CONFLICT (user_cpf)
                                    DO UPDATE SET visible_columns = NULL,
                                                  updated_at = NOW()"
                                : @"INSERT INTO user_metadata_policy
                                        (user_cpf, visible_columns, updated_at)
                                    VALUES (@cpf, @cols::jsonb, NOW())
                                    ON CONFLICT (user_cpf)
                                    DO UPDATE SET visible_columns = @cols::jsonb,
                                                  updated_at = NOW()";

                            using (NpgsqlCommand cmd =
                                new NpgsqlCommand(upsertColQuery, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("cpf", cpf);
                                if (colJson != null)
                                    cmd.Parameters.AddWithValue("cols", colJson);
                                cmd.ExecuteNonQuery();
                            }

                            tx.Commit();
                            ShowFeedback(lblPolicyFeedback,
                                "✔ Access policy saved for CPF: " + cpf, true);
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowFeedback(lblPolicyFeedback, "Error saving policy: " + ex.Message, false);
            }
        }

        // ════════════════════════════════════════════════════════
        //  4. DOCUMENT INGESTION
        //
        //  FIX 1 & FIX 3: Full ingestion logic is restored.
        //  The datasets INSERT is now inside the per-file loop
        //  (not commented out), so each uploaded filename is
        //  registered in datasets before the UI refresh.
        //
        //  Flow per uploaded file:
        //    a) Parse every data row → INSERT into indexed_documents
        //    b) Register the filename in datasets (ON CONFLICT DO NOTHING
        //       means re-uploading the same file is safe)
        //    c) After the loop → BindDatasetCheckBoxList() refreshes
        //       the policy panel so the new dataset is immediately
        //       available without a full page reload.
        // ════════════════════════════════════════════════════════
        protected void btnIngestData_Click(object sender, EventArgs e)
        {
            if (!filePayload.HasFiles)
            {
                ShowFeedback(lblStatusFeedback, "⚠️ Please upload at least one Excel file.", false);
                return;
            }

            int successCount = 0;

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    foreach (System.Web.HttpPostedFile uploadedFile in filePayload.PostedFiles)
                    {
                        using (var workbook = new XLWorkbook(uploadedFile.InputStream))
                        {
                            var worksheet = workbook.Worksheet(1);
                            int lastRow = worksheet.LastRowUsed().RowNumber();
                            int lastColumn = worksheet.LastColumnUsed().ColumnNumber();

                            // Build header list from row 1
                            List<string> headers = new List<string>();
                            for (int col = 1; col <= lastColumn; col++)
                                headers.Add(
                                    worksheet.Cell(1, col)
                                             .GetValue<string>()
                                             .Trim()
                                             .ToLower()
                                             .Replace(" ", "_"));

                            // Process each data row
                            for (int row = 2; row <= lastRow; row++)
                            {
                                string actualFileName = "";
                                string actualFilePath = "";
                                var dynamicMetadata = new Dictionary<string, string>();

                                for (int col = 1; col <= lastColumn; col++)
                                {
                                    string val = worksheet.Cell(row, col)
                                                          .GetValue<string>()
                                                          .Trim();
                                    if (string.IsNullOrWhiteSpace(val)) continue;

                                    string header = headers[col - 1];
                                    if (header == "file_name") actualFileName = val;
                                    else if (header == "file_path" ||
                                             header == "path") actualFilePath = val;
                                    else dynamicMetadata[header] = val;
                                }

                                // Only insert rows that have both required fields
                                if (string.IsNullOrWhiteSpace(actualFileName) ||
                                    string.IsNullOrWhiteSpace(actualFilePath))
                                    continue;

                                using (NpgsqlCommand cmd = new NpgsqlCommand(
                                    "INSERT INTO indexed_documents " +
                                    "  (file_name, file_path, source_excel_file, dynamic_metadata) " +
                                    "VALUES (@fn, @fp, @sef, @meta::jsonb)", conn))
                                {
                                    cmd.Parameters.AddWithValue("fn", actualFileName);
                                    cmd.Parameters.AddWithValue("fp", actualFilePath);
                                    cmd.Parameters.AddWithValue("sef", uploadedFile.FileName);
                                    cmd.Parameters.AddWithValue("meta",
                                        JsonConvert.SerializeObject(dynamicMetadata));
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        } // workbook disposed here

                        // ── FIX 1 & 3: register filename in datasets table ──
                        // This was the missing / commented-out step.
                        // ON CONFLICT (datasetname) DO NOTHING makes re-upload safe.
                        using (NpgsqlCommand regCmd = new NpgsqlCommand(
                            "INSERT INTO datasets (datasetname) " +
                            "VALUES (@name) " +
                            "ON CONFLICT (datasetname) DO NOTHING", conn))
                        {
                            regCmd.Parameters.AddWithValue("name", uploadedFile.FileName);
                            regCmd.ExecuteNonQuery();
                        }

                        successCount++;
                    }
                }

                // ── FIX 3: refresh dataset list so new entry appears immediately ──
                BindDatasetCheckBoxList();

                ShowFeedback(lblStatusFeedback,
                    successCount + " file(s) ingested and registered successfully.", true);
            }
            catch (Exception ex)
            {
                ShowFeedback(lblStatusFeedback, "Ingestion failed: " + ex.Message, false);
            }
        }

        // ════════════════════════════════════════════════════════
        //  HELPERS
        // ════════════════════════════════════════════════════════
        private void ShowFeedback(Label label, string message, bool success)
        {
            label.Text = message;
            label.ForeColor = success
                ? System.Drawing.Color.MediumSeaGreen
                : System.Drawing.Color.Crimson;
        }
    }
}
