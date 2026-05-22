using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using Npgsql;
using ExcelDataReader;

namespace ongc_webapp
{
    public partial class Upload : System.Web.UI.Page
    {
        private readonly string connString =
            System.Configuration.ConfigurationManager.ConnectionStrings["PostgresConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null) Response.Redirect("Login.aspx", true);
        }

        protected void btnRunIngestion_Click(object sender, EventArgs e)
        {
            lblStatusOutput.Visible = false;
            string tag = txtDepartmentTag.Text.Trim();

            if (!fileExcelPayload.HasFile && string.IsNullOrEmpty(tag)) return;

            try
            {
                string rawFileName = fileExcelPayload.HasFile ? Path.GetFileName(fileExcelPayload.FileName) : "Manual_Entry_Template";
                string virtualPath = fileExcelPayload.HasFile ? "~/vault/uploads/" + rawFileName : "Manual_Ingest_Vector";

                SniperParseResult parseResult;

                if (fileExcelPayload.HasFile)
                {
                    parseResult = RunHeaderSniper(fileExcelPayload.PostedFile.InputStream);
                }
                else
                {
                    // Fallback framework setup to allow graceful compilation matches
                    parseResult = new SniperParseResult
                    {
                        Headers = new List<string> { "Column_1", "Column_2" },
                        DataRows = new List<List<string>> { new List<string> { "", "" } }
                    };
                }

                var envelope = new Dictionary<string, object> {
                    { "headers", parseResult.Headers }, { "rows", parseResult.DataRows }
                };

                string json = new JavaScriptSerializer { MaxJsonLength = 20971520 }.Serialize(envelope);
                string sql = "INSERT INTO public.indexed_documents (id, file_name, file_path, dynamic_metadata, uploaded_at) VALUES (uuid_generate_v4(), @Name, @Path, @Json::jsonb, NOW());";

                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", rawFileName);
                        cmd.Parameters.AddWithValue("@Path", virtualPath);
                        cmd.Parameters.AddWithValue("@Json", json);
                        cmd.ExecuteNonQuery();
                    }
                }

                lblStatusOutput.Text = "🚀 Master repository ingestion sync complete!";
                lblStatusOutput.Visible = true;
            }
            catch (Exception ex)
            {
                lblStatusOutput.Text = "❌ Error: " + ex.Message;
                lblStatusOutput.Visible = true;
            }
        }

        private SniperParseResult RunHeaderSniper(Stream stream)
        {
            var res = new SniperParseResult();
            using (var rdr = ExcelReaderFactory.CreateReader(stream))
            {
                bool hLocked = false;
                while (rdr.Read())
                {
                    var row = new List<string>();
                    bool hasVal = false;
                    for (int c = 0; c < rdr.FieldCount; c++)
                    {
                        string v = rdr.GetValue(c)?.ToString()?.Trim() ?? string.Empty;
                        row.Add(v); if (!string.IsNullOrEmpty(v)) hasVal = true;
                    }
                    if (!hasVal) continue;

                    if (!hLocked) { res.Headers = row; hLocked = true; }
                    else { res.DataRows.Add(row); }
                }
            }
            return res;
        }
    }

    // Explicitly nested here so the compiler maps the namespace references automatically
    public class SniperParseResult
    {
        public List<string> Headers { get; set; } = new List<string>();
        public List<List<string>> DataRows { get; set; } = new List<List<string>>();
    }
}