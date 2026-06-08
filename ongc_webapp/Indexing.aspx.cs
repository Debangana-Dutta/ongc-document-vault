// ============================================================
//  Indexing.aspx.cs
//  ONGC Document Portal – Search Page Code-Behind
//
//  Security model implemented:
//    1. RestoreDynamicFilters()  – OnInit – reads user's metadata
//       policy from user_metadata_policy and only renders allowed
//       columns in the sidebar PlaceHolder (phDynamicFilters).
//
//    2. BindDynamicVaultData()   – builds the SQL query with a
//       JOIN / IN clause against user_dataset_access so results
//       are restricted to datasets the user is permitted to see.
//
//  DB tables consumed (read-only):
//    • indexed_documents     – document rows + dynamic_metadata JSONB
//    • user_dataset_access   – per-user allowed source_excel_file values
//    • user_metadata_policy  – per-user allowed sidebar column names (JSONB array)
// ============================================================

using System;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ongc_webapp
{
    public partial class Indexing : System.Web.UI.Page
    {
        // ── Connection string ──────────────────────────────────
        private string connString =
            ConfigurationManager
            .ConnectionStrings["PostgresConn"]
            .ConnectionString;

        // Column to scroll-highlight in the results table
        private string focusedColumn = "";

        // ════════════════════════════════════════════════════════
        //  ONINIT – runs before ViewState is restored.
        //  We must rebuild the dynamic sidebar controls here so
        //  that ASP.NET can match posted form values to them.
        // ════════════════════════════════════════════════════════
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Session["UserID"] == null)
                return;

            BindDatasetFilter();

            RestoreDynamicFilters();
        }

        // ════════════════════════════════════════════════════════
        //  PAGE LOAD
        // ════════════════════════════════════════════════════════

        private string GetSelectedDataset()
        {
            if (string.IsNullOrEmpty(rblDatasets.SelectedValue))
                return "";

            return rblDatasets.SelectedValue;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // Wire up row-data-bound event
            gvDocuments.RowDataBound += gvDocuments_RowDataBound;

            if (!IsPostBack)
            {
                BindDatasetFilter();
                RestoreDynamicFilters();
                BindDynamicVaultData();
            }
        }

        // ════════════════════════════════════════════════════════
        //  EVENT HANDLERS
        // ════════════════════════════════════════════════════════

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindDynamicVaultData();
        }

        protected void btnApplyFilters_Click(object sender, EventArgs e)
        {
            BindDynamicVaultData();
        }

        protected void btnApplyColumns_Click(object sender, EventArgs e)
        {
            BindDynamicVaultData();
        }

        

        // ════════════════════════════════════════════════════════
        //  SECURITY HELPERS
        // ════════════════════════════════════════════════════════

        /// <summary>
        /// Returns the CPF of the currently logged-in user.
        /// Session["CPF"] is set at login; falls back to Session["UserID"]
        /// if your login code stores the CPF there instead.
        /// Adjust the key to match your Login.aspx.cs code.
        /// </summary>

        /// <summary>
        /// Loads the list of datasets (source_excel_file values) the
        /// current user is allowed to query.
        /// Returns NULL if the user has no restrictions (admin / no policy row).
        ///
        /// DB table: user_dataset_access
        /// </summary>


        /// <summary>
        /// Returns the set of metadata column names the current user is
        /// allowed to see in the sidebar. Returns NULL meaning "all columns".
        ///
        /// DB table: user_metadata_policy
        /// </summary>
        private HashSet<string> GetAllowedMetadataColumns()
        {
            try
            {
                int userId = GetCurrentUserId();

                HashSet<string> cols =
                    new HashSet<string>(
                        StringComparer.OrdinalIgnoreCase);

                using (NpgsqlConnection conn =
                    new NpgsqlConnection(connString))
                {
                    conn.Open();

                    string query =
                      @"SELECT metadata_name
                      FROM user_metadata_access
                      WHERE userid = @userid
                      AND dataset_name = @dataset";

                    string dataset = GetSelectedDataset();

                    if (string.IsNullOrWhiteSpace(dataset))
                    {
                        return new HashSet<string>();
                    }

                    using (NpgsqlCommand cmd =
                        new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "userid",
                            userId);

                        cmd.Parameters.AddWithValue(
                            "dataset",
                            dataset);

                        using (NpgsqlDataReader reader =
                            cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cols.Add(
                                    reader["metadata_name"]
                                        .ToString());
                            }
                        }
                    }
                }

                if (cols.Count == 0)
                    return new HashSet<string>();

                return cols;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "GetAllowedMetadataColumns Error: " +
                    ex.Message);

                return new HashSet<string>();
            }
        }

        // ════════════════════════════════════════════════════════
        //  RESTORE DYNAMIC FILTERS  (called from OnInit)
        //
        //  Reads the user's metadata column policy and builds
        //  the sidebar PlaceHolder with only the permitted columns.
        //  On PostBack, also restores checkbox / textbox values
        //  from Request.Form so filters survive the round-trip.
        // ════════════════════════════════════════════════════════
        private void RestoreDynamicFilters()
        {
            
            // ── Determine which columns this user may see ──────
            HashSet<string> allowedCols =
            GetAllowedMetadataColumns();
            // NULL = all columns allowed

            // Show the "Restricted View" badge if a policy exists
            if (allowedCols != null && lblAccessBadge != null)
                lblAccessBadge.Visible = true;

            // ── Collect all known metadata keys from the DB ────
            // We query only up to 500 rows to discover column names.
            HashSet<string> allKeys = new HashSet<string>();

            try
            {
                using (NpgsqlConnection conn =
                    new NpgsqlConnection(connString))
                {
                    conn.Open();

                    // Use Postgres's jsonb_object_keys for efficiency
                    string selectedDataset = GetSelectedDataset();

                    string query =
                        "SELECT DISTINCT jsonb_object_keys(dynamic_metadata) " +
                        "FROM indexed_documents " +
                        "WHERE dynamic_metadata IS NOT NULL " +
                        "AND dataset_name = @dataset " +
                        "ORDER BY 1";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "dataset",
                            selectedDataset);

                        using (NpgsqlDataReader reader =
                            cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                allKeys.Add(reader.GetString(0));
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "RestoreDynamicFilters Error: " + ex.Message);
            }

            // ── Filter keys against the user's policy ──────────
            HashSet<string> visibleKeys;

            if (allowedCols == null)
            {
                visibleKeys =
                    new HashSet<string>(
                        allKeys,
                        StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                visibleKeys =
                    new HashSet<string>(
                        allKeys.Where(k =>
                            allowedCols.Contains(k)),
                        StringComparer.OrdinalIgnoreCase);
            }

            // ── Build the sidebar controls ─────────────────────
            GenerateDynamicFilters(visibleKeys);
        }

        // ════════════════════════════════════════════════════════
        //  GENERATE DYNAMIC FILTERS
        //
        //  Creates one "filter-row" per column containing:
        //    • CheckBox  (ID = cb_<column>)
        //    • Literal   (display label)
        //    • Panel     (ID = box_<column>) + TextBox (ID = txt_<column>)
        //
        //  On PostBack, checkbox state and textbox text are
        //  read back from Request.Form using the control's UniqueID.
        // ════════════════════════════════════════════════════════
        private void GenerateDynamicFilters(HashSet<string> availableColumns)
        {
            phDynamicFilters.Controls.Clear();
            cblColumns.Items.Clear();

            if (availableColumns == null || availableColumns.Count == 0)
            {
                Literal empty = new Literal();
                empty.Text =
                    "<div class='sidebar-empty'>No metadata filters available.</div>";
                phDynamicFilters.Controls.Add(empty);
                return;
            }

            foreach (string column in availableColumns.OrderBy(c => c))
            {
                Panel panel = new Panel();
                panel.CssClass = "filter-row";

                // ── Checkbox ──
                CheckBox cb = new CheckBox();
                cb.ID = "cb_" + column;
                bool isChecked = Request.Form[cb.UniqueID] == "on";
                cb.Checked = isChecked;

                // ── Label literal ──
                Literal lbl = new Literal();
                lbl.Text =
                    "<span class='filter-column-name'>" +
                    System.Web.HttpUtility.HtmlEncode(column) +
                    "</span>";

                // ── Textbox panel ──
                Panel textboxPanel = new Panel();
                string panelId = "box_" + column.Replace(" ", "_");
                textboxPanel.Attributes["id"] = panelId;
                textboxPanel.CssClass = "filter-input-box";
                textboxPanel.Style["display"] = "block";

                TextBox txt = new TextBox();
                txt.ID = "txt_" + column;
                txt.CssClass = "form-control";
                txt.Attributes["placeholder"] = "Filter value…";

                // Restore textbox value from posted form data
                string postedValue = Request.Form[txt.UniqueID];
                if (!string.IsNullOrWhiteSpace(postedValue))
                    txt.Text = postedValue;

                textboxPanel.Controls.Add(txt);

                panel.Controls.Add(cb);
                panel.Controls.Add(lbl);
                panel.Controls.Add(textboxPanel);

                phDynamicFilters.Controls.Add(panel);

                // Also populate the Column Visibility checkboxlist
                if (cblColumns.Items.FindByValue(column) == null)
                    cblColumns.Items.Add(new ListItem(column, column));
            }
        }

        // ════════════════════════════════════════════════════════
        //  GET FILTERED METADATA COLUMNS
        //
        //  Returns the set of metadata keys that are actually
        //  present in the data rows AND (if the user selected
        //  specific columns via cblColumns) are in that selection.
        // ════════════════════════════════════════════════════════
        private HashSet<string> GetFilteredMetadataColumns(
            List<Dictionary<string, string>> allRows)
        {
            HashSet<string> columns =
            new HashSet<string>();

            HashSet<string> allowedColumns =
                GetAllowedMetadataColumns();

            HashSet<string> selectedColumns =
                new HashSet<string>();

            bool anySelected = false;

            foreach (ListItem item in cblColumns.Items)
            {
                if (item.Selected)
                {
                    anySelected = true;
                    selectedColumns.Add(item.Value);
                }
            }

            foreach (var row in allRows)
            {
                foreach (var kv in row)
                {
                    string key = kv.Key;
                    string value = kv.Value ?? "";

                    // Skip the fixed display columns
                    if (key.Equals("file_name", StringComparison.OrdinalIgnoreCase) ||
                        key.Equals("view", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (string.IsNullOrWhiteSpace(value) || value == "NULL")
                        continue;

                    if (allowedColumns != null &&
                        !allowedColumns.Contains(key))
                        continue;

                    if (anySelected &&
                        !selectedColumns.Contains(key))
                        continue;

                    columns.Add(key);
                }
            }

            return columns;
        }

        // ════════════════════════════════════════════════════════
        //  BIND DYNAMIC VAULT DATA  (core search + security)
        //
        //  Security enforcement happens in TWO places:
        //    A) SQL WHERE clause: restricts source_excel_file to
        //       only datasets in the user's user_dataset_access rows.
        //    B) Sidebar generation (RestoreDynamicFilters): only
        //       columns in user_metadata_policy are rendered.
        // ════════════════════════════════════════════════════════
        private void BindDynamicVaultData()
        {
            int totalResults = 0;


            // If the user has a policy but zero datasets, return early


            // ── Keyword search setup ───────────────────────────
            string rawSearch = txtSearch.Text.Trim();
            List<string> keywords = rawSearch
                .Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .Take(3)
                .ToList();

            string searchMode = rblSearchMode.SelectedValue; // "OR" or "AND"

            // ── Build SQL ──────────────────────────────────────
            string query = @"
                SELECT
                    id,
                    source_excel_file,
                    file_name,
                    file_path,
                    dynamic_metadata
                FROM indexed_documents
                WHERE 1=1";

            List<string> whereConditions = new List<string>();
            List<string> allowedDatasets =
            GetAllowedDatasets();

            string selectedDataset = GetSelectedDataset();

            if (!string.IsNullOrEmpty(selectedDataset))
            {
                allowedDatasets =
                    allowedDatasets
                    .Where(d =>
                        d.Equals(
                            selectedDataset,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (Session["Role"] == null ||
            Session["Role"].ToString().ToUpper() != "ADMIN")
            {
                if (allowedDatasets.Count == 0)
                {
                    gvDocuments.DataSource = null;
                    gvDocuments.DataBind();

                    lblStatus.Text =
                        "No datasets have been assigned.";

                    return;
                }
            }

            if (allowedDatasets.Count > 0)
            {
                List<string> datasetConditions =
                    new List<string>();

                for (int i = 0;
                     i < allowedDatasets.Count;
                     i++)
                {
                    datasetConditions.Add(
                        "dataset_name = @dataset" + i);
                }

                whereConditions.Add(
                    "(" +
                    string.Join(
                        " OR ",
                        datasetConditions) +
                    ")");
            }

            // ── SECURITY: dataset restriction ─────────────────
            // Only add the clause when a policy exists;
            // if allowedDatasets is null the user sees everything.


            // ── Keyword search conditions ──────────────────────
            if (keywords.Count > 0)
            {
                List<string> kwConditions = new List<string>();
                for (int i = 0; i < keywords.Count; i++)
                {
                    kwConditions.Add(
                        "(file_name ILIKE @kw" + i +
                        " OR dynamic_metadata::text ILIKE @kw" + i + ")");
                }

                whereConditions.Add(
                    "(" +
                    string.Join(" " + searchMode + " ", kwConditions) +
                    ")");
            }

            if (whereConditions.Count > 0)
                query += " AND " + string.Join(" AND ", whereConditions);

                string countQuery =
                @"SELECT COUNT(*)
                  FROM indexed_documents
                  WHERE 1=1";

                    if (whereConditions.Count > 0)
                    {
                        countQuery +=
                            " AND " +
                            string.Join(" AND ", whereConditions);
                    }

            query += " ORDER BY uploaded_at DESC LIMIT 500";

            // ── Execute query ──────────────────────────────────
            List<Dictionary<string, string>> allRows =
                new List<Dictionary<string, string>>();

            

            try
            {
                using (NpgsqlConnection conn =
                    new NpgsqlConnection(connString))
                {
                    conn.Open();

                    using (NpgsqlCommand cmd =
                        new NpgsqlCommand(query, conn))
                    {
                        // Bind dataset security parameters

                        for (int i = 0;
                           i < allowedDatasets.Count;
                           i++)
                        {
                            cmd.Parameters.AddWithValue(
                                "dataset" + i,
                                allowedDatasets[i]);
                        }


                        // Bind keyword parameters
                        for (int i = 0; i < keywords.Count; i++)
                        {
                            cmd.Parameters.AddWithValue(
                                "kw" + i,
                                "%" + keywords[i] + "%");
                        }

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var rowMap = new Dictionary<string, string>();

                                rowMap["file_name"] =
                                    reader["file_name"].ToString();

                                rowMap["view"] =
                                    "<a target='_blank' " +
                                    "class='btn btn-sm btn-primary' " +
                                    "href='ViewFile.aspx?id=" +
                                    reader["id"].ToString() +
                                    "'>View</a>";

                                string metadataJson =
                                    reader["dynamic_metadata"].ToString();

                                if (!string.IsNullOrWhiteSpace(metadataJson))
                                {
                                    JObject metadata =
                                        JsonConvert.DeserializeObject<JObject>(
                                            metadataJson);

                                    foreach (var item in metadata)
                                        rowMap[item.Key] = item.Value != null
                                            ? item.Value.ToString()
                                            : "NULL";
                                }

                                allRows.Add(rowMap);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Query error: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Crimson;
                return;
            }

            // ── Auto-detect the best matching column ──────────
            Dictionary<string, int> columnScores =
                new Dictionary<string, int>();

            foreach (var rowMap in allRows)
            {
                foreach (var kv in rowMap)
                {
                    if (kv.Key.Equals("file_name",
                        StringComparison.OrdinalIgnoreCase) ||
                        kv.Key.Equals("view",
                        StringComparison.OrdinalIgnoreCase)) continue;

                    string value = kv.Value?.ToLower() ?? "";
                    foreach (string keyword in keywords)
                    {
                        if (value.Contains(keyword.ToLower()))
                        {
                            if (!columnScores.ContainsKey(kv.Key))
                                columnScores[kv.Key] = 0;
                            columnScores[kv.Key]++;
                        }
                    }
                }
            }

            if (columnScores.Count > 0)
                focusedColumn = columnScores
                    .OrderByDescending(x => x.Value)
                    .First().Key;

            // ── Determine visible columns ──────────────────────
            HashSet<string> filteredColumns =
                GetFilteredMetadataColumns(allRows);

            // ── Show/hide sidebar filter rows ──────────────────
            foreach (Control ctrl in phDynamicFilters.Controls)
            {
                Panel panel = ctrl as Panel;
                if (panel == null) continue;

                CheckBox cb = null;
                foreach (Control inner in panel.Controls)
                    if (inner is CheckBox) { cb = (CheckBox)inner; break; }

                if (cb == null) continue;

                string colName = cb.ID.Replace("cb_", "");
                panel.Visible = filteredColumns.Contains(colName);
            }

            // ── Apply metadata text filters ────────────────────
            List<Dictionary<string, string>> filteredRows =
                new List<Dictionary<string, string>>();

            foreach (var rowMap in allRows)
            {
                bool matches = true;

                foreach (Control ctrl in phDynamicFilters.Controls)
                {
                    Panel panel = ctrl as Panel;
                    if (panel == null) continue;

                    CheckBox cb = null;
                    TextBox txt = null;

                    foreach (Control inner in panel.Controls)
                    {
                        if (inner is CheckBox) cb = (CheckBox)inner;

                        if (inner is Panel)
                        {
                            foreach (Control sub in ((Panel)inner).Controls)
                                if (sub is TextBox) txt = (TextBox)sub;
                        }
                    }

                    if (cb == null || txt == null)
                        continue;

                    if (string.IsNullOrWhiteSpace(txt.Text))
                        continue;

                    string colName = cb.ID.Replace("cb_", "");
                    string actualValue = rowMap.ContainsKey(colName)
                        ? (rowMap[colName] ?? "").Trim()
                        : "";

                    string actualLower = actualValue.ToLower();
                    string filterValue = (txt.Text ?? "").Trim().ToLower();

                    if (filterValue == "!null")
                    {
                        if (string.IsNullOrWhiteSpace(actualLower) ||
                            actualLower == "null")
                        { matches = false; break; }
                    }
                    else if (filterValue == "null")
                    {
                        if (!string.IsNullOrWhiteSpace(actualLower) &&
                            actualLower != "null")
                        { matches = false; break; }
                    }
                    else if (!string.IsNullOrWhiteSpace(filterValue))
                    {
                        if (!actualLower.Contains(filterValue))
                        { matches = false; break; }
                    }
                }

                if (matches) filteredRows.Add(rowMap);
            }

            allRows = filteredRows;

            totalResults = filteredRows.Count;

            // ── Build DataTable for GridView ───────────────────
            DataTable finalTable = new DataTable();
            finalTable.Columns.Add("file_name");
            finalTable.Columns.Add("view");

            bool anyColsSelected = cblColumns.Items
                .Cast<ListItem>().Any(i => i.Selected);

            if (!anyColsSelected)
            {
                foreach (string col in filteredColumns)
                    if (!finalTable.Columns.Contains(col))
                        finalTable.Columns.Add(col);
            }
            else
            {
                foreach (ListItem item in cblColumns.Items)
                {
                    if (item.Selected &&
                        filteredColumns.Contains(item.Value) &&
                        !finalTable.Columns.Contains(item.Value))
                        finalTable.Columns.Add(item.Value);
                }
            }

            // ── Populate rows with search highlight ───────────
            foreach (var rowMap in allRows)
            {
                DataRow row = finalTable.NewRow();

                foreach (DataColumn col in finalTable.Columns)
                {
                    string cellValue = rowMap.ContainsKey(col.ColumnName)
                        ? rowMap[col.ColumnName] ?? "NULL"
                        : "NULL";

                    if (!col.ColumnName.Equals(
                            "view",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (string keyword in keywords)
                        {
                            if (!string.IsNullOrWhiteSpace(keyword))
                            {
                                cellValue =
                                    System.Text.RegularExpressions.Regex.Replace(
                                        cellValue,
                                        System.Text.RegularExpressions.Regex
                                            .Escape(keyword),
                                        "<span class='search-highlight'>" +
                                        keyword +
                                        "</span>",
                                        System.Text.RegularExpressions.RegexOptions
                                            .IgnoreCase);
                            }
                        }
                    }

                    row[col.ColumnName] = cellValue;
                }

                finalTable.Rows.Add(row);
            }

            // ── Bind GridView ──────────────────────────────────
            const int PageSize = 20;

            int totalRows = finalTable.Rows.Count;

            int totalPages =
                (int)Math.Ceiling(
                    (double)totalRows / PageSize);

            if (totalPages == 0)
                totalPages = 1;

            if (CurrentPage > totalPages)
                CurrentPage = totalPages;

            lblPageInfo.Text =
                "Page " + CurrentPage +
                " of " + totalPages;

            DataTable pageTable;

            if (finalTable.Rows.Count > 0)
            {
                pageTable =
                    finalTable.AsEnumerable()
                              .Skip((CurrentPage - 1) * PageSize)
                              .Take(PageSize)
                              .CopyToDataTable();
            }
            else
            {
                pageTable = finalTable.Clone();
            }

            gvDocuments.DataSource = pageTable;
            gvDocuments.DataBind();

            lblStatus.Text =
                "Filtered rows = " +
                filteredRows.Count;

            lblStatus.Text =
                " ";

            lblStatus.ForeColor =
                System.Drawing.Color.FromArgb(0x18, 0x80, 0x38);
        }

        private int GetCurrentUserId()
        {
            using (NpgsqlConnection conn =
                new NpgsqlConnection(connString))
            {
                conn.Open();

                using (NpgsqlCommand cmd =
                    new NpgsqlCommand(
                        @"SELECT id
                  FROM users
                  WHERE username = @username",
                        conn))
                {
                    cmd.Parameters.AddWithValue(
                        "username",
                        Session["UserID"].ToString());

                    object result =
                        cmd.ExecuteScalar();

                    return Convert.ToInt32(result);
                }
            }
        }

        private void BindDatasetFilter()
        {
            List<string> datasets =
                GetAllowedDatasets();

            rblDatasets.Items.Clear();

            foreach (string dataset in datasets)
            {
                rblDatasets.Items.Add(
                    new ListItem(dataset, dataset));
            }

            if (rblDatasets.Items.Count > 0 &&
                string.IsNullOrEmpty(rblDatasets.SelectedValue))
            {
                rblDatasets.SelectedIndex = 0;
            }
        }

        protected void rblDatasets_SelectedIndexChanged(
                        object sender,
                        EventArgs e)
        {
            CurrentPage = 1;

            RestoreDynamicFilters();

            BindDynamicVaultData();
        }



        private List<string> GetAllowedDatasets()
        {
            try
            {
                int userId =
                    GetCurrentUserId();

                List<string> datasets =
                    new List<string>();

                using (NpgsqlConnection conn =
                    new NpgsqlConnection(connString))
                {
                    conn.Open();

                    using (NpgsqlCommand cmd =
                        new NpgsqlCommand(
                            @"SELECT dataset_name
                      FROM user_dataset_access
                      WHERE userid = @userid",
                            conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "userid",
                            userId);

                        using (NpgsqlDataReader dr =
                            cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                datasets.Add(
                                    dr["dataset_name"]
                                        .ToString());
                            }
                        }
                    }
                }

                return datasets;
            }
            catch
            {
                return new List<string>();
            }
        }

        private int CurrentPage
        {
            get
            {
                return ViewState["CurrentPage"] == null
                    ? 1
                    : Convert.ToInt32(ViewState["CurrentPage"]);
            }
            set
            {
                ViewState["CurrentPage"] = value;
            }
        }

        protected void btnPrevPage_Click(
        object sender,
        EventArgs e)
        {
            if (CurrentPage > 1)
                CurrentPage--;

            BindDynamicVaultData();
        }

        protected void btnNextPage_Click(
            object sender,
            EventArgs e)
        {
            CurrentPage++;

            BindDynamicVaultData();
        }

        // ════════════════════════════════════════════════════════
        //  ROW DATA BOUND – header focus + HTML decode
        // ════════════════════════════════════════════════════════
        protected void gvDocuments_RowDataBound(
            object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text.Equals(
                        focusedColumn,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        e.Row.Cells[i].Attributes["class"] =
                            "auto-focus-column";
                    }
                }
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    // Decode HTML entities so <a> tags render correctly
                    e.Row.Cells[i].Text =
                        Server.HtmlDecode(e.Row.Cells[i].Text);
                }
            }
        }
    }
}
