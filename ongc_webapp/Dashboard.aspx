<%@ Page Title="System Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="ongc_webapp.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* Core Dashboard Layout Adjustments */
        body { background-color: #f4f6f9; color: #334155; }
        
        /* Streamlined Professional Welcome Strip */
        .welcome-identity-strip { 
            background: linear-gradient(135deg, #7a0616 0%, #4a030c 100%); 
            color: #ffffff; 
            padding: 24px 35px; 
            border-radius: 0 0 12px 12px; 
            box-shadow: 0 4px 15px rgba(0,0,0,0.06); 
            margin-bottom: 35px; 
        }

        /* Existing Metric & Action Cards Sync */
        .stat-card { background: #ffffff; border-radius: 8px; border: 1px solid #e2e8f0; padding: 20px; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05); transition: transform 0.2s; }
        .stat-card:hover { transform: translateY(-2px); }
        .action-card { background: #1e293b; color: #ffffff; border-radius: 8px; padding: 25px; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05); }
        
        /* Scrollable Section Header Utilities */
        .section-division-title { 
            font-size: 1.15rem; 
            font-weight: 700; 
            color: #1e293b; 
            text-transform: uppercase; 
            letter-spacing: 0.5px; 
            border-left: 5px solid #7a0616; 
            padding-left: 12px; 
            margin-bottom: 20px; 
        }
        
        /* Grid Alignment Layouts for Compliance Cards */
        .compliance-resource-card { background: #ffffff; border: 1px solid #e2e8f0; border-radius: 8px; padding: 25px; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.03); border-top: 4px solid #7a0616; height: 100%; transition: all 0.2s; }
        .compliance-resource-card:hover { transform: translateY(-2px); box-shadow: 0 10px 15px -3px rgba(0,0,0,0.05); }
        
        /* Status Badges */
        .badge-success { background-color: #10b981; color: white; padding: 5px 10px; border-radius: 4px; font-size: 0.8rem; text-transform: uppercase; }
        .badge-warning { background-color: #f59e0b; color: white; padding: 5px 10px; border-radius: 4px; font-size: 0.8rem; text-transform: uppercase; }
        .table th { font-size: 0.8rem; letter-spacing: 0.5px; color: #64748b; }
        .table td { font-size: 0.95rem; color: #334155; }

        /* Premium Corporate Footer Implementation */
        .ongc-portal-footer { background-color: #4a030c; color: #f1f5f9; border-top: 4px solid #7a0616; padding: 45px 0 20px 0; margin-top: 55px; }
        .footer-col-header { font-size: 0.85rem; font-weight: 700; text-transform: uppercase; color: #fcd34d; letter-spacing: 1px; margin-bottom: 18px; }
        .footer-nav-links { list-style: none; padding: 0; margin: 0; }
        .footer-nav-links li { margin-bottom: 10px; }
        .footer-nav-links a { color: #cbd5e1; text-decoration: none; font-size: 0.9rem; transition: color 0.15s; }
        .footer-nav-links a:hover { color: #ffffff; }
        
        /* Updated Social Icon Vector Placement Rules */
        .footer-social-box a { color: #fcd34d !important; font-size: 1.4rem; margin-right: 22px; text-decoration: none; display: inline-block; transition: all 0.2s ease; }
        .footer-social-box a:hover { color: #ffffff !important; transform: scale(1.15); }
        .footer-copyright-bar { border-top: 1px solid #7a0616; padding-top: 20px; margin-top: 40px; font-size: 0.8rem; color: #94a3b8; }
    </style>

    <div class="welcome-identity-strip">
        <div class="row align-items-center">
            <div class="col-md-12">
                <h2 class="fw-bold m-0">Welcome Back, <asp:Label ID="lblUserName" runat="server" Text="User"></asp:Label></h2>
            </div>
        </div>
    </div>

    <div class="container-fluid px-4">
        
        <div class="row g-4 mb-4">
            <div class="col-md-3">
                <div class="stat-card text-center" style="border-left: 5px solid #64748b;">
                    <span class="text-muted text-uppercase fw-bold small" style="font-size: 0.75rem; letter-spacing: 0.5px;">Total Files</span>
                    <h2 class="fw-bold mt-2 text-dark"><asp:Label ID="lblTotalFiles" runat="server" Text="0"></asp:Label></h2>
                </div>
            </div>
            <div class="col-md-3">
                <div class="stat-card text-center" style="border-left: 5px solid #10b981;">
                    <span class="text-uppercase fw-bold small" style="color: #10b981 !important; font-size: 0.75rem; letter-spacing: 0.5px;">Indexed Successfully</span>
                    <h2 class="fw-bold mt-2" style="color: #10b981;"><asp:Label ID="lblIndexedSuccess" runat="server" Text="0"></asp:Label></h2>
                </div>
            </div>
            <div class="col-md-3">
                <div class="stat-card text-center" style="border-left: 5px solid #f59e0b;">
                    <span class="text-uppercase fw-bold small" style="color: #f59e0b !important; font-size: 0.75rem; letter-spacing: 0.5px;">Pending Indexing</span>
                    <h2 class="fw-bold mt-2" style="color: #f59e0b;"><asp:Label ID="lblPendingIndexing" runat="server" Text="0"></asp:Label></h2>
                </div>
            </div>
            <div class="col-md-3">
                <div class="stat-card text-center" style="border-left: 5px solid #ef4444;">
                    <span class="text-uppercase fw-bold small" style="color: #ef4444 !important; font-size: 0.75rem; letter-spacing: 0.5px;">System Alerts</span>
                    <h2 class="fw-bold mt-2" style="color: #ef4444;">02</h2>
                </div>
            </div>
        </div>

        <div class="row g-4 mb-5">
            <div class="col-lg-8">
                <div class="card p-4 border-0 shadow-sm bg-white" style="border-radius: 8px; height: 100%;">
                    <h5 class="fw-bold mb-4 text-dark"><i class="fas fa-list-alt text-secondary me-2"></i>Recent Indexing Logs</h5>
                    <div class="table-responsive">
                        <table class="table table-hover align-middle mb-0">
                            <head class="table-light text-uppercase">
                                <tr>
                                    <th>File ID</th>
                                    <th>Filename</th>
                                    <th>Status</th>
                                    <th>Timestamp</th>
                                </tr>
                            </head>
                            <tbody>
                                <asp:Repeater ID="rptRecentLogs" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="text-secondary font-monospace fw-bold"><%# Eval("index_id") %></td>
                                            <td class="fw-bold text-dark"><%# Eval("file_name") %></td>
                                            <td>
                                                <span class='<%# Eval("file_status").ToString() == "Success" ? "badge-success" : "badge-warning" %>'>
                                                    <%# Eval("file_status") %>
                                                </span>
                                            </td>
                                            <td class="text-muted">
                                                <%# Convert.ToDateTime(Eval("upload_date")).ToString("dd-MMM-yyyy") %> <%# Eval("upload_time") %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            <div class="col-lg-4">
                <div class="action-card h-100 d-flex flex-column justify-content-between">
                    <div>
                        <h5 class="fw-bold mb-2">Quick Actions</h5>
                        <p class="text-white-50 small mb-4">Direct execution triggers mapped across repository clusters.</p>
                    </div>
                    <div class="d-grid gap-3">
                        <a href="Indexing.aspx" class="btn btn-primary fw-bold py-2.5 shadow-sm">Upload New Batch</a>
                        <a href="Indexing.aspx" class="btn btn-outline-light fw-bold py-2.5">Search Index</a>
                        <button type="button" class="btn btn-outline-light fw-bold py-2.5">Export Logs</button>
                    </div>
                </div>
            </div>
        </div>

        <div class="section-division-title">Corporate Compliance & Guidelines Hub (Public Resources Concepts)</div>
        <div class="row g-4">
            <div class="col-md-4">
                <div class="compliance-resource-card">
                    <h5 class="fw-bold text-dark mb-2"><i class="fas fa-shield-alt text-danger me-2"></i>HSE Safety Protocols</h5>
                    <p class="text-muted small mb-0">Conceptual links to standard offshore engineering safety manuals, refinery compliance files, and operational checklists.</p>
                    <button type="button" class="btn btn-sm btn-outline-secondary mt-4">View Conceptual SOPs</button>
                </div>
            </div>
            <div class="col-md-4">
                <div class="compliance-resource-card">
                    <h5 class="fw-bold text-dark mb-2"><i class="fas fa-file-invoice text-warning me-2"></i>Conceptual Active Tenders</h5>
                    <p class="text-muted small mb-0">Simulated vendor registration workflows and formatting standard operating procedures for IT assets and logistics solutions.</p>
                    <button type="button" class="btn btn-sm btn-outline-secondary mt-4">View Conceptual Notices</button>
                </div>
            </div>
            <div class="col-md-4">
                <div class="compliance-resource-card">
                    <h5 class="fw-bold text-dark mb-2"><i class="fas fa-users text-success me-2"></i>ONGC Conceptual CSR</h5>
                    <p class="text-muted small mb-0">Mock information regarding regional community welfare, skill training projects, and environmental initiatives led by the ONGC Foundation.</p>
                    <button type="button" class="btn btn-sm btn-outline-secondary mt-4">Explore Conceptual CSR</button>
                </div>
            </div>
        </div>
    </div>

    <footer class="ongc-portal-footer">
        <div class="container-fluid px-5">
            <div class="row g-4">
                <div class="col-md-3">
                    <div class="footer-col-header">Legal & Compliance</div>
                    <ul class="footer-nav-links">
                        <li><a href="#">Privacy Policy</a></li>
                        <li><a href="#">Data Disclaimer</a></li>
                        <li><a href="#">Terms of Portal Use</a></li>
                        <li><a href="#">Site Map</a></li>
                    </ul>
                </div>
                <div class="col-md-3">
                    <div class="footer-col-header">IT System Support</div>
                    <ul class="footer-nav-links">
                        <li><a href="#">Contact Helpdesk</a></li>
                        <li><a href="#">Data Security FAQ</a></li>
                        <li><a href="#">Classification Policy</a></li>
                        <li><a href="#">System Alert Logs</a></li>
                    </ul>
                </div>
                <div class="col-md-3">
                    <div class="footer-col-header">Operational Assets</div>
                    <ul class="footer-nav-links">
                        <li><a href="#">Northern Region Assets</a></li>
                        <li><a href="#">Eastern Region Assets</a></li>
                        <li><a href="#">Assam Asset Hub</a></li>
                        <li><a href="#">Southern Marine Basin</a></li>
                    </ul>
                </div>
                <div class="col-md-3">
                    <div class="footer-col-header">Stay Connected</div>
                    <p class="small text-white-50 mb-3">Access official external communications handles:</p>
                    <div class="footer-social-box">
                        <a href="https://in.linkedin.com/company/oilandnaturalgascorporation" target="_blank" title="ONGC Official LinkedIn"><i class="fab fa-linkedin"></i></a>
                        <a href="https://x.com/ONGC_" target="_blank" title="ONGC Official X (Twitter)"><i class="fab fa-twitter"></i></a>
                        <a href="https://www.instagram.com/ongcofficial/" target="_blank" title="ONGC Official Instagram"><i class="fab fa-instagram"></i></a>
                        <a href="https://www.youtube.com/channel/UCwNb8itlkVOKkk3yuZtBsvg" target="_blank" title="ONGC Official YouTube"><i class="fab fa-youtube"></i></a>
                    </div>
                </div>
            </div>
            
            <div class="footer-copyright-bar d-flex flex-column flex-md-row justify-content-between align-items-center">
                <p class="mb-0">© 2026 Powered by ONGC Foundation & Systems Architecture Suite. All data fields shown are simulated for operational presentation context.</p>
                <p class="mb-0 mt-2 mt-md-0 fw-bold text-light">Last Refreshed Sync: 20-May-2026 02:00 PM IST</p>
            </div>
        </div>
    </footer>
</asp:Content>