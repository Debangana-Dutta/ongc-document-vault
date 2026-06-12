<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ongc_webapp.Register" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ONGC - Register</title>
    <style>
        :root { --maroon: #8B0000; --maroon-mid: #660000; --white: #ffffff; --muted: #666; }
        body { margin: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f0f0f0; display: flex; justify-content: center; align-items: center; min-height: 100vh; }
        .login-shell { width: 100%; max-width: 900px; min-height: 600px; background: var(--white); border-radius: 20px; overflow: hidden; box-shadow: 0 15px 35px rgba(0,0,0,0.1); display: flex; }
        .visual-panel { flex: 1; background: linear-gradient(rgba(139, 0, 0, 0.5), rgba(139, 0, 0, 0.5)), url('employee.jpg') center/cover; display: flex; flex-direction: column; justify-content: flex-end; padding: 40px; color: white; }
        .form-panel { flex: 1.2; padding: 40px; display: flex; flex-direction: column; justify-content: center; }
        h2 { color: var(--maroon); margin: 0 0 5px 0; font-size: 24px; }
        .subtitle { color: var(--muted); margin-bottom: 20px; font-size: 14px; }
        .form-group { margin-bottom: 15px; }
        .form-group label { display: block; font-weight: 600; font-size: 12px; color: #333; margin-bottom: 5px; text-transform: uppercase; }
        .form-control { width: 100%; padding: 12px; border: 1px solid #ccc; border-radius: 8px; box-sizing: border-box; }
        .btn-register { width: 100%; padding: 14px; background: var(--maroon); color: white; border: none; border-radius: 8px; font-weight: bold; cursor: pointer; transition: background .2s; }
        .btn-register:hover { background: var(--maroon-mid); }
        .status { color: #d9534f; font-size: 13px; margin-bottom: 10px; }
        .footer-link { margin-top: 20px; text-align: center; font-size: 14px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-shell">
            <div class="visual-panel">
                <h1>Powering India's Future</h1>
                <p>Oil and Natural Gas Corporation<br />Employee Enterprise Portal</p>
            </div>
            <div class="form-panel">
                <h2>Create Account</h2>
                <p class="subtitle">Join the ONGC Enterprise Portal</p>
                <asp:Label ID="lblStatus" runat="server" CssClass="status" />

                <div class="form-group"><label>Full Name</label><asp:TextBox ID="txtEmployeeName" runat="server" CssClass="form-control" /></div>
                <div class="form-group"><label>Department</label><asp:TextBox ID="txtDepartment" runat="server" CssClass="form-control" /></div>
                <div class="form-group"><label>CPF</label><asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" /></div>
                <div class="form-group"><label>Password</label><asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" /></div>
                <div class="form-group"><label>Confirm Password</label><asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" /></div>

                <asp:Button ID="btnRegister" runat="server" Text="REGISTER" CssClass="btn-register" OnClick="btnRegister_Click" />

                <div class="footer-link">Already have an account? <a href="Login.aspx" style="color:var(--maroon); font-weight:bold;">Login</a></div>
            </div>
        </div>
    </form>
</body>
</html>