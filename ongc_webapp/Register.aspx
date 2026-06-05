<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs"
    Inherits="ongc_webapp.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ONGC - Register</title>

    <style>
        body {
            margin: 0;
            font-family: Arial, sans-serif;
            background: #f4f6f9;
        }

        .container {
            display: flex;
            height: 100vh;
        }

        .left-panel {
            flex: 1;
            background: #8B0000;
            color: white;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
        }

        .left-panel h1 {
            font-size: 40px;
            margin-bottom: 10px;
        }

        .left-panel p {
            font-size: 18px;
        }

        .right-panel {
            width: 500px;
            background: white;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .register-card {
            width: 400px;
        }

        .register-card h2 {
            color: #8B0000;
            margin-bottom: 25px;
        }

        .form-group {
            margin-bottom: 15px;
        }

        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: bold;
        }

        .form-control {
            width: 100%;
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 6px;
            box-sizing: border-box;
        }

        .btn-register {
            width: 100%;
            padding: 12px;
            background: #8B0000;
            color: white;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            font-size: 15px;
        }

        .btn-register:hover {
            background: #a00000;
        }

        .footer-link {
            margin-top: 15px;
            text-align: center;
        }

        .footer-link a {
            color: #8B0000;
            text-decoration: none;
            font-weight: bold;
        }

        .status {
            color: red;
            margin-bottom: 15px;
        }
    </style>
</head>

<body>

<form id="form1" runat="server">

<div class="container">

    <div class="left-panel">
        <h1>ONGC</h1>
        <p>Create your account</p>
    </div>

    <div class="right-panel">

        <div class="register-card">

            <h2>Register</h2>

            <asp:Label
                ID="lblStatus"
                runat="server"
                CssClass="status" />

            <div class="form-group">
                <label>Full Name</label>
                <asp:TextBox
                    ID="txtEmployeeName"
                    runat="server"
                    CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>CPF Number</label>
                <asp:TextBox
                    ID="txtCPF"
                    runat="server"
                    CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>Department</label>
                <asp:TextBox
                    ID="txtDepartment"
                    runat="server"
                    CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>Username</label>
                <asp:TextBox
                    ID="txtUsername"
                    runat="server"
                    CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>Password</label>
                <asp:TextBox
                    ID="txtPassword"
                    runat="server"
                    TextMode="Password"
                    CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>Confirm Password</label>
                <asp:TextBox
                    ID="txtConfirmPassword"
                    runat="server"
                    TextMode="Password"
                    CssClass="form-control" />
            </div>

            <asp:Button
                ID="btnRegister"
                runat="server"
                Text="Create Account"
                CssClass="btn-register"
                OnClick="btnRegister_Click" />

            <div class="footer-link">
                Already have an account?
                <a href="Login.aspx">Login</a>
            </div>

        </div>

    </div>

</div>

</form>

</body>
</html>