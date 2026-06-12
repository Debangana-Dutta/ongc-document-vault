<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="ManageUserAccess.aspx.cs"
    Inherits="ongc_webapp.ManageUserAccess" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Manage User Access</title>

    <style>

        body {
            font-family: Arial, Helvetica, sans-serif;
            background-color: #f5f7fa;
            margin: 0;
            padding: 0;
        }

        .page-container {
            max-width: 1100px;
            margin: 30px auto;
            padding: 20px;
        }

        .card {
            background: white;
            border-radius: 12px;
            padding: 24px;
            margin-bottom: 20px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        h2 {
            margin-top: 0;
            color: #1f2937;
        }

        h3 {
            margin-top: 0;
            color: #374151;
        }

        .section-title {
            margin-bottom: 15px;
            font-weight: 600;
            color: #374151;
        }

        .checkbox-box {
            border: 1px solid #d1d5db;
            border-radius: 8px;
            padding: 12px;
            max-height: 350px;
            overflow-y: auto;
            background: #fafafa;
        }

        .button-row {
            margin-top: 20px;
        }

        .btn {
            padding: 10px 16px;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            font-size: 14px;
            margin-right: 10px;
        }

        .btn-primary {
            background-color: #2563eb;
            color: white;
        }

        .btn-success {
            background-color: #16a34a;
            color: white;
        }

        .btn-secondary {
            background-color: #6b7280;
            color: white;
        }

        .user-info {
            line-height: 1.8;
            font-size: 15px;
        }

        .status-label {
            margin-top: 15px;
            display: block;
            font-weight: bold;
        }

    </style>

</head>

<body>

    <form id="form1" runat="server">

        <div class="page-container">

            <div class="card">

                <h2>Manage User Access</h2>

                <div class="user-info">

                    <asp:Label
                        ID="lblUserInfo"
                        runat="server" />

                </div>

            </div>

            <div class="card">

                <h3>Step 1 — Dataset Access</h3>

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

                <h3>Step 2 — Metadata Visibility</h3>
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