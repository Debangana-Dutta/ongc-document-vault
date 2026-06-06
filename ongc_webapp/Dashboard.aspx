<%@ Page Title="System Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="ongc_webapp.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        body { background-color: #f8fafc; color: #334155; font-family: 'Segoe UI', sans-serif; }

        /* Professional Hero Banner */
        .hero-banner {
            background: linear-gradient(135deg, #7a0616 0%, #4a030c 100%);
            color: #ffffff;
            padding: 50px 40px;
            border-radius: 0 0 40px 40px;
            box-shadow: 0 10px 25px rgba(0,0,0,0.15);
            margin-bottom: 40px;
            text-align: center;
        }

        /* Interactive Pulsing Cards */
        @keyframes pulse {
            0% { transform: scale(1); }
            50% { transform: scale(1.02); }
            100% { transform: scale(1); }
        }

        .stat-card {
            background: #ffffff;
            border-radius: 14px;
            padding: 25px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
            border-top: 5px solid #e2e8f0;
            animation: pulse 3s infinite ease-in-out;
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }
        .stat-card:hover { animation: none; transform: scale(1.05); box-shadow: 0 20px 40px rgba(0,0,0,0.2); cursor: pointer; }
        .stat-value { font-size: 2.2rem; font-weight: 800; font-family: 'Segoe UI', monospace; margin-top: 10px; }

        /* Action Card & Table Styles */
        .action-card { background: #1e293b; color: #ffffff; border-radius: 14px; padding: 30px; height: 100%; }
        .table thead { background-color: #f1f5f9; }
        .table th { padding: 15px !important; font-size: 0.75rem; letter-spacing: 0.8px; color: #475569; }
        .table tbody tr:hover { background-color: #f8fafc; cursor: pointer; }
    </style>

    <!-- Interactive Hero Section -->
    <div class="hero-banner">
        <h2 class="display-5 fw-bold">Welcome Back, User</h2>
        <p id="tagline" class="mt-2 fw-medium" style="font-size: 1.2rem; opacity: 0.9; min-height: 1.5em;"></p>
    </div>

    <div class="container-fluid px-4">
        
        <!-- Dashboard Stats -->
        <div class="row g-4 mb-5">
            <div class="col-md-4">
                <div class="stat-card" style="border-top-color: #64748b;" onclick="window.location.href='Upload.aspx';">
                    <div class="text-secondary small fw-bold text-uppercase">Total Files</div>
                    <div class="stat-value text-dark"><asp:Label ID="lblTotalFiles" runat="server" Text="0"></asp:Label></div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="stat-card" style="border-top-color: #10b981;">
                    <div class="text-success small fw-bold text-uppercase">Indexed Successfully</div>
                    <div class="stat-value text-success"><asp:Label ID="lblIndexedSuccess" runat="server" Text="0"></asp:Label></div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="stat-card" style="border-top-color: #f59e0b;">
                    <div class="text-warning small fw-bold text-uppercase">Pending Indexing</div>
                    <div class="stat-value text-warning"><asp:Label ID="lblPendingIndexing" runat="server" Text="0"></asp:Label></div>
                </div>
            </div>
        </div>

        <div class="row g-4 mb-5">
            <div class="col-lg-8">
    <div class="card p-4 border-0 shadow-sm" style="border-radius: 14px;">

        <h5 class="fw-bold mb-4 text-dark">
            <i class="fas fa-users me-2 text-primary"></i>
            User Management Summary
        </h5>

        <div class="row g-3">

            <div class="col-md-4">
                <div class="stat-card" style="border-top-color:#10b981;">
                    <div class="text-success small fw-bold text-uppercase">
                        Approved Users
                    </div>
                    <div class="stat-value text-success">
                        <asp:Label ID="lblApprovedUsers"
                                   runat="server"
                                   Text="0" />
                    </div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="stat-card" style="border-top-color:#f59e0b;">
                    <div class="text-warning small fw-bold text-uppercase">
                        Pending Users
                    </div>
                    <div class="stat-value text-warning">
                        <asp:Label ID="lblPendingUsers"
                                   runat="server"
                                   Text="0" />
                    </div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="stat-card" style="border-top-color:#ef4444;">
                    <div class="text-danger small fw-bold text-uppercase">
                        Rejected Users
                    </div>
                    <div class="stat-value text-danger">
                        <asp:Label ID="lblRejectedUsers"
                                   runat="server"
                                   Text="0" />
                    </div>
                </div>
            </div>

        </div>

    </div>
</div>

            <div class="col-lg-4">
                <div class="action-card d-flex flex-column justify-content-between shadow-lg">
                    <div>
                        <h4 class="fw-bold mb-3">Quick Actions</h4>
                        <p class="text-white-50 small">Manage your enterprise data pipelines with these primary tools.</p>
                    </div>
                    <div class="d-grid gap-3">
                        <a href="Upload.aspx" class="btn btn-primary btn-lg fw-bold shadow">Upload New Batch</a>
                        <a href="Indexing.aspx" class="btn btn-outline-light btn-lg fw-bold">Search Index</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
const text = "ONGC Document Indexing & Enterprise Retrieval System";
let i = 0;
function typeWriter() {
    const taglineElement = document.getElementById("tagline");
    if (taglineElement) {
        if (i < text.length) {
            taglineElement.innerHTML += text.charAt(i);
            i++;
            setTimeout(typeWriter, 50);
        }
    }
}
window.onload = typeWriter;
</script>
</asp:Content>