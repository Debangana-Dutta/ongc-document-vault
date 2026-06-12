<%@ Page Title="Enterprise Search Index"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Indexing.aspx.cs"
    Inherits="ongc_webapp.Indexing" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <style>

        @import url('https://fonts.googleapis.com/css2?family=Google+Sans:wght@400;500;700&family=Roboto:wght@300;400;500&display=swap');

        *, *::before, *::after { box-sizing: border-box; }

        body {
            background-color: #f8f9fa;
            font-family: 'Roboto', Arial, sans-serif;
            color: #202124;
            margin: 0;
        }
        .search-hero {
            display: flex;
            flex-direction: column;
            align-items: center;
            padding: 36px 24px 28px;
            max-width: 900px;
            margin: 0 auto;
        }

        .search-hero-title {
            font-family: 'Google Sans', sans-serif;
            font-size: 1.75rem;
            font-weight: 500;
            color: #1a0a0e;
            margin-bottom: 28px;
            letter-spacing: -0.3px;
            text-align: center;
        }

        .search-row {
            display: flex;
            align-items: center;
            gap: 14px;
            width: 100%;
        }

        .search-bar-wrap {
            flex: 1;
            position: relative;
        }

        .search-bar-wrap input[type="text"] {
            width: 100% !important;
            height: 48px !important;
            padding: 0 20px !important;
            border: 1.5px solid #dfe1e5 !important;
            border-radius: 28px !important;
            font-size: 0.95rem !important;
            color: #202124 !important;
            background: #fff !important;
            box-shadow: 0 1px 6px rgba(32,33,36,0.08) !important;
            transition: box-shadow 0.18s, border-color 0.18s !important;
            outline: none !important;
        }

        .search-bar-wrap input[type="text"]:focus {
            border-color: #aaa !important;
            box-shadow: 0 2px 10px rgba(32,33,36,.16) !important;
        }

        .mode-wrap {
            display: flex;
            flex-direction: column;
            gap: 2px;
            white-space: nowrap;
            font-size: 0.82rem;
            color: #5f6368;
        }

        .mode-wrap label { margin: 0; cursor: pointer; }

        input.btn-search-hero,
        .btn-search-hero {
            height: 48px;
            padding: 0 28px;
            background: #7a0616 !important;
            color: #fff !important;
            border: none !important;
            border-radius: 28px !important;
            font-family: 'Google Sans', sans-serif;
            font-size: 0.9rem;
            font-weight: 500;
            cursor: pointer;
            white-space: nowrap;
            transition: background 0.15s, box-shadow 0.15s;
            box-shadow: 0 1px 4px rgba(0,0,0,0.22);
        }

        input.btn-search-hero:hover,
        .btn-search-hero:hover {
            background: #5e0410 !important;
            box-shadow: 0 2px 10px rgba(0,0,0,0.28);
        }

        .btn-col-toggle {
            margin-top: 18px;
            padding: 8px 20px;
            border: 1.5px solid #dadce0;
            border-radius: 22px;
            background: #fff;
            color: #3c4043;
            font-size: 0.85rem;
            cursor: pointer;
            transition: background 0.12s, border-color 0.12s;
            font-family: 'Roboto', Arial, sans-serif;
        }

        .btn-col-toggle:hover {
            background: #f1f3f4;
            border-color: #bbbfc3;
        }

        /* ═══════════════════════════
           COLUMN PANEL
        ═══════════════════════════ */
        .column-panel {
            width: 100%;
            margin: 14px 0 0;
            border: 1px solid #e0e0e0;
            border-radius: 10px;
            padding: 16px;
            background: #fff;
            display: none;
        }

        .column-panel h5 {
            font-size: 0.88rem;
            font-weight: 600;
            color: #3c4043;
            margin-bottom: 10px;
        }

        .column-filter-wrapper {
            max-height: 160px;
            overflow-y: auto;
            border: 1px solid #e2e8f0;
            border-radius: 6px;
            padding: 10px;
        }

        .checkbox-list {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
        }

        .column-panel-actions {
            display: flex;
            gap: 10px;
            margin-top: 12px;
        }

        .column-panel-actions .btn {
            flex: 1;
            font-size: 0.83rem;
            border-radius: 20px;
            padding: 6px 0;
        }

        .layout-wrapper {
            display: flex;
            gap: 18px;
            align-items: flex-start;
            padding: 0 24px 32px;
        }

        /* LEFT SIDEBAR */
        .filter-sidebar {
            width: 270px;
            min-width: 270px;
            max-width: 270px;
            background: #fff;
            border-radius: 12px;
            padding: 18px 16px;
            box-shadow: 0 1px 4px rgba(0,0,0,0.08);
            position: sticky;
            top: 14px;
            max-height: 82vh;
            display: flex;
            flex-direction: column;
        }

        .filter-sidebar h5 {
            font-family: 'Google Sans', sans-serif;
            font-size: 0.95rem;
            font-weight: 600;
            color: #3c4043;
            margin-bottom: 12px;
        }

        /* ── Access-restricted notice badge ── */
        .access-badge {
            display: inline-block;
            font-size: 0.72rem;
            font-weight: 500;
            background: #fce8eb;
            color: #7a0616;
            border: 1px solid #f4c2ca;
            border-radius: 20px;
            padding: 2px 10px;
            margin-bottom: 12px;
        }

        .metadata-filter-search { margin-bottom: 12px; }

        .metadata-filter-search input[type="text"] {
            border-radius: 20px !important;
            font-size: 0.83rem !important;
            border: 1px solid #dadce0 !important;
            padding: 6px 14px !important;
            height: 36px !important;
            width: 100% !important;
            outline: none !important;
        }

        .metadata-scroll-area {
            flex: 1;
            overflow-y: auto;
            padding-right: 4px;
        }

        /* ── Dynamic filter rows (generated by code-behind) ── */
        .filter-row {
            border-bottom: 1px solid #f1f3f4;
            padding-bottom: 10px;
            margin-bottom: 10px;
        }

        .filter-column-name {
            font-size: 0.85rem;
            font-weight: 600;
            color: #3c4043;
            margin-left: 4px;
        }

        .filter-input-box {
            margin-top: 8px;
            display: none;
        }

        /* Textbox inside sidebar filter rows */
        .filter-input-box input[type="text"] {
            width: 100% !important;
            border: 1px solid #dadce0 !important;
            border-radius: 6px !important;
            padding: 5px 10px !important;
            font-size: 0.82rem !important;
            height: 32px !important;
            outline: none !important;
            transition: border-color 0.15s !important;
        }

        .filter-input-box input[type="text"]:focus {
            border-color: #7a0616 !important;
        }

        .metadata-filter-footer {
            margin-top: 14px;
            padding-top: 12px;
            border-top: 1px solid #e2e8f0;
        }

        input.btn-apply-dark,
        .btn-apply-dark {
            width: 100% !important;
            background: #1a1a1a !important;
            color: #fff !important;
            border: none !important;
            border-radius: 20px !important;
            padding: 9px 0 !important;
            font-size: 0.875rem !important;
            font-weight: 500 !important;
            cursor: pointer !important;
            transition: background 0.12s !important;
        }

        input.btn-apply-dark:hover,
        .btn-apply-dark:hover { background: #000 !important; }

        /* empty-state for sidebar */
        .sidebar-empty {
            font-size: 0.82rem;
            color: #80868b;
            text-align: center;
            padding: 20px 0;
        }

        /* RIGHT: Results */
        .main-results { flex: 1; min-width: 0; }

        .results-card {
            background: #fff;
            border-radius: 12px;
            padding: 20px;
            box-shadow: 0 1px 4px rgba(0,0,0,0.08);
        }

        .status-label {
            font-family: 'Google Sans', sans-serif;
            font-size: 0.95rem;
            font-weight: 500;
            color: #188038;
            margin-bottom: 10px;
            display: block;
        }

 
        .scroll-track {
            overflow-x: auto;
            overflow-y: hidden;
            height: 14px;
            background: #f5f5f5;
            border: 1px solid #e0e0e0;
            border-radius: 4px;
        }

        .scroll-track-top  { margin-bottom: 4px; }
        .scroll-track-bottom { margin-top: 4px; }

        #topScrollContent,
        #bottomScrollContent { height: 1px; }

        .results-wrapper {
            overflow-x: hidden;
            overflow-y: auto;
            max-height: 70vh;
            border: 1px solid #e0e0e0;
            border-radius: 8px;
        }

        .grid-table {
            width: auto;
            border-collapse: collapse;
            background: #fff;
            font-size: 0.875rem;
            table-layout: auto;
        }

        .grid-table tr th {
            position: sticky;
            top: 0;
            z-index: 4;
            background-color: #7a0616 !important;
            color: #fff !important;
            padding: 11px 18px;
            text-align: left;
            font-family: 'Google Sans', sans-serif;
            font-size: 0.82rem;
            font-weight: 500;
            letter-spacing: 0.3px;
            border: 1px solid rgba(255,255,255,0.15) !important;
            white-space: nowrap !important;
        }

        .grid-table tr th:nth-child(1) {
            position: sticky;
            left: 0;
            z-index: 6;
            background-color: #7a0616 !important;
            color: #fff !important;
            box-shadow: 3px 0 0 rgba(255,255,255,0.15);
        }

        .grid-table tr th:nth-child(2) {
            position: sticky;
            z-index: 5;
            background-color: #7a0616 !important;
            color: #fff !important;
            box-shadow: 3px 0 8px rgba(0,0,0,0.18);
        }

        .grid-table tr td {
            padding: 10px 18px;
            border: 1px solid #e8eaed;
            vertical-align: middle;
            background-color: #fff;
            color: #202124;
            white-space: nowrap !important;
            transition: background-color 0.1s;
        }

        .grid-table tr td:nth-child(1) {
            position: sticky;
            left: 0;
            z-index: 2;
            background-color: #fff;
            box-shadow: 3px 0 0 #e8eaed;
        }

        .grid-table tr td:nth-child(2) {
            position: sticky;
            z-index: 1;
            background-color: #fff;
            box-shadow: 3px 0 8px rgba(0,0,0,0.08);
        }

        .grid-table tr:hover td {
            background-color: #eef2f6 !important;
        }

        .grid-table tr:nth-child(even) td {
            background-color: #fafbfc;
        }

        .grid-table tr:nth-child(even):hover td {
            background-color: #eef2f6 !important;
        }

        .grid-table tr:has(th) td,
        .grid-table tr:has(th):hover td {
            background-color: transparent !important;
        }

        .search-highlight {
            background-color: yellow !important;
            color: #000 !important;
            font-weight: bold !important;
            padding: 1px 3px;
            border-radius: 2px;
        }

        .grid-table tr td input[type="button"],
        .grid-table tr td a.btn,
        .grid-table tr td .btn {
            display: inline-block;
            padding: 4px 16px;
            background: #1a73e8;
            color: #fff !important;
            border: none;
            border-radius: 14px;
            font-size: 0.8rem;
            font-weight: 500;
            cursor: pointer;
            transition: background 0.12s;
            white-space: nowrap;
            text-decoration: none;
        }

        .grid-table tr td input[type="button"]:hover,
        .grid-table tr td a.btn:hover,
        .grid-table tr td .btn:hover {
            background: #1558b0;
        }

        /* Pager styling */

        .grid-table tr.gridview-pager td,
        .grid-table tr[align="center"] td {
            background: #ffffff !important;
            text-align: center !important;
            border-top: 2px solid #e0e0e0 !important;
            padding: 14px !important;
        }

        .grid-table tr.gridview-pager a,
        .grid-table tr.gridview-pager span,
        .grid-table tr[align="center"] a,
        .grid-table tr[align="center"] span {
            display: inline-block;
            padding: 6px 12px;
            margin: 0 3px;
            border-radius: 6px;
            text-decoration: none;
            font-weight: 500;
        }

        .grid-table tr.gridview-pager span,
        .grid-table tr[align="center"] span {
            background: #7a0616;
            color: white;
        }

        .grid-table tr.gridview-pager a,
        .grid-table tr[align="center"] a {
            color: #7a0616;
            border: 1px solid #dadce0;
        }

        .sql-query-bar {
            width: 100%;
            max-width: 100%;
            padding: 12px;
            border: 1px solid #ccc;
            border-radius: 12px;
            background: #f8f9fa;

            overflow-x: auto;
            overflow-y: hidden;

            white-space: pre;
            word-break: normal;

            font-family: Consolas, monospace;
            font-size: 13px;
        }

        .copy-sql-btn {
            margin-bottom: 8px;
            padding: 6px 12px;
            border: none;
            border-radius: 6px;
            background: #7a0616;
            color: white;
            cursor: pointer;
            font-size: 12px;
        }

        .copy-sql-btn:hover {
            background: #5e0410;
        }
        
        .suggestion-box {
            position: absolute;
            top: 52px;
            left: 0;
            right: 0;
            background: white;
            border: 1px solid #ddd;
            border-radius: 12px;
            box-shadow: 0 4px 12px rgba(0,0,0,.12);
            z-index: 9999;
            display: none;
            overflow: hidden;
        }

        .suggestion-item {
            padding: 10px 14px;
            cursor: pointer;
            font-size: 14px;
        }

        .suggestion-item:hover {
            background: #f3f4f6;
        }

    </style>

    <script type="text/javascript">
// @ts-nocheck

function toggleFilterTextbox(id) {
    var box = document.getElementById(id);
    if (!box) return;
    box.style.display =
        (box.style.display === "none" || box.style.display === "")
            ? "block" : "none";
}

function toggleQueryBar() {
    var panel =
        document.getElementById(
            "sqlQueryPanel");

    panel.style.display =
        panel.style.display === "none"
            ? "block"
            : "none";
}

function toggleColumnFilter() {
    var panel = document.getElementById("columnPanel");
    if (!panel) return;
    panel.style.display =
        (panel.style.display === "none" || panel.style.display === "")
            ? "block" : "none";
}

function filterMetadataFilters() {
    var input = document.getElementById("txtMetadataSearch");
    if (!input) return;
    var filter = input.value.toLowerCase();
    var rows = document.getElementsByClassName("filter-row");
    for (var i = 0; i < rows.length; i++) {
        rows[i].style.display =
            (rows[i].innerText.toLowerCase().indexOf(filter) > -1)
                ? "" : "none";
    }
}

function selectAllColumns(selectAll) {
    var container = document.getElementById("columnPanel");
    if (!container) return;
    var checkboxes = container.querySelectorAll("input[type='checkbox']");
    for (var i = 0; i < checkboxes.length; i++) {
        checkboxes[i].checked = selectAll;
    }
}

function scrollToFocusedColumn() {
    var target = document.querySelector(".auto-focus-column");
    if (!target) return;
    target.scrollIntoView({ behavior: "smooth", inline: "center", block: "nearest" });
}

function wireScrollSync() {
    var results = document.getElementById("mainResults");
    var topTrack = document.getElementById("topScroll");
    var botTrack = document.getElementById("bottomScroll");
    var topInner = document.getElementById("topScrollContent");
    var botInner = document.getElementById("bottomScrollContent");

    if (!results) return;

    function refreshWidth() {
        var w = results.scrollWidth + "px";
        if (topInner) topInner.style.width = w;
        if (botInner) botInner.style.width = w;
    }
    refreshWidth();

    var syncing = false;

    function onResults() {
        if (syncing) return; syncing = true;
        if (topTrack) topTrack.scrollLeft = results.scrollLeft;
        if (botTrack) botTrack.scrollLeft = results.scrollLeft;
        syncing = false;
    }
    function onTop() {
        if (syncing) return; syncing = true;
        results.scrollLeft = topTrack.scrollLeft;
        if (botTrack) botTrack.scrollLeft = topTrack.scrollLeft;
        syncing = false;
    }
    function onBottom() {
        if (syncing) return; syncing = true;
        results.scrollLeft = botTrack.scrollLeft;
        if (topTrack) topTrack.scrollLeft = botTrack.scrollLeft;
        syncing = false;
    }

    results.addEventListener("scroll", onResults);
    if (topTrack) topTrack.addEventListener("scroll", onTop);
    if (botTrack) botTrack.addEventListener("scroll", onBottom);
    window.addEventListener("resize", refreshWidth);
}

function moveViewColumnFirst() {
    var table = document.querySelector(".grid-table");
    if (!table) return;

    var allRows = table.querySelectorAll("tr");
    if (!allRows.length) return;

    var viewColIndex = -1;

    for (var i = 0; i < allRows.length; i++) {
        var cells = allRows[i].querySelectorAll("th, td");
        if (allRows[i].querySelectorAll("th").length > 0) {
            for (var j = 0; j < cells.length; j++) {
                if (cells[j].innerText.trim().toLowerCase() === "view") {
                    viewColIndex = j;
                    break;
                }
            }
            break;
        }
    }

    if (viewColIndex <= 0) return;

    for (var r = 0; r < allRows.length; r++) {
        var row = allRows[r];
        var rowCells = row.querySelectorAll("th, td");
        if (rowCells.length <= viewColIndex) continue;
        var viewCell = rowCells[viewColIndex];
        var firstCell = rowCells[0];
        row.insertBefore(viewCell, firstCell);
    }
}

function pinSecondColumn() {
    var table = document.querySelector(".grid-table");
    if (!table) return;
    var firstBodyCell = table.querySelector("tr td:nth-child(1)");
    if (!firstBodyCell) return;
    var col1Width = firstBodyCell.offsetWidth;
    var col2Cells = table.querySelectorAll(
        "tr th:nth-child(2), tr td:nth-child(2)");
    for (var i = 0; i < col2Cells.length; i++) {
        col2Cells[i].style.left = col1Width + "px";
    }
}

window.onload = function () {
    wireScrollSync();
    scrollToFocusedColumn();
    moveViewColumnFirst();
    pinSecondColumn();

    var table = document.querySelector(".grid-table");
    if (!table) return;
    var allRows = table.querySelectorAll("tr");
    for (var i = 0; i < allRows.length; i++) {
        var ths = allRows[i].querySelectorAll("th");
        if (ths.length > 0) {
            allRows[i].style.backgroundColor = "#7a0616";
            for (var j = 0; j < ths.length; j++) {
                ths[j].style.backgroundColor = "#7a0616";
                ths[j].style.color = "#ffffff";
                ths[j].style.whiteSpace = "nowrap";
            }
            break;
        }
    }
};

document.addEventListener(
    "DOMContentLoaded",
    function () {

        const txt =
            document.getElementById(
                '<%= txtSearch.ClientID %>');

        const box =
            document.getElementById(
                'suggestionBox');

        txt.addEventListener(
            'keyup',
            function () {

                let q =
                    txt.value.trim();

                if (q.length < 2) {
                    box.style.display =
                        'none';
                    return;
                }

                fetch(
                    'SearchSuggestions.aspx?q=' +
                    encodeURIComponent(q))
                .then(r => r.json())
                .then(data => {

                    box.innerHTML = '';

                    if (data.length === 0) {
                        box.style.display =
                            'none';
                        return;
                    }

                    data.forEach(item => {

                        let div =
                            document.createElement(
                                'div');

                        div.className =
                            'suggestion-item';

                        div.innerText =
                            item;

                        div.onclick =
                            function () {

                                txt.value =
                                    item;

                                box.style.display =
                                    'none';
                            };

                        box.appendChild(div);
                    });

                    box.style.display =
                        'block';
                });
            });

        document.addEventListener(
            'click',
            function (e) {

                if (!box.contains(e.target)
                    &&
                    e.target !== txt)
                {
                    box.style.display =
                        'none';
                }
            });
    });

    function copySqlQuery() {

        var sql =
            document.querySelector(".sql-query-bar");

        if (!sql)
            return;

        navigator.clipboard.writeText(
            sql.innerText
        );

        alert("SQL query copied.");
    }

    </script>


    <div class="search-hero">

        <div class="search-hero-title">
            Enterprise Metadata Search Engine
        </div>

        <div class="search-row">

            <div class="search-bar-wrap">

                <asp:TextBox
                    ID="txtSearch"
                    runat="server"
                    CssClass="form-control"
                    placeholder="Search documents, metadata…">
                </asp:TextBox>

                <div id="suggestionBox"
                     class="suggestion-box">
                </div>

            </div>

            <div class="mode-wrap">
                <asp:RadioButtonList ID="rblSearchMode"
                    runat="server"
                    RepeatDirection="Vertical">
                    <asp:ListItem Text="UNION"        Value="OR"  Selected="True" />
                    <asp:ListItem Text="INTERSECTION" Value="AND" />
                </asp:RadioButtonList>
            </div>

            <asp:Button ID="btnSearch"
                runat="server"
                Text="Search"
                CssClass="btn-search-hero"
                OnClick="btnSearch_Click" />

        </div>

        <button type="button"
            class="btn-col-toggle"
            onclick="toggleColumnFilter()">
            &#9776;&nbsp; Show Column Filters
        </button>

        <button type="button"
            class="btn-col-toggle"
            onclick="toggleQueryBar()">
            Show SQL Query
        </button>

        <div id="sqlQueryPanel"
             style="display:none;margin-top:10px;">

            <button type="button"
                    class="copy-sql-btn"
                    onclick="copySqlQuery()">
                Copy SQL
            </button>

            <asp:Literal
                ID="litSqlQuery"
                runat="server" />

        </div>

        <div id="columnPanel" class="column-panel">

            <h5>Column Visibility</h5>

            <div class="column-filter-wrapper">
                <asp:CheckBoxList ID="cblColumns"
                    runat="server"
                    RepeatDirection="Horizontal"
                    RepeatLayout="Flow"
                    CssClass="checkbox-list">
                </asp:CheckBoxList>
            </div>

            <div class="column-panel-actions">
                <button type="button" class="btn btn-success"   onclick="selectAllColumns(true)">Select All</button>
                <button type="button" class="btn btn-secondary" onclick="selectAllColumns(false)">Clear All</button>
                <asp:Button ID="btnApplyColumns"
                    runat="server"
                    Text="Apply Columns"
                    CssClass="btn btn-primary"
                    OnClick="btnApplyColumns_Click" />
            </div>

        </div>

    </div>

    <div class="layout-wrapper">

        <!-- LEFT: Metadata Sidebar (populated dynamically by Page_Load) -->
        <div class="filter-sidebar">

            <h5>Metadata Filters</h5>

            <%-- Access badge: shown/hidden by code-behind via Visible property --%>
            <asp:Label ID="lblAccessBadge"
                runat="server"
                CssClass="access-badge"
                Visible="false">
                🔒 Restricted View
            </asp:Label>

            <div class="metadata-filter-search">
                <input type="text"
                    id="txtMetadataSearch"
                    placeholder="Search metadata filters…"
                    onkeyup="filterMetadataFilters()" />
            </div>


            <div class="metadata-scroll-area">
                <asp:PlaceHolder ID="phDynamicFilters" runat="server" />
            </div>

            <hr style="margin:12px 0;" />

            <h5>Dataset Filter</h5>

            <div class="metadata-scroll-area">

                <asp:RadioButtonList
                    ID="rblDatasets"
                    runat="server"
                    AutoPostBack="true"
                    RepeatDirection="Vertical"
                    RepeatLayout="Table"
                    OnSelectedIndexChanged="rblDatasets_SelectedIndexChanged">
                </asp:RadioButtonList>

            </div>

            <div class="metadata-filter-footer">
                <asp:Button ID="btnApplyFilters"
                    runat="server"
                    Text="Apply Filters"
                    CssClass="btn-apply-dark"
                    OnClick="btnApplyFilters_Click" />
            </div>

        </div><!-- /filter-sidebar -->

        <!-- RIGHT: Results -->
        <div class="main-results">
            <div class="results-card">

                <asp:Label ID="lblStatus"
                    runat="server"
                    CssClass="status-label">
                </asp:Label>

                <!-- TOP sync scrollbar -->
                <div class="scroll-track scroll-track-top" id="topScroll">
                    <div id="topScrollContent"></div>
                </div>

                <!-- Table viewport -->
                <div class="results-wrapper" id="mainResults">

                    <asp:GridView ID="gvDocuments"
                        runat="server"
                        CssClass="grid-table"
                        AutoGenerateColumns="True"
                        GridLines="Both"
                        BorderStyle="None"
                        BorderWidth="0"
                        EnableViewState="false">
                    </asp:GridView>

                </div>
                <!-- BOTTOM sync scrollbar -->
                <div class="scroll-track scroll-track-bottom" id="bottomScroll">
                    <div id="bottomScrollContent"></div>
                </div>

                <div class="custom-pager">

                    <div class="custom-pager">

                        <asp:Button
                            ID="btnFirstPage"
                            runat="server"
                            Text="<< First"
                            CssClass="btn-page"
                            OnClick="btnFirstPage_Click" />

                        <asp:Button
                            ID="btnPrevPage"
                            runat="server"
                            Text="Previous"
                            CssClass="btn-page"
                            OnClick="btnPrevPage_Click" />

                        <asp:Label
                            ID="lblPageInfo"
                            runat="server"
                            Text="Page 1 of 1"
                            CssClass="page-info" />

                        <asp:Button
                            ID="btnNextPage"
                            runat="server"
                            Text="Next"
                            CssClass="btn-page"
                            OnClick="btnNextPage_Click" />

                        <asp:Button
                            ID="btnLastPage"
                            runat="server"
                            Text="Last >>"
                            CssClass="btn-page"
                            OnClick="btnLastPage_Click" />

                    </div>

                </div>

               
            </div>
        </div><!-- /main-results -->

    </div><!-- /layout-wrapper -->

</asp:Content>
