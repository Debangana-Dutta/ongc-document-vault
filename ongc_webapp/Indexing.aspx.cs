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
    // Changed: BasePage instead of Page.
    // No RequiredRole override — login check is sufficient for this page.
    public partial class Indexing : BasePage
    {
        private string connString =
            ConfigurationManager
            .ConnectionStrings["PostgresConn"]
            .ConnectionString;

        private string focusedColumn = "";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Session["UserID"] == null) return;
            RestoreDynamicFilters();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            gvDocuments.RowDataBound += gvDocuments_RowDataBound;

            if (!IsPostBack)
            {
                BindDynamicVaultData();
            }
        }

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

        protected void gvDocuments_PageIndexChanging(
            object sender, GridViewPageEventArgs e)
        {
            gvDocuments.PageIndex = e.NewPageIndex;
            BindDynamicVaultData();
        }

        // ════════════════════════════════════════════════════════
        //  SECURITY HELPERS
        // ════════════════════════════════════════════════════════
        private string GetCurrentUserCPF()
        {
            // Session["UserCPF"] is now set by Login.aspx.cs.
            // Session["UserID"] is kept as a fallback for compatibility.
            return Session["UserCPF"] != null ? Session["UserCPF"].ToString()
                 : Session["UserID"] != null ? Session["UserID"].ToString()
                 : "";
        }

        private List<string> GetAllowedDatasets(string cpf)
        {
            if (string.IsNullOrEmpty(cpf)) return null;

            List<string> datasets = new List<string>();

            try
            {
                using (NpgsqlConnection conn =
                    new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query =
                        "SELECT datasetid " +
                        "FROM user_dataset_access " +
                        "WHERE userid = (SELECT id FROM users WHERE cpf = @cpf)";

                    using (NpgsqlCommand cmd =
                        new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("cpf", cpf);
                        using (NpgsqlDataReader dr = cmd.ExecuteReader())
                            while (dr.Read())
                                datasets.Add(dr["datasetid"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "GetAllowedDatasets Error: " + ex.Message);
            }

            return datasets;
        }

        private HashSet<string> GetAllowedMetadataColumns(string cpf)
        {
            if (string.IsNullOrEmpty(cpf)) return null;

            try
            {
                using (NpgsqlConnection conn =
                    new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query =
                        "SELECT visible_columns FROM user_metadata_policy " +
                        "WHERE user_cpf = @cpf";

                    using (NpgsqlCommand cmd =
                        new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("cpf", cpf);
                        object result = cmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                            return null;

                        string json = result.ToString();
                        if (string.IsNullOrWhiteSpace(json))
                            return null;

                        List<string> cols =
                            JsonConvert.DeserializeObject<List<string>>(json);

                        return new HashSet<string>(cols,
                            StringComparer.OrdinalIgnoreCase);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "GetAllowedMetadataColumns Error: " + ex.Message);
                return null;
            }
        }

        // ════════════════════════════════════════════════════════
        //  RESTORE DYNAMIC FILTERS
        // ════════════════════════════════════════════════════════
        private void RestoreDynamicFilters()
        {
            string cpf = GetCurrentUserCPF();
            HashSet<string> allowedCols = GetAllowedMetadataColumns(cpf);

            if (allowedCols != null && lblAccessBadge != null)
                lblAccessBadge.Visible = true;

            HashSet<string> allKeys = new HashSet<string>();

            try
            {
                using (NpgsqlConnection conn =
                    new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query =
                        "SELECT DISTINCT jsonb_object_keys(dynamic_metadata) " +
                        "FROM indexed_documents " +
                        "WHERE dynamic_metadata IS NOT NULL " +
                        "ORDER BY 1";

                    using (NpgsqlCommand cmd =
                        new NpgsqlCommand(query, conn))
                    using (NpgsqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            allKeys.Add(reader.GetString(0));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "RestoreDynamicFilters Error: " + ex.Message);
            }

            HashSet<string> visibleKeys = (allowedCols == null)
                ? allKeys
                : new HashSet<string>(
                    allKeys.Where(k => allowedCols.Contains(k)),
                    StringComparer.OrdinalIgnoreCase);

            GenerateDynamicFilters(visibleKeys);
        }

        // ════════════════════════════════════════════════════════
        //  GENERATE DYNAMIC FILTERS
        // ════════════════════════════════════════════════════════
        private void GenerateDynamicFilters(HashSet<string> availableColumns)
        {
            phDynamicFilters.Controls.Clear();

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

                CheckBox cb = new CheckBox();
                cb.ID = "cb_" + column;
                bool isChecked = Request.Form[cb.UniqueID] == "on";
                cb.Checked = isChecked;

                Literal lbl = new Literal();
                lbl.Text =
                    "<span class='filter-column-name'>" +
                    System.Web.HttpUtility.HtmlEncode(column) +
                    "</span>";

                Panel textboxPanel = new Panel();
                string panelId = "box_" + column.Replace(" ", "_");
                textboxPanel.Attributes["id"] = panelId;
                textboxPanel.CssClass = "filter-input-box";
                textboxPanel.Style["display"] = isChecked ? "block" : "none";

                cb.InputAttributes.Add(
                    "onclick",
                    "toggleFilterTextbox('" + panelId + "')");

                TextBox txt = new TextBox();
                txt.ID = "txt_" + column;
                txt.CssClass = "form-control";
                txt.Attributes["placeholder"] = "Filter value…";

                string postedValue = Request.Form[txt.UniqueID];
                if (!string.IsNullOrWhiteSpace(postedValue))
                    txt.Text = postedValue;

                textboxPanel.Controls.Add(txt);

                panel.Controls.Add(cb);
                panel.Controls.Add(lbl);
                panel.Controls.Add(textboxPanel);

                phDynamicFilters.Controls.Add(panel);

                if (cblColumns.Items.FindByValue(column) == null)
                    cblColumns.Items.Add(new ListItem(column, column));
            }
        }

        // ════════════════════════════════════════════════════════
        //  GET FILTERED METADATA COLUMNS
        // ════════════════════════════════════════════════════════
        private HashSet<string> GetFilteredMetadataColumns(
            List<Dictionary<string, string>> allRows)
        {
            HashSet<string> columns = new HashSet<string>();
            HashSet<string> selectedColumns = new HashSet<string>();
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

                    if (key.Equals("file_name", StringComparison.OrdinalIgnoreCase) ||
                        key.Equals("view", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (string.IsNullOrWhiteSpace(value) || value == "NULL")
                        continue;

                    if (anySelected && !selectedColumns.Contains(key))
                        continue;

                    columns.Add(key);
                }
            }

            return columns;
        }

        // ════════════════════════════════════════════════════════
        //  BIND DYNAMIC VAULT DATA
        // ════════════════════════════════════════════════════════
        private void BindDynamicVaultData()
        {
            string cpf = GetCurrentUserCPF();

            List<string> allowedDatasets = GetAllowedDatasets(cpf);

            if (allowedDatasets != null && allowedDatasets.Count == 0)
            {
                gvDocuments.DataSource = new DataTable();
                gvDocuments.DataBind();
                lblStatus.Text = "⚠ You do not have access to any datasets. "
                               + "Contact your administrator.";
                lblStatus.ForeColor = System.Drawing.Color.Crimson;
                return;
            }

            string rawSearch = txtSearch.Text.Trim();
            List<string> keywords = rawSearch
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .Take(3)
                .ToList();

            string searchMode = rblSearchMode.SelectedValue;

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

            if (allowedDatasets != null && allowedDatasets.Count > 0)
            {
                List<string> dsParams = new List<string>();
                for (int i = 0; i < allowedDatasets.Count; i++)
                    dsParams.Add("@ds" + i);

                whereConditions.Add(
                    "source_excel_file IN (" +
                    string.Join(", ", dsParams) +
                    ")");
            }

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

            query += " ORDER BY uploaded_at DESC LIMIT 500";

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
                        if (allowedDatasets != null)
                        {
                            for (int i = 0; i < allowedDatasets.Count; i++)
                                cmd.Parameters.AddWithValue(
                                    "@ds" + i, allowedDatasets[i]);
                        }

                        for (int i = 0; i < keywords.Count; i++)
                            cmd.Parameters.AddWithValue(
                                "@kw" + i, "%" + keywords[i] + "%");

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

            HashSet<string> filteredColumns =
                GetFilteredMetadataColumns(allRows);

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

                    if (cb == null || !cb.Checked || txt == null) continue;

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

            foreach (var rowMap in allRows)
            {
                DataRow row = finalTable.NewRow();

                foreach (DataColumn col in finalTable.Columns)
                {
                    string cellValue = rowMap.ContainsKey(col.ColumnName)
                        ? rowMap[col.ColumnName] ?? "NULL"
                        : "NULL";

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
                                    keyword + "</span>",
                                    System.Text.RegularExpressions.RegexOptions
                                        .IgnoreCase);
                        }
                    }

                    row[col.ColumnName] = cellValue;
                }

                finalTable.Rows.Add(row);
            }

            gvDocuments.DataSource = finalTable;
            gvDocuments.DataBind();

            lblStatus.Text = finalTable.Rows.Count + " result(s) found.";
            lblStatus.ForeColor =
                System.Drawing.Color.FromArgb(0x18, 0x80, 0x38);
        }

        // ════════════════════════════════════════════════════════
        //  ROW DATA BOUND
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
                    e.Row.Cells[i].Text =
                        Server.HtmlDecode(e.Row.Cells[i].Text);
                }
            }
        }
    }
}