<%@ Page Title="Admin Panel"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="AdminPanel.aspx.cs"
    Inherits="ongc_webapp.AdminPanel" %>

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

    /* ── Page wrapper ── */
    .admin-page {
        max-width: 1100px;
        margin: 0 auto;
        padding: 28px 24px 48px;
    }

    .admin-page-title {
        font-family: 'Google Sans', sans-serif;
        font-size: 1.6rem;
        font-weight: 600;
        color: #7a0616;
        margin-bottom: 28px;
        letter-spacing: -0.2px;
    }

    /* ── Section cards ── */
    .admin-card {
        background: #fff;
        border-radius: 12px;
        padding: 24px 28px;
        box-shadow: 0 1px 6px rgba(0,0,0,0.09);
        margin-bottom: 28px;
    }

    .admin-card h4 {
        font-family: 'Google Sans', sans-serif;
        font-size: 1.05rem;
        font-weight: 600;
        color: #3c4043;
        margin: 0 0 18px;
        padding-bottom: 10px;
        border-bottom: 2px solid #f1f3f4;
    }

    /* ── Form rows ── */
    .form-row {
        display: flex;
        flex-wrap: wrap;
        gap: 14px;
        align-items: flex-end;
        margin-bottom: 16px;
    }

    .form-group {
        display: flex;
        flex-direction: column;
        flex: 1;
        min-width: 160px;
    }

    .form-group label {
        font-size: 0.78rem;
        font-weight: 500;
        color: #5f6368;
        margin-bottom: 5px;
        text-transform: uppercase;
        letter-spacing: 0.4px;
    }

    .form-group input[type="text"],
    .form-group select {
        height: 40px;
        border: 1.5px solid #dfe1e5;
        border-radius: 8px;
        padding: 0 12px;
        font-size: 0.875rem;
        color: #202124;
        background: #fff;
        transition: border-color 0.15s, box-shadow 0.15s;
        outline: none;
    }

    .form-group input[type="text"]:focus,
    .form-group select:focus {
        border-color: #7a0616;
        box-shadow: 0 0 0 3px rgba(122,6,22,0.08);
    }

    /* ── Primary ONGC button ── */
    .btn-ongc {
        height: 40px;
        padding: 0 24px;
        background: #7a0616;
        color: #fff;
        border: none;
        border-radius: 8px;
        font-family: 'Google Sans', sans-serif;
        font-size: 0.875rem;
        font-weight: 500;
        cursor: pointer;
        transition: background 0.14s, box-shadow 0.14s;
        white-space: nowrap;
    }

    .btn-ongc:hover {
        background: #5e0410;
        box-shadow: 0 2px 8px rgba(0,0,0,0.22);
    }

    .btn-ongc-outline {
        height: 40px;
        padding: 0 24px;
        background: transparent;
        color: #7a0616;
        border: 1.5px solid #7a0616;
        border-radius: 8px;
        font-family: 'Google Sans', sans-serif;
        font-size: 0.875rem;
        font-weight: 500;
        cursor: pointer;
        transition: background 0.14s;
        white-space: nowrap;
    }

    .btn-ongc-outline:hover { background: #fdf3f5; }

    /* ── GridView ── */
    .admin-grid {
        width: 100%;
        border-collapse: collapse;
        font-size: 0.875rem;
    }

    .admin-grid tr th {
        background: #7a0616;
        color: #fff;
        padding: 10px 16px;
        text-align: left;
        font-family: 'Google Sans', sans-serif;
        font-weight: 500;
        font-size: 0.8rem;
        letter-spacing: 0.3px;
    }

    .admin-grid tr td {
        padding: 10px 16px;
        border-bottom: 1px solid #f1f3f4;
        color: #3c4043;
    }

    .admin-grid tr:hover td { background: #fdf3f5; }
    .admin-grid tr:nth-child(even) td { background: #fafbfc; }
    .admin-grid tr:nth-child(even):hover td { background: #fdf3f5; }

    /* ── Policy panel: two columns ── */
    .policy-cols {
        display: flex;
        gap: 28px;
        flex-wrap: wrap;
    }

    .policy-col {
        flex: 1;
        min-width: 220px;
    }

    .policy-col h5 {
        font-size: 0.82rem;
        font-weight: 600;
        text-transform: uppercase;
        letter-spacing: 0.5px;
        color: #5f6368;
        margin-bottom: 10px;
    }

    .checkbox-scroll-box {
        border: 1.5px solid #dfe1e5;
        border-radius: 8px;
        padding: 12px;
        max-height: 240px;
        overflow-y: auto;
        background: #fafbfc;
    }

    /* CheckBoxList items */
    .cbl-policy span,
    .cbl-policy label {
        font-size: 0.875rem;
        color: #3c4043;
    }

    .cbl-policy td { padding: 3px 0; }

    /* ── Feedback labels ── */
    .feedback-label {
        display: block;
        font-size: 0.85rem;
        font-weight: 500;
        margin-top: 10px;
        min-height: 20px;
    }

    /* ── Upload area ── */
    .upload-hint {
        font-size: 0.8rem;
        color: #80868b;
        margin-top: 6px;
    }

    /* ── User selector ── */
    .user-select-wrap {
        max-width: 380px;
        margin-bottom: 20px;
    }

    .user-select-wrap select {
        width: 100%;
        height: 40px;
        border: 1.5px solid #dfe1e5;
        border-radius: 8px;
        padding: 0 12px;
        font-size: 0.875rem;
        color: #202124;
        background: #fff;
        outline: none;
        cursor: pointer;
        transition: border-color 0.15s;
    }

    .user-select-wrap select:focus {
        border-color: #7a0616;
        box-shadow: 0 0 0 3px rgba(122,6,22,0.08);
    }

    .section-divider {
        height: 1px;
        background: #f1f3f4;
        margin: 20px 0;
    }
</style>

<div class="admin-page">

    <div class="admin-page-title">ONGC Admin Panel</div>

    <!-- ══════════════════════════════════════════
     ACCOUNT APPROVALS
══════════════════════════════════════════ -->
<div class="admin-card">
    <h4>Pending Account Approvals</h4>

    <div style="margin-bottom:15px;">
        <asp:Button ID="btnShowPending"
            runat="server"
            Text="Show Pending"
            CssClass="btn-ongc"
            OnClick="btnShowPending_Click" />

        <asp:Button ID="btnShowAllUsers"
            runat="server"
            Text="Show All Users"
            CssClass="btn-ongc-outline"
            OnClick="btnShowAllUsers_Click"
            Style="margin-left:10px;" />
    </div>

<asp:GridView ID="gvPendingUsers"
    runat="server"
    CssClass="admin-grid"
    AutoGenerateColumns="False"
    GridLines="None"
    BorderStyle="None"
    OnRowCommand="gvPendingUsers_RowCommand">

    <Columns>

            <asp:BoundField
                DataField="username"
                HeaderText="Username" />

            <asp:BoundField
                DataField="role"
                HeaderText="Role" />

            <asp:BoundField
                DataField="department"
                HeaderText="Department" />

            <asp:BoundField
                DataField="account_status"
                HeaderText="Status" />

            <asp:TemplateField HeaderText="Action">
                <ItemTemplate>

                    <asp:Button ID="btnApprove"
                        runat="server"
                        Text="Approve"
                        CommandName="Approve"
                        CommandArgument='<%# Eval("username") %>'
                        CssClass="btn-ongc" />

                    <asp:Button ID="btnReject"
                        runat="server"
                        Text="Reject"
                        CommandName="Reject"
                        CommandArgument='<%# Eval("username") %>'
                        CssClass="btn-ongc-outline"
                        Style="margin-left:6px;" />

                    <asp:Button ID="btnResetPassword"
                        runat="server"
                        Text="Reset Password"
                        CommandName="ResetPassword"
                        CommandArgument='<%# Eval("username") %>'
                        CssClass="btn-ongc-outline"
                        Style="margin-left:6px;" />

                </ItemTemplate>
            </asp:TemplateField>

        </Columns>

    </asp:GridView>

    <asp:Label ID="lblApprovalFeedback"
        runat="server"
        CssClass="feedback-label" />
</div>

    <!-- ══════════════════════════════════════════
         SECTION 1 – USER MANAGEMENT
    ══════════════════════════════════════════ -->
    <div class="admin-card">
        <h4>User Management</h4>

        <div class="form-row">
            <div class="form-group">
                <label>Full Name</label>
                <asp:TextBox ID="txtUserName" runat="server"
                    placeholder="e.g. Rajesh Kumar" />
            </div>
            <div class="form-group">
                <label>CPF / Employee ID</label>
                <asp:TextBox ID="txtCPF" runat="server"
                    placeholder="e.g. EMP-001" />
            </div>
            <div class="form-group">
                <label>Department</label>
                <asp:TextBox ID="txtDept" runat="server"
                    placeholder="e.g. Drilling" />
            </div>
            <asp:Button ID="btnAddUser" runat="server"
                Text="Add User"
                CssClass="btn-ongc"
                OnClick="btnAddUser_Click" />
        </div>

        <asp:Label ID="lblAdminFeedback" runat="server"
            CssClass="feedback-label" />

        <asp:GridView ID="gvUsers" runat="server"
            CssClass="admin-grid"
            AutoGenerateColumns="True"
            GridLines="None"
            BorderStyle="None" />
    </div>

    <!-- ══════════════════════════════════════════
         SECTION 2 – ACCESS POLICY
    ══════════════════════════════════════════ -->
    <div class="admin-card">
        <h4>Dataset &amp; Metadata Access Policy</h4>

        <p style="font-size:0.875rem;color:#5f6368;margin-bottom:16px;">
            Select a user, then choose which <strong>Datasets</strong> they can search
            and which <strong>Metadata columns</strong> appear in their sidebar.
        </p>

        <!-- User selector (AutoPostBack to pre-load existing policy) -->
    

        <div class="policy-cols">

            <!-- LEFT: Dataset access -->
            
                <p class="upload-hint" style="margin-top:6px;">
                    Ticked datasets will appear in this user's search results.
                </p>
            </div>

            <!-- RIGHT: Metadata column visibility -->
            <div class="policy-col">
                <h5>Visible Metadata Columns</h5>
                <div class="checkbox-scroll-box">
                    <asp:CheckBoxList ID="cblMetadataColumns" runat="server"
                        RepeatDirection="Vertical"
                        RepeatLayout="Table"
                        CssClass="cbl-policy" />
                </div>
                <p class="upload-hint" style="margin-top:6px;">
                    Only ticked columns will show in this user's sidebar filters.
                    Select all = unrestricted.
                </p>
            </div>

        </div><!-- /policy-cols -->

        <div class="section-divider"></div>

        <div style="display:flex;align-items:center;gap:16px;flex-wrap:wrap;">
            <asp:Button ID="btnSaveAccessPolicy" runat="server"
                Text="Save Access Policy"
                CssClass="btn-ongc"
                OnClick="btnSaveAccessPolicy_Click" />
        </div>

        <asp:Label ID="lblPolicyFeedback" runat="server"
            CssClass="feedback-label" />
    </div>

    <!-- ══════════════════════════════════════════
         SECTION 3 – DOCUMENT INGESTION
    ══════════════════════════════════════════ -->
    <div class="admin-card">
        <h4>Document Ingestion (Excel Upload)</h4>

        <div class="form-row" style="align-items:center;">
            <asp:FileUpload ID="filePayload" runat="server"
                AllowMultiple="true"
                style="flex:1;" />
            <asp:Button ID="btnIngestData" runat="server"
                Text="Upload &amp; Ingest"
                CssClass="btn-ongc"
                OnClick="btnIngestData_Click" />
        </div>

        <p class="upload-hint">
            Accepted: .xlsx files. First row = headers. Required columns:
            <code>file_name</code>, <code>file_path</code>.
            All other columns become searchable metadata.
        </p>

        <asp:Label ID="lblStatusFeedback" runat="server"
            CssClass="feedback-label" />
    </div>

</div><!-- /admin-page -->

</asp:Content>
