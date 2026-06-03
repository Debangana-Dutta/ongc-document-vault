using System;
using System.Collections.Generic;
using System.Web.UI;
using ClosedXML.Excel;
using Npgsql;
using Newtonsoft.Json;

namespace ongc_webapp
{
    public partial class Upload : System.Web.UI.Page
    {
        private string connString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["PostgresConn"]
            .ConnectionString;

        protected void Page_Load(
            object sender,
            EventArgs e)
        {

        }

        protected void btnIngestData_Click(
            object sender,
            EventArgs e)
        {
            if (!filePayload.HasFiles)
            {
                lblStatusFeedback.Text =
                    "⚠️ Please upload Excel files.";

                lblStatusFeedback.ForeColor =
                    System.Drawing.Color.OrangeRed;

                return;
            }

            int successCount = 0;

            try
            {
                using (
                    NpgsqlConnection conn =
                    new NpgsqlConnection(connString)
                )
                {
                    conn.Open();

                    using (
                        NpgsqlTransaction transaction =
                        conn.BeginTransaction()
                    )
                    {
                        foreach (
                            System.Web.HttpPostedFile uploadedFile
                            in filePayload.PostedFiles
                        )
                        {
                            if (
                                uploadedFile == null ||
                                uploadedFile.ContentLength == 0
                            )
                            {
                                continue;
                            }

                            string sourceExcelFile =
                                uploadedFile.FileName;

                            using (
                                var stream =
                                uploadedFile.InputStream
                            )

                            using (
                                var workbook =
                                new XLWorkbook(stream)
                            )
                            {
                                var worksheet =
                                    workbook.Worksheet(1);

                                int headerRow = 1;

                                int lastRow =
                                    worksheet
                                    .LastRowUsed()
                                    .RowNumber();

                                int lastColumn =
                                    worksheet
                                    .LastColumnUsed()
                                    .ColumnNumber();

                                List<string> headers =
                                    new List<string>();

                                for (
                                    int col = 1;
                                    col <= lastColumn;
                                    col++
                                )
                                {
                                    string header =
                                        worksheet
                                        .Cell(headerRow, col)
                                        .GetValue<string>()
                                        .Trim()
                                        .ToLower()
                                        .Replace(" ", "_");

                                    headers.Add(header);
                                }

                                for (
                                    int row = headerRow + 1;
                                    row <= lastRow;
                                    row++
                                )
                                {
                                    string actualFileName = "";
                                    string actualFilePath = "";

                                    var dynamicMetadata =
                                        new Dictionary<string, string>();

                                    for (
                                        int col = 1;
                                        col <= lastColumn;
                                        col++
                                    )
                                    {
                                        string header =
                                            headers[col - 1];

                                        string cellValue =
                                            worksheet
                                            .Cell(row, col)
                                            .GetValue<string>()
                                            .Trim();

                                        if (
                                            string.IsNullOrWhiteSpace(
                                                cellValue)
                                        )
                                        {
                                            continue;
                                        }

                                        if (
                                            header == "file_name"
                                        )
                                        {
                                            actualFileName =
                                                cellValue;
                                        }

                                        else if (
                                            header == "file_path" ||
                                            header == "path"
                                        )
                                        {
                                            actualFilePath =
                                                cellValue;
                                        }

                                        else
                                        {
                                            dynamicMetadata[header] =
                                                cellValue;
                                        }
                                    }

                                    if (
                                        string.IsNullOrWhiteSpace(
                                            actualFileName)
                                        ||

                                        string.IsNullOrWhiteSpace(
                                            actualFilePath)
                                    )
                                    {
                                        continue;
                                    }

                                    string metadataJson =
                                        JsonConvert.SerializeObject(
                                            dynamicMetadata);

                                    string insertQuery = @"
                                        INSERT INTO indexed_documents
                                        (
                                            file_name,
                                            file_path,
                                            source_excel_file,
                                            dynamic_metadata,
                                            extracted_text
                                        )
                                        VALUES
                                        (
                                            @file_name,
                                            @file_path,
                                            @source_excel_file,
                                            @meta::jsonb,
                                            ''
                                        )";

                                    using (
                                        NpgsqlCommand cmd =
                                        new NpgsqlCommand(
                                            insertQuery,
                                            conn)
                                    )
                                    {
                                        cmd.Transaction =
                                            transaction;

                                        cmd.Parameters.AddWithValue(
                                            "file_name",
                                            actualFileName);

                                        cmd.Parameters.AddWithValue(
                                            "file_path",
                                            actualFilePath);

                                        cmd.Parameters.AddWithValue(
                                            "source_excel_file",
                                            sourceExcelFile);

                                        cmd.Parameters.AddWithValue(
                                            "meta",
                                            metadataJson);

                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            successCount++;
                        }

                        transaction.Commit();
                    }
                }

                lblStatusFeedback.Text =
                    successCount +
                    " Excel files uploaded successfully.";

                lblStatusFeedback.ForeColor =
                    System.Drawing.Color.MediumSeaGreen;
            }
            catch (Exception ex)
            {
                lblStatusFeedback.Text =
                    "Upload failed: " + ex.Message;

                lblStatusFeedback.ForeColor =
                    System.Drawing.Color.Red;
            }
        }
    }
}