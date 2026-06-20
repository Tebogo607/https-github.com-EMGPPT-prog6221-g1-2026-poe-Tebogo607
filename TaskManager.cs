using System;
using System.Collections.Generic;
using MySqlConnector;

namespace CybersecurityChatbot
{

    public class TaskManager
    {

        private readonly List<TaskItem> _fallback = new();
        private int _fallbackNextId = 1;
        public bool UsingDatabase { get; private set; } = true;
        public string LastDbMessage { get; private set; } = "";

        public TaskManager()
        {
            var (success, message) = DatabaseHelper.TestConnection();
            UsingDatabase = success;
            LastDbMessage = message;
        }

        // Try and catch method 
        // w3schools.com

        public TaskItem AddTask(string title, string description, DateTime? reminderDate = null)
        {
            if (UsingDatabase)
            {
                try
                {
                    using var conn = DatabaseHelper.GetConnection();
                    conn.Open();
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = @"
                        INSERT INTO Tasks (Title, Description, IsComplete, HasReminder, ReminderDate)
                        VALUES (@title, @desc, 0, @hasReminder, @reminderDate);
                        SELECT LAST_INSERT_ID();";
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@desc", description);
                    cmd.Parameters.AddWithValue("@hasReminder", reminderDate.HasValue);
                    cmd.Parameters.AddWithValue("@reminderDate", (object?)reminderDate ?? DBNull.Value);

                    long newId = Convert.ToInt64(cmd.ExecuteScalar());

                    return new TaskItem
                    {
                        Id = (int)newId,
                        Title = title,
                        Description = description,
                        IsComplete = false,
                        HasReminder = reminderDate.HasValue,
                        ReminderDate = reminderDate
                    };
                }
                catch (MySqlException) { UsingDatabase = false; /* fall through to in-memory */ }
            }

            // Fallback path
            //geeksforgeeks.com
            var task = new TaskItem
            {
                Id = _fallbackNextId++,
                Title = title,
                Description = description,
                HasReminder = reminderDate.HasValue,
                ReminderDate = reminderDate
            };
            _fallback.Add(task);
            return task;
        }

        public bool CompleteTask(int id)
        {
            if (UsingDatabase)
            {
                try
                {
                    using var conn = DatabaseHelper.GetConnection();
                    conn.Open();
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE Tasks SET IsComplete = 1 WHERE TaskId = @id;";
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (MySqlException) { UsingDatabase = false; }
            }
            var t = _fallback.Find(x => x.Id == id);
            if (t == null) return false;
            t.IsComplete = true;
            return true;
        }
        //deleting the task
        //w3schools.com
        public bool DeleteTask(int id)
        {
            if (UsingDatabase)
            {
                try
                {
                    using var conn = DatabaseHelper.GetConnection();
                    conn.Open();
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "DELETE FROM Tasks WHERE TaskId = @id;";
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (MySqlException) { UsingDatabase = false; }
            }
            return _fallback.RemoveAll(x => x.Id == id) > 0;
        }

        public List<TaskItem> GetAll()
        {
            if (UsingDatabase)
            {
                try
                {
                    var results = new List<TaskItem>();
                    using var conn = DatabaseHelper.GetConnection();
                    conn.Open();
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT TaskId, Title, Description, IsComplete, HasReminder, ReminderDate FROM Tasks ORDER BY TaskId;";
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        results.Add(new TaskItem
                        {
                            Id = reader.GetInt32("TaskId"),
                            Title = reader.GetString("Title"),
                            Description = reader.GetString("Description"),
                            IsComplete = reader.GetBoolean("IsComplete"),
                            HasReminder = reader.GetBoolean("HasReminder"),
                            ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate")) ? null : reader.GetDateTime("ReminderDate")
                        });
                    }
                    return results;
                }
                catch (MySqlException) { UsingDatabase = false; }
            }
            return new List<TaskItem>(_fallback);
        }

        public List<TaskItem> GetPending() => GetAll().FindAll(t => !t.IsComplete);
        public List<TaskItem> GetComplete() => GetAll().FindAll(t => t.IsComplete);
        public TaskItem? GetById(int id) => GetAll().Find(t => t.Id == id);

        // Seed with common cybersecurity tasks (only if table is empty) 
        public void SeedDefaults()
        {
            if (GetAll().Count > 0) return; // already has data — don't duplicate

            AddTask("Enable Two-Factor Authentication",
                    "Set up 2FA on your email, banking, and social media accounts.",
                    DateTime.Now.AddDays(7));
            AddTask("Update All Passwords",
                    "Change weak or reused passwords to strong unique ones.",
                    DateTime.Now.AddDays(3));
            AddTask("Review Privacy Settings",
                    "Check privacy settings on Facebook, Instagram, and Google.",
                    DateTime.Now.AddDays(14));
        }
    }
}
