using System;
using System.Configuration;
using Npgsql;

namespace ongc_webapp
{
    public static class AuditLogger
    {
        public static void LogActivity(
            string username,
            string activityType,
            string details,
            string dataset,
            string fileName,
            string searchQuery)
        {
            string connString =
                ConfigurationManager
                .ConnectionStrings["PostgresConn"]
                .ConnectionString;

            using (NpgsqlConnection conn =
                new NpgsqlConnection(connString))
            {
                conn.Open();

                string sql = @"
                INSERT INTO user_activity_logs
                (
                    username,
                    activity_type,
                    activity_details,
                    dataset_name,
                    file_name,
                    search_query
                )
                VALUES
                (
                    @username,
                    @type,
                    @details,
                    @dataset,
                    @file,
                    @search
                )";

                using (NpgsqlCommand cmd =
                    new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("username",
                        (object)username ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("type",
                        activityType);

                    cmd.Parameters.AddWithValue("details",
                        (object)details ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("dataset",
                        (object)dataset ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("file",
                        (object)fileName ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("search",
                        (object)searchQuery ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}