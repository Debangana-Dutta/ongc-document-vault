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

        protected void gvPendingUsers_RowCommand(
    object sender,
    GridViewCommandEventArgs e)
        {
            try
            {
                string username =
                    e.CommandArgument.ToString();

                if (e.CommandName == "ResetPassword")
                {
                    string tempPassword =
                        Guid.NewGuid()
                            .ToString("N")
                            .Substring(0, 10);

                    using (NpgsqlConnection conn =
                        new NpgsqlConnection(connString))
                    {
                        conn.Open();

                        string query =
                            @"UPDATE users
                      SET password_hash = @pwd
                      WHERE username = @username";

                        using (NpgsqlCommand cmd =
                            new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue(
                                "pwd",
                                tempPassword);

                            cmd.Parameters.AddWithValue(
                                "username",
                                username);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    ShowFeedback(
                        lblApprovalFeedback,
                        "Temporary password for '" +
                        username +
                        "' is: " +
                        tempPassword,
                        true);

                    return;
                }

                string newStatus = "";

                if (e.CommandName == "Approve")
                    newStatus = "APPROVED";
                else if (e.CommandName == "Reject")
                    newStatus = "REJECTED";
                else
                    return;

                using (NpgsqlConnection conn =
                    new NpgsqlConnection(connString))
                {
                    conn.Open();

                    string query =
                        @"UPDATE users
                  SET account_status = @status
                  WHERE username = @username";

                    using (NpgsqlCommand cmd =
                        new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "status",
                            newStatus);

                        cmd.Parameters.AddWithValue(
                            "username",
                            username);

                        cmd.ExecuteNonQuery();
                    }
                }

                BindPendingUsersGrid(false);

                ShowFeedback(
                    lblApprovalFeedback,
                    "User '" + username +
                    "' updated successfully.",
                    true);
            }
            catch (Exception ex)
            {
                ShowFeedback(
                    lblApprovalFeedback,
                    "Error: " + ex.Message,
                    false);
            }
        }

        protected void btnShowPending_Click(object sender, EventArgs e)
        {
            BindPendingUsersGrid(false);
        }

        protected void btnShowAllUsers_Click(object sender, EventArgs e)
        {
            BindPendingUsersGrid(true);
        }
        private string connString =
            System.Configuration.ConfigurationManager
                .ConnectionStrings["PostgresConn"]
                .ConnectionString;

        // ════════════════════════════════════════════════════════
        //  PAGE LOAD
        // ════════════════════════════════════════════════════════
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // NOT ADMIN
            if (Session["Role"] == null ||
                Session["Role"].ToString().ToUpper() != "ADMIN")
            {
                Response.Redirect("Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindUserGrid();
                BindColumnCheckBoxList();
                BindPendingUsersGrid(false);
            }
        }

        private void BindPendingUsersGrid(bool showAll)
        {
            try
            {
                using (NpgsqlConnection conn =
                    new NpgsqlConnection(connString))
                {
                    conn.Open();

                    string query;

                    if (showAll)
                    {
                        query = @"
                    SELECT
                        username,
                        role,
                        department,
                        account_status
                    FROM users
                    ORDER BY username";
                    }
                    else
                    {
                        query = @"
                    SELECT
                        username,
                        role,
                        department,
                        account_status
                    FROM users
                    WHERE account_status = 'PENDING'
                    ORDER BY username";
                    }

                    NpgsqlDataAdapter da =
                        new NpgsqlDataAdapter(query, conn);

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    gvPendingUsers.DataSource = dt;
                    gvPendingUsers.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowFeedback(
                    lblApprovalFeedback,
                    "Error loading approvals: " + ex.Message,
                    false
                );
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

       

        // Populates datasets from distinct source_excel_file values
        // DB table: indexed_documents
        

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
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    // ── Dataset grants ──────────────────────────────────
                    // FIX: userid is INTEGER. Resolve it via subquery on cpf.
                    // Column is datasetid (not "dataset").
            

                    // ── Metadata column policy ──────────────────────────
                    // DB table: user_metadata_policy (user_cpf TEXT) — no change needed
                    string colQuery =
                      "SELECT visible_columns " +
                      "FROM global_metadata_policy " +
                      "WHERE id = 1";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(colQuery, conn))
                    {
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
                ShowFeedback(lblPolicyFeedback, "Changes Applied.", true);
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
            
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    using (NpgsqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            
                            // Collect selected column names
                            // ── Global metadata policy ───────────────────────

                            List<string> selectedCols =
                                new List<string>();

                            foreach (ListItem item in cblMetadataColumns.Items)
                            {
                                if (item.Selected)
                                    selectedCols.Add(item.Value);
                            }

                            string colJson =
                                JsonConvert.SerializeObject(selectedCols);

                            string saveGlobalPolicy =
                             @"INSERT INTO global_metadata_policy
                              (id, visible_columns)
                              VALUES (1, @cols::jsonb)
                              ON CONFLICT (id)
                              DO UPDATE
                              SET visible_columns = EXCLUDED.visible_columns";

                            using (NpgsqlCommand cmd =
                                new NpgsqlCommand(
                                    saveGlobalPolicy,
                                    conn,
                                    tx))
                            {
                                cmd.Parameters.AddWithValue(
                                    "cols",
                                    colJson
                                );

                                cmd.ExecuteNonQuery();
                            }

                            tx.Commit();
                            ShowFeedback(lblPolicyFeedback,
                                "✔ Access policy saved successfully.", true);
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

                        using (NpgsqlCommand deleteCmd = new NpgsqlCommand(
                            "DELETE FROM indexed_documents WHERE source_excel_file = @sef",
                            conn))
                        {
                            deleteCmd.Parameters.AddWithValue(
                                "sef",
                                uploadedFile.FileName);

                            deleteCmd.ExecuteNonQuery();
                        }

                        using (var workbook = new XLWorkbook(uploadedFile.InputStream))
                        {
                            var worksheet = workbook.Worksheet(1);

                            if (worksheet.LastRowUsed() == null)
                                continue;

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
                BindColumnCheckBoxList();
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
