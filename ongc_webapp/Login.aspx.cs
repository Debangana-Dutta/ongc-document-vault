using System;
using System.Configuration;
using System.Web.UI;
using Npgsql;

namespace ongc_webapp
{
    /// <summary>
    /// Handles three auth modes driven by the hidden field hdnAuthState:
    ///   LOGIN    — validates credentials, sets session, redirects.
    ///   REGISTER — creates a new portal_users row (role defaults to 'employee').
    ///   RECOVERY — simulates sending a recovery email.
    ///
    /// IMPORTANT: This page intentionally does NOT inherit from BasePage.
    /// BasePage would redirect unauthenticated users, creating an infinite loop.
    /// </summary>
    public partial class Login : Page
    {
        // Reads connection string from Web.config <connectionStrings> section.
        // Key must match exactly: name="PostgresConnection"
        private readonly string _connString =
            ConfigurationManager
                .ConnectionStrings["PostgresConnection"]
                .ConnectionString;

        // ── PAGE LOAD ──────────────────────────────────────────────────────
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // If a valid session already exists, skip the login page entirely.
                // Both keys must be present — UserRole alone is insufficient.
                if (Session["UserRole"] != null && Session["UserCPF"] != null)
                {
                    Response.Redirect("~/Dashboard.aspx", true);
                }
            }
        }

        // ── MAIN BUTTON HANDLER ────────────────────────────────────────────
        // Bound to btnLogin via OnClick="btnLogin_Click" in the .aspx file.
        // The variable names below (txtUsername, txtPassword, etc.) MUST match
        // the ID attributes of the corresponding controls in Login.aspx exactly.
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Read the current auth mode from the hidden field.
            // hdnAuthState is declared in Login.aspx as ID="hdnAuthState"
            string authMode = hdnAuthState.Value ?? "LOGIN";

            // Trim all inputs immediately to avoid whitespace edge cases
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // ── SHARED VALIDATION: Username is required for all modes ──────
            if (string.IsNullOrEmpty(username))
            {
                ShowAlert("Validation Error: Username is required.", "BlankUser");
                return;
            }

            // ── BRANCH: RECOVERY MODE ──────────────────────────────────────
            if (authMode == "RECOVERY")
            {
                // txtCorporateEmail is declared in Login.aspx as ID="txtCorporateEmail"
                string corporateEmail = txtCorporateEmail.Text.Trim();

                if (string.IsNullOrEmpty(corporateEmail))
                {
                    ShowAlert("Please enter your corporate email.", "BlankEmail");
                    return;
                }

                // In a production system, send a real email here.
                // For now, confirm to the user without revealing DB state.
                ShowAlert($"If that account exists, a recovery link has been sent to {corporateEmail}.");
                txtCorporateEmail.Text = "";
                return;
            }

            // ── SHARED VALIDATION: Password required for LOGIN and REGISTER ─
            if (string.IsNullOrEmpty(password))
            {
                ShowAlert("Password cannot be empty.", "BlankPass");
                return;
            }

            // ── BRANCH: REGISTER MODE ──────────────────────────────────────
            if (authMode == "REGISTER")
            {
                // txtConfirmPassword is declared in Login.aspx as ID="txtConfirmPassword"
                string confirmPassword = txtConfirmPassword.Text.Trim();

                if (password != confirmPassword)
                {
                    ShowAlert("Passwords do not match. Please try again.", "PassMismatch");
                    return;
                }

                HandleRegistration(username, password);
                return;
            }

            // ── BRANCH: LOGIN MODE (default) ───────────────────────────────
            HandleLogin(username, password);
        }

        // ── REGISTRATION LOGIC ─────────────────────────────────────────────
        private void HandleRegistration(string username, string password)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connString))
                {
                    conn.Open();

                    // Check for duplicate username (case-insensitive)
                    // Table: portal_users | Column: username
                    const string checkQuery = @"
                        SELECT COUNT(1)
                        FROM public.portal_users
                        WHERE LOWER(username) = LOWER(@Username)";

                    using (NpgsqlCommand checkCmd = new NpgsqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", username);
                        long exists = (long)checkCmd.ExecuteScalar();

                        if (exists > 0)
                        {
                            ShowAlert("That username is already taken. Please choose another.",
                                      "DuplicateUser");
                            return;
                        }
                    }

                    // Insert new user.
                    // Columns used: username, password_hash, employee_name, role, account_status
                    // NOTE: password is stored as plain text here. Replace @Password with a
                    //       BCrypt hash (e.g. BCrypt.Net.BCrypt.HashPassword(password))
                    //       before deploying to production.
                    const string insertQuery = @"
                        INSERT INTO public.portal_users
                            (username, password_hash, employee_name, role, account_status)
                        VALUES
                            (@Username, @Password, @EmployeeName, 'employee', 'Active')";

                    using (NpgsqlCommand insertCmd = new NpgsqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@Username", username);
                        insertCmd.Parameters.AddWithValue("@Password", password);
                        insertCmd.Parameters.AddWithValue("@EmployeeName", username); // default display name
                        insertCmd.ExecuteNonQuery();
                    }
                }

                // Clear sensitive fields after successful registration
                txtPassword.Text = "";
                txtConfirmPassword.Text = "";

                ShowAlert("Account created successfully! Please sign in.", "RegisterSuccess");
            }
            catch (Exception ex)
            {
                // Sanitize the exception message before embedding in JS
                ShowAlert("Registration Error: " + SanitizeForJs(ex.Message), "RegError");
            }
        }

        // ── LOGIN LOGIC ────────────────────────────────────────────────────
        private void HandleLogin(string username, string password)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connString))
                {
                    conn.Open();

                    // Single parameterized query — fetches username, cpf, and role
                    // in one round-trip. account_status check is inline.
                    // Table: portal_users
                    // Columns read: username, cpf, role
                    // Columns filtered: username (LOWER), password_hash, account_status
                    const string loginQuery = @"
                        SELECT username, cpf, role
                        FROM   public.portal_users
                        WHERE  LOWER(username)  = LOWER(@Username)
                        AND    password_hash    = @Password
                        AND    account_status   = 'Active'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(loginQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // ── SET SESSION KEYS ───────────────────────
                                // These exact key names are checked by BasePage.EnforceAccess()
                                // and by other pages throughout the portal.
                                Session["UserID"] = reader["username"].ToString();
                                Session["UserCPF"] = reader["cpf"].ToString();
                                Session["UserRole"] = reader["role"].ToString();
                                Session["LoginTime"] = DateTime.UtcNow.ToString("o");

                                // Redirect based on role so admins land on AdminPanel
                                // and employees land on Dashboard
                                string role = Session["UserRole"].ToString();
                                string redirectTarget = string.Equals(
                                    role, "admin", StringComparison.OrdinalIgnoreCase)
                                    ? "~/AdminPanel.aspx"
                                    : "~/Dashboard.aspx";

                                Response.Redirect(redirectTarget, true);
                            }
                            else
                            {
                                // Generic message — never reveal whether it was the
                                // username or the password that was wrong
                                ShowAlert("Invalid credentials or account inactive. Please try again.",
                                          "LoginError");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Database Error: " + SanitizeForJs(ex.Message), "DBError");
            }
        }

        // ── HELPERS ────────────────────────────────────────────────────────

        /// <summary>
        /// Registers a JavaScript alert via ClientScript.
        /// scriptKey prevents duplicate script registration within a single postback.
        /// </summary>
        private void ShowAlert(string message, string scriptKey = "GenericAlert")
        {
            // Escape the message so single quotes and backslashes don't break the JS string
            string safe = SanitizeForJs(message);
            ClientScript.RegisterStartupScript(
                GetType(),
                scriptKey,
                $"alert('{safe}');",
                addScriptTags: true);
        }

        /// <summary>
        /// Strips characters that would break a single-quoted JavaScript string literal.
        /// </summary>
        private static string SanitizeForJs(string input)
        {
            return input
                .Replace("\\", "\\\\")  // escape backslashes first
                .Replace("'", "\\'")    // escape single quotes
                .Replace("\r", "")      // strip carriage returns
                .Replace("\n", " ");    // replace newlines with spaces
        }
    }
}