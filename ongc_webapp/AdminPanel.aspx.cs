// ============================================================
//  AdminPanel.aspx.cs
//  ONGC Document Portal – Admin Panel Code-Behind
//
//  KEY FIX (2025-05-29):
//  The real schema for user_dataset_access is:
//    userid   INTEGER  (FK → users.id)
//    datasetid TEXT    (stores source_excel_file value)
//
//  The dropdown value in C# is the user's CPF (a string).
//  Every query against user_dataset_access therefore uses a
//  subquery:   (SELECT id FROM users WHERE cpf = @cpf)
//  to resolve the integer userid — no unsafe cast required.
//
//  user_metadata_policy still uses user_cpf TEXT, unchanged.
// ============================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using Npgsql;
using Newtonsoft.Json;

namespace ongc_webapp
{
    public partial class AdminPanel : System.Web.UI.Page
    {
        private bool ShowPendingOnly
        {
            get
            {
                return ViewState["ShowPendingOnly"] != null
                    && (bool)ViewState["ShowPendingOnly"];
            }
            set
            {
                ViewState["ShowPendingOnly"] = value;
            }
        }

        protected void btnSavePassword_Click(
                    object sender,
                    EventArgs e)
                {
                    try
                    {
                    string username =
                    hfResetUsername.Value;

                    string newPassword =
                    txtNewPassword.Text.Trim();

                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    ShowFeedback(
                        lblStatusFeedback,
                        "Password cannot be empty.",
                        false);

                    return;
                }

                using (NpgsqlConnection conn =
                    new NpgsqlConnection(connString))
                {
                    conn.Open();

                    string query =
                       @"UPDATE users
                      SET password_hash=@pwd
                      WHERE username=@username";

                    using (NpgsqlCommand cmd =
                        new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "pwd",
                            newPassword);

                        cmd.Parameters.AddWithValue(
                            "username",
                            username);

                        cmd.ExecuteNonQuery();
                    }
                }

                pnlPasswordReset.Visible =
                    false;

