using System;
using System.Configuration;
using System.Web.UI;
using Npgsql;

namespace ongc_webapp
{
    public partial class Login : System.Web.UI.Page
    {
        private string connString =
            ConfigurationManager
            .ConnectionStrings["PostgresConnection"]
            .ConnectionString;

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
            string authMode = "LOGIN";
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // USERNAME VALIDATION
            if (string.IsNullOrEmpty(username))
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "BlankUser",
                    "alert('Validation Error: Username is required.');",
                    true
                );

                return;
            }

            // PASSWORD RECOVERY MODE
            if (authMode == "RECOVERY")
            {
                string corporateEmail =
                    txtCorporateEmail.Text.Trim();

                if (string.IsNullOrEmpty(corporateEmail))
                {
                    ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "BlankEmail",
                        "alert('Please enter corporate email.');",
                        true
                    );

                    return;
                }

                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "RecoverySuccess",
                    $"alert('Recovery link sent to {corporateEmail}');",
                    true
                );

                txtCorporateEmail.Text = "";

                return;
            }

            // PASSWORD VALIDATION
            if (string.IsNullOrEmpty(password))
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "BlankPass",
                    "alert('Password cannot be empty.');",
                    true
                );

                return;
            }

            // REGISTER MODE
            if (authMode == "REGISTER")
            {
                string confirmPassword =
                    txtConfirmPassword.Text.Trim();
                string role = "EMPLOYEE";


                if (password != confirmPassword)
                {
                    ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "PassMismatch",
                        "alert('Passwords do not match.');",
                        true
                    );

                    return;
                }

                try
                {
                    using (NpgsqlConnection conn =
                        new NpgsqlConnection(connString))
                    {
                        conn.Open();

                        // CHECK IF USER EXISTS
                        string checkQuery = @"
                            SELECT COUNT(*)
                            FROM public.users
                            WHERE LOWER(username) = LOWER(@Username)";

                        using (NpgsqlCommand checkCmd =
                            new NpgsqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue(
                                "@Username",
                                username
                            );

                            long exists =
                                (long)checkCmd.ExecuteScalar();

                            if (exists > 0)
                            {
                                ClientScript.RegisterStartupScript(
                                    this.GetType(),
                                    "DuplicateUser",
                                    "alert('Username already exists.');",
                                    true
                                );

                                return;
                            }
                        }

                        // INSERT NEW USER
                        string insertQuery = @"
                            INSERT INTO public.users
                            (
                                username,
                                password_hash,
                                employee_name,
                                account_status,
                                role
                            )
                            VALUES
                            (
                                @Username,
                                @Password,
                                @EmployeeName,
                                'PENDING',
                                @Role
                            )";

                        using (NpgsqlCommand insertCmd =
                            new NpgsqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue(
                                "@Username",
                                username
                            );

                            insertCmd.Parameters.AddWithValue(
                                "@Password",
                                password
                            );

                            insertCmd.Parameters.AddWithValue(
                                "@EmployeeName",
                                username
                            );

                            insertCmd.Parameters.AddWithValue(
                                "@Role",
                                role
                            );

                            insertCmd.ExecuteNonQuery();
                        }
                    }

                    ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "RegisterSuccess",
                        "alert('Account created successfully.');",
                        true
                    );

                    txtPassword.Text = "";
                    txtConfirmPassword.Text = "";
                }
                catch (Exception ex)
                {
                    string msg =
                        ex.Message.Replace("'", "");

                    Response.Write(
                        "<script>alert('Registration Error: "
                        + msg +
                        "');</script>"
                    );
                }
            }

            // LOGIN MODE
            else
            {
                try
                {
                    using (NpgsqlConnection conn =
                        new NpgsqlConnection(connString))
                    {
                        conn.Open();

                        string loginQuery = @"
                            SELECT username, role, account_status
                            FROM public.users
                            WHERE LOWER(username) = LOWER(@Username)
                            AND password_hash = @Password";

                        using (NpgsqlCommand cmd =
                            new NpgsqlCommand(loginQuery, conn))
                        {
                            cmd.Parameters.AddWithValue(
                                "@Username",
                                username
                            );

                            cmd.Parameters.AddWithValue(
                                "@Password",
                                password
                            );

                            using (NpgsqlDataReader reader =
                                cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string accountStatus =
                                        reader["account_status"].ToString();

                                    if (accountStatus == "PENDING")
                                    {
                                        ClientScript.RegisterStartupScript(
                                            this.GetType(),
                                            "PendingApproval",
                                            "alert('Your account is awaiting administrator approval.');",
                                            true
                                        );

                                        return;
                                    }

                                    if (accountStatus == "REJECTED")
                                    {
                                        ClientScript.RegisterStartupScript(
                                            this.GetType(),
                                            "RejectedAccount",
                                            "alert('Your account has been rejected. Please contact an administrator.');",
                                            true
                                        );

                                        return;
                                    }

                                    Session["UserID"] =
                                        reader["username"].ToString();

                                    AuditLogger.LogActivity(
                                        username,
                                        "LOGIN",
                                        "User logged in",
                                        null,
                                        null,
                                        null);

                                    Session["Username"] =
                                        reader["username"].ToString();

                                    Session["Role"] =
                                        reader["role"].ToString();

                                    Session["LoginTime"] =
                                        DateTime.Now.ToString();

                                    Response.Redirect("Dashboard.aspx");
                                }
                                else
                                {
                                    ClientScript.RegisterStartupScript(
                                        this.GetType(),
                                        "LoginError",
                                        "alert('Invalid username or password.');",
                                        true
                                    );
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg =
                        ex.Message.Replace("'", "");

                    Response.Write(
                        "<script>alert('Database Error: "
                        + msg +
                        "');</script>"
                    );
                }
            }
        }
    }
}