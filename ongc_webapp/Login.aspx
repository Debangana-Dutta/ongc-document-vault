<%@ Page Title="Login" Language="C#" AutoEventWireup="true" 
         CodeBehind="Login.aspx.cs" Inherits="ongc_webapp.Login" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Enterprise Portal Login - ONGC</title>
    <link href="https://fonts.googleapis.com/css2?family=Public+Sans:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />

    <style>
        * { font-family: 'Public Sans', sans-serif; box-sizing: border-box; }
        body { background-color: #f8f9fa !important; margin: 0; padding: 0; }
        .login-page-wrapper {
            display: flex; justify-content: center; align-items: center;
            min-height: 100vh; padding: 20px;
        }
        .login-container-wrapper {
            display: flex; width: 100%; max-width: 950px; height: 580px;
            background: #fff; border-radius: 16px; overflow: hidden;
            box-shadow: 0 15px 35px rgba(0,0,0,0.12);
        }
        .login-visual-side {
            flex: 1.1;
            background: url('images/ongc_refinery.jpg') no-repeat center center;
            background-size: cover; position: relative;
        }
        .login-visual-side::before {
            content: ""; position: absolute; inset: 0;
            background: linear-gradient(135deg, rgba(128,0,0,0.2), rgba(0,0,0,0.15));
        }
        .login-form-side {
            flex: 0.9; padding: 40px 45px;
            display: flex; flex-direction: column; justify-content: center;
            background-color: #ffffff;
        }
        .ongc-brand-logo { width: 85px; height: auto; margin-bottom: 20px; }
        .portal-title { color: #800000; font-weight: 800; margin-bottom: 4px; font-size: 1.75rem; }
        .form-control-custom {
            width: 100%; border: 1.5px solid #E2E8F0; padding: 12px 14px;
            border-radius: 6px; background-color: #FAFAFA;
            font-size: 0.95rem; transition: all 0.2s ease;
        }
        .form-control-custom:focus {
            border-color: #800000; background-color: #fff;
            box-shadow: 0 0 0 3px rgba(128,0,0,0.1); outline: none;
        }
        .btn-ongc-submit {
            width: 100%; background-color: #800000; border: none;
            padding: 14px; font-weight: 600; color: white;
            border-radius: 6px; font-size: 1rem; cursor: pointer;
            box-shadow: 0 4px 12px rgba(128,0,0,0.15); margin-top: 5px;
            transition: all 0.2s ease;
        }
        .btn-ongc-submit:hover { background-color: #600000; transform: translateY(-1px); }
        .auth-toggle-link { color: #800000; font-weight: 600; text-decoration: none; cursor: pointer; }
        .auth-toggle-link:hover { text-decoration: underline; color: #600000; }
        .copyright-text { font-size: 0.75rem; color: #A0AEC0; margin-top: 25px; }
        @media (max-width: 850px) {
            .login-container-wrapper { height: auto; max-width: 450px; flex-direction: column; }
            .login-visual-side { display: none; }
            .login-form-side { padding: 40px 30px; }
        }
    </style>
</head>
<body>
<%-- NOTE: Only ONE <form runat="server"> is allowed per WebForms page --%>
<form id="form1" runat="server">
    <div class="login-page-wrapper">
        <div class="login-container-wrapper">

            <%-- ── LEFT: Visual Panel ─────────────────────────────── --%>
            <div class="login-visual-side"></div>

            <%-- ── RIGHT: Form Panel ──────────────────────────────── --%>
            <div class="login-form-side">
                <div>
                    <img src="images/ongclogo.png" alt="ONGC Logo" class="ongc-brand-logo" />

                    <%-- Dynamic headers swapped by JS based on auth state --%>
                    <div id="loginHeader">
                        <h2 class="portal-title">Enterprise Portal</h2>
                        <p class="text-muted mb-4" style="font-size:0.95rem;">Management &amp; Indexing System</p>
                    </div>
                    <div id="registerHeader" style="display:none;">
                        <h2 class="portal-title">Create Account</h2>
                        <p class="text-muted mb-4" style="font-size:0.95rem;">Register corporate access profile</p>
                    </div>
                    <div id="recoveryHeader" style="display:none;">
                        <h2 class="portal-title">Recover Password</h2>
                        <p class="text-muted mb-4" style="font-size:0.95rem;">Verify system credential registry</p>
                    </div>
                </div>

                <%-- ── USERNAME ─────────────────────────────────────── --%>
                <%-- ID="txtUsername" maps to: TextBox txtUsername in code-behind --%>
                <div class="mb-3" id="usernameGroup">
                    <label class="form-label fw-bold text-secondary small">USER ID / CPF NUMBER</label>
                    <asp:TextBox ID="txtUsername" runat="server" 
                                 CssClass="form-control-custom"
                                 placeholder="Enter your username or CPF" />
                </div>

                <%-- ── PASSWORD ─────────────────────────────────────── --%>
                <%-- ID="txtPassword" maps to: TextBox txtPassword in code-behind --%>
                <div class="mb-3" id="passwordGroup">
                    <label class="form-label fw-bold text-secondary small">PASSWORD</label>
                    <asp:TextBox ID="txtPassword" runat="server"
                                 TextMode="Password"
                                 CssClass="form-control-custom"
                                 placeholder="Enter your password" />
                </div>

                <%-- ── CONFIRM PASSWORD (Register only) ────────────── --%>
                <%-- ID="txtConfirmPassword" maps to: TextBox txtConfirmPassword --%>
                <div class="mb-3" id="confirmPasswordGroup" style="display:none;">
                    <label class="form-label fw-bold text-secondary small">CONFIRM PASSWORD</label>
                    <asp:TextBox ID="txtConfirmPassword" runat="server"
                                 TextMode="Password"
                                 CssClass="form-control-custom"
                                 placeholder="Re-enter your password" />
                </div>

                <%-- ── CORPORATE EMAIL (Recovery only) ─────────────── --%>
                <%-- ID="txtCorporateEmail" maps to: TextBox txtCorporateEmail --%>
                <div class="mb-3" id="emailGroup" style="display:none;">
                    <label class="form-label fw-bold text-secondary small">CORPORATE EMAIL</label>
                    <asp:TextBox ID="txtCorporateEmail" runat="server"
                                 CssClass="form-control-custom"
                                 placeholder="yourname@ongc.co.in" />
                </div>

                <%-- ── HIDDEN FIELD: Tracks current auth mode ──────── --%>
                <%-- ID="hdnAuthState" maps to: HiddenField hdnAuthState --%>
                <asp:HiddenField ID="hdnAuthState" runat="server" Value="LOGIN" />

                <%-- ── SUBMIT BUTTON ────────────────────────────────── --%>
                <%-- ID="btnLogin" maps to: Button btnLogin in code-behind --%>
                <asp:Button ID="btnLogin" runat="server"
                            Text="Sign In"
                            CssClass="btn-ongc-submit"
                            OnClick="btnLogin_Click" />

                <%-- ── AUTH MODE TOGGLES ────────────────────────────── --%>
                <div class="mt-3 text-center" style="font-size:0.875rem;" id="loginLinks">
                    <span class="text-muted">New here? </span>
                    <a class="auth-toggle-link" onclick="switchMode('REGISTER')">Create Account</a>
                    &nbsp;|&nbsp;
                    <a class="auth-toggle-link" onclick="switchMode('RECOVERY')">Forgot Password?</a>
                </div>
                <div class="mt-3 text-center" style="font-size:0.875rem; display:none;" id="backToLoginLink">
                    <a class="auth-toggle-link" onclick="switchMode('LOGIN')">← Back to Sign In</a>
                </div>

                <p class="copyright-text text-center">
                    &copy; <%= DateTime.Now.Year %> Oil and Natural Gas Corporation Ltd. All rights reserved.
                </p>
            </div>
        </div>
    </div>
</form>

<script>
// ── AUTH MODE SWITCHER ─────────────────────────────────────────────────
// Uses ClientID to get the correct ASP.NET-generated IDs at runtime.
// This is the correct pattern — never hardcode IDs in JS for server controls.
var hdnAuthState = document.getElementById('<%= hdnAuthState.ClientID %>');
    var btnLogin        = document.getElementById('<%= btnLogin.ClientID %>');
    var txtConfirmPass  = document.getElementById('<%= txtConfirmPassword.ClientID %>');
    var txtEmail        = document.getElementById('<%= txtCorporateEmail.ClientID %>');

    function switchMode(mode) {
        // Reset all panels first
        document.getElementById('loginHeader').style.display    = 'none';
        document.getElementById('registerHeader').style.display = 'none';
        document.getElementById('recoveryHeader').style.display = 'none';
        document.getElementById('passwordGroup').style.display  = 'block';
        document.getElementById('confirmPasswordGroup').style.display = 'none';
        document.getElementById('emailGroup').style.display     = 'none';
        document.getElementById('loginLinks').style.display     = 'none';
        document.getElementById('backToLoginLink').style.display = 'block';

        hdnAuthState.value = mode;

        if (mode === 'LOGIN') {
            document.getElementById('loginHeader').style.display  = 'block';
            document.getElementById('loginLinks').style.display   = 'block';
            document.getElementById('backToLoginLink').style.display = 'none';
            btnLogin.value = 'Sign In';
        } else if (mode === 'REGISTER') {
            document.getElementById('registerHeader').style.display = 'block';
            document.getElementById('confirmPasswordGroup').style.display = 'block';
            btnLogin.value = 'Create Account';
        } else if (mode === 'RECOVERY') {
            document.getElementById('recoveryHeader').style.display = 'block';
            document.getElementById('passwordGroup').style.display  = 'none';
            document.getElementById('emailGroup').style.display     = 'block';
            btnLogin.value = 'Send Recovery Link';
        }
    }

    // Initialize to LOGIN mode on page load
    switchMode('LOGIN');
</script>
</body>
</html>