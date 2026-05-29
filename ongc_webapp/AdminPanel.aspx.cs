// ============================================================
//  AdminPanel.aspx.cs
//  ONGC Document Portal – Admin Panel Code-Behind
//
//  KEY FIX (2025-05-29):
//  The real schema for user_dataset_access is:
//    userid   INTEGER  (FK → users.id)
//    datasetid TEXT    (stores source_excel_file value)
//
//  The dropdown value in C# is the user's CPF (a string).
//  Every query against user_dataset_access therefore uses a
//  subquery:   (SELECT id FROM users WHERE cpf = @cpf)
//  to resolve the integer userid — no unsafe cast required.
//
//  user_metadata_policy still uses user_cpf TEXT, unchanged.
// ============================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using Npgsql;
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
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    // ON CONFLICT (cpf) safeguards against duplicate employee IDs
                    string query =
                        "INSERT INTO users (username, cpf, department) " +
                        "VALUES (@username, @cpf, @dept) " +
                        "ON CONFLICT (cpf) DO NOTHING";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("username", txtUserName.Text.Trim());
                        cmd.Parameters.AddWithValue("cpf", txtCPF.Text.Trim());
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
                System.Diagnostics.Debug.WriteLine("BindUserGrid Error: " + ex.Message);
            }
        }

        // ════════════════════════════════════════════════════════
        //  2. POLICY PANEL — populate dropdowns & checkboxlists
        // ════════════════════════════════════════════════════════

        private void BindUserDropDown()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    using (NpgsqlCommand cmd =
                        new NpgsqlCommand(
                            "SELECT cpf, username FROM users ORDER BY username",
                            conn))
                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        ddlSelectUser.Items.Clear();
                        ddlSelectUser.Items.Add(new ListItem("-- Select User --", ""));
                        while (dr.Read())
                            ddlSelectUser.Items.Add(
                                new ListItem(
                                    dr["username"] + "  (" + dr["cpf"] + ")",
                                    dr["cpf"].ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BindUserDropDown Error: " + ex.Message);
            }
        }

        // Populates datasets from distinct source_excel_file values
        // DB table: indexed_documents
        private void BindDatasetCheckBoxList()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query =
                        "SELECT DISTINCT source_excel_file " +
                        "FROM indexed_documents " +
                        "WHERE source_excel_file IS NOT NULL " +
                        "ORDER BY source_excel_file";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        cblDatasets.Items.Clear();
                        while (dr.Read())
                        {
                            string val = dr["source_excel_file"].ToString();
                            cblDatasets.Items.Add(new ListItem(val, val));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BindDatasetCheckBoxList Error: " + ex.Message);
            }
        }

        // Populates metadata column names by inspecting JSONB keys
        // DB table: indexed_documents → dynamic_metadata
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
                System.Diagnostics.Debug.WriteLine("BindColumnCheckBoxList Error: " + ex.Message);
            }
        }

        // ════════════════════════════════════════════════════════
        //  3. LOAD EXISTING POLICY (admin selects a user)
        // ════════════════════════════════════════════════════════

        protected void ddlSelectUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cpf = ddlSelectUser.SelectedValue;
            if (string.IsNullOrEmpty(cpf)) return;

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    // ── Dataset grants ──────────────────────────────────
                    // FIX: userid is INTEGER. Resolve it via subquery on cpf.
                    // Column is datasetid (not "dataset").
                    HashSet<string> grantedDatasets = new HashSet<string>();
                    string dsQuery =
                        "SELECT datasetid " +
                        "FROM user_dataset_access " +
                        "WHERE userid = (SELECT id FROM users WHERE cpf = @cpf)";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(dsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("cpf", cpf);
                        using (NpgsqlDataReader dr = cmd.ExecuteReader())
                            while (dr.Read())
                                grantedDatasets.Add(dr["datasetid"].ToString());
                    }

                    foreach (ListItem item in cblDatasets.Items)
                        item.Selected = grantedDatasets.Contains(item.Value);

                    // ── Metadata column policy ──────────────────────────
                    // DB table: user_metadata_policy (user_cpf TEXT) — no change needed
                    string colQuery =
                        "SELECT visible_columns FROM user_metadata_policy " +
                        "WHERE user_cpf = @cpf";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(colQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("cpf", cpf);
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            List<string> visibleCols =
                                JsonConvert.DeserializeObject<List<string>>(
                                    result.ToString());
                            HashSet<string> colSet = new HashSet<string>(visibleCols);
                            foreach (ListItem item in cblMetadataColumns.Items)
                                item.Selected = colSet.Contains(item.Value);
                        }
                        else
                        {
                            // No policy row yet → show all as selected (unrestricted)
                            foreach (ListItem item in cblMetadataColumns.Items)
                                item.Selected = true;
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
        //  4. SAVE ACCESS POLICY
        //
        //  FIX SUMMARY for user_dataset_access:
        //    • DELETE uses:  WHERE userid = (SELECT id FROM users WHERE cpf = @cpf)
        //    • INSERT uses:  (SELECT id FROM users WHERE cpf = @cpf), @datasetid
        //    • Column name is "datasetid", not "dataset"
        //    • No type cast required — subquery returns INTEGER naturally
        //
        //  user_metadata_policy is unchanged (user_cpf TEXT = CPF string directly).
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
                            // DELETE: resolve integer userid via subquery — no cast needed
                            string deleteQuery =
                                "DELETE FROM user_dataset_access " +
                                "WHERE userid = (SELECT id FROM users WHERE cpf = @cpf)";

                            using (NpgsqlCommand cmd =
                                new NpgsqlCommand(deleteQuery, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("cpf", cpf);
                                cmd.ExecuteNonQuery();
                            }

                            // INSERT: use subquery for userid, bind datasetid as text
                            foreach (ListItem item in cblDatasets.Items)
                            {
                                if (!item.Selected) continue;

                                string insertQuery =
                                    "INSERT INTO user_dataset_access (userid, datasetid) " +
                                    "VALUES (" +
                                    "  (SELECT id FROM users WHERE cpf = @cpf), " +
                                    "  @datasetid" +
                                    ")";

                                using (NpgsqlCommand cmd =
                                    new NpgsqlCommand(insertQuery, conn, tx))
                                {
                                    cmd.Parameters.AddWithValue("cpf", cpf);
                                    cmd.Parameters.AddWithValue("datasetid", item.Value);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // ── b) Metadata column policy ────────────────────
                            // Collect selected column names
                            List<string> selectedCols = new List<string>();
                            foreach (ListItem item in cblMetadataColumns.Items)
                                if (item.Selected) selectedCols.Add(item.Value);

                            // NULL stored when all columns selected = unrestricted access
                            bool allSelected =
                                selectedCols.Count == cblMetadataColumns.Items.Count;

                            string colJson = allSelected
                                ? null
                                : JsonConvert.SerializeObject(selectedCols);

                            // Two separate queries to avoid binding a null @cols
                            // with a ::jsonb cast (which Npgsql rejects for DBNull)
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
                                "✔ Access policy saved successfully for CPF: " + cpf, true);
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
        //  5. DOCUMENT INGESTION
        // ════════════════════════════════════════════════════════

        protected void btnIngestData_Click(object sender, EventArgs e)
        {
            if (!filePayload.HasFiles)
            {
                ShowFeedback(lblStatusFeedback, "⚠️ Please upload Excel files.", false);
                return;
            }

            int successCount = 0;
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    foreach (System.Web.HttpPostedFile uploadedFile
                        in filePayload.PostedFiles)
                    {
                        using (var workbook = new XLWorkbook(uploadedFile.InputStream))
                        {
                            var worksheet = workbook.Worksheet(1);
                            int lastRow = worksheet.LastRowUsed().RowNumber();
                            int lastColumn = worksheet.LastColumnUsed().ColumnNumber();

                            List<string> headers = new List<string>();
                            for (int col = 1; col <= lastColumn; col++)
                                headers.Add(worksheet.Cell(1, col).GetValue<string>()
                                    .Trim().ToLower().Replace(" ", "_"));

                            for (int row = 2; row <= lastRow; row++)
                            {
                                string actualFileName = "", actualFilePath = "";
                                var dynamicMetadata = new Dictionary<string, string>();

                                for (int col = 1; col <= lastColumn; col++)
                                {
                                    string val = worksheet.Cell(row, col)
                                        .GetValue<string>().Trim();
                                    if (string.IsNullOrWhiteSpace(val)) continue;

                                    if (headers[col - 1] == "file_name")
                                        actualFileName = val;
                                    else if (headers[col - 1] == "file_path" ||
                                             headers[col - 1] == "path")
                                        actualFilePath = val;
                                    else
                                        dynamicMetadata[headers[col - 1]] = val;
                                }

                                if (!string.IsNullOrWhiteSpace(actualFileName) &&
                                    !string.IsNullOrWhiteSpace(actualFilePath))
                                {
                                    using (NpgsqlCommand cmd = new NpgsqlCommand(
                                        "INSERT INTO indexed_documents " +
                                        "(file_name, file_path, source_excel_file, dynamic_metadata) " +
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
                            }
                        }
                        successCount++;
                    }
                }

                BindDatasetCheckBoxList();
                ShowFeedback(lblStatusFeedback,
                    successCount + " file(s) ingested successfully.", true);
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
