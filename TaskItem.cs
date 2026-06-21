using System;

namespace CybersecurityChatbot
{

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsComplete { get; set; } = false;

        // Optional reminder getters and setters
        public bool HasReminder { get; set; } = false;
        public DateTime? ReminderDate { get; set; } = null;
        // w3Schools.com
        public string ReminderText => HasReminder && ReminderDate.HasValue
            ? $"Reminder: {ReminderDate.Value:dd MMM yyyy}"
            : "No reminder";

        public string StatusText => IsComplete ? "✅ Done" : "🔲 Pending";
    }
}
