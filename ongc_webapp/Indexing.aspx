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

        body {
            background-color: #f4f6f9;
        }

        .card-box {
            background: white;
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 20px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        .search-header {
            font-size: 1.3rem;
            font-weight: 700;
            color: #1e293b;
            margin-bottom: 20px;
        }

        .layout-wrapper {
            display: flex;
            gap: 20px;
            align-items: flex-start;
        }

        .filter-sidebar {
            width: 320px;
            min-width: 320px;
            max-width: 320px;
            height: 82vh;
            background: white;
            border-radius: 10px;
            padding: 18px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            position: sticky;
            top: 15px;
            display: flex;
            flex-direction: column;
        }

        .main-results {
            flex: 1;
            min-width: 0;
        }

        .filter-row {
            border-bottom: 1px solid #e2e8f0;
            padding-bottom: 12px;
            margin-bottom: 12px;
        }

        .filter-column-name {
            font-size: 0.9rem;
            font-weight: 700;
            color: #334155;
            margin-left: 5px;
        }

        .filter-input-box {
            margin-top: 10px;
            display: none;
        }

        .metadata-filter-search {
            margin-bottom: 15px;
        }

        .metadata-scroll-area {
            flex: 1;
            overflow-y: auto;
            padding-right: 5px;
        }

        .metadata-filter-footer {
            margin-top: 15px;
            padding-top: 12px;
            border-top: 1px solid #e2e8f0;
            background: white;
        }

        .sticky-horizontal-scroll {
            overflow-x: auto;
            overflow-y: hidden;
            height: 18px;
            position: sticky;
            top: 0;
            background: white;
            z-index: 20;
        }

        .sticky-horizontal-scroll div {
            height: 1px;
        }

        .results-wrapper {
            width: 100%;
            overflow-x: auto;
            overflow-y: auto;
            border: 1px solid #e2e8f0;
            border-radius: 8px;
            max-height: 78vh;
        }

        .grid-table {
            width: 100%;
            border-collapse: collapse;
            min-width: 1600px;
            background: white;
        }

        .grid-table th {
            background-color: #7a0616;
            color: white;
            padding: 12px;
            text-align: left;
            font-size: 0.85rem;
            border: 1px solid #ddd;
            white-space: nowrap;
            position: sticky;
            top: 0;
            z-index: 2;
        }

        .grid-table td {
            padding: 12px;
            border: 1px solid #ddd;
            vertical-align: top;
            word-break: break-word;
            font-size: 0.9rem;
            background-color: white;
            min-width: 180px;
        }

        .column-panel {
            margin-top: 20px;
            border-top: 1px solid #e2e8f0;
            padding-top: 15px;
            display: none;
        }

        .column-filter-wrapper {
            max-height: 180px;
            overflow-y: auto;
            overflow-x: hidden;
            border: 1px solid #e2e8f0;
            border-radius: 6px;
            padding: 10px;
        }

        .checkbox-list {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
        }

        .search-highlight {
            background-color: yellow;
            color: black;
            font-weight: bold;
            padding: 2px;
            border-radius: 2px;
        }

        

    </style>

    <script type="text/javascript">

// @ts-nocheck

function toggleFilterTextbox(id) {

    var box =
        document.getElementById(id);

    if (!box)
        return;

    if (
        box.style.display === "none" ||
        box.style.display === ""
    ) {

        box.style.display = "block";
    }
    else {

        box.style.display = "none";
    }
}

function toggleColumnFilter() {

    var panel =
        document.getElementById("columnPanel");

    if (!panel)
        return;

    if (
        panel.style.display === "none" ||
        panel.style.display === ""
    ) {

        panel.style.display = "block";
    }
    else {

        panel.style.display = "none";
    }
}

function filterMetadataFilters() {

    var input =
        document.getElementById("txtMetadataSearch");

    if (!input)
        return;

    var filter =
        input.value.toLowerCase();

    var rows =
        document.getElementsByClassName("filter-row");

    for (var i = 0; i < rows.length; i++) {

        var row = rows[i];

        var text =
            row.innerText.toLowerCase();

        if (text.indexOf(filter) > -1) {

            row.style.display = "";
        }
        else {

            row.style.display = "none";
        }
    }
}

function syncHorizontalScroll(source, targetId) {

    var target =
        document.getElementById(targetId);

    if (!target)
        return;

    target.scrollLeft =
        source.scrollLeft;
}

function selectAllColumns(selectAll) {

    var container =
        document.getElementById("columnPanel");

    if (!container)
        return;

    var checkboxes =
        container.querySelectorAll(
            "input[type='checkbox']"
        );

    for (var i = 0; i < checkboxes.length; i++) {

        checkboxes[i].checked =
            selectAll;
    }
}

function scrollToFocusedColumn() {

    var target =
        document.querySelector(
            ".auto-focus-column");

    if (!target)
        return;

    target.scrollIntoView({
        behavior: "smooth",
        inline: "center",
        block: "nearest"
    });
}

window.onload = function () {

    var results =
        document.getElementById("mainResults");

    var topScroll =
        document.getElementById("topScroll");

    var topScrollContent =
        document.getElementById("topScrollContent");

    if (
        results &&
        topScroll &&
        topScrollContent
    ) {
        topScrollContent.style.width =
            results.scrollWidth + "px";

        topScroll.onscroll = function () {

            results.scrollLeft =
                topScroll.scrollLeft;
        };
    }

    scrollToFocusedColumn();
};

</script>

    <div class="container-fluid mt-4">

        <div class="card-box">

            <div class="search-header">
                Enterprise Metadata Search Engine
            </div>

            <div class="row">

                <div class="col-md-8">

                    <label>
                        Keyword Search
                    </label>

                    <asp:TextBox ID="txtSearch"
                        runat="server"
                        CssClass="form-control">
                    </asp:TextBox>

                </div>

                <div class="col-md-2">

                    <label>
                        Search Mode
                    </label>

                    <asp:RadioButtonList ID="rblSearchMode"
                        runat="server">

                        <asp:ListItem Text="UNION"
                            Value="OR"
                            Selected="True">
                        </asp:ListItem>

                        <asp:ListItem Text="INTERSECTION"
                            Value="AND">
                        </asp:ListItem>

                    </asp:RadioButtonList>

                </div>

                <div class="col-md-2 d-flex align-items-end">

                    <asp:Button ID="btnSearch"
                        runat="server"
                        Text="Search"
                        CssClass="btn btn-danger w-100"
                        OnClick="btnSearch_Click" />

                </div>

            </div>

            <button type="button"
                class="btn btn-secondary mt-4"
                onclick="toggleColumnFilter()">

                Show Column Filters

            </button>

            <div id="columnPanel"
                class="column-panel">

                <h5>
                    Column Visibility
                </h5>

                <div class="column-filter-wrapper">

                    <asp:CheckBoxList ID="cblColumns"
                        runat="server"
                        RepeatDirection="Horizontal"
                        RepeatLayout="Flow"
                        CssClass="checkbox-list">
                    </asp:CheckBoxList>

                </div>

                <br />

                <div class="row">

                    <div class="col-md-4">

                        <button type="button"
                            class="btn btn-success w-100"
                            onclick="selectAllColumns(true)">

                            Select All

                        </button>

                    </div>

                    <div class="col-md-4">

                        <button type="button"
                            class="btn btn-secondary w-100"
                            onclick="selectAllColumns(false)">

                            Clear All

                        </button>

                    </div>

                    <div class="col-md-4">

                        <asp:Button ID="btnApplyColumns"
                            runat="server"
                            Text="Apply Columns"
                            CssClass="btn btn-primary w-100"
                            OnClick="btnApplyColumns_Click" />

                    </div>

                </div>

            </div>

        </div>

        <div class="layout-wrapper">

            <div class="filter-sidebar">

                <h5>
                    Metadata Filters
                </h5>

                <div class="metadata-filter-search">

                    <input type="text"
                        id="txtMetadataSearch"
                        class="form-control"
                        placeholder="Search metadata filters..."
                        onkeyup="filterMetadataFilters()" />

                </div>

                <div class="metadata-scroll-area">

                    <asp:PlaceHolder ID="phDynamicFilters"
                        runat="server">
                    </asp:PlaceHolder>

                </div>

                <div class="metadata-filter-footer">

                    <asp:Button ID="btnApplyFilters"
                        runat="server"
                        Text="Apply Metadata Filters"
                        CssClass="btn btn-dark w-100"
                        OnClick="btnApplyFilters_Click" />

                </div>

            </div>

            <div class="main-results">

                <div class="card-box">

                    <asp:Label ID="lblStatus"
                        runat="server"
                        ForeColor="Green"
                        Font-Bold="true">
                    </asp:Label>

                    <br />
                    <br />

                    <div class="sticky-horizontal-scroll"
                        id="topScroll">

                        <div id="topScrollContent"></div>

                    </div>

                    <div class="results-wrapper"
                        id="mainResults"
                        onscroll="syncHorizontalScroll(this, 'topScroll')">

                        <asp:GridView ID="gvDocuments"
                            runat="server"
                            CssClass="grid-table"
                            AutoGenerateColumns="True"
                            GridLines="Both"
                            BorderStyle="None"
                            BorderWidth="0"
                            EnableViewState="false"
                            AllowPaging="true"
                            PageSize="100"
                            OnPageIndexChanging="gvDocuments_PageIndexChanging">

                        </asp:GridView>

                    </div>

                </div>

            </div>

        </div>

    </div>

</asp:Content>