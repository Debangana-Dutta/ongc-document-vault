<%@ Page Language="C#" AutoEventWireup="true"
CodeBehind="ViewFile.aspx.cs"
Inherits="ongc_webapp.ViewFile" %>

<!DOCTYPE html>

<html>
<head runat="server">

    <title>
        File Viewer
    </title>

    <meta charset="utf-8" />

    <style>

        body {
            margin: 0;
            font-family: Arial;
            background: #f4f6f9;
        }

        .topbar {
            background: #7a0616;
            color: white;
            padding: 14px 20px;
            font-size: 18px;
            font-weight: bold;
            box-shadow: 0 2px 6px rgba(0,0,0,0.15);
        }

        .viewer-container {
            padding: 20px;
            height: calc(100vh - 70px);
            box-sizing: border-box;
        }

        iframe {
            width: 100%;
            height: 100%;
            border: none;
            background: white;
            border-radius: 8px;
        }

        .text-viewer {
            background: white;
            padding: 25px;
            border-radius: 10px;
            white-space: pre-wrap;
            overflow: auto;
            height: 100%;
            box-sizing: border-box;
            font-family: Consolas;
            font-size: 14px;
            border: 1px solid #ddd;
        }

        .image-viewer {
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100%;
            background: white;
            border-radius: 10px;
            overflow: auto;
        }

        .image-viewer img {
            max-width: 100%;
            max-height: 100%;
        }

        .download-box {
            background: white;
            padding: 40px;
            border-radius: 10px;
            text-align: center;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
        }

        .download-btn {
            display: inline-block;
            margin-top: 20px;
            padding: 14px 25px;
            background: #7a0616;
            color: white;
            text-decoration: none;
            border-radius: 6px;
            font-weight: bold;
        }

    </style>

</head>

<body>

    <form id="form1" runat="server">

        <div class="topbar">
            Enterprise File Viewer
        </div>

        <div class="viewer-container">

            <asp:Literal
                ID="litViewer"
                runat="server">
            </asp:Literal>

        </div>

    </form>

</body>
</html>