                ShowFeedback(
                    lblStatusFeedback,
                    "Password updated successfully.",
                    true);
            }
            catch (Exception ex)
            {
                ShowFeedback(
                    lblStatusFeedback,
                    ex.Message,
                    false);
            }
        }

        protected void btnCancelPassword_Click(
        object sender,
        EventArgs e)
        {
            pnlPasswordReset.Visible =
                false;
        }

        private string connString =
            System.Configuration.ConfigurationManager
                .ConnectionStrings["PostgresConn"]
                .ConnectionString;

        // ════════════════════════════════════════════════════════
        //  PAGE LOAD
        // ════════════════════════════════════════════════════════
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // NOT ADMIN
            if (Session["Role"] == null ||
                Session["Role"].ToString().ToUpper() != "ADMIN")
            {
                Response.Redirect("Dashboard.aspx");
                return;
            }

            if(!IsPostBack)
            {
                BindUserAccessGrid();

            }
        }

            


        protected void gvUserAccess_RowCommand(
                        object sender,
                        GridViewCommandEventArgs e)
        {
            string username =
                e.CommandArgument.ToString();

            if (e.CommandName == "ManageAccess")
            {
                Response.Redirect(
                    "ManageUserAccess.aspx?userid=" +
                    e.CommandArgument);

                return;
            }

            if (e.CommandName == "ResetPassword")
            {
                hfResetUsername.Value =
                    username;

                txtNewPassword.Text = "";

                pnlPasswordReset.Visible =
                    true;

                return;
            }

            if (e.CommandName == "ToggleStatus")
            {
                using (NpgsqlConnection conn =
                    new NpgsqlConnection(connString))
                {
                    conn.Open();

                    string query =
                    @"
                    UPDATE users
                    SET account_status =
                        CASE
                            WHEN account_status = 'APPROVED'
                                THEN 'REJECTED'
                            ELSE 'APPROVED'
                        END
                    WHERE username = @username";

                    using (NpgsqlCommand cmd =
                        new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "username",
                            username);

                        cmd.ExecuteNonQuery();
                    }
                }

                BindUserAccessGrid();

                return;
            }
        }

        

        // ════════════════════════════════════════════════════════
        //  1. USER MANAGEMENT
        // ════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════
        //  2. POLICY PANEL — populate dropdowns & checkboxlists
        // ════════════════════════════════════════════════════════

       

        // Populates datasets from distinct source_excel_file values
        // DB table: indexed_documents
        

        // Populates metadata column names by inspecting JSONB keys
        // DB table: indexed_documents → dynamic_metadata
        

        // ════════════════════════════════════════════════════════
        //  3. LOAD EXISTING POLICY (admin selects a user)
        // ════════════════════════════════════════════════════════



        // ════════════════════════════════════════════════════════
        //  4. SAVE ACCESS POLICY
        //
        //  FIX SUMMARY for user_dataset_access:
        //    • DELETE uses:  WHERE userid = (SELECT id FROM users WHERE cpf = @cpf)
        //    • INSERT uses:  (SELECT id FROM users WHERE cpf = @cpf), @datasetid
        //    • Column name is "datasetid", not "dataset"
        //    • No type cast required — subquery returns INTEGER naturally
        //
        //  user_metadata_policy is unchanged (user_cpf TEXT = CPF string directly).
        // ════════════════════════════════════════════════════════

        private void BindUserAccessGrid()
        {
            using (NpgsqlConnection conn =
                new NpgsqlConnection(connString))
            {
                conn.Open();

                string whereClause = "";

                if (ShowPendingOnly)
                {
                    whereClause =
                        "WHERE u.account_status = 'PENDING'";
                }

                string query =
                $@"
                    SELECT
                        u.id,
                        u.username,
                        u.role,
                        u.account_status,
                        COUNT(uda.dataset_name) AS dataset_count
                    FROM users u
                    LEFT JOIN user_dataset_access uda
                        ON uda.userid = u.id
                    {whereClause}
                    GROUP BY
                        u.id,
                        u.username,
                        u.role,
                        u.account_status
                    ORDER BY u.username";

                using (NpgsqlDataAdapter da =
                    new NpgsqlDataAdapter(query, conn))
                {
                    DataTable dt =
                        new DataTable();

                    da.Fill(dt);

                    gvUserAccess.DataSource = dt;
                    gvUserAccess.DataBind();
                }
            }
        }



        // ════════════════════════════════════════════════════════
        //  5. DOCUMENT INGESTION
        // ════════════════════════════════════════════════════════

        protected void btnIngestData_Click(object sender, EventArgs e)
        {

            string datasetName =
            txtDatasetName.Text.Trim();

            if (string.IsNullOrWhiteSpace(datasetName))
            {
                ShowFeedback(
                    lblStatusFeedback,
                    "Please enter a dataset name.",
                    false);

                return;
            }

            if (!filePayload.HasFiles)
            {
                ShowFeedback(lblStatusFeedback, "⚠️ Please upload Excel files.", false);
                return;
            }

            int successCount = 0;
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    using (NpgsqlCommand dsCmd =
                    new NpgsqlCommand(
                        @"INSERT INTO datasets(dataset_name)
                          VALUES(@name)
                          ON CONFLICT(dataset_name)
                          DO NOTHING",
                        conn))
                    {
                        dsCmd.Parameters.AddWithValue(
                            "name",
                            datasetName);

                        dsCmd.ExecuteNonQuery();
                    }

                    foreach (System.Web.HttpPostedFile uploadedFile
                        in filePayload.PostedFiles)
                    {

                        using (NpgsqlCommand deleteCmd = new NpgsqlCommand(
                            "DELETE FROM indexed_documents WHERE source_excel_file = @sef",
                            conn))
                        {
                            deleteCmd.Parameters.AddWithValue(
                                "sef",
                                uploadedFile.FileName);

                            deleteCmd.ExecuteNonQuery();
                        }

                        using (var workbook = new XLWorkbook(uploadedFile.InputStream))
                        {
                            var worksheet = workbook.Worksheet(1);

                            if (worksheet.LastRowUsed() == null)
                                continue;

                            int lastRow = worksheet.LastRowUsed().RowNumber();
                            int lastColumn = worksheet.LastColumnUsed().ColumnNumber();

                            List<string> headers = new List<string>();
                            for (int col = 1; col <= lastColumn; col++)
                                headers.Add(worksheet.Cell(1, col).GetValue<string>()
                                    .Trim().ToLower().Replace(" ", "_"));

                            for (int row = 2; row <= lastRow; row++)
                            {
                                string actualFileName = "", actualFilePath = "";
                                var dynamicMetadata = new Dictionary<string, string>();

                                for (int col = 1; col <= lastColumn; col++)
                                {
                                    string val = worksheet.Cell(row, col)
                                        .GetValue<string>().Trim();
                                    if (string.IsNullOrWhiteSpace(val)) continue;

                                    if (headers[col - 1] == "file_name")
                                        actualFileName = val;
                                    else if (headers[col - 1] == "file_path" ||
                                             headers[col - 1] == "path")
                                        actualFilePath = val;
                                    else
                                        dynamicMetadata[headers[col - 1]] = val;
                                }

                                if (!string.IsNullOrWhiteSpace(actualFileName) &&
                                    !string.IsNullOrWhiteSpace(actualFilePath))
                                {
                                    using (NpgsqlCommand cmd = new NpgsqlCommand(
                                        "INSERT INTO indexed_documents " +
                                        "(\r\n    file_name,\r\n    file_path,\r\n    source_excel_file,\r\n    dataset_name,\r\n    dynamic_metadata\r\n) " +
                                        "VALUES (\r\n    @fn,\r\n    @fp,\r\n    @sef,\r\n    @dataset,\r\n    @meta::jsonb\r\n)", conn))
                                    {
                                        cmd.Parameters.AddWithValue("fn", actualFileName);
                                        cmd.Parameters.AddWithValue("fp", actualFilePath);
                                        cmd.Parameters.AddWithValue("sef", uploadedFile.FileName);
                                        cmd.Parameters.AddWithValue("meta",
                                            JsonConvert.SerializeObject(dynamicMetadata));

                                        cmd.Parameters.AddWithValue(
                                        "dataset",
                                        datasetName);

                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        successCount++;
                    }
                }
                
                ShowFeedback(lblStatusFeedback,
                    successCount + " file(s) ingested successfully.", true);
            }
            catch (Exception ex)
            {
                ShowFeedback(lblStatusFeedback, "Ingestion failed: " + ex.Message, false);
            }
        }

        // ════════════════════════════════════════════════════════ 
        //  HELPERS
        // 

        private void ShowFeedback(Label label, string message, bool success)
        {
            label.Text = message;
            label.ForeColor = success
                ? System.Drawing.Color.MediumSeaGreen
                : System.Drawing.Color.Crimson;
        }

        protected void btnShowAllUsers_Click(
                        object sender,
                        EventArgs e)
        {
            ShowPendingOnly = false;
            BindUserAccessGrid();
        }

        protected void btnShowPending_Click(
            object sender,
            EventArgs e)
        {
            ShowPendingOnly = true;
            BindUserAccessGrid();
        }


    }
}
