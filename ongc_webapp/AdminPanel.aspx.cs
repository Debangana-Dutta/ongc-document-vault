using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;

namespace ongc_webapp
{
    /// <summary>
    /// Admin control panel. Inherits BasePage which enforces authentication.
    /// RequiredRole = "admin" means any non-admin will be redirected to Indexing.aspx
    /// before any page content is rendered.
    ///
    /// Responsibilities:
    ///   1. User Management   — view users, add users (portal_users table)
    ///   2. Access Policies   — assign dataset + metadata column access per user
    ///   3. Document Ingestion — parse Excel files into indexed_documents
    /// </summary>
    public partial class AdminPanel : BasePage
    {
        // ── ROLE GATE ──────────────────────────────────────────────────────
        // BasePage.OnPreInit reads this before the page renders anything.
        // The value must match what is stored in portal_users.role exactly
        // (case-insensitive comparison is used in BasePage).
        protected override string RequiredRole => "admin";

        // Connection string key must match Web.config: name="PostgresConnection"
        private readonly string _connString =
            System.Configuration.ConfigurationManager
                .ConnectionStrings["PostgresConnection"]
                .ConnectionString;

        // ── PAGE LOAD ──────────────────────────────────────────────────────
        protected void Page_Load(object sender, EventArgs e)
        {
            // BasePage has already verified authentication and authorization
            // by the time we reach here. Safe to proceed.
            if (!IsPostBack)
            {
                BindUserGrid();
                BindUserDropDown();
                BindDatasetCheckBoxList();
                BindColumnCheckBoxList();
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  SECTION 1 — USER MANAGEMENT
        //  Controls: txtUserName, txtCPF, txtDept, gvUsers, lblAdminFeedback
        //  Table:    portal_users
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Adds a new employee to portal_users.
        /// Triggered by: btnAddUser OnClick="btnAddUser_Click" in AdminPanel.aspx
        /// </summary>
        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            // txtUserName, txtCPF, txtDept IDs must match AdminPanel.aspx exactly
            string newUsername = txtUserName.Text.Trim();
            string newCpf = txtCPF.Text.Trim();
            string newDept = txtDept.Text.Trim();

            if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(newCpf))
            {
                ShowFeedback(lblAdminFeedback, "⚠ Username and CPF are required.", success: false);
                return;
            }

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connString))
                {
                    conn.Open();

                    // ── DUPLICATE CPF CHECK ────────────────────────────────
                    // Table: portal_users | Column: cpf
                    const string checkQuery =
                        "SELECT COUNT(1) FROM public.portal_users WHERE cpf = @cpf";

                    using (NpgsqlCommand chk = new NpgsqlCommand(checkQuery, conn))
                    {
                        chk.Parameters.AddWithValue("cpf", newCpf);
                        long existing = (long)chk.ExecuteScalar();

                        if (existing > 0)
                        {
                            ShowFeedback(lblAdminFeedback,
                                $"⚠ A user with CPF {newCpf} already exists.", success: false);
                            return;
                        }
                    }

                    // ── INSERT NEW USER ────────────────────────────────────
                    // Columns: username, cpf, employee_name, role, account_status
                    // department is stored in employee_name for now — add a dedicated
                    // column to portal_users if you need it separately.
                    const string insertQuery = @"
                        INSERT INTO public.portal_users
                            (username, cpf, employee_name, role, account_status)
                        VALUES
                            (@username, @cpf, @employeeName, 'employee', 'Active')
                        ON CONFLICT (cpf) DO NOTHING";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("username", newUsername);
                        cmd.Parameters.AddWithValue("cpf", newCpf);
                        // Store department inside employee_name or add a dept column
                        cmd.Parameters.AddWithValue("employeeName", newDept);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Clear form fields after success
                txtUserName.Text = txtCPF.Text = txtDept.Text = "";
                BindUserGrid();
                BindUserDropDown();
                ShowFeedback(lblAdminFeedback, "✔ User added successfully.", success: true);
            }
            catch (Exception ex)
            {
                ShowFeedback(lblAdminFeedback, "Error adding user: " + ex.Message, success: false);
            }
        }

        /// <summary>
        /// Populates gvUsers (GridView) with all portal_users rows.
        /// </summary>
        private void BindUserGrid()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connString))
                {
                    // Alias columns to friendly display names for the GridView
                    const string query = @"
                        SELECT
                            username      AS ""Name"",
                            cpf           AS ""CPF"",
                            employee_name AS ""Department"",
                            role          AS ""Role"",
                            account_status AS ""Status""
                        FROM public.portal_users
                        ORDER BY username";

                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // gvUsers ID must match AdminPanel.aspx GridView ID exactly
                    gvUsers.DataSource = dt;
                    gvUsers.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BindUserGrid error: {ex.Message}");
            }
        }

        /// <summary>
        /// Populates ddlSelectUser DropDownList for the access policy section.
        /// </summary>
        private void BindUserDropDown()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connString))
                {
                    conn.Open();

                    const string query =
                        "SELECT cpf, username FROM public.portal_users ORDER BY username";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        // ddlSelectUser ID must match AdminPanel.aspx DropDownList ID
                        ddlSelectUser.Items.Clear();
                        ddlSelectUser.Items.Add(new ListItem("-- Select User --", ""));

                        while (dr.Read())
                        {
                            ddlSelectUser.Items.Add(new ListItem(
                                $"{dr["username"]} ({dr["cpf"]})",
                                dr["cpf"].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BindUserDropDown error: {ex.Message}");
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  SECTION 2 — ACCESS POLICY BINDING
        //  Controls: ddlSelectUser, cblDatasets, cblMetadataColumns
        //  Tables:   datasets, indexed_documents, user_dataset_access,
        //            user_metadata_policy
        // ════════════════════════════════════════════════════════════════════

        private void BindDatasetCheckBoxList()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connString))
                {
                    conn.Open();

                    // Table: datasets | Columns: datasetid, datasetname
                    const string query =
                        "SELECT datasetid, datasetname FROM public.datasets ORDER BY datasetname";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        // cblDatasets ID must match AdminPanel.aspx CheckBoxList ID
                        cblDatasets.Items.Clear();

                        while (dr.Read())
                        {
                            cblDatasets.Items.Add(new ListItem(
                                dr["datasetname"].ToString(),
                                dr["datasetid"].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BindDatasetCheckBoxList error: {ex.Message}");
            }
        }

        private void BindColumnCheckBoxList()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connString))
                {
                    conn.Open();

                    // Extract all distinct dynamic metadata keys from indexed_documents.
                    // jsonb_object_keys returns one row per key.
                    const string query = @"
                        SELECT DISTINCT jsonb_object_keys(dynamic_metadata) AS col_key
                        FROM   public.indexed_documents
                        WHERE  dynamic_metadata IS NOT NULL
                        ORDER  BY col_key";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        // cblMetadataColumns ID must match AdminPanel.aspx CheckBoxList ID
                        cblMetadataColumns.Items.Clear();

                        while (dr.Read())
                        {
                            string key = dr.GetString(0);
                            cblMetadataColumns.Items.Add(new ListItem(key, key));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BindColumnCheckBoxList error: {ex.Message}");
            }
        }

        /// <summary>
        /// Fires when the user dropdown selection changes.
        /// Loads and pre-checks whatever policy already exists for that user.
        /// Triggered by: ddlSelectUser OnSelectedIndexChanged="ddlSelectUser_SelectedIndexChanged"
        /// </summary>
        protected void ddlSelectUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cpf = ddlSelectUser.SelectedValue;
            if (string.IsNullOrEmpty(cpf)) return;

            // Refresh lists before applying selections so they are up-to-date
            BindDatasetCheckBoxList();
            BindColumnCheckBoxList();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connString))
                {
                    conn.Open();

                    // ── LOAD DATASET GRANTS ────────────────────────────────
                    // Table: user_dataset_access | Columns: userid, datasetid
                    // We look up the integer userid via the cpf in portal_users
                    HashSet<int> grantedDatasetIds = new HashSet<int>();
                    const string dsQuery = @"
                        SELECT datasetid
                        FROM   public.user_dataset_access
                        WHERE  userid = (
                            SELECT id FROM public.portal_users WHERE cpf = @cpf
                        )";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(dsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("cpf", cpf);
                        using (NpgsqlDataReader dr = cmd.ExecuteReader())
                            while (dr.Read())
                                grantedDatasetIds.Add(dr.GetInt32(0));
                    }

                    // Pre-check the checkboxes that match granted dataset IDs
                    foreach (ListItem item in cblDatasets.Items)
                        if (int.TryParse(item.Value, out int dsId))
                            item.Selected = grantedDatasetIds.Contains(dsId);

                    // ── LOAD COLUMN POLICY ─────────────────────────────────
                    // Table: user_metadata_policy | Columns: user_cpf, visible_columns (jsonb)
                    const string colQuery = @"
                        SELECT visible_columns
                        FROM   public.user_metadata_policy
                        WHERE  user_cpf = @cpf";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(colQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("cpf", cpf);
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            // Deserialize the stored JSON array of column names
                            List<string> visibleCols =
                                JsonConvert.DeserializeObject<List<string>>(result.ToString());
                            HashSet<string> colSet = new HashSet<string>(visibleCols);

                            foreach (ListItem item in cblMetadataColumns.Items)
                                item.Selected = colSet.Contains(item.Value);
                        }
                        else
                        {
                            // NULL policy = no restrictions stored = uncheck all
                            foreach (ListItem item in cblMetadataColumns.Items)
                                item.Selected = false;
                        }
                    }
                }

                ShowFeedback(lblPolicyFeedback, "Policy loaded for selected user.", success: true);
            }
            catch (Exception ex)
            {
                ShowFeedback(lblPolicyFeedback,
                    "Error loading policy: " + ex.Message, success: false);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  SECTION 3 — SAVE ACCESS POLICY
        //  Triggered by: btnSaveAccessPolicy OnClick="btnSaveAccessPolicy_Click"
        //  Tables written: user_dataset_access, user_metadata_policy
        // ════════════════════════════════════════════════════════════════════

        protected void btnSaveAccessPolicy_Click(object sender, EventArgs e)
        {
            string cpf = ddlSelectUser.SelectedValue;

            if (string.IsNullOrEmpty(cpf))
            {
                ShowFeedback(lblPolicyFeedback, "⚠ Please select a user first.", success: false);
                return;
            }

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connString))
                {
                    conn.Open();

                    // Wrap both writes in a transaction so they succeed or fail together
                    using (NpgsqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            // ── STEP A: CLEAR OLD DATASET GRANTS ──────────
                            // Table: user_dataset_access
                            const string deleteQuery = @"
                                DELETE FROM public.user_dataset_access
                                WHERE userid = (
                                    SELECT id FROM public.portal_users WHERE cpf = @cpf
                                )";

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteQuery, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("cpf", cpf);
                                cmd.ExecuteNonQuery();
                            }

                            // ── STEP B: INSERT NEWLY SELECTED DATASETS ────
                            foreach (ListItem item in cblDatasets.Items)
                            {
                                if (!item.Selected || !int.TryParse(item.Value, out int datasetId))
                                    continue;

                                const string insertQuery = @"
                                    INSERT INTO public.user_dataset_access (userid, datasetid)
                                    VALUES (
                                        (SELECT id FROM public.portal_users WHERE cpf = @cpf),
                                        @datasetid
                                    )";

                                using (NpgsqlCommand cmd =
                                    new NpgsqlCommand(insertQuery, conn, tx))
                                {
                                    cmd.Parameters.AddWithValue("cpf", cpf);
                                    cmd.Parameters.Add(new NpgsqlParameter("datasetid",
                                        NpgsqlDbType.Integer)
                                    { Value = datasetId });
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // ── STEP C: BUILD COLUMN JSON ─────────────────
                            List<string> selectedCols = new List<string>();
                            foreach (ListItem item in cblMetadataColumns.Items)
                                if (item.Selected) selectedCols.Add(item.Value);

                            // NULL means "show all columns" — no restriction
                            bool allSelected =
                                selectedCols.Count == cblMetadataColumns.Items.Count;
                            string colJson = allSelected
                                ? null
                                : JsonConvert.SerializeObject(selectedCols);

                            // ── STEP D: UPSERT COLUMN POLICY ──────────────
                            // Table: user_metadata_policy
                            // Two SQL strings because Npgsql can't bind a null parameter
                            // to a typed @cols placeholder gracefully in all driver versions
                            string upsertQuery = (colJson == null)
                                ? @"INSERT INTO public.user_metadata_policy
                                        (user_cpf, visible_columns, updated_at)
                                    VALUES (@cpf, NULL, NOW())
                                    ON CONFLICT (user_cpf)
                                    DO UPDATE SET visible_columns = NULL, updated_at = NOW()"
                                : @"INSERT INTO public.user_metadata_policy
                                        (user_cpf, visible_columns, updated_at)
                                    VALUES (@cpf, @cols::jsonb, NOW())
                                    ON CONFLICT (user_cpf)
                                    DO UPDATE SET visible_columns = @cols::jsonb, updated_at = NOW()";

                            using (NpgsqlCommand cmd =
                                new NpgsqlCommand(upsertQuery, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("cpf", cpf);
                                if (colJson != null)
                                    cmd.Parameters.AddWithValue("cols", colJson);
                                cmd.ExecuteNonQuery();
                            }

                            tx.Commit();
                            ShowFeedback(lblPolicyFeedback,
                                $"✔ Access policy saved for CPF: {cpf}", success: true);
                        }
                        catch
                        {
                            tx.Rollback();
                            throw; // Re-throw to outer catch for feedback
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowFeedback(lblPolicyFeedback,
                    "Error saving policy: " + ex.Message, success: false);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  SECTION 4 — DOCUMENT INGESTION
        //  Triggered by: btnIngestData OnClick="btnIngestData_Click"
        //  Controls:     filePayload (FileUpload), lblStatusFeedback
        //  Tables written: indexed_documents, datasets
        // ════════════════════════════════════════════════════════════════════

        protected void btnIngestData_Click(object sender, EventArgs e)
        {
            // filePayload ID must match AdminPanel.aspx FileUpload control ID
            if (!filePayload.HasFiles)
            {
                ShowFeedback(lblStatusFeedback,
                    "⚠ Please upload at least one Excel (.xlsx) file.", success: false);
                return;
            }

            int successCount = 0;

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connString))
                {
                    conn.Open();

                    foreach (System.Web.HttpPostedFile uploadedFile in filePayload.PostedFiles)
                    {
                        using (var workbook = new XLWorkbook(uploadedFile.InputStream))
                        {
                            var worksheet = workbook.Worksheet(1);
                            int lastRow = worksheet.LastRowUsed().RowNumber();
                            int lastColumn = worksheet.LastColumnUsed().ColumnNumber();

                            // ── READ HEADERS from row 1 ────────────────────
                            // Normalize: lowercase, spaces → underscores
                            var headers = new List<string>();
                            for (int col = 1; col <= lastColumn; col++)
                            {
                                string raw = worksheet.Cell(1, col).GetValue<string>();
                                headers.Add(raw.Trim().ToLower().Replace(" ", "_"));
                            }

                            // ── PROCESS DATA ROWS ──────────────────────────
                            for (int row = 2; row <= lastRow; row++)
                            {
                                string fileName = "";
                                string filePath = "";
                                var dynamicMeta = new Dictionary<string, string>();

                                for (int col = 1; col <= lastColumn; col++)
                                {
                                    string val = worksheet.Cell(row, col)
                                                             .GetValue<string>().Trim();
                                    string header = headers[col - 1];

                                    if (string.IsNullOrWhiteSpace(val)) continue;

                                    switch (header)
                                    {
                                        case "file_name":
                                            fileName = val;
                                            break;
                                        case "file_path":
                                        case "path":
                                            filePath = val;
                                            break;
                                        default:
                                            // Everything else goes into dynamic_metadata (JSONB)
                                            dynamicMeta[header] = val;
                                            break;
                                    }
                                }

                                // Skip rows missing required fields
                                if (string.IsNullOrWhiteSpace(fileName) ||
                                    string.IsNullOrWhiteSpace(filePath))
                                    continue;

                                // ── INSERT ROW into indexed_documents ──────
                                // Columns: file_name, file_path, source_excel_file,
                                //          dynamic_metadata (jsonb)
                                const string insertDocQuery = @"
                                    INSERT INTO public.indexed_documents
                                        (file_name, file_path, source_excel_file, dynamic_metadata)
                                    VALUES
                                        (@fn, @fp, @sef, @meta::jsonb)";

                                using (NpgsqlCommand cmd =
                                    new NpgsqlCommand(insertDocQuery, conn))
                                {
                                    cmd.Parameters.AddWithValue("fn", fileName);
                                    cmd.Parameters.AddWithValue("fp", filePath);
                                    cmd.Parameters.AddWithValue("sef", uploadedFile.FileName);
                                    cmd.Parameters.AddWithValue("meta",
                                        JsonConvert.SerializeObject(dynamicMeta));
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        // ── REGISTER DATASET NAME ──────────────────────────
                        // Table: datasets | Column: datasetname
                        const string registerDatasetQuery = @"
                            INSERT INTO public.datasets (datasetname)
                            VALUES (@name)
                            ON CONFLICT (datasetname) DO NOTHING";

                        using (NpgsqlCommand regCmd =
                            new NpgsqlCommand(registerDatasetQuery, conn))
                        {
                            regCmd.Parameters.AddWithValue("name", uploadedFile.FileName);
                            regCmd.ExecuteNonQuery();
                        }

                        successCount++;
                    }
                }

                // Refresh dataset list so newly ingested files appear in the policy section
                BindDatasetCheckBoxList();
                ShowFeedback(lblStatusFeedback,
                    $"✔ {successCount} file(s) ingested and registered successfully.",
                    success: true);
            }
            catch (Exception ex)
            {
                ShowFeedback(lblStatusFeedback,
                    "Ingestion failed: " + ex.Message, success: false);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  HELPERS
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Sets the text and colour of a feedback Label.
        /// success=true  → MediumSeaGreen
        /// success=false → Crimson
        /// </summary>
        private void ShowFeedback(Label label, string message, bool success)
        {
            label.Text = message;
            label.ForeColor = success
                ? System.Drawing.Color.MediumSeaGreen
                : System.Drawing.Color.Crimson;
        }
    }
}