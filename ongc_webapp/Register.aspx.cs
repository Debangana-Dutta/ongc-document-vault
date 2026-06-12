using System;
using System.Configuration;
using Npgsql;

namespace ongc_webapp
{
    public partial class Register : System.Web.UI.Page
    {
        string connString =
            ConfigurationManager.ConnectionStrings["PostgresConnection"]
            .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRegister_Click(
            object sender,
            EventArgs e)
        {
            string employeeName =
                txtEmployeeName.Text.Trim();

            string cpf = txtUsername.Text.Trim();

            string department =
                txtDepartment.Text.Trim();

            string username =
                txtUsername.Text.Trim();

            if (!long.TryParse(username, out _))
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "InvalidCPF",
                    "alert('CPF must contain only numbers.');",
                    true
                );

                return;
            }

            string password =
                txtPassword.Text.Trim();

            string confirmPassword =
                txtConfirmPassword.Text.Trim();

            if (password != confirmPassword)
            {
                lblStatus.Text =
                    "Passwords do not match.";
                return;
            }

            try
            {
                using (NpgsqlConnection conn =
                    new NpgsqlConnection(connString))
                {
                    conn.Open();

                    string checkQuery =
                        @"SELECT COUNT(*)
                          FROM users
                          WHERE LOWER(username)
                          = LOWER(@Username)";

                    using (NpgsqlCommand checkCmd =
                        new NpgsqlCommand(
                            checkQuery,
                            conn))
                    {
                        checkCmd.Parameters.AddWithValue(
                            "@Username",
                            username);

                        long exists =
                            Convert.ToInt64(
                                checkCmd.ExecuteScalar());

                        if (exists > 0)
                        {
                            lblStatus.Text =
                                "Username already exists.";
                            return;
                        }
                    }

                    string insertQuery =
                        @"INSERT INTO users
                        (
                            username,
                            password_hash,
                            cpf,
                            department,
                            employee_name,
                            role,
                            account_status,
                            created_at
                        )
                        VALUES
                        (
                            @Username,
                            @Password,
                            @CPF,
                            @Department,
                            @EmployeeName,
                            'EMPLOYEE',
                            'PENDING',
                            NOW()
                        )";

                    using (NpgsqlCommand cmd =
                        new NpgsqlCommand(
                            insertQuery,
                            conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Username",
                            username);

                        cmd.Parameters.AddWithValue(
                            "@Password",
                            password);

                        cmd.Parameters.AddWithValue(
                            "@CPF",
                            cpf);

                        cmd.Parameters.AddWithValue(
                            "@Department",
                            department);

                        cmd.Parameters.AddWithValue(
                            "@EmployeeName",
                            employeeName);

                        cmd.ExecuteNonQuery();
                    }
                }

                Response.Redirect(
                    "Login.aspx?registered=1");
            }
            catch (Exception ex)
            {
                lblStatus.Text =
                    ex.Message;
            }
        }
    }
}