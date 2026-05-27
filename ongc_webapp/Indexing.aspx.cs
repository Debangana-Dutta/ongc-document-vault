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
        private string connString =
            ConfigurationManager
            .ConnectionStrings["PostgresConn"]
            .ConnectionString;

        private string focusedColumn = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            gvDocuments.RowDataBound +=
                gvDocuments_RowDataBound;

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

        private void GenerateDynamicFilters(
            HashSet<string> availableColumns)
        {
            phDynamicFilters.Controls.Clear();

            foreach (string column in
                availableColumns.OrderBy(c => c))
            {
                Panel panel = new Panel();
                panel.CssClass = "filter-row";

                CheckBox cb = new CheckBox();

                cb.ID = "cb_" + column;

                bool isChecked =
                    Request.Form[cb.UniqueID] == "on";

                cb.Checked = isChecked;

                Literal lbl = new Literal();

                lbl.Text =
                    "<span class='filter-column-name'>" +
                    column +
                    "</span>";

                Panel textboxPanel = new Panel();

                string panelId =
                    "box_" +
                    column.Replace(" ", "_");

                textboxPanel.Attributes["id"] =
                    panelId;

                bool shouldShow =
                    Request.Form[cb.UniqueID] == "on";

                textboxPanel.Style["display"] =
                    shouldShow
                    ? "block"
                    : "none";

                textboxPanel.CssClass =
                    "filter-input-box";

                cb.InputAttributes.Add(
                    "onclick",
                    "toggleFilterTextbox('" +
                    panelId +
                    "')");

                TextBox txt = new TextBox();

                txt.ID = "txt_" + column;

                string textboxValue =
                    Request.Form[txt.UniqueID];

                if (!string.IsNullOrWhiteSpace(textboxValue))
                {
                    txt.Text = textboxValue;
                }

                txt.CssClass =
                    "form-control";

                txt.Attributes["placeholder"] =
                    "Filter value...";

                textboxPanel.Controls.Add(txt);

                panel.Controls.Add(cb);
                panel.Controls.Add(lbl);
                panel.Controls.Add(textboxPanel);

                phDynamicFilters.Controls.Add(panel);

                if (cblColumns.Items.FindByValue(column)
                    == null)
                {
                    cblColumns.Items.Add(
                        new ListItem(column, column));
                }
            }
        }

        private HashSet<string> GetFilteredMetadataColumns(
    List<Dictionary<string, string>> allRows)
        {
            HashSet<string> columns =
                new HashSet<string>();

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

                    string value =
                        kv.Value != null
                        ? kv.Value.ToString()
                        : "";

                    // SKIP SYSTEM COLUMNS ONLY

                    if (
                        key.Equals(
                            "file_name",
                            StringComparison.OrdinalIgnoreCase)
                    )
                    {
                        continue;
                    }

                    if (
                        string.IsNullOrWhiteSpace(value) ||
                        value == "NULL"
                    )
                    {
                        continue;
                    }

                    if (anySelected)
                    {
                        if (!selectedColumns.Contains(key))
                        {
                            continue;
                        }
                    }

                    columns.Add(key);
                }
            }

            return columns;
        }

        private void BindDynamicVaultData()
        {
            string rawSearch =
                txtSearch.Text.Trim();

            List<string> keywords =
                rawSearch
                .Split(
                    new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .Take(3)
                .ToList();

            string searchMode =
                rblSearchMode.SelectedValue;

            DataTable finalTable =
                new DataTable();

            
            finalTable.Columns.Add("file_name");


            List<Dictionary<string, string>> allRows =
                new List<Dictionary<string, string>>();

            string query = @"
                SELECT
                    id,
                    source_excel_file,
                    file_name,
                    file_path,
                    dynamic_metadata
                FROM indexed_documents";

            List<string> whereConditions =
                new List<string>();

            if (keywords.Count > 0)
            {
                List<string> keywordConditions =
                    new List<string>();

                for (int i = 0; i < keywords.Count; i++)
                {
                    keywordConditions.Add($@"
                    (
                        (
                     file_name ILIKE @kw{i}
                     OR dynamic_metadata::text ILIKE @kw{i}
)
                    )");
                }

                whereConditions.Add(
                    "(" +
                    string.Join(
                        $" {searchMode} ",
                        keywordConditions) +
                    ")");
            }

            if (whereConditions.Count > 0)
            {
                query +=
                    " WHERE " +
                    string.Join(
                        " AND ",
                        whereConditions);
            }

            query +=
                " ORDER BY uploaded_at DESC";

            using (NpgsqlConnection conn =
                new NpgsqlConnection(connString))
            {
                conn.Open();

                using (NpgsqlCommand cmd =
                    new NpgsqlCommand(query, conn))
                {
                    for (int i = 0; i < keywords.Count; i++)
                    {
                        cmd.Parameters.AddWithValue(
                            $"@kw{i}",
                            "%" + keywords[i] + "%");
                    }

                    using (NpgsqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, string> rowMap =
                                new Dictionary<string, string>();


                            rowMap["file_name"] =
                                reader["file_name"].ToString();

                            

                            string metadataJson =
                                reader["dynamic_metadata"]
                                .ToString();

                            if (!string.IsNullOrWhiteSpace(metadataJson))
                            {
                                JObject metadata =
                                    JsonConvert
                                    .DeserializeObject<JObject>(
                                        metadataJson);

                                foreach (var item in metadata)
                                {
                                    string key =
                                        item.Key;

                                    string value =
                                        item.Value != null
                                        ? item.Value.ToString()
                                        : "NULL";

                                    rowMap[key] = value;
                                }
                            }

                            allRows.Add(rowMap);
                        }
                    }
                }
            }

            // AUTO DETECT BEST COLUMN

            Dictionary<string, int> columnScores =
                new Dictionary<string, int>();

            foreach (var rowMap in allRows)
            {
                foreach (var kv in rowMap)
                {
                    if (
                        kv.Key.Equals(
                            "file_name",
                            StringComparison.OrdinalIgnoreCase)
                    )
                    {
                        continue;
                    }
                    string column =
                        kv.Key;

                    string value =
                        kv.Value != null
                        ? kv.Value.ToString().ToLower()
                        : "";

                    foreach (string keyword in keywords)
                    {
                        if (
                            value.Contains(
                                keyword.ToLower())
                        )
                        {
                            if (!columnScores.ContainsKey(column))
                            {
                                columnScores[column] = 0;
                            }

                            columnScores[column]++;
                        }
                    }
                }
            }

            if (columnScores.Count > 0)
            {
                focusedColumn =
                    columnScores
                    .OrderByDescending(x => x.Value)
                    .First()
                    .Key;
            }

            HashSet<string> filteredColumns =
                GetFilteredMetadataColumns(allRows);

            // DYNAMICALLY HIDE FILTERS
            // BASED ON CURRENT DATA

            foreach (Control ctrl in phDynamicFilters.Controls)
            {
                Panel panel = ctrl as Panel;

                if (panel == null)
                    continue;

                CheckBox cb = null;

                foreach (Control inner in panel.Controls)
                {
                    if (inner is CheckBox)
                    {
                        cb = (CheckBox)inner;
                        break;
                    }
                }

                if (cb == null)
                    continue;

                string columnName =
                    cb.ID.Replace("cb_", "");

                if (
                    filteredColumns.Contains(columnName)
                )
                {
                    panel.Visible = true;
                }
                else
                {
                    panel.Visible = false;
                }
            }

            // APPLY METADATA FILTERS

            List<Dictionary<string, string>> filteredRows =
                new List<Dictionary<string, string>>();

            foreach (var rowMap in allRows)
            {
                bool matches = true;

                foreach (Control ctrl in phDynamicFilters.Controls)
                {
                    Panel panel = ctrl as Panel;

                    if (panel == null)
                        continue;

                    CheckBox cb = null;
                    TextBox txt = null;

                    foreach (Control inner in panel.Controls)
                    {
                        if (inner is CheckBox)
                        {
                            cb = (CheckBox)inner;
                        }

                        if (inner is Panel)
                        {
                            Panel txtPanel =
                                (Panel)inner;

                            foreach (
                                Control txtCtrl
                                in txtPanel.Controls
                            )
                            {
                                if (txtCtrl is TextBox)
                                {
                                    txt = (TextBox)txtCtrl;
                                }
                            }
                        }
                    }

                    if (
    cb != null &&
    cb.Checked &&
    txt != null
)
                    {
                        string columnName =
                            cb.ID.Replace("cb_", "");

                        string actualValue = "";

                        if (
                            rowMap.ContainsKey(columnName)
                        )
                        {
                            actualValue =
                                rowMap[columnName] != null
                                ? rowMap[columnName].Trim()
                                : "";
                        }

                        string actualLower =
                            actualValue.ToLower();

                        string filterValue =
                            txt.Text != null
                            ? txt.Text.Trim().ToLower()
                            : "";

                        // !NULL
                        // SHOW ONLY NON-NULL VALUES

                        if (filterValue == "!null")
                        {
                            if (
                                string.IsNullOrWhiteSpace(actualLower) ||
                                actualLower == "null"
                            )
                            {
                                matches = false;
                                break;
                            }
                        }

                        // NULL
                        // SHOW ONLY NULL VALUES

                        else if (filterValue == "null")
                        {
                            if (
                                !string.IsNullOrWhiteSpace(actualLower) &&
                                actualLower != "null"
                            )
                            {
                                matches = false;
                                break;
                            }
                        }

                        // NORMAL TEXT FILTER

                        else if (
                            !string.IsNullOrWhiteSpace(filterValue)
                        )
                        {
                            if (
                                !actualLower.Contains(filterValue)
                            )
                            {
                                matches = false;
                                break;
                            }
                        }
                    }
                }

                if (matches)
                {
                    filteredRows.Add(rowMap);
                }
            }

            // REPLACE ORIGINAL ROWS
            allRows = filteredRows;

            bool anyColumnsSelected = false;

            foreach (ListItem item in cblColumns.Items)
            {
                if (item.Selected)
                {
                    anyColumnsSelected = true;
                    break;
                }
            }

            if (!anyColumnsSelected)
            {
                foreach (string column in filteredColumns)
                {
                    if (!finalTable.Columns.Contains(column))
                    {
                        finalTable.Columns.Add(column);
                    }
                }
            }
            else
            {
                foreach (ListItem item in cblColumns.Items)
                {
                    if (item.Selected)
                    {
                        if (filteredColumns.Contains(item.Value))
                        {
                            if (!finalTable.Columns.Contains(item.Value))
                            {
                                finalTable.Columns.Add(item.Value);
                            }
                        }
                    }
                }
            }

            foreach (var rowMap in allRows)
            {
                DataRow row =
                    finalTable.NewRow();

                foreach (DataColumn col in finalTable.Columns)
                {
                    if (rowMap.ContainsKey(col.ColumnName))
                    {
                        string cellValue =
                            rowMap[col.ColumnName];

                        foreach (string keyword in keywords)
                        {
                            if (
                                !string.IsNullOrWhiteSpace(keyword)
                            )
                            {
                                cellValue =
                                    System.Text.RegularExpressions.Regex
                                    .Replace(
                                        cellValue,
                                        keyword,
                                        "<span class='search-highlight'>" +
                                        keyword +
                                        "</span>",
                                        System.Text.RegularExpressions.RegexOptions.IgnoreCase
                                    );
                            }
                        }

                        row[col.ColumnName] =
                            cellValue;
                    }
                    else
                    {
                        row[col.ColumnName] = "NULL";
                    }
                }

                finalTable.Rows.Add(row);
            }

            gvDocuments.DataSource =
                finalTable;

            gvDocuments.DataBind();

            lblStatus.Text =
                finalTable.Rows.Count +
                " result(s) found.";
        }

        protected void gvDocuments_PageIndexChanging(
            object sender,
            GridViewPageEventArgs e)
        {
            gvDocuments.PageIndex =
                e.NewPageIndex;

            BindDynamicVaultData();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RestoreDynamicFilters();
        }

        private void RestoreDynamicFilters()
        {
            HashSet<string> columns =
                new HashSet<string>();

            using (
                NpgsqlConnection conn =
                new NpgsqlConnection(connString)
            )
            {
                conn.Open();

                string query = @"
            SELECT dynamic_metadata
            FROM indexed_documents
            LIMIT 200";

                using (
                    NpgsqlCommand cmd =
                    new NpgsqlCommand(query, conn)
                )
                {
                    using (
                        NpgsqlDataReader reader =
                        cmd.ExecuteReader()
                    )
                    {
                        while (reader.Read())
                        {
                            string metadataJson =
                                reader["dynamic_metadata"]
                                .ToString();

                            if (
                                string.IsNullOrWhiteSpace(
                                    metadataJson)
                            )
                            {
                                continue;
                            }

                            JObject metadata =
                                JsonConvert
                                .DeserializeObject<JObject>(
                                    metadataJson);

                            foreach (var item in metadata)
                            {
                                if (
                                    item.Key.Equals(
                                        "file_name",
                                        StringComparison
                                        .OrdinalIgnoreCase)
                                )
                                {
                                    continue;
                                }

                                columns.Add(item.Key);
                            }
                        }
                    }
                }
            }

            GenerateDynamicFilters(columns);
        }

        protected void gvDocuments_RowDataBound(
      object sender,
      GridViewRowEventArgs e)
        {
            if (
                e.Row.RowType ==
                DataControlRowType.Header
            )
            {
                for (
                    int i = 0;
                    i < e.Row.Cells.Count;
                    i++
                )
                {
                    string header =
                        e.Row.Cells[i].Text;

                    if (
                        header.Equals(
                            focusedColumn,
                            StringComparison
                            .OrdinalIgnoreCase)
                    )
                    {
                        e.Row.Cells[i].Attributes["class"] =
                            "auto-focus-column";
                    }
                }
            }

            if (
                e.Row.RowType ==
                DataControlRowType.DataRow
            )
            {
                for (
                    int i = 0;
                    i < e.Row.Cells.Count;
                    i++
                )
                {
                    e.Row.Cells[i].Text =
                        Server.HtmlDecode(
                            e.Row.Cells[i].Text);
                }
            }
        }
    }
}