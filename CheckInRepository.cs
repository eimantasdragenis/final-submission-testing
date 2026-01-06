using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace PSEngagementSystem
{
    public static class CheckInRepository
    {
        public static string ConnectionString = "Data Source=ps_engagement.db";

        public static void InitializeDatabase()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS CheckIns (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                StudentID INTEGER NOT NULL,
                Mood INTEGER NOT NULL,
                Date TEXT NOT NULL,
                Comment TEXT
            );
            ";
            command.ExecuteNonQuery();

            // Ensure Comment column exists (for older DBs)
            TryAddCommentColumn(connection);
        }

        private static void TryAddCommentColumn(SqliteConnection connection)
        {
            try
            {
                var alter = connection.CreateCommand();
                alter.CommandText = "ALTER TABLE CheckIns ADD COLUMN Comment TEXT;";
                alter.ExecuteNonQuery();
            }
            catch
            {
                // Column already exists → ignore
            }
        }

        public static void AddCheckIn(int studentId, int mood, string comment)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"INSERT INTO CheckIns (StudentID, Mood, Date, Comment)
                  VALUES ($studentId, $mood, $date, $comment);";

            command.Parameters.AddWithValue("$studentId", studentId);
            command.Parameters.AddWithValue("$mood", mood);
            command.Parameters.AddWithValue("$date", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            command.Parameters.AddWithValue("$comment", comment ?? "");

            command.ExecuteNonQuery();
        }

        public static List<CheckIn> GetCheckIns()
        {
            var checkIns = new List<CheckIn>();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                "SELECT ID, StudentID, Mood, Date, IFNULL(Comment, '') FROM CheckIns;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                checkIns.Add(new CheckIn
                {
                    ID = reader.GetInt32(0),
                    StudentID = reader.GetInt32(1),
                    Mood = reader.GetInt32(2),
                    Date = DateTime.Parse(reader.GetString(3)),
                    Comment = reader.GetString(4)
                });
            }

            return checkIns;
        }
    }
}
