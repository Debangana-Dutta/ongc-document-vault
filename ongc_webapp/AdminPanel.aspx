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

<!-- ACCOUNT APPROVALS -->

<div class="admin-card">

    <h4>Account Approvals</h4>

    <div style="margin-bottom:15px;">

    <asp:Button
        ID="btnShowPending"
        runat="server"
        Text="Pending Only"
        CssClass="btn-ongc-outline"
        OnClick="btnShowPending_Click" />

    <asp:Button
        ID="btnShowAllUsers"
        runat="server"
        Text="All Users"
        CssClass="btn-ongc"
        Style="margin-left:8px;"
        OnClick="btnShowAllUsers_Click" />

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

                    <asp:Button
                        ID="btnToggleStatus"
                        runat="server"
                        Text='<%# Eval("account_status").ToString() == "APPROVED"
                                    ? "Reject"
                                    : "Approve" %>'
                        CommandName="ToggleStatus"
                        CommandArgument='<%# Eval("username") %>'
                        CssClass="btn-ongc" />

                    <asp:Button
                        ID="btnResetPassword"
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

    <asp:Label
        ID="lblApprovalFeedback"
        runat="server"
        CssClass="feedback-label" />

</div>



    <!-- ══════════════════════════════════════════
         SECTION 2 – ACCESS POLICY
    ══════════════════════════════════════════ -->

    

    <div class="admin-card">

    <h4>User Access Management</h4>

    <p class="upload-hint">
        Manage dataset and metadata visibility for each user.
    </p>

    <asp:GridView
        ID="gvUserAccess"
        runat="server"
        CssClass="admin-grid"
        AutoGenerateColumns="False"
        GridLines="None"
        BorderStyle="None"
        OnRowCommand="gvUserAccess_RowCommand">

        <Columns>

            <asp:BoundField
                DataField="username"
                HeaderText="User" />

            <asp:BoundField
                DataField="role"
                HeaderText="Role" />

            <asp:BoundField
                DataField="account_status"
                HeaderText="Status" />

            <asp:BoundField
                DataField="dataset_count"
                HeaderText="Datasets Assigned" />

            <asp:TemplateField HeaderText="Manage">

                <ItemTemplate>

                    <asp:Button
                        ID="btnManage"
                        runat="server"
                        Text="Manage"
                        CommandName="ManageAccess"
                        CommandArgument='<%# Eval("id") %>'
                        CssClass="btn-ongc" />

                </ItemTemplate>

            </asp:TemplateField>

        </Columns>

    </asp:GridView>

</div>


    <!-- ══════════════════════════════════════════
         SECTION 3 – DOCUMENT INGESTION
    ══════════════════════════════════════════ -->
    <div class="admin-card">
        <h4>Document Ingestion (Excel Upload)</h4>

        <div class="form-row" style="align-items:center;">

            <div class="form-row">

                <div class="form-group">
                    <label>Dataset Name</label>

                    <asp:TextBox
                        ID="txtDatasetName"
                        runat="server"
                        placeholder="e.g. Finance, HR, Geology" />
                </div>

            </div>

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

    <asp:Panel
    ID="pnlPasswordReset"
    runat="server"
    Visible="false"
    CssClass="admin-card"
    Style="position:fixed;
           top:50%;
           left:50%;
           transform:translate(-50%,-50%);
           z-index:9999;
           width:400px;
           background:white;
           border:1px solid #ddd;
           box-shadow:0 4px 20px rgba(0,0,0,0.25);">

    <h4>Reset Password</h4>

    <asp:HiddenField
        ID="hfResetUsername"
        runat="server" />

    <div class="form-group">

        <label>New Password</label>

        <asp:TextBox
            ID="txtNewPassword"
            runat="server"
            TextMode="Password" />

    </div>

    <br />

    <asp:Button
        ID="btnSavePassword"
        runat="server"
        Text="Save Password"
        CssClass="btn-ongc"
        OnClick="btnSavePassword_Click" />

    <asp:Button
        ID="btnCancelPassword"
        runat="server"
        Text="Cancel"
        CssClass="btn-ongc-outline"
        Style="margin-left:8px;"
        OnClick="btnCancelPassword_Click" />

</asp:Panel>

</asp:Content>
