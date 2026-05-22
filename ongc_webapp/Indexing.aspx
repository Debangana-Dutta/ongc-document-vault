<%@ Page Title="Enterprise Metadata Search" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Indexing.aspx.cs" Inherits="ongc_webapp.Indexing" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid px-4 mt-4">
        <h4 class="mb-3 text-secondary fw-semibold">Enterprise Metadata Search</h4>
        
        <div class="card shadow-sm border-0 p-3 mb-4 bg-light">
            <small class="text-uppercase fw-bold text-muted small d-block mb-2">Search Documents</small>
            <div class="d-flex gap-2">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control py-2 shadow-none" placeholder="Search file name, path, metadata values..."></asp:TextBox>
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-danger px-5 fw-semibold" OnClick="btnSearch_Click" style="background-color: #c02434; border: none;" />
            </div>
        </div>

        <div class="mb-2">
            <span class="text-success fw-bold small"><asp:Literal ID="litMatchCount" runat="server">0</asp:Literal> result(s) found.</span>
        </div>

        <asp:Panel ID="pnlResultsSummary" runat="server" CssClass="table-responsive bg-white rounded shadow-sm border">
            <asp:PlaceHolder ID="phResultsContainer" runat="server"></asp:PlaceHolder>
        </asp:Panel>

        <asp:Panel ID="pnlEmptyState" runat="server" CssClass="text-center py-5 bg-white border rounded shadow-sm" Visible="false">
            <i class="fas fa-database text-muted fa-3x mb-3"></i>
            <h5 class="text-secondary">No matching document indexes found</h5>
            <p class="text-muted small">Try refining your keyword phrase or verify your data pipeline connection hooks.</p>
        </asp:Panel>
    </div>
</asp:Content>