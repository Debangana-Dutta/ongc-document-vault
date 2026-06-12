using System;
using System.Collections.Generic;
using System.Configuration;
using Newtonsoft.Json;
using Npgsql;

namespace ongc_webapp
{
    public partial class SearchSuggestions :
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
            Response.ContentType =
                "application/json";

            string q =
                Request.QueryString["q"];

            if (string.IsNullOrWhiteSpace(q))
            {
                Response.Write("[]");
                Response.End();
                return;
            }

            List<string> suggestions =
                new List<string>();

            using (NpgsqlConnection conn =
                new NpgsqlConnection(connString))
            {
                conn.Open();

                string metadataSql =
                    @"
                    (
                        SELECT DISTINCT file_name AS value
                        FROM indexed_documents
                        WHERE file_name ILIKE @q
                        AND LENGTH(file_name) > 2
                    )
                    UNION
                    (
                        SELECT DISTINCT value
                        FROM
                        (
                            SELECT jsonb_each_text(dynamic_metadata) AS kv
                            FROM indexed_documents
                        ) x,
                        LATERAL
                        (
                            SELECT (kv).value
                        ) y(value)
                        WHERE value ILIKE @q
                        AND LENGTH(value) > 2
                    )
                    LIMIT 10";

                using (NpgsqlCommand cmd =
                    new NpgsqlCommand(
                        metadataSql,
                        conn))
                {
                    cmd.Parameters.AddWithValue(
                        "q",
                        q + "%");

                    using (NpgsqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string value =
                                reader[0].ToString();

                            if (!suggestions.Contains(value))
                            {
                                suggestions.Add(value);
                            }
                        }
                    }
                }
            }

            Response.Write(
                JsonConvert.SerializeObject(
                    suggestions));

            Response.End();
        }
    }
}