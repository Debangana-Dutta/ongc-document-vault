using DocumentFormat.OpenXml.Office.Word;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ongc_webapp
{
    public partial class Indexing : System.Web.UI.Page
    {
        int totalRows = 0;
        // ── Connection string ──────────────────────────────────
        private string connString =
            ConfigurationManager
            .ConnectionStrings["PostgresConn"]
            .ConnectionString;

        // Column to scroll-highlight in the results table
        private string focusedColumn = "";
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Session["UserID"] == null)
                return;

            BindDatasetFilter();

            if (Session["SelectedDataset"] == null &&
                rblDatasets.Items.Count > 0)
            {
                Session["SelectedDataset"] =
                    rblDatasets.Items[0].Value;
            }

            RestoreDynamicFilters();
        }


        private string GetSelectedDataset()
        {
            if (Session["SelectedDataset"] != null)
                return Session["SelectedDataset"].ToString();

            if (!string.IsNullOrEmpty(rblDatasets.SelectedValue))
                return rblDatasets.SelectedValue;

            return "";
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


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Session["SelectedDataset"] =
                GetSelectedDataset();

            CurrentPage = 1;

            BindDynamicVaultData();
        }

        protected void btnApplyFilters_Click(object sender, EventArgs e)
        {
            Session["SelectedDataset"] =
                GetSelectedDataset();

            CurrentPage = 1;

            BindDynamicVaultData();
        }

        protected void btnApplyColumns_Click(object sender, EventArgs e)
        {
            BindDynamicVaultData();
        }

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
                      @"SELECT TRIM(metadata_name)
                      FROM user_metadata_access
                      WHERE userid = @userid
                      AND dataset_name = @dataset";

                    string dataset = GetSelectedDataset();

                    if (string.IsNullOrWhiteSpace(dataset))
                    {
                        return new HashSet<string>();
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
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
                                    reader[0]
                                        .ToString()
                                        .Trim());
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

        private void RestoreDynamicFilters()
        {
            lblStatus.Text =
            "Dataset = [" +
            GetSelectedDataset() +
            "]";

            // ── Determine which columns this user may see ──────
            HashSet<string> allowedCols =
            GetAllowedMetadataColumns();
            // NULL = all columns allowed

            // ── Collect all known metadata keys from the DB ────
            // We query only up to 500 rows to discover column names.
            HashSet<string> visibleKeys =
            GetAllowedMetadataColumns();

            GenerateDynamicFilters(visibleKeys);
        }

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

            int debugIndex = 0;

            foreach (string column in availableColumns.OrderBy(c => c))
            {
                System.Diagnostics.Debug.WriteLine(
                    debugIndex + " => " + column);

                debugIndex++;
                {
                    Panel panel = new Panel();
                    panel.CssClass = "filter-row";

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

                    string safeColumn =
                    column.Replace(" ", "_")
                          .Replace(".", "_")
                          .Replace("-", "_");

                    TextBox txt = new TextBox();

                    txt.ID = "txt_" + safeColumn;

                    System.Diagnostics.Debug.WriteLine("CREATED TEXTBOX: " + txt.ID);

                    

                    txt.CssClass = "form-control";
                    txt.Attributes["placeholder"] = "Filter value…";
                    txt.Attributes["data-column"] = column;

                    // Restore textbox value from posted form data
                    if (Request.Form[txt.UniqueID] != null)
                    {
                        txt.Text = Request.Form[txt.UniqueID];
                    }

                    textboxPanel.Controls.Add(txt);
                    panel.Controls.Add(lbl);
                    panel.Controls.Add(textboxPanel);

                    phDynamicFilters.Controls.Add(panel);

                    // Also populate the Column Visibility checkboxlist
                    if (cblColumns.Items.FindByValue(column) == null)
                        cblColumns.Items.Add(new ListItem(column, column));
                }
            }
        }

        
        private void BindDynamicVaultData()
        {

            lblStatus.Text =
            "DATASET = " +
            GetSelectedDataset();
            int totalResults = 0;


            // If the user has a policy but zero datasets, return early


            // ── Keyword search setup ───────────────────────────
            string rawSearch =
            System.Text.RegularExpressions.Regex.Replace(
                txtSearch.Text,
                @"[^\w\s]",
                " "
            ).Trim();
            List<string> keywords = rawSearch
                .Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .Take(6)
                .ToList();

            // ── Build SQL ──────────────────────────────────────
            string query = @"
                SELECT
                    id,
                    source_excel_file,
                    file_name,
                    file_path,
                    dynamic_metadata,
                    0 as relevance
                FROM indexed_documents
                WHERE 1=1";

            string relevanceExpression = "0";

            List<string> whereConditions = new List<string>();

            if (keywords.Count > 0)
            {
                HashSet<string> visibleColumns =
                    GetAllowedMetadataColumns();

                List<string> vectorParts =
                    new List<string>();

                vectorParts.Add(
                    "to_tsvector('simple', regexp_replace(\r\n    COALESCE(file_name,''),\r\n    '[_\\.-]',\r\n    ' ',\r\n    'g'\r\n))");

                vectorParts.Add(
                    "to_tsvector('simple', COALESCE(extracted_text,''))");

                foreach (string col in visibleColumns)
                {
                    vectorParts.Add(
                        "to_tsvector('simple', regexp_replace(COALESCE(dynamic_metadata->>'" +
                        col.Replace("'", "") +
                        "',''), '[_-]', ' ', 'g'))");
                }

                string userSearchVector =
                    "(" +
                    string.Join(" || ", vectorParts) +
                    ")";

                whereConditions.Add(
                "(" +
                userSearchVector +
                @" @@ websearch_to_tsquery(
                    'simple',
                    @searchText)
                OR
                LOWER(file_name) LIKE LOWER('%' || @searchText || '%')
                )");

                relevanceExpression =
                    @"ts_rank(
            " + userSearchVector + @",
            websearch_to_tsquery(
                'simple',
                @searchText
            )
            )";
            }
            
           

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

            
            int metadataIndex = 0;

            foreach (Control ctrl in phDynamicFilters.Controls)
            {
                Panel panel = ctrl as Panel;
                if (panel == null) continue;

                TextBox txt = null;

                foreach (Control inner in panel.Controls)
                {
                    if (inner is Panel)
                    {
                        foreach (Control sub in ((Panel)inner).Controls)
                        {
                            if (sub is TextBox)
                                txt = (TextBox)sub;
                        }
                    }
                }

                if (txt == null)
                    continue;

                if (string.IsNullOrWhiteSpace(txt.Text))
                    continue;

                string colName =
                    txt.Attributes["data-column"];

                string param =
                    "meta" + metadataIndex;

                whereConditions.Add(
                    "dynamic_metadata->>'" +
                    colName.Replace("'", "") +
                    "' ILIKE @" +
                    param);

                metadataIndex++;
            }


            if (whereConditions.Count > 0)
                query += " AND " + string.Join(" AND ", whereConditions);

            query =
            query.Replace(
                "0 as relevance",
                relevanceExpression + " as relevance");

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

            int pageSize = 20;

            int offset =
                (CurrentPage - 1) * pageSize;

            query +=
                " ORDER BY relevance DESC, uploaded_at DESC " +
                " LIMIT @limit " +
                " OFFSET @offset";

            string displayQuery =
            whereConditions.Count > 0
            ? "WHERE " + string.Join(" AND ", whereConditions)
            : "WHERE 1=1";

            foreach (Control ctrl in phDynamicFilters.Controls)
            {
                Panel panel = ctrl as Panel;
                if (panel == null) continue;
                TextBox txt = null;

                foreach (Control inner in panel.Controls)
                {
                    

                    if (inner is Panel)
                    {
                        foreach (Control sub in ((Panel)inner).Controls)
                        {
                            if (sub is TextBox)
                                txt = (TextBox)sub;
                        }
                    }
                }

                if (txt == null)
                    continue;

                if (string.IsNullOrWhiteSpace(txt.Text))
                    continue;

            }


            for (int i = 0; i < allowedDatasets.Count; i++)
            {
                displayQuery =
                    displayQuery.Replace(
                        "@dataset" + i,
                        "'" + allowedDatasets[i] + "'");
            }

            string shortQuery = displayQuery;

            if (shortQuery.Length > 200)
            {
                shortQuery =
                    shortQuery.Substring(0, 200) + "...";
            }

            litSqlQuery.Text =
                "<div class='sql-query-bar'>" +
                Server.HtmlEncode(shortQuery) +
                "</div>" +
                "<textarea id='fullSqlQuery' style='display:none;'>" +
                Server.HtmlEncode(displayQuery) +
                "</textarea>";

            // ── Execute query ──────────────────────────────────
            List<Dictionary<string, string>> allRows =
                new List<Dictionary<string, string>>();

            try
            {
                using (NpgsqlConnection conn =
                    new NpgsqlConnection(connString))
                {
                    conn.Open();

                    using (NpgsqlCommand countCmd =
                        new NpgsqlCommand(countQuery, conn))
                    {
                        if (keywords.Count > 0)
                        {
                            countCmd.Parameters.AddWithValue(
                                "searchText",
                                string.Join(" OR ", keywords));
                        }

                        for (int i = 0; i < allowedDatasets.Count; i++)
                        {
                            countCmd.Parameters.AddWithValue(
                                "dataset" + i,
                                allowedDatasets[i]);
                        }

                        metadataIndex = 0;

                        foreach (Control ctrl in phDynamicFilters.Controls)
                        {
                            Panel panel = ctrl as Panel;
                            if (panel == null)
                                continue;

                            TextBox txt = null;

                            foreach (Control inner in panel.Controls)
                            {
                                if (inner is Panel)
                                {
                                    foreach (Control sub in ((Panel)inner).Controls)
                                    {
                                        if (sub is TextBox)
                                            txt = (TextBox)sub;
                                    }
                                }
                            }

                            if (txt == null ||
                                string.IsNullOrWhiteSpace(txt.Text))
                                continue;

                            countCmd.Parameters.AddWithValue(
                                "meta" + metadataIndex,
                                "%" + txt.Text.Trim() + "%");

                            metadataIndex++;
                        }

                        totalRows =
                            Convert.ToInt32(
                                countCmd.ExecuteScalar());
                    }

                    using (NpgsqlCommand cmd =
                        new NpgsqlCommand(query, conn))
                    {

                        if (keywords.Count > 0)
                        {
                            cmd.Parameters.AddWithValue(
                            "searchText",
                            string.Join(" OR ", keywords));
                        }

                        // Bind dataset security parameters

                        for (int i = 0;
                           i < allowedDatasets.Count;
                           i++)
                        {

                            metadataIndex = 0;

                            foreach (Control ctrl in phDynamicFilters.Controls)
                            {
                                Panel panel = ctrl as Panel;
                                if (panel == null)
                                    continue;

                                TextBox txt = null;

                                foreach (Control inner in panel.Controls)
                                {
                                    if (inner is Panel)
                                    {
                                        foreach (Control sub in ((Panel)inner).Controls)
                                        {
                                            if (sub is TextBox)
                                                txt = (TextBox)sub;
                                        }
                                    }
                                }

                                if (txt == null)
                                    continue;

                                if (string.IsNullOrWhiteSpace(txt.Text))
                                    continue;

                                cmd.Parameters.AddWithValue(
                                    "meta" + metadataIndex,
                                    "%" + txt.Text.Trim() + "%");

                                metadataIndex++;
                            }

                            cmd.Parameters.AddWithValue(
                                "dataset" + i,
                                allowedDatasets[i]);
                        }

                        cmd.Parameters.AddWithValue(
                            "limit",
                            pageSize);

                        cmd.Parameters.AddWithValue(
                            "offset",
                            offset);


                        // Bind keyword parameters

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
                                    {
                                        rowMap[item.Key.Trim()] =
                                            item.Value != null
                                                ? item.Value.ToString()
                                                : "NULL";
                                    }
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

            metadataIndex = 0;

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
                GetAllowedMetadataColumns();

            if (filteredColumns == null)
            {
                filteredColumns = new HashSet<string>();

                foreach (var row in allRows)
                {
                    foreach (var key in row.Keys)
                    {
                        if (key != "file_name" &&
                            key != "view")
                        {
                            filteredColumns.Add(key);
                        }
                    }
                }
            }

            // ── Show/hide sidebar filter rows ──────────────────


            // ── Apply metadata text filters ────────────────────


            totalResults = allRows.Count;

            if (totalResults == 0)
            {
                AuditLogger.LogActivity(
                    Session["UserID"].ToString(),
                    "ZERO_RESULT_SEARCH",
                    "No results found",
                    GetSelectedDataset(),
                    null,
                    txtSearch.Text.Trim());
            }

            AuditLogger.LogActivity(
                Session["UserID"].ToString(),
                "SEARCH",
                "Search executed",
                GetSelectedDataset(),
                null,
                txtSearch.Text.Trim());

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
                                string normalizedCell =
                                    cellValue
                                    .Replace("_", " ")
                                    .Replace("-", " ")
                                    .Replace(".", " ");

                                if (normalizedCell.IndexOf(
                                        keyword,
                                        StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    cellValue =
                                        System.Text.RegularExpressions.Regex.Replace(
                                            cellValue,
                                            System.Text.RegularExpressions.Regex.Escape(keyword),
                                            "<span class='search-highlight'>$0</span>",
                                            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                                }
                            }
                        }
                    }

                    row[col.ColumnName] = cellValue;
                }

                finalTable.Rows.Add(row);
            }

            // ── Bind GridView ──────────────────────────────────
            const int PageSize = 20;

           

            int totalPages =
                (int)Math.Ceiling(
                    (double)totalRows / PageSize);

            if (totalPages == 0)
                totalPages = 1;

            TotalPages = totalPages;

            if (CurrentPage > totalPages)
                CurrentPage = totalPages;

            lblPageInfo.Text =
                "Page " + CurrentPage +
                " of " + totalPages;

            btnFirstPage.Enabled = CurrentPage > 1;

            btnPrevPage.Enabled =
                CurrentPage > 1;

            btnNextPage.Enabled =
                CurrentPage < totalPages;

            btnLastPage.Enabled =
                CurrentPage < totalPages;

            gvDocuments.DataSource = finalTable;
            gvDocuments.DataBind();

            lblStatus.Text =
                "Results found = " +
                totalRows;
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

            string selected = "";

            if (Session["SelectedDataset"] != null)
            {
                selected =
                    Session["SelectedDataset"].ToString();
            }

            rblDatasets.Items.Clear();

            foreach (string dataset in datasets)
            {
                rblDatasets.Items.Add(
                    new ListItem(dataset, dataset));
            }

            if (!string.IsNullOrEmpty(selected))
            {
                ListItem item =
                    rblDatasets.Items.FindByValue(selected);

                if (item != null)
                {
                    item.Selected = true;
                }
            }

            if (rblDatasets.Items.Count > 0 &&
                string.IsNullOrEmpty(rblDatasets.SelectedValue))
            {
                rblDatasets.SelectedIndex = 0;

                Session["SelectedDataset"] =
                    rblDatasets.SelectedValue;
            }
        }

        protected void rblDatasets_SelectedIndexChanged(
                        object sender,
                        EventArgs e)
        {

            Session["SelectedDataset"] = rblDatasets.SelectedValue;

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

        private int TotalPages
        {
            get
            {
                return ViewState["TotalPages"] == null
                    ? 1
                    : Convert.ToInt32(ViewState["TotalPages"]);
            }
            set
            {
                ViewState["TotalPages"] = value;
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

        protected void btnFirstPage_Click(
                    object sender,
                    EventArgs e)
        {
            CurrentPage = 1;

            BindDynamicVaultData();
        }

        protected void btnNextPage_Click(
                        object sender,
                        EventArgs e)
        {
            if (CurrentPage < TotalPages)
                CurrentPage++;

            BindDynamicVaultData();
        }

        protected void btnLastPage_Click(
                        object sender,
                        EventArgs e)
        {
            CurrentPage = TotalPages;

            BindDynamicVaultData();
        }

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
                    string text =
                            Server.HtmlDecode(
                                e.Row.Cells[i].Text);

                    e.Row.Cells[i].Controls.Clear();

                    e.Row.Cells[i].Controls.Add(
                        new LiteralControl(text));
                }
            }
        }
    }
}
