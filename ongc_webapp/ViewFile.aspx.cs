using System;
using System.Configuration;
using System.IO;
using Npgsql;

namespace ongc_webapp
{
    public partial class ViewFile :
        System.Web.UI.Page
    {

        private string connString =
            ConfigurationManager
            .ConnectionStrings["PostgresConn"]
            .ConnectionString;

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            string id =
            Server.UrlDecode(
            Request.QueryString["id"]);

            Guid documentId;
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!Guid.TryParse(id, out documentId))
            {
                litViewer.Text =
                    "<h2>Invalid File ID</h2>";

                return;
            }

            if (
                string.IsNullOrWhiteSpace(id)
            )
            {
                litViewer.Text =
                    "<h2>Invalid File ID</h2>";

                return;
            }

            string filePath = "";

            using (
                NpgsqlConnection conn =
                new NpgsqlConnection(connString)
            )
            {
                conn.Open();

                string query = @"
                    SELECT
                        file_path,
                        dataset_name,
                        file_name
                    FROM indexed_documents
                    WHERE id = @id";

                using (
                    NpgsqlCommand cmd =
                    new NpgsqlCommand(query, conn)
                )
                {
                    cmd.Parameters.AddWithValue(
                        "@id",
                        documentId);

                    object result =
                        cmd.ExecuteScalar();

                    if (result != null)
                    {
                        filePath =
                            result.ToString();
                    }
                }
            }

            if (
                string.IsNullOrWhiteSpace(filePath)
            )
            {
                litViewer.Text =
                    "<h2>File path missing</h2>";

                return;
            }

            if (!File.Exists(filePath))
            {
                litViewer.Text =
                    "<h2>Physical file missing</h2>";

                return;
            }

            string fileName =
            Path.GetFileName(filePath);

            AuditLogger.LogActivity(
                Session["UserID"].ToString(),
                "VIEW_DOCUMENT",
                "Viewed document",
                null,
                fileName,
                null);

            string extension =
                Path.GetExtension(filePath)
                .ToLower();

            string encodedPath =
                Server.UrlEncode(filePath);

            // TEXT FILES

            if (extension == ".txt")
            {
                string text =
                    File.ReadAllText(filePath);

                text =
                    Server.HtmlEncode(text);

                litViewer.Text =
                    "<div class='text-viewer'>" +
                    text +
                    "</div>";
            }

            // PDF

            else if (extension == ".pdf")
            {
                litViewer.Text =
                    "<iframe src='RawFile.aspx?id=" +
                    id +
                    "'></iframe>";
            }

            // IMAGES

            else if (
                extension == ".png" ||
                extension == ".jpg" ||
                extension == ".jpeg"
            )
            {
                litViewer.Text =
                    "<div class='image-viewer'>" +

                    "<img src='RawFile.aspx?id=" +
                    id +
                    "' />" +

                    "</div>";
            }

            // OTHER FILES

            else
            {
                litViewer.Text =
                    "<div class='download-box'>" +

                    "<h2>Preview not available</h2>" +

                    "<p>" +
                    Path.GetFileName(filePath) +
                    "</p>" +

                    "<a class='download-btn' " +
                    "href='RawFile.aspx?id=" +
                    id +
                    "'>" +

                    "Open / Download File" +

                    "</a>" +

                    "</div>";
            }
        }
    }
}