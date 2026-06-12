using System;
using System.Configuration;
using System.IO;
using Npgsql;

namespace ongc_webapp
{
    public partial class RawFile :
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
                Request.QueryString["id"];

            if (
                string.IsNullOrWhiteSpace(id)
            )
            {
                return;
            }

            string filePath = "";
            string datasetName = "";
            string fileName = "";

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
                        Guid.Parse(id));

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            filePath =
                                reader["file_path"].ToString();

                            datasetName =
                                reader["dataset_name"].ToString();

                            fileName =
                                reader["file_name"].ToString();
                        }
                    }
                }
            }

            if (
                string.IsNullOrWhiteSpace(filePath)
            )
            {
                return;
            }

            if (!File.Exists(filePath))
            {
                return;
            }

            if (Session["UserID"] != null)
            {
                AuditLogger.LogActivity(
                    Session["UserID"].ToString(),
                    "DOWNLOAD_DOCUMENT",
                    "Downloaded document",
                    datasetName,
                    fileName,
                    null);
            }

            Response.Clear();

            string extension =
                Path.GetExtension(filePath)
                .ToLower();

            switch (extension)
            {
                case ".txt":
                    Response.ContentType =
                        "text/plain";
                    break;

                case ".pdf":
                    Response.ContentType =
                        "application/pdf";
                    break;

                case ".png":
                    Response.ContentType =
                        "image/png";
                    break;

                case ".jpg":
                case ".jpeg":
                    Response.ContentType =
                        "image/jpeg";
                    break;

                default:
                    Response.ContentType =
                        "application/octet-stream";
                    break;
            }

            Response.WriteFile(filePath);

            Response.End();
        }
    }
}