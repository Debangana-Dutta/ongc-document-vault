﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ongc_webapp.Register" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ONGC - Register</title>
    <style>
        body {
            margin: 0;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f0f0f0;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
        }

        .login-container {
            display: flex;
            width: 900px;
            /* Changed height to min-height to allow natural expansion */
            min-height: 600px;
            background: white;
            border-radius: 20px;
            overflow: hidden;
            box-shadow: 0 15px 35px rgba(0,0,0,0.1);
        }

        .left-panel {
            width: 40%;
            background: linear-gradient(rgba(139, 0, 0, 0.5), rgba(139, 0, 0, 0.5)), url('employee.jpg');
            background-size: cover;
            background-position: center;
            display: flex;
            flex-direction: column;
            justify-content: flex-end;
            padding: 40px;
            color: white;
        }

        .right-panel {
            width: 60%;
            padding: 40px;
            /* Allows gentle scrolling if content exceeds height */
            overflow-y: auto; 
            display: flex;
            flex-direction: column;
            justify-content: center;
        }

        h2 { color: #8B0000; margin: 0 0 5px 0; font-size: 24px; }
        .subtitle { color: #666; margin-bottom: 20px; font-size: 14px; }

        .form-group { margin-bottom: 15px; }
        .form-group label { display: block; font-weight: 600; font-size: 12px; color: #333; margin-bottom: 5px; text-transform: uppercase; }

        .form-control {
            width: 100%;
            padding: 12px;
            border: 1px solid #ccc;
            border-radius: 8px;
            box-sizing: border-box;
            font-size: 14px;
        }

        .btn-register {
            width: 100%;
            padding: 14px;
            background: #8B0000;
            color: white;
            border: none;
            border-radius: 8px;
            font-weight: bold;
            cursor: pointer;
            margin-top: 10px;
        }

        .btn-register:hover { background: #660000; }
        .footer-link { margin-top: 20px; text-align: center; font-size: 14px; }
        .footer-link a { color: #8B0000; font-weight: bold; text-decoration: none; }
        .status { color: #d9534f; font-size: 13px; margin-bottom: 10px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <div class="left-panel">
                <h1 style="font-size: 28px;">Powering India's Future</h1>
                <p>Oil and Natural Gas Corporation<br />Employee Enterprise Portal</p>
            </div>
            <div class="right-panel">
                <h2>Create Account</h2>
                <p class="subtitle">Join the ONGC Enterprise Portal</p>
                <asp:Label ID="lblStatus" runat="server" CssClass="status" />

                <div class="form-group"><label>Full Name</label><asp:TextBox ID="txtEmployeeName" runat="server" CssClass="form-control" placeholder="Enter full name" /></div>
                <div class="form-group"><label>Department</label><asp:TextBox ID="txtDepartment" runat="server" CssClass="form-control" placeholder="Enter department" /></div>
                <div class="form-group"><label>CPF</label><asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter CPF" /></div>
                <div class="form-group"><label>Password</label><asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter password" /></div>
                <div class="form-group"><label>Confirm Password</label><asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Confirm password" /></div>

                <asp:Button ID="btnRegister" runat="server" Text="REGISTER" CssClass="btn-register" OnClick="btnRegister_Click" />

                <div class="footer-link">Already have an account? <a href="Login.aspx">Login</a></div>
            </div>
        </div>
    </form>
</body>
</html>