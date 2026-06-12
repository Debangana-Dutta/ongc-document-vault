<%@ Page Title="System Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="ongc_webapp.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <style>
        body { background-color: #f4f6f8; color: #2d3436; font-family: 'Segoe UI', Arial, sans-serif; }
        
        /* Corporate Hero Banner */
        .hero-banner { background: #7a0616; color: #ffffff; padding: 40px; border-radius: 0 0 10px 10px; margin-bottom: 30px; }
        
        /* Industrial Stat Cards */
        .stat-card { background: #ffffff; padding: 20px; border-left: 4px solid #7a0616; border-radius: 4px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); height: 100%; }
        .stat-label { font-size: 0.75rem; text-transform: uppercase; letter-spacing: 1px; color: #636e72; font-weight: 600; margin-bottom: 5px; }
        .stat-value { font-size: 1.75rem; font-weight: 700; color: #2d3436; }

    </style>

    <div class="hero-banner">
        <h2 class="fw-bold">Welcome Back, User</h2>
        <p class="mb-0" style="opacity: 0.85;">ONGC Document Indexing & Enterprise Retrieval System</p>
    </div>

    <div class="container-fluid px-4">
        <div class="row g-3 mb-4">
            <div class="col-md-4"><div class="stat-card"><div class="stat-label">Total Files</div><div class="stat-value"><asp:Label ID="lblTotalFiles" runat="server" Text="53,978" /></div></div></div>
            <div class="col-md-4"><div class="stat-card"><div class="stat-label">Indexed Successfully</div><div class="stat-value"><asp:Label ID="lblIndexedSuccess" runat="server" Text="53,978" /></div></div></div>
            <div class="col-md-4"><div class="stat-card"><div class="stat-label">Pending Indexing</div><div class="stat-value"><asp:Label ID="lblPendingIndexing" runat="server" Text="0" /></div></div></div>
        </div>

        <div class="card p-4 border-0 shadow-sm mb-5" style="border-radius: 4px;">
            <h6 class="text-uppercase fw-bold mb-4" style="color: #7a0616;">User Management Summary</h6>
            <table class="table table-bordered table-hover">
                <thead class="table-light"><tr><th>STATUS</th><th>COUNT</th></tr></thead>
                <tbody>
                    <tr><td>Approved</td><td><strong><asp:Label ID="lblApprovedUsers" runat="server" Text="4" /></strong></td></tr>
                    <tr><td>Pending</td><td><strong><asp:Label ID="lblPendingUsers" runat="server" Text="3" /></strong></td></tr>
                    <tr><td>Rejected</td><td><strong><asp:Label ID="lblRejectedUsers" runat="server" Text="0" /></strong></td></tr>
                </tbody>
            </table>
        </div>
    </div>
</asp:Content>