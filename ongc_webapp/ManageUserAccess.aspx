<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="ManageUserAccess.aspx.cs"
    Inherits="ongc_webapp.ManageUserAccess" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Manage User Access</title>

    <style>

        body {
            background-color: #f4f6f8;
            font-family: 'Public Sans', Arial, sans-serif;
            margin: 0;
            padding: 0;
        }

        .page-container {
            max-width: 1400px;
            margin: 25px auto;
            padding: 0 20px;
        }

        .hero-banner {
            background: #7a0616;
            color: white;
            padding: 35px;
            border-radius: 0 0 10px 10px;
            margin-bottom: 25px;
        }

        .hero-title {
            font-size: 2rem;
            font-weight: 700;
            margin-bottom: 8px;
        }

        .hero-subtitle {
            opacity: 0.85;
        }

        .card {
            background: #ffffff;
            border: none;
            border-radius: 6px;
            padding: 25px;
            margin-bottom: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        h2 {
            color: #7a0616;
            margin-top: 0;
            font-size: 1.7rem;
            font-weight: 700;
        }

        h3 {
            color: #7a0616;
            margin-top: 0;
            font-size: 1.3rem;
            font-weight: 700;
        }

        .section-title {
            font-weight: 600;
            color: #374151;
            margin-bottom: 15px;
        }

        .checkbox-box {
            border: 1px solid #e5e7eb;
            background: #fafafa;
            border-radius: 6px;
            padding: 15px;
            max-height: 400px;
            overflow-y: auto;
        }

        .checkbox-box table {
            width: 100%;
        }

        .checkbox-box input[type=checkbox] {
            margin-right: 8px;
        }

        .button-row {
            margin-top: 20px;
        }

        .btn {
            padding: 10px 18px;
            border-radius: 4px;
            border: none;
            cursor: pointer;
            font-weight: 600;
        }

        .btn-primary {
            background: #7a0616;
            color: white;
        }

        .btn-primary:hover {
            background: #94091b;
        }

        .btn-success {
            background: #198754;
            color: white;
        }

        .btn-success:hover {
            background: #157347;
        }

        .btn-secondary {
            background: #6c757d;
            color: white;
        }

        .btn-secondary:hover {
            background: #5c636a;
        }

        .user-info {
            font-size: 1rem;
            line-height: 2;
        }

        .status-label {
            display: block;
            margin-top: 20px;
            font-weight: 700;
            font-size: 1rem;
        }

        input[type=text],
        select,
        textarea {
            width: 100%;
            max-width: 500px;
            padding: 10px;
            border: 1px solid #ced4da;
            border-radius: 4px;
        }

        select {
            width: 350px;
        }

        asp\:CheckBoxList {
            width: 100%;
        }

    </style>

</head>

<body>

    <form id="form1" runat="server">

        <div class="page-container">

            <div class="hero-banner">
                <div class="hero-title">
                    Manage User Access
                </div>

                <div class="hero-subtitle">
                    Configure dataset permissions and metadata visibility for users.
                </div>
            </div>

            <div class="card">

                <div class="user-info">

                    <asp:Label
                        ID="lblUserInfo"
                        runat="server" />

                </div>

            </div>

            <div class="card">

                <h3>Dataset Access Management</h3>

                <div style="margin-bottom:15px;">

                    <asp:DropDownList
                        ID="ddlPresets"
                        runat="server"
                        Width="300px">
                    </asp:DropDownList>

                    <asp:Button
                        ID="btnLoadPreset"
                        runat="server"
                        Text="Load Preset"
                        CssClass="btn btn-primary"
                        OnClick="btnLoadPreset_Click" />

                </div>

                <asp:TextBox
                    ID="txtPresetName"
                    runat="server"
                    CssClass="form-control"
                    placeholder="Preset name" />

                <asp:Button
                    ID="btnSavePreset"
                    runat="server"
                    Text="Save As Preset"
                    OnClick="btnSavePreset_Click" />

                <p class="section-title">
                    Select the datasets this user is allowed to search.
                </p>

                <div class="checkbox-box">

                    <asp:CheckBox
                        ID="chkSelectAllDatasets"
                        runat="server"
                        Text="Select All Datasets"
                        AutoPostBack="true"
                        OnCheckedChanged="chkSelectAllDatasets_CheckedChanged" />

                    <br /><br />

                    <asp:CheckBoxList
                        ID="cblDatasets"
                        runat="server"
                        RepeatDirection="Vertical"
                        RepeatLayout="Table">
                    </asp:CheckBoxList>

                </div>

                <div class="button-row">

                    <asp:Button
                        ID="btnLoadMetadata"
                        runat="server"
                        Text="Load Metadata Columns"
                        CssClass="btn btn-primary"
                        OnClick="btnLoadMetadata_Click" />

                </div>

            </div>

            <div class="card">

                <h3>Filter Visibility Settings</h3>
                <br /><br />

                <p class="section-title">
                    Select which metadata columns should appear
                    in the user's search filters.
                </p>

                <div class="checkbox-box">

                    <asp:PlaceHolder
                        ID="phMetadataSections"
                        runat="server">
                    </asp:PlaceHolder>

                </div>

                <div class="button-row">

                    <asp:Button
                        ID="btnSave"
                        runat="server"
                        Text="Save Access Settings"
                        CssClass="btn btn-success"
                        OnClick="btnSave_Click" />

                    <asp:Button
                        ID="btnBack"
                        runat="server"
                        Text="Back to Admin Panel"
                        CssClass="btn btn-secondary"
                        PostBackUrl="~/AdminPanel.aspx" />

                </div>

                <asp:Label
                    ID="lblStatus"
                    runat="server"
                    CssClass="status-label" />

            </div>

        </div>

    </form>

</body>
</html>