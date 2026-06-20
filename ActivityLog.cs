using System;
using System.Collections.Generic;
using System.Linq;
using MySqlConnector;

namespace CybersecurityChatbot
{
    
    public class ActivityLog
    {
        private readonly List<(DateTime Time, string Description)> _fallback = new();
        private readonly bool _usingDatabase;

        public ActivityLog()
        {
            _usingDatabase = DatabaseHelper.TestConnection().success;
        }

        public void Add(string description)
        {
            if (_usingDatabase)
            {
                try
                {
                    using var conn = DatabaseHelper.GetConnection();
                    conn.Open();
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "INSERT INTO ActivityLog (Description) VALUES (@desc);";
                    cmd.Parameters.AddWithValue("@desc", description);
                    cmd.ExecuteNonQuery();
                    return;
                }
                catch (MySqlException)
                {
                    // fall through to in-memory if the DB write fails mid-session
                }
            }
            _fallback.Add((DateTime.Now, description));
        }

        /// <summary>Returns the last N entries (default 10) formatted for display.</summary>
        public string GetSummary(int count = 10)
        {
            if (_usingDatabase)
            {
                try
                {
                    var lines = new List<string>();
                    using var conn = DatabaseHelper.GetConnection();
                    conn.Open();
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT Description, LoggedAt FROM ActivityLog ORDER BY LogId DESC LIMIT @count;";
                    cmd.Parameters.AddWithValue("@count", count);
                    using var reader = cmd.ExecuteReader();
                    int i = 1;
                    while (reader.Read())
                        lines.Add($"{i++}. [{reader.GetDateTime("LoggedAt"):HH:mm:ss}] {reader.GetString("Description")}");

                    lines.Reverse(); // show oldest-first like the in-memory version
                    return lines.Count == 0 ? "No activity recorded yet." : string.Join("\n", lines);
                }
                catch (MySqlException)
                {
                    // fall through to in-memory summary
                }
            }

            if (_fallback.Count == 0) return "No activity recorded yet.";
            var recent = _fallback.TakeLast(count).ToList();
            return string.Join("\n", recent.Select((e, i) => $"{i + 1}. [{e.Time:HH:mm:ss}] {e.Description}"));
        }

        public int Count => _fallback.Count;
    }
}
