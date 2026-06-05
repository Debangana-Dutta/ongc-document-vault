﻿<%@ Page Title="Login" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ongc_webapp.Login" %>
<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Enterprise Portal — ONGC</title>
    <link href="https://fonts.googleapis.com/css2?family=Public+Sans:wght@300;400;500;600;700;800&family=Playfair+Display:wght@700&display=swap" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet">

    <style>
        :root {
            --maroon:      #800000;
            --maroon-dark: #5c0000;
            --maroon-mid:  #a80000;
            --gold:        #c9a84c;
            --bg:          #eeece8;
            --white:       #ffffff;
            --text:        #1a1a1a;
            --muted:       #6b7280;
            --border:      #dddad2;
            --input-bg:    #f7f6f3;
        }

        *, *::before, *::after { font-family: 'Public Sans', sans-serif; box-sizing: border-box; margin: 0; padding: 0; }

        html, body {
            min-height: 100%;
            background: var(--bg);
        }
        body {
            display: flex;
            justify-content: center;
            align-items: flex-start;
            min-height: 100vh;
            padding: 24px 16px;
        }

        /* ════════════════════════════════════════
           SHELL
           ════════════════════════════════════════ */
        .login-shell {
            width: 100%;
            max-width: 1160px;
            min-height: 90vh;
            background: var(--white);
            border-radius: 22px;
            overflow: hidden;
            box-shadow: 0 24px 72px rgba(0,0,0,0.14), 0 4px 16px rgba(0,0,0,0.06);
            display: flex;
            align-self: flex-start;
        }

        /* ════════════════════════════════════════
           VISUAL PANEL
           ════════════════════════════════════════ */
        .visual-panel {
            flex: 1.2;
            position: relative;
            background: url('images/ongc_refinery.jpg') center 25% / cover no-repeat;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
        }
        .visual-panel::before {
            content: "";
            position: absolute; inset: 0;
            background: linear-gradient(155deg, rgba(92,0,0,0.75) 0%, rgba(10,10,10,0.42) 100%);
        }
        .visual-top   { position: relative; z-index: 1; padding: 20px 44px; }
        .visual-top img { width: 76px; filter: brightness(0) invert(1); }
        .visual-bottom { position: relative; z-index: 1; padding: 0 44px 24px; color: rgba(255,255,255,0.88); }
        .visual-bottom h1 {
            font-family: 'Playfair Display', serif;
            font-size: 2.3rem;
            color: #fff;
            line-height: 1.2;
            margin-bottom: 10px;
        }
        .visual-bottom p { font-size: 0.9rem; font-weight: 400; line-height: 1.6; }

        /* ════════════════════════════════════════
           FORM PANEL
           ════════════════════════════════════════ */
        .form-panel {
            flex: 0 0 480px;
            display: flex;
            flex-direction: column;
            justify-content: flex-start;
            padding: 36px 52px 40px;
            background: var(--white);
            overflow-y: auto;
        }

        /* ── Top-panel role tabs ── */
        .role-tabs {
            display: flex;
            gap: 0;
            background: var(--input-bg);
            border: 1.5px solid var(--border);
            border-radius: 10px;
            padding: 4px;
            margin-bottom: 28px;
            flex-shrink: 0;
        }
        .role-tab {
            flex: 1;
            padding: 10px 14px;
            border-radius: 7px;
            border: none;
            background: transparent;
            color: var(--muted);
            font-weight: 600;
            font-size: 0.85rem;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 7px;
            transition: all .2s;
        }
        .role-tab i { font-size: 1rem; }
        .role-tab.active {
            background: var(--white);
            color: var(--maroon);
            box-shadow: 0 1px 6px rgba(0,0,0,0.1);
        }

        /* ── Section headings ── */
        .sec-head { margin-bottom: 20px; }
        .sec-head h2 { font-size: 1.7rem; font-weight: 800; color: var(--maroon); line-height: 1.2; }
        .sec-head p  { font-size: 0.84rem; color: var(--muted); margin-top: 5px; }

        /* ── Admin badge ── */
        .admin-badge {
            display: inline-flex; align-items: center; gap: 5px;
            background: #fff8e6; border: 1px solid #dbb84a; color: #7a5200;
            font-size: 0.72rem; font-weight: 700;
            padding: 3px 10px; border-radius: 20px;
            letter-spacing: .05em; text-transform: uppercase;
            margin-bottom: 10px;
        }

        /* ── Back link ── */
        .back-link {
            display: none;
            align-items: center; gap: 5px;
            font-size: 0.8rem; color: var(--muted);
            cursor: pointer; background: none; border: none;
            padding: 0; font-family: inherit; margin-bottom: 14px;
            transition: color .15s;
        }
        .back-link:hover { color: var(--maroon); }

        /* ════════════════════════════════════════
           FIELD GROUPS
           ════════════════════════════════════════ */
        .field-group { margin-bottom: 14px; }

        .field-group label {
            display: block;
            font-size: 0.76rem; font-weight: 700;
            color: #4b5563;
            text-transform: uppercase; letter-spacing: .05em;
            margin-bottom: 6px;
        }

        .input-wrap {
            position: relative;
            display: flex;
            align-items: center;
        }

        .input-wrap .ico-lead {
            position: absolute;
            left: 14px;
            color: #9ca3af;
            font-size: 1rem;
            pointer-events: none;
            z-index: 2;
            top: 50%; transform: translateY(-50%);
        }

        .btn-eye {
            position: absolute;
            right: 12px;
            top: 50%; transform: translateY(-50%);
            background: none; border: none; padding: 4px;
            color: #9ca3af; cursor: pointer; font-size: 1rem;
            line-height: 1; z-index: 2;
            display: flex; align-items: center;
        }
        .btn-eye:hover { color: var(--maroon); }

        .input-wrap input,
        .input-wrap select {
            width: 100%;
            padding: 13px 46px 13px 42px;
            border: 1.5px solid var(--border);
            border-radius: 9px;
            background: var(--input-bg);
            font-size: 0.91rem;
            color: var(--text);
            outline: none;
            transition: border-color .2s, box-shadow .2s, background .2s;
        }
        .input-wrap.no-eye input,
        .input-wrap.no-eye select {
            padding-right: 14px;
        }
        .input-wrap input:focus,
        .input-wrap select:focus {
            border-color: var(--maroon);
            box-shadow: 0 0 0 3px rgba(128,0,0,0.09);
            background: #fff;
        }
        .input-wrap input::placeholder { color: #b0aaa0; }
        .input-wrap select { padding-right: 14px; cursor: pointer; appearance: none; }

        /* ── Account-type picker (shared) ── */
        .acct-type-row {
            display: flex;
            gap: 10px;
            margin-bottom: 14px;
        }
        .acct-type-btn {
            flex: 1;
            padding: 11px 10px;
            border-radius: 9px;
            border: 1.5px solid var(--border);
            background: var(--input-bg);
            color: var(--muted);
            font-weight: 600; font-size: 0.82rem;
            cursor: pointer;
            display: flex; align-items: center; justify-content: center; gap: 6px;
            transition: all .2s;
        }
        .acct-type-btn.active {
            border-color: var(--maroon);
            background: #fff4f4;
            color: var(--maroon);
        }
        .acct-type-label {
            font-size: 0.76rem; font-weight: 700; color: #4b5563;
            text-transform: uppercase; letter-spacing: .05em;
            margin-bottom: 8px;
        }

        /* ── Submit button ── */
        .btn-submit {
            width: 100%;
            padding: 14px;
            background: var(--maroon);
            border: none; border-radius: 9px;
            color: #fff; font-weight: 700;
            font-size: 0.93rem; letter-spacing: .05em;
            cursor: pointer; margin-top: 8px;
            transition: background .2s, box-shadow .2s, transform .1s;
        }
        .btn-submit:hover  { background: var(--maroon-mid); box-shadow: 0 6px 20px rgba(128,0,0,0.28); }
        .btn-submit:active { transform: scale(0.99); }

        /* ── Footer links ── */
        .form-footer {
            margin-top: 16px; font-size: 0.83rem; color: var(--muted);
            text-align: center;
        }
        .link-btn {
            color: var(--maroon); font-weight: 600; cursor: pointer;
            background: none; border: none; padding: 0;
            font-size: inherit; font-family: inherit;
        }
        .link-btn:hover { text-decoration: underline; }
        .footer-row {
            display: flex; justify-content: space-between; align-items: center;
        }

        /* ── Inline alert ── */
        .inline-alert {
            display: none;
            padding: 10px 14px;
            border-radius: 8px; font-size: 0.83rem; font-weight: 500;
            margin-bottom: 14px; gap: 8px; align-items: flex-start;
        }
        .inline-alert.error   { background:#fff1f1; color:#991b1b; border:1px solid #fecaca; }
        .inline-alert.success { background:#f0fdf4; color:#166534; border:1px solid #bbf7d0; }
        .inline-alert.show    { display: flex; }
        .inline-alert i       { font-size:1rem; flex-shrink:0; margin-top:1px; }

        /* ── Helper note ── */
        .field-note {
            font-size: 0.74rem; color: var(--muted);
            margin-top: 5px; display: flex; align-items: center; gap: 4px;
        }

        /* ── Audit footer ── */
        .audit-note {
            margin-top: 14px; font-size: 0.78rem; color: #9ca3af;
            text-align: center; display: flex; align-items: center;
            justify-content: center; gap: 5px;
        }

        /* ── Responsive ── */
        @media (max-width: 900px) {
            .form-panel { flex: 0 0 420px; padding: 32px 36px; }
        }
        @media (max-width: 768px) {
            body { padding: 16px; }
            .login-shell { flex-direction: column; max-width: 500px; min-height: auto; }
            .visual-panel { display: none; }
            .form-panel { flex: none; width: 100%; padding: 32px 28px; }
        }
        @media (max-width: 480px) {
            .form-panel { padding: 24px 18px; }
        }
    </style>
</head>
<body>
<form id="form1" runat="server">

<div class="login-shell">

    <!-- ═══════════════════ VISUAL SIDE ═══════════════════ -->
    <div class="visual-panel">
        <div class="visual-top">
        </div>
        <div class="visual-bottom">
            <h1>Powering<br>India's Future</h1>
            <p>Oil and Natural Gas Corporation<br>Employee Enterprise Portal</p>
        </div>
    </div>

    <!-- ═══════════════════ FORM SIDE ═══════════════════ -->
    <div class="form-panel">

        <!-- Role tabs: Employee / Admin -->
        <div class="role-tabs">
            <button type="button" class="role-tab active" id="tabEmployee" onclick="setRole('EMPLOYEE')">
                <i class="bi bi-person-badge"></i> Employee
            </button>
            <button type="button" class="role-tab" id="tabAdmin" onclick="setRole('ADMIN')">
                <i class="bi bi-shield-lock"></i> Admin
            </button>
        </div>

        <!-- Inline alert (shared) -->
        <div class="inline-alert error" id="alertBox">
            <i class="bi bi-exclamation-circle-fill"></i>
            <span id="alertMsg">Something went wrong.</span>
        </div>

        <!-- Hidden state fields -->
        <asp:HiddenField ID="hdnRegAcctType"    runat="server" Value="EMPLOYEE" />
        

        <!-- ═══════════════════════════════════════════════
             EMPLOYEE SECTION
             ═══════════════════════════════════════════════ -->
        <div id="emp-section">

            <button type="button" class="back-link" id="backBtn" onclick="switchEmployeeView('LOGIN')">
                <i class="bi bi-arrow-left"></i> Back to Login
            </button>

            <div class="sec-head" id="hdr-login">
                <h2>Welcome Back</h2>
                <p>Sign in to the ONGC Enterprise Portal</p>
            </div>
            <div class="sec-head" id="hdr-register" style="display:none;">
                <h2>Create Account</h2>
                <p>Register your ONGC portal account</p>
            </div>
            <div class="sec-head" id="hdr-recovery" style="display:none;">
                <h2>Recover Password</h2>
                <p>We'll send a reset link to your corporate email</p>
            </div>

            <!-- Register: account type selector -->
            <div id="reg-acct-type-group" style="display:none; margin-bottom:14px;">
                <p class="acct-type-label">Registering as</p>
                <div class="acct-type-row">
                    <button type="button" class="acct-type-btn active" id="acctEmployee"
                            onclick="setRegAcctType('EMPLOYEE')">
                        <i class="bi bi-person-badge"></i> Employee
                    </button>
                    <button type="button" class="acct-type-btn" id="acctAdmin"
                            onclick="setRegAcctType('ADMIN')">
                        <i class="bi bi-shield-lock"></i> Admin
                    </button>
                </div>
            </div>

            <div class="field-group">
                <label>User ID / CPF Number</label>
                <div class="input-wrap no-eye">
                    <i class="bi bi-person ico-lead"></i>
                    <asp:TextBox ID="txtUsername" runat="server" placeholder="Enter your User ID or CPF"></asp:TextBox>
                </div>
            </div>

            <div class="field-group" id="emp-pwd-group">
                <label>Password</label>
                <div class="input-wrap" id="pwd-wrap">
                    <i class="bi bi-lock ico-lead"></i>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Enter your password"></asp:TextBox>
                    <button type="button" class="btn-eye" id="eyeMain"
                            onclick="togglePwd('<%=txtPassword.ClientID%>','eyeMain')">
                        <i class="bi bi-eye"></i>
                    </button>
                </div>
            </div>

            <div class="field-group" id="emp-confirm-group" style="display:none;">
                <label>Confirm Password</label>
                <div class="input-wrap">
                    <i class="bi bi-lock-fill ico-lead"></i>
                    <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" placeholder="Re-enter password"></asp:TextBox>
                    <button type="button" class="btn-eye" id="eyeConfirm"
                            onclick="togglePwd('<%=txtConfirmPassword.ClientID%>','eyeConfirm')">
                        <i class="bi bi-eye"></i>
                    </button>
                </div>
            </div>

            <div class="field-group" id="emp-email-group" style="display:none;">
                <label>Corporate Email</label>
                <div class="input-wrap no-eye">
                    <i class="bi bi-envelope ico-lead"></i>
                    <asp:TextBox ID="txtCorporateEmail" runat="server" placeholder="yourname@ongc.co.in"></asp:TextBox>
                </div>
            </div>

            <div class="field-group" id="emp-dept-group" style="display:none;">
                <label>Department</label>
                <div class="input-wrap no-eye">
                    <i class="bi bi-building ico-lead"></i>
                    <asp:TextBox ID="txtDepartment" runat="server" placeholder="e.g. Drilling, Finance, HR…"></asp:TextBox>
                </div>
            </div>

            <div class="field-group" id="emp-adminkey-group" style="display:none;">
                <label>Admin Authorisation Key</label>
                <div class="input-wrap">
                    <i class="bi bi-key-fill ico-lead"></i>
                    <asp:TextBox ID="txtRegAdminKey" runat="server" TextMode="Password" placeholder="Key issued by system admin"></asp:TextBox>
                    <button type="button" class="btn-eye" id="eyeRegAdminKey"
                            onclick="togglePwd('<%=txtRegAdminKey.ClientID%>','eyeRegAdminKey')">
                        <i class="bi bi-eye"></i>
                    </button>
                </div>
                <p class="field-note"><i class="bi bi-info-circle"></i> Required to register an admin account.</p>
            </div>

            <div class="field-group" id="emp-desig-group" style="display:none;">
                <label>Designation / Role</label>
                <div class="input-wrap no-eye">
                    <i class="bi bi-briefcase ico-lead"></i>
                    <asp:TextBox ID="txtRegDesignation" runat="server" placeholder="e.g. GM – IT, HR Head…"></asp:TextBox>
                </div>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="LOGIN"
                CssClass="btn-submit" OnClick="btnLogin_Click" />

            <div class="form-footer" id="emp-footer-login">
                <div class="footer-row">
                    <button type="button" class="link-btn" onclick="switchEmployeeView('RECOVERY')">Forgot Password?</button>
                    <span>
                        New here?
                        <a href="Register.aspx"
                           class="link-btn">
                           Register
                        </a>
                    </span>
                </div>
            </div>
            <div class="form-footer" id="emp-footer-register" style="display:none;">
                Already registered? <button type="button" class="link-btn" onclick="switchEmployeeView('LOGIN')">Log In</button>
            </div>
            <div class="form-footer" id="emp-footer-recovery" style="display:none;">
                Remembered it? <button type="button" class="link-btn" onclick="switchEmployeeView('LOGIN')">Back to Login</button>
            </div>
        </div>

        <!-- ═══════════════════════════════════════════════
             ADMIN SECTION  (Login + Register + Recovery)
             ═══════════════════════════════════════════════ -->
        <div id="adm-section" style="display:none;">

            <!-- Back button (register / recovery views) -->
            <button type="button" class="back-link" id="admBackBtn" onclick="switchAdminView('LOGIN')">
                <i class="bi bi-arrow-left"></i> Back to Admin Login
            </button>

            <!-- Admin badge — only on login view -->
            <div class="admin-badge" id="adm-badge"><i class="bi bi-shield-fill-check"></i> Administrator Access</div>

            <!-- Headers -->
            <div class="sec-head" id="adm-hdr-login">
                <h2>Admin Sign In</h2>
                <p>Restricted — authorised personnel only</p>
            </div>
            <div class="sec-head" id="adm-hdr-register" style="display:none;">
                <h2>Create Admin Account</h2>
                <p>Register a new administrator account</p>
            </div>
            <div class="sec-head" id="adm-hdr-recovery" style="display:none;">
                <h2>Recover Admin Password</h2>
                <p>We'll send a reset link to your corporate email</p>
            </div>

            <!-- ── LOGIN: Employee ID ── -->
            <div class="field-group" id="adm-empid-group">
                <label>Employee ID</label>
                <div class="input-wrap no-eye">
                    <i class="bi bi-person-badge ico-lead"></i>
                    <asp:TextBox ID="txtAdminEmpID" runat="server" placeholder="Admin Employee ID"></asp:TextBox>
                </div>
            </div>

            <!-- ── REGISTER: Full Name ── -->
            <div class="field-group" id="adm-name-group" style="display:none;">
                <label>Full Name</label>
                <div class="input-wrap no-eye">
                    <i class="bi bi-person ico-lead"></i>
                    <asp:TextBox ID="txtAdminFullName" runat="server" placeholder="Your full name"></asp:TextBox>
                </div>
            </div>

            <!-- ── REGISTER: New Employee ID ── -->
            <div class="field-group" id="adm-reg-empid-group" style="display:none;">
                <label>Employee ID</label>
                <div class="input-wrap no-eye">
                    <i class="bi bi-person-badge ico-lead"></i>
                    <asp:TextBox ID="txtAdminRegEmpID" runat="server" placeholder="Your Employee ID"></asp:TextBox>
                </div>
            </div>

            <!-- ── ALL: Corporate Email ── -->
            <div class="field-group" id="adm-email-group" style="display:none;">
                <label>Corporate Email</label>
                <div class="input-wrap no-eye">
                    <i class="bi bi-envelope ico-lead"></i>
                    <asp:TextBox ID="txtAdminEmail" runat="server" placeholder="yourname@ongc.co.in"></asp:TextBox>
                </div>
            </div>

            <!-- ── LOGIN + REGISTER: Password ── -->
            <div class="field-group" id="adm-pwd-group">
                <label>Password</label>
                <div class="input-wrap">
                    <i class="bi bi-lock ico-lead"></i>
                    <asp:TextBox ID="txtAdminPassword" runat="server" TextMode="Password" placeholder="Admin password"></asp:TextBox>
                    <button type="button" class="btn-eye" id="eyeAdminPwd"
                            onclick="togglePwd('<%=txtAdminPassword.ClientID%>','eyeAdminPwd')">
                        <i class="bi bi-eye"></i>
                    </button>
                </div>
            </div>

            <!-- ── REGISTER: Confirm Password ── -->
            <div class="field-group" id="adm-confirm-group" style="display:none;">
                <label>Confirm Password</label>
                <div class="input-wrap">
                    <i class="bi bi-lock-fill ico-lead"></i>
                    <asp:TextBox ID="txtAdminConfirmPassword" runat="server" TextMode="Password" placeholder="Re-enter password"></asp:TextBox>
                    <button type="button" class="btn-eye" id="eyeAdminConfirm"
                            onclick="togglePwd('<%=txtAdminConfirmPassword.ClientID%>','eyeAdminConfirm')">
                        <i class="bi bi-eye"></i>
                    </button>
                </div>
            </div>

            <!-- ── LOGIN + REGISTER: Admin Access Key ── -->
            <div class="field-group" id="adm-key-group">
                <label>Admin Access Key</label>
                <div class="input-wrap">
                    <i class="bi bi-key-fill ico-lead"></i>
                    <asp:TextBox ID="txtAdminKey" runat="server" TextMode="Password" placeholder="One-time or static admin key"></asp:TextBox>
                    <button type="button" class="btn-eye" id="eyeAdminKey"
                            onclick="togglePwd('<%=txtAdminKey.ClientID%>','eyeAdminKey')">
                        <i class="bi bi-eye"></i>
                    </button>
                </div>
                <p class="field-note"><i class="bi bi-info-circle"></i> Contact your system administrator if you don't have a key.</p>
            </div>

            <!-- ── REGISTER: Designation ── -->
            <div class="field-group" id="adm-desig-group" style="display:none;">
                <label>Designation / Role</label>
                <div class="input-wrap no-eye">
                    <i class="bi bi-briefcase ico-lead"></i>
                    <asp:TextBox ID="txtAdminDesignation" runat="server" placeholder="e.g. GM – IT, HR Head…"></asp:TextBox>
                </div>
            </div>

            <!-- ── REGISTER: Department ── -->
            <div class="field-group" id="adm-dept-group" style="display:none;">
                <label>Department</label>
                <div class="input-wrap no-eye">
                    <i class="bi bi-building ico-lead"></i>
                    <asp:TextBox ID="txtAdminDepartment" runat="server" placeholder="e.g. IT, Finance, HR…"></asp:TextBox>
                </div>
            </div>

           <button type="button"
                id="btnAdminSubmit"
                class="btn-submit"
                onclick="alert('Admin functionality not implemented yet');">
                ADMIN SIGN IN
            </button>

            <!-- Footers -->
            <div class="form-footer" id="adm-footer-login">
                <div class="footer-row">
                    <button type="button" class="link-btn" onclick="switchAdminView('RECOVERY')">Forgot Password?</button>
                    <span>New admin? <button type="button" class="link-btn" onclick="switchAdminView('REGISTER')">Register</button></span>
                </div>
            </div>
            <div class="form-footer" id="adm-footer-register" style="display:none;">
                Already registered? <button type="button" class="link-btn" onclick="switchAdminView('LOGIN')">Admin Log In</button>
            </div>
            <div class="form-footer" id="adm-footer-recovery" style="display:none;">
                Remembered it? <button type="button" class="link-btn" onclick="switchAdminView('LOGIN')">Back to Admin Login</button>
            </div>

            <div class="audit-note" id="adm-audit">
                <i class="bi bi-lock-fill"></i> All access attempts are logged and audited.
            </div>
        </div>

    </div>
</div>
</form>
<script>
// @ts-nocheck

    /* ══════════════════════════════════════════════════════════
       ROLE TAB  (Employee ↔ Admin)
    ══════════════════════════════════════════════════════════ */
    function setRole(role) {
        var isAdmin = (role === 'ADMIN');
        document.getElementById('emp-section').style.display = isAdmin ? 'none' : 'block';
        document.getElementById('adm-section').style.display = isAdmin ? 'block' : 'none';
        document.getElementById('tabEmployee').classList.toggle('active', !isAdmin);
        document.getElementById('tabAdmin').classList.toggle('active', isAdmin);
        var currentRole = role;
    hideAlert();
}

/* ══════════════════════════════════════════════════════════
   EMPLOYEE VIEW  (Login / Register / Recovery)
══════════════════════════════════════════════════════════ */
function switchEmployeeView(mode) {
    show('hdr-login',    mode === 'LOGIN');
    show('hdr-register', mode === 'REGISTER');
    show('hdr-recovery', mode === 'RECOVERY');

    show('emp-pwd-group',     mode !== 'RECOVERY');
    show('emp-confirm-group', mode === 'REGISTER');
    show('emp-email-group',   mode !== 'LOGIN');
    show('emp-dept-group',    mode === 'REGISTER');
    show('reg-acct-type-group', mode === 'REGISTER');

    if (mode === 'REGISTER') {
        setRegAcctType('EMPLOYEE');
    } else {
        show('emp-adminkey-group', false);
        show('emp-desig-group',    false);
    }

    var back = document.getElementById('backBtn');
    if (back) {
        back.style.display = (mode !== 'LOGIN') ? 'flex' : 'none';
    }

    show('emp-footer-login',    mode === 'LOGIN');
    show('emp-footer-register', mode === 'REGISTER');
    show('emp-footer-recovery', mode === 'RECOVERY');

    var labels = { LOGIN: 'LOG IN', REGISTER: 'CREATE ACCOUNT', RECOVERY: 'SEND RESET LINK' };
    var loginBtn = document.getElementById('<%= btnLogin.ClientID %>');
            if (loginBtn) {
                loginBtn.value = labels[mode];
                loginBtn.innerText = labels[mode];
            }
    hideAlert();
}

/* ══════════════════════════════════════════════════════════
   EMPLOYEE REGISTER: Account type (Employee / Admin)
══════════════════════════════════════════════════════════ */
function setRegAcctType(type) {
    var regAccountType = type;
    var isAdmin = (type === 'ADMIN');
    document.getElementById('acctEmployee').classList.toggle('active', !isAdmin);
    document.getElementById('acctAdmin').classList.toggle('active', isAdmin);
    show('emp-adminkey-group', isAdmin);
    show('emp-desig-group', isAdmin);
}

/* ══════════════════════════════════════════════════════════
   ADMIN VIEW  (Login / Register / Recovery)
══════════════════════════════════════════════════════════ */
function switchAdminView(mode) {
    // Headers
    show('adm-hdr-login',    mode === 'LOGIN');
    show('adm-hdr-register', mode === 'REGISTER');
    show('adm-hdr-recovery', mode === 'RECOVERY');

    // Badge only on login
    show('adm-badge', mode === 'LOGIN');

    // Back button
    var back = document.getElementById('admBackBtn');
    if (back) {
        back.style.display = (mode !== 'LOGIN') ? 'flex' : 'none';
    };

    // Field visibility
    show('adm-empid-group',     mode === 'LOGIN');
    show('adm-name-group',      mode === 'REGISTER');
    show('adm-reg-empid-group', mode === 'REGISTER');
    show('adm-email-group',     mode !== 'LOGIN');
    show('adm-pwd-group',       mode !== 'RECOVERY');
    show('adm-confirm-group',   mode === 'REGISTER');
    show('adm-key-group',       mode !== 'RECOVERY');
    show('adm-desig-group',     mode === 'REGISTER');
    show('adm-dept-group',      mode === 'REGISTER');

    // Footers
    show('adm-footer-login',    mode === 'LOGIN');
    show('adm-footer-register', mode === 'REGISTER');
    show('adm-footer-recovery', mode === 'RECOVERY');

    // Audit note only on login
    show('adm-audit', mode === 'LOGIN');

    // Submit label
    var labels = {
        LOGIN: 'ADMIN SIGN IN',
        REGISTER: 'CREATE ADMIN ACCOUNT',
        RECOVERY: 'SEND RESET LINK'
    };

    var adminBtn = document.getElementById('btnAdminSubmit');
    if (adminBtn) {
        adminBtn.innerText = labels[mode];
    }
    hideAlert();
}

/* ══════════════════════════════════════════════════════════
   PASSWORD EYE TOGGLE
══════════════════════════════════════════════════════════ */
function togglePwd(inputId, btnId) {
    var input = document.getElementById(inputId);
    var btn = document.getElementById(btnId);
    if (!input || !btn) return;
    var showing = (input.type === 'text');
    input.type = showing ? 'password' : 'text';
    btn.querySelector('i').className = showing ? 'bi bi-eye' : 'bi bi-eye-slash';
}

/* ══════════════════════════════════════════════════════════
   ALERT HELPERS
══════════════════════════════════════════════════════════ */
function showAlert(msg, type) {
    var box = document.getElementById('alertBox');
    document.getElementById('alertMsg').textContent = msg;
    box.className = 'inline-alert ' + (type || 'error') + ' show';
    box.querySelector('i').className = (type === 'success')
        ? 'bi bi-check-circle-fill' : 'bi bi-exclamation-circle-fill';
}
function hideAlert() {
    document.getElementById('alertBox').className = 'inline-alert error';
}
function portalAlert(msg, type) { showAlert(msg, type); }

function show(id, visible) {
    var el = document.getElementById(id);
    if (el) el.style.display = visible ? 'block' : 'none';
}
</script>
</body>
</html>