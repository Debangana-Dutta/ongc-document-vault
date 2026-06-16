﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ongc_webapp.Register" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ONGC - Register</title>
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

        .visual-panel {
            flex: 1.2;
            position: relative;
            background: url('ongc_refinery.jpg') center 25% / cover no-repeat;
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


        .form-panel {
            width: 520px;
            max-width: 100%;
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


        .field-group {
            margin-bottom: 20px;
            width: 100%;
        }

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
            width: 100%;
        }

        .input-wrap input,
        .input-wrap select {
            width: 100%;
            min-width: 0;
            padding: 13px 46px 13px 42px;
            border: 1.5px solid var(--border);
            border-radius: 9px;
            background: var(--input-bg);
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
            cursor: pointer; margin-top: 20px;
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
            <div class="visual-panel">
                <div class="visual-top">
                </div>

                <div class="visual-bottom">
                    <h1>Powering<br>India's Future</h1>
                    <p>
                        Oil and Natural Gas Corporation
                        <br>
                        Employee Enterprise Portal
                    </p>
                </div>
            </div>
            <div class="form-panel">
                <div class="sec-head">
                    <h2>Create Account</h2>
                    <p>Register your ONGC portal account</p>
                </div>
                <div class="inline-alert error">
                    <asp:Label ID="lblStatus"
                        runat="server" />
                </div>

                <div class="field-group">
                    <label>Full Name</label>

                    <div class="input-wrap no-eye">
                        <i class="bi bi-person-badge ico-lead"></i>

                        <asp:TextBox
                            ID="txtEmployeeName"
                            runat="server"
                            placeholder="Enter full name">
                        </asp:TextBox>
                    </div>
                </div>
                <div class="field-group">
                    <label>Department</label>

                    <div class="input-wrap no-eye">
                        <i class="bi bi-building ico-lead"></i>

                        <asp:TextBox
                            ID="txtDepartment"
                            runat="server"
                            placeholder="Enter department">
                        </asp:TextBox>
                    </div>
                </div>
                <div class="field-group">
                    <label>CPF Number</label>

                    <div class="input-wrap no-eye">
                        <i class="bi bi-person ico-lead"></i>

                        <asp:TextBox
                            ID="txtUsername"
                            runat="server"
                            placeholder="Enter CPF">
                        </asp:TextBox>
                    </div>
                </div>
                <div class="field-group">
                    <label>Password</label>

                    <div class="input-wrap no-eye">
                        <i class="bi bi-lock ico-lead"></i>

                        <asp:TextBox
                            ID="txtPassword"
                            runat="server"
                            TextMode="Password"
                            placeholder="Enter password">
                        </asp:TextBox>

                    </div>
                </div>

                <div class="field-group">
                    <label>Confirm Password</label>

                    <div class="input-wrap no-eye">
                        <i class="bi bi-lock ico-lead"></i>

                        <asp:TextBox
                            ID="txtConfirmPassword"
                            runat="server"
                            TextMode="Password"
                            placeholder="Confirm password">
                        </asp:TextBox>
                    </div>
                </div>

                <asp:Button ID="btnRegister"
                    runat="server"
                    Text="CREATE ACCOUNT"
                    CssClass="btn-submit"
                    OnClick="btnRegister_Click" />

                <div class="form-footer">
                    Already have an account?
                    <a href="Login.aspx" class="link-btn">
                        Log In
                    </a>
                </div>
            </div>
        </div>
    </form>




</body>
</html>