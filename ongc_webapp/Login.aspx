<%@ Page Title="Login" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ongc_webapp.Login" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Enterprise Portal Login - ONGC</title>
    
    <link href="https://fonts.googleapis.com/css2?family=Public+Sans:wght@400;500;600;700;800&display=swap" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />

    <style>
        * {
            font-family: 'Public Sans', sans-serif;
            box-sizing: border-box;
        }

        body {
            background-color: #f8f9fa !important;
            margin: 0;
            padding: 0;
        }

        .login-page-wrapper {
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            padding: 20px;
            box-sizing: border-box;
        }

        .login-container-wrapper {
            display: flex;
            width: 100%;
            max-width: 950px; 
            height: 580px;    
            background: #fff;
            border-radius: 16px;
            overflow: hidden;
            box-shadow: 0 15px 35px rgba(0,0,0,0.12);
        }

        .login-visual-side {
            flex: 1.1; 
            background: url('images/ongc_refinery.jpg') no-repeat;
            background-size: cover;
            background-position: 75% center; 
            position: relative;
            image-rendering: -webkit-optimize-contrast;
            image-rendering: crisp-edges;
        }

        .login-visual-side::before {
            content: "";
            position: absolute;
            top: 0; left: 0; width: 100%; height: 100%;
            background: linear-gradient(135deg, rgba(128, 0, 0, 0.2), rgba(0, 0, 0, 0.15));
            pointer-events: none;
        }

        .login-form-side {
            flex: 0.9; 
            padding: 40px 45px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            background-color: #ffffff;
        }

        .ongc-brand-logo {
            width: 85px;
            height: auto;
            margin-bottom: 20px;
            image-rendering: -webkit-optimize-contrast;
        }

        .portal-title {
            color: #800000;
            font-weight: 800;
            margin-bottom: 4px;
            letter-spacing: -0.5px;
            font-size: 1.75rem;
        }

        .form-control-custom {
            border: 1.5px solid #E2E8F0;
            padding: 12px 14px;
            border-radius: 6px;
            background-color: #FAFAFA;
            font-size: 0.95rem;
            transition: all 0.2s ease;
        }

        .form-control-custom:focus {
            border-color: #800000;
            background-color: #ffffff;
            box-shadow: 0 0 0 3px rgba(128, 0, 0, 0.1);
            outline: none;
        }

        .btn-ongc-submit {
            background-color: #800000;
            border: none;
            padding: 14px;
            font-weight: 600;
            color: white;
            transition: all 0.2s ease;
            border-radius: 6px;
            letter-spacing: 0.5px;
            font-size: 1rem;
            box-shadow: 0 4px 12px rgba(128, 0, 0, 0.15);
            cursor: pointer;
            margin-top: 5px;
        }

        .btn-ongc-submit:hover {
            background-color: #600000;
            transform: translateY(-1px);
            box-shadow: 0 6px 166px rgba(128, 0, 0, 0.23);
        }

        .auth-toggle-link {
            color: #800000;
            font-weight: 600;
            text-decoration: none;
            cursor: pointer;
        }

        .auth-toggle-link:hover {
            text-decoration: underline;
            color: #600000;
        }

        .copyright-text {
            font-size: 0.75rem;
            color: #A0AEC0;
            margin-top: 25px;
        }

        @media (max-width: 850px) {
            .login-container-wrapper {
                height: auto;
                max-width: 450px;
                flex-direction: column;
            }
            .login-visual-side { display: none; }
            .login-form-side { padding: 40px 30px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-page-wrapper">
            <div class="login-container-wrapper">
                
                <div class="login-visual-side"></div>

                <div class="login-form-side">
                    <div class="text-center text-md-start">
                        <img src="images/ongclogo.png" alt="ONGC Logo" class="ongc-brand-logo" />
                        
                        <div id="loginHeader">
                            <h2 class="portal-title">Enterprise Portal</h2>
                            <p class="text-muted mb-4" style="font-size: 0.95rem;">Management & Indexing System</p>
                        </div>
                        <div id="registerHeader" style="display: none;">
                            <h2 class="portal-title">Create Account</h2>
                            <p class="text-muted mb-4" style="font-size: 0.95rem;">Register corporate access profile</p>
                        </div>
                        <div id="recoveryHeader" style="display: none;">
                            <h2 class="portal-title">Recover Password</h2>
                            <p class="text-muted mb-4" style="font-size: 0.95rem;">Verify system credential registry link</p>
                        </div>
                    </div>

                    <div class="mb-3" id="usernameGroup">
                        <label class="form-label fw-bold text-secondary small" style="letter-spacing: 0.3px;">USER ID / CPF NUMBER</label>
                        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control form-control-custom" placeholder="Enter your ID"></asp:TextBox>
                    </div>

                    <div class="mb-3" id="passwordGroup">
                        <label class="form-label fw-bold text-secondary small" style="letter-spacing: 0.3px;">PASSWORD</label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control form-control-custom" TextMode="Password" placeholder="••••••••"></asp:TextBox>
                    </div>

                    <div class="mb-4" id="confirmPasswordGroup" style="display: none;">
                        <label class="form-label fw-bold text-secondary small" style="letter-spacing: 0.3px;">CONFIRM PASSWORD</label>
                        <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control form-control-custom" TextMode="Password" placeholder="••••••••"></asp:TextBox>
                    </div>

                    <div class="mb-4" id="corporateEmailGroup" style="display: none;">
                        <label class="form-label fw-bold text-secondary small" style="letter-spacing: 0.3px;">REGISTERED CORPORATE EMAIL</label>
                        <asp:TextBox ID="txtCorporateEmail" runat="server" CssClass="form-control form-control-custom" placeholder="alias@ongc.co.in"></asp:TextBox>
                    </div>

                    <asp:HiddenField ID="hdnAuthState" runat="server" Value="LOGIN" />

                    <div class="mb-2 text-center text-md-start">
                        <asp:Label ID="lblAuthMessage" runat="server" CssClass="small fw-bold"></asp:Label>
                    </div>

                    <div class="d-grid">
                        <asp:Button ID="btnLogin" runat="server" Text="LOG IN" 
                            CssClass="btn-ongc-submit" OnClick="btnLogin_Click" />
                    </div>

                    <div class="text-center text-md-start mt-3 small">
                        <div id="loginFooterLinks">
                            <a class="auth-toggle-link text-muted me-3" style="font-weight:400;" onclick="switchAuthenticationView('RECOVERY')">Forgot Password?</a>
                            <span class="text-muted">New user? </span>
                            <a class="auth-toggle-link" onclick="switchAuthenticationView('REGISTER')">Register Here</a>
                        </div>
                        <div id="registerFooterLinks" style="display: none;">
                            <span class="text-muted">Already registered? </span>
                            <a class="auth-toggle-link" onclick="switchAuthenticationView('LOGIN')">Log In Instead</a>
                        </div>
                        <div id="recoveryFooterLinks" style="display: none;">
                            <span class="text-muted">Remember access sequence? </span>
                            <a class="auth-toggle-link" onclick="switchAuthenticationView('LOGIN')">Back to Login</a>
                        </div>
                    </div>

                    <p class="copyright-text text-center text-md-start">
                        &copy; <%: DateTime.Now.Year %> Oil and Natural Gas Corporation Limited. All rights reserved.
                    </p>
                </div>

            </div>
        </div>
    </form>

    <script type="text/javascript">
        // @ts-nocheck
        function switchAuthenticationView(selectedMode) {
            var mode = String(selectedMode);
            
            // DOM References
            var loginHead = document.getElementById('loginHeader');
            var regHead = document.getElementById('registerHeader');
            var recHead = document.getElementById('recoveryHeader');
            
            var userGroup = document.getElementById('usernameGroup');
            var passGroup = document.getElementById('passwordGroup');
            var confirmGroup = document.getElementById('confirmPasswordGroup');
            var emailGroup = document.getElementById('corporateEmailGroup');
            
            var loginFoot = document.getElementById('loginFooterLinks');
            var regFoot = document.getElementById('registerFooterLinks');
            var recFoot = document.getElementById('recoveryFooterLinks');
            
            var actionButton = document.getElementById('<%= btnLogin.ClientID %>');
            var hiddenState = document.getElementById('<%= hdnAuthState.ClientID %>');

            // Set all sections to hidden before selectively displaying the target layout mode
            if(loginHead) loginHead.style.display = 'none';
            if(regHead) regHead.style.display = 'none';
            if(recHead) recHead.style.display = 'none';
            if(userGroup) userGroup.style.display = 'block'; 
            if(passGroup) passGroup.style.display = 'block';
            if(confirmGroup) confirmGroup.style.display = 'none';
            if(emailGroup) emailGroup.style.display = 'none';
            if(loginFoot) loginFoot.style.display = 'none';
            if(regFoot) regFoot.style.display = 'none';
            if(recFoot) recFoot.style.display = 'none';

            if (mode === 'REGISTER') {
                if(regHead) regHead.style.display = 'block';
                if(confirmGroup) confirmGroup.style.display = 'block';
                if(regFoot) regFoot.style.display = 'block';
                if(actionButton) actionButton.value = 'REGISTER ACCOUNT';
                if(hiddenState) hiddenState.value = 'REGISTER';
            } 
            else if (mode === 'RECOVERY') {
                if(recHead) recHead.style.display = 'block';
                if(passGroup) passGroup.style.display = 'none'; // Clear out inputs irrelevant to validation tracking
                if(emailGroup) emailGroup.style.display = 'block';
                if(recFoot) recFoot.style.display = 'block';
                if(actionButton) actionButton.value = 'RESET CREDEENTIALS';
                if(hiddenState) hiddenState.value = 'RECOVERY';
            } 
            else {
                // Default fallback to LOGIN mode layout properties
                if(loginHead) loginHead.style.display = 'block';
                if(loginFoot) loginFoot.style.display = 'block';
                if(actionButton) actionButton.value = 'LOG IN';
                if(hiddenState) hiddenState.value = 'LOGIN';
            }

            var labelFeedback = document.getElementById('<%= lblAuthMessage.ClientID %>');
            if (labelFeedback) labelFeedback.innerHTML = '';
        }

        window.onload = function () {
    var hiddenState = document.getElementById('<%= hdnAuthState.ClientID %>');
    if (hiddenState && hiddenState.value) {
        switchAuthenticationView(hiddenState.value);
    }
};
</script>
</body>
</html>