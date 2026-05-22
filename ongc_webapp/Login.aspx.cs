using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;

namespace ongc_webapp
{
    public partial class Login : System.Web.UI.Page
    {
        private string connString = ConfigurationManager.ConnectionStrings["PostgresConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    Response.Redirect("Dashboard.aspx");
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string authMode = Request.Form["ctl00$MainContent$hdnAuthState"] ?? "LOGIN";

            if (string.IsNullOrEmpty(username))
            {
                string blankUserScript = "alert('Validation Error: User ID / CPF identification is required.');";
                ClientScript.RegisterStartupScript(this.GetType(), "BlankUser", blankUserScript, true);
                return;
            }

            // ---- BRANCH 1: PASSWORD RECOVERY VIEW STATE ----
            if (authMode == "RECOVERY")
            {
                string corporateEmail = txtCorporateEmail.Text.Trim();
                if (string.IsNullOrEmpty(corporateEmail))
                {
                    string blankEmailScript = "alert('Validation Error: Please supply your registered corporate email routing link.');";
                    ClientScript.RegisterStartupScript(this.GetType(), "BlankEmail", blankEmailScript, true);
                    return;
                }

                // Modern tech mock processing confirmation pipeline logic
                string notificationScript = $"alert('🚀 Security System Token Dispatched! Registry synchronization link sent to {corporateEmail}. Check your inbox.'); switchAuthenticationView('LOGIN');";
                ClientScript.RegisterStartupScript(this.GetType(), "RecoverySuccess", notificationScript, true);

                txtCorporateEmail.Text = "";
                return;
            }

            // Check passwords parameter blocks for both standard LOGIN and REGISTER mode loops
            if (string.IsNullOrEmpty(password))
            {
                string blankPassScript = "alert('Validation Error: Security credential parameter is empty.');";
                ClientScript.RegisterStartupScript(this.GetType(), "BlankPass", blankPassScript, true);
                return;
            }

            // ---- BRANCH 2: REGISTER ACCOUNT STATE ----
            if (authMode == "REGISTER")
            {
                string confirmPassword = txtConfirmPassword.Text.Trim();

                if (password != confirmPassword)
                {
                    string matchScript = "alert('Registration Error: Passwords do not match.');";
                    ClientScript.RegisterStartupScript(this.GetType(), "RegisterMatchError", matchScript, true);
                    return;
                }

                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                    {
                        conn.Open();
                        string checkQuery = "SELECT COUNT(*) FROM public.portal_users WHERE LOWER(username) = LOWER(@Username)";
                        using (NpgsqlCommand checkCmd = new NpgsqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@Username", username);
                            long profileExists = (long)checkCmd.ExecuteScalar();

                            if (profileExists > 0)
                            {
                                string duplicateScript = "alert('Registration Warning: User ID / CPF Number is already registered.');";
                                ClientScript.RegisterStartupScript(this.GetType(), "RegisterDuplicateError", duplicateScript, true);
                                return;
                            }
                        }

                        string insertQuery = @"
                            INSERT INTO public.portal_users (username, password_hash, employee_name, account_status) 
                            VALUES (@Username, @Password, @EmployeeName, 'Active')";

                        using (NpgsqlCommand insertCmd = new NpgsqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@Username", username);
                            insertCmd.Parameters.AddWithValue("@Password", password);
                            insertCmd.Parameters.AddWithValue("@EmployeeName", username);
                            insertCmd.ExecuteNonQuery();
                        }
                    }

                    string successScript = "alert('🚀 Account Created Successfully! You can now log in.'); switchAuthenticationView('LOGIN');";
                    ClientScript.RegisterStartupScript(this.GetType(), "RegisterSuccess", successScript, true);

                    txtPassword.Text = "";
                    txtConfirmPassword.Text = "";
                }
                catch (Exception ex)
                {
                    string errorMsg = ex.Message.Replace("'", "\\'");
                    string exceptionScript = $"alert('Database Registration Engine Exception: {errorMsg}');";
                    ClientScript.RegisterStartupScript(this.GetType(), "RegisterException", exceptionScript, true);
                }
            }
            // ---- BRANCH 3: STANDARD ACCESS VERIFICATION LOGIN ----
            else
            {
                string authQuery = @"
                    SELECT employee_name, account_status 
                    FROM public.portal_users 
                    WHERE LOWER(username) = LOWER(@Username) AND password_hash = @Password";

                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                    {
                        using (NpgsqlCommand cmd = new NpgsqlCommand(authQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", username);
                            cmd.Parameters.AddWithValue("@Password", password);
                            conn.Open();

                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string officialName = reader["employee_name"].ToString();
                                    string status = reader["account_status"].ToString();

                                    if (status != "Active")
                                    {
                                        string inactiveScript = "alert('Access Suspended: This profile has been deactivated.');";
                                        ClientScript.RegisterStartupScript(this.GetType(), "LoginStatusError", inactiveScript, true);
                                        return;
                                    }

                                    Session["UserID"] = officialName;
                                    Session["RawUsername"] = username;
                                    Session["LoginTime"] = DateTime.Now.ToString();

                                    Response.Redirect("Dashboard.aspx", true);
                                }
                                else
                                {
                                    string failedScript = "alert('Access Denied: Invalid Credentials.');";
                                    ClientScript.RegisterStartupScript(this.GetType(), "LoginError", failedScript, true);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMsg = ex.Message.Replace("'", "\\'");
                    string exceptionScript = $"alert('Database Engine Pipeline Exception: {errorMsg}');";
                    ClientScript.RegisterStartupScript(this.GetType(), "LoginException", exceptionScript, true);
                }
            }
        }
    }
}