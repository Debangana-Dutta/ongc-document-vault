<%@ Page Title="Upload Document Data" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="ongc_webapp.Upload" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid px-4 mt-4">
        <h4 class="mb-4 text-secondary fw-semibold">
            <i class="fas fa-th text-danger me-2" style="color: #c02434;"></i>Upload Document Data
        </h4>
        
        <div class="card p-4 border-0 shadow-sm bg-white">
            <div class="row g-3 mb-4">
                <div class="col-md-6">
                    <label class="form-label small text-uppercase fw-bold text-muted">Company Name / Dataset</label>
                    <asp:TextBox ID="txtDepartmentTag" runat="server" CssClass="form-control shadow-none bg-light" placeholder="e.g. _Vendor_Data"></asp:TextBox>
                </div>
                <div class="col-md-6">
                    <label class="form-label small text-uppercase fw-bold text-muted">Select Document File</label>
                    <div class="input-group">
                        <asp:FileUpload ID="fileExcelPayload" runat="server" CssClass="form-control shadow-none bg-light" />
                    </div>
                </div>
            </div>

            <div class="d-flex justify-content-end gap-2 mb-3">
                <button type="button" class="btn btn-outline-secondary btn-sm px-3 fw-medium" onclick="appendColumnHandle()">
                    <i class="fas fa-plus me-1 text-success"></i>Column
                </button>
                <button type="button" class="btn btn-outline-secondary btn-sm px-3 fw-medium" onclick="removeColumnHandle()">
                    <i class="fas fa-minus me-1 text-danger"></i>
                </button>
                <button type="button" class="btn btn-outline-secondary btn-sm px-4 fw-medium" onclick="appendRowHandle()">
                    <i class="fas fa-plus me-1 text-primary"></i>Add Row
                </button>
            </div>

            <div class="table-responsive border rounded mb-4 bg-light">
                <table class="table table-bordered mb-0 small" id="interactiveBuilderGrid">
                    <thead>
                        <tr class="bg-white text-muted text-center" id="headerVectorRow">
                            <th style="width: 50px;">#</th>
                            <th>Column_1</th>
                            <th>Column_2</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr class="bg-white text-center">
                            <td>1</td>
                            <td><input type="text" class="form-control form-control-sm border-0 text-center shadow-none" /></td>
                            <td><input type="text" class="form-control form-control-sm border-0 text-center shadow-none" /></td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="mb-3">
                <asp:Label ID="lblStatusOutput" runat="server" CssClass="alert alert-success d-block text-center shadow-sm fw-semibold" style="display:none !important;" Visible="false"></asp:Label>
            </div>

            <div class="d-flex justify-content-end">
                <asp:Button ID="btnRunIngestion" runat="server" Text="Save Data" CssClass="btn btn-danger px-5 py-2 fw-bold" OnClick="btnRunIngestion_Click" style="background-color: #c02434; border: none;" />
            </div>
        </div>
    </div>

    <script type="text/javascript">
let currentColsCount = 2;
let currentRowsCount = 1;

function appendColumnHandle() {
    currentColsCount++;
    const headerRow = document.getElementById("headerVectorRow");
    if (!headerRow) return;

    const th = document.createElement("th");
    th.innerText = "Column_" + currentColsCount;
    headerRow.appendChild(th);

    const rows = document.querySelectorAll("#interactiveBuilderGrid tbody tr");
    rows.forEach(row => {
        if (row) {
            const td = document.createElement("td");
            td.innerHTML = '<input type="text" class="form-control form-control-sm border-0 text-center shadow-none" />';
            row.appendChild(td);
        }
    });
}

function removeColumnHandle() {
    if (currentColsCount <= 1) return;
    const headerRow = document.getElementById("headerVectorRow");
    if (!headerRow || !headerRow.lastChild) return;
    headerRow.removeChild(headerRow.lastChild);

    const rows = document.querySelectorAll("#interactiveBuilderGrid tbody tr");
    rows.forEach(row => {
        if (row && row.lastChild) {
            row.removeChild(row.lastChild);
        }
    });
    currentColsCount--;
}

function appendRowHandle() {
    currentRowsCount++;
    const tbody = document.querySelector("#interactiveBuilderGrid tbody");
    if (!tbody) return;

    const tr = document.createElement("tr");
    tr.className = "bg-white text-center";

    let html = `<td>${currentRowsCount}</td>`;
    for (let i = 0; i < currentColsCount; i++) {
        html += '<td><input type="text" class="form-control form-control-sm border-0 text-center shadow-none" /></td>';
    }
    tr.innerHTML = html;
    tbody.appendChild(tr);
}
</script>
</asp:Content>