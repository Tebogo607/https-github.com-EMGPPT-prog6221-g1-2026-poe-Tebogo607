using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CybersecurityChatbot
{
    class CybersecurityBot : Chatbot
    {

        public string FavouriteTopic = "";
        public List<string> TopicsDiscussed = new List<string>();
        private string _lastTopic = "";
        private bool _awaitingFavTopic = false;


        public readonly TaskManager Tasks = new TaskManager();
        private bool _awaitingTaskTitle = false;
        private bool _awaitingTaskDesc = false;
        private bool _awaitingTaskReminder = false;
        private bool _awaitingReminderDate = false;
        private bool _awaitingDeleteId = false;
        private bool _awaitingCompleteId = false;
        private string _pendingTaskTitle = "";
        private string _pendingTaskDesc = "";

        //  Quiz 
        // (Farrell,2018)
        public readonly QuizEngine Quiz = new QuizEngine();
        private bool _awaitingQuizAnswer = false;

        //  Activity Log 

        public readonly ActivityLog Log = new ActivityLog();

        public CybersecurityBot()
        {
            Name = "Cybersecurity Awareness Bot";
            Tasks.SeedDefaults();
        }

        // 
        public override void Start()
        {
            base.Start();
            ShowCybersecurityHeader();
            ShowPersonalizedWelcome();
            ShowHelp();
            StartConversation();
        }
        // (Farrell,2018)
        public override void ShowLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════╗");
            Console.WriteLine("║     CYBERSECURITY AWARENESS BOT            ║");
            Console.WriteLine("║     Protecting South Africans Online       ║");
            Console.WriteLine("╚════════════════════════════════════════════╝");
            Console.ResetColor();
        }


        public override string ProcessInput(string input)
        {
            string lower = input.ToLower().Trim();


            if (_awaitingQuizAnswer && Quiz.IsActive)
                return HandleQuizAnswer(lower);
            // (Farrell,2018)

            if (_awaitingTaskTitle) return CaptureTaskTitle(input);
            if (_awaitingTaskDesc) return CaptureTaskDesc(input);
            if (_awaitingTaskReminder) return CaptureReminderChoice(lower);
            if (_awaitingReminderDate) return CaptureReminderDate(input);
            if (_awaitingDeleteId) return CaptureDeleteId(lower);
            if (_awaitingCompleteId) return CaptureCompleteId(lower);


            if (_awaitingFavTopic)
            {
                _awaitingFavTopic = false;
                FavouriteTopic = input.Trim();
                AddTopic(FavouriteTopic.ToLower());
                Log.Add($"Favourite topic set to '{FavouriteTopic}'");
                return $"Great! I'll remember that you're interested in {FavouriteTopic}. " +
                       $"It's a crucial part of staying safe online, {UserName}!";
            }

            // (Farrell,2018)
            string sentiment = DetectSentiment(lower);


            if (lower.Contains("activity log") || lower.Contains("what have you done") ||
                lower.Contains("show log") || lower.Contains("recent actions"))
            {
                Log.Add("User viewed activity log");
                return $"📋 Here's a summary of recent actions:\n\n{Log.GetSummary()}";
            }


            if (NlpMatch(lower, new[] { "quiz", "test", "game", "test my knowledge", "start quiz", "play quiz" }))
            {
                Quiz.Start();
                _awaitingQuizAnswer = true;
                Log.Add("Quiz started");
                return FormatQuizQuestion();
            }



            // View tasks
            if (NlpMatch(lower, new[] { "view tasks", "show tasks", "list tasks", "my tasks", "see tasks", "what are my tasks" }))
                return FormatTaskList();

            // Add task NLP catches many phrasings
            if (NlpMatch(lower, new[]{"add task","new task","create task","add a task","set a task",
                                       "remind me to","can you remind","i need to","add reminder"}))
            {
                // Try to extract inline title from the message
                string extracted = ExtractTaskTitle(lower, input);
                if (!string.IsNullOrEmpty(extracted))
                {
                    _pendingTaskTitle = extracted;
                    _awaitingTaskDesc = true;
                    return $"Got it! Adding task: \"{_pendingTaskTitle}\"\nPlease give a short description for this task:";
                }
                _awaitingTaskTitle = true;
                return "Sure! What is the title of the task you'd like to add?";
            }

            // Complete task
            if (NlpMatch(lower, new[] { "complete task", "mark complete", "done task", "finish task", "mark as done" }))
            {
                _awaitingCompleteId = true;
                return FormatTaskList() + "\n\nWhich task number would you like to mark as complete?";
            }

            // Delete task
            if (NlpMatch(lower, new[] { "delete task", "remove task", "cancel task" }))
            {
                _awaitingDeleteId = true;
                return FormatTaskList() + "\n\nWhich task number would you like to delete?";
            }

            // (Farrell,2018)
            if (lower.Contains("more") || lower.Contains("another tip") ||
                lower.Contains("tell me more") || lower.Contains("explain more"))
            {
                if (!string.IsNullOrEmpty(_lastTopic))
                    return sentiment + GetRandomResponse(new[] { "Sure! Here's more:", "One more thing:", "Happy to help:" })
                           + "\n" + GetTopicResponse(_lastTopic);
                return "Sure! What topic — passwords, phishing, browsing, viruses, or privacy?";
            }

            if (lower.Contains("favourite") || lower.Contains("favorite"))
            { _awaitingFavTopic = true; return "What is your favourite cybersecurity topic? I'll remember it! 😊"; }

            if ((lower.Contains("remind") || lower.Contains("my favourite")) && !string.IsNullOrEmpty(FavouriteTopic))
                return $"As someone interested in {FavouriteTopic}, {UserName}, be sure to " +
                       $"stay updated on the latest {FavouriteTopic} threats and check your account settings regularly.";

            if (lower.StartsWith("i'm interested in") || lower.StartsWith("i am interested in"))
            {
                FavouriteTopic = input.Replace("i'm interested in", "", StringComparison.OrdinalIgnoreCase)
                                      .Replace("i am interested in", "", StringComparison.OrdinalIgnoreCase)
                                      .Trim().Trim('.');
                AddTopic(FavouriteTopic.ToLower());
                return $"Great! I'll remember you're interested in {FavouriteTopic}. Stay curious!";
            }

            if (lower.Contains("hello") || lower.Contains("hi")) return sentiment + HandleGreeting();
            if (lower.Contains("how are you")) return sentiment + HandleHowAreYou();
            if (lower.Contains("purpose") || lower.Contains("what can you do")) return sentiment + HandlePurpose();

            if (lower.Contains("password") || lower.Contains("credentials") || lower.Contains("login"))
            { _lastTopic = "password"; AddTopic("password"); Log.Add("Discussed: passwords"); return sentiment + HandlePassword(); }

            if (lower.Contains("phishing") || lower.Contains("scam") || lower.Contains("fake email"))
            { _lastTopic = "phishing"; AddTopic("phishing"); Log.Add("Discussed: phishing"); return sentiment + HandlePhishing(); }

            if (lower.Contains("browsing") || lower.Contains("internet") || lower.Contains("website") || lower.Contains("https"))
            { _lastTopic = "browsing"; AddTopic("browsing"); Log.Add("Discussed: safe browsing"); return sentiment + HandleSafeBrowsing(); }

            if (lower.Contains("virus") || lower.Contains("malware") || lower.Contains("antivirus") || lower.Contains("ransomware"))
            { _lastTopic = "virus"; AddTopic("virus"); Log.Add("Discussed: malware/viruses"); return sentiment + HandleViruses(); }

            if (lower.Contains("privacy") || lower.Contains("private") || lower.Contains("vpn"))
            { _lastTopic = "privacy"; AddTopic("privacy"); Log.Add("Discussed: privacy"); return sentiment + HandlePrivacy(); }

            if (lower.Contains("thank")) return sentiment + HandleThanks();

            return sentiment + HandleUnknown();
        }


        // (Farrell,2018)
        private string CaptureTaskTitle(string raw)
        {
            _awaitingTaskTitle = false;
            _pendingTaskTitle = raw.Trim();
            _awaitingTaskDesc = true;
            return $"Task title: \"{_pendingTaskTitle}\"\nGive a short description:";
        }

        private string CaptureTaskDesc(string raw)
        {
            _awaitingTaskDesc = false;
            _pendingTaskDesc = raw.Trim();
            _awaitingTaskReminder = true;
            return "Would you like to set a reminder date for this task? (yes / no)";
        }

        private string CaptureReminderChoice(string lower)
        {
            _awaitingTaskReminder = false;
            if (lower.Contains("yes") || lower.Contains("y"))
            {
                _awaitingReminderDate = true;
                return "Enter the reminder date (e.g. 25 Jun 2026 or in 7 days):";
            }
            var t = Tasks.AddTask(_pendingTaskTitle, _pendingTaskDesc);
            Log.Add($"Task added: '{t.Title}' (no reminder)");
            return $"✅ Task added!\n🔲 \"{t.Title}\"\n{t.Description}\nNo reminder set.";
        }
        // (Farrell,2018)
        private string CaptureReminderDate(string raw)
        {
            _awaitingReminderDate = false;
            DateTime? date = ParseDate(raw);
            var t = Tasks.AddTask(_pendingTaskTitle, _pendingTaskDesc, date);
            string reminderText = date.HasValue ? $"Reminder: {date.Value:dd MMM yyyy}" : "Could not parse date — no reminder set.";
            Log.Add($"Task added: '{t.Title}' — {reminderText}");
            return $"✅ Task added!\n🔲 \"{t.Title}\"\n{t.Description}\n{reminderText}";
        }

        private string CaptureCompleteId(string lower)
        {
            _awaitingCompleteId = false;
            if (int.TryParse(lower.Trim(), out int id) && Tasks.CompleteTask(id))
            {
                Log.Add($"Task #{id} marked complete");
                return $"✅ Task #{id} marked as complete! Great job staying cybersafe! 🛡️";
            }
            return "I couldn't find that task number. Type 'view tasks' to see the list.";
        }

        private string CaptureDeleteId(string lower)
        {
            _awaitingDeleteId = false;
            if (int.TryParse(lower.Trim(), out int id) && Tasks.DeleteTask(id))
            {
                Log.Add($"Task #{id} deleted");
                return $"🗑️ Task #{id} has been deleted.";
            }
            return "I couldn't find that task number. Type 'view tasks' to see the list.";
        }


        // (Farrell,2018)
        private string HandleQuizAnswer(string lower)
        {
            var q = Quiz.CurrentQuestion;
            if (q == null) { _awaitingQuizAnswer = false; return "Quiz complete!"; }

            int idx = -1;
            if (q.IsTrueFalse)
            {
                if (lower.Contains("true") || lower == "t") idx = 0;
                else if (lower.Contains("false") || lower == "f") idx = 1;
            }
            else
            {
                if (lower.StartsWith("a") || lower == "1") idx = 0;
                else if (lower.StartsWith("b") || lower == "2") idx = 1;
                else if (lower.StartsWith("c") || lower == "3") idx = 2;
                else if (lower.StartsWith("d") || lower == "4") idx = 3;
            }

            if (idx == -1)
                return q.IsTrueFalse
                    ? "Please answer True or False."
                    : "Please answer A, B, C, or D.";

            var (correct, explanation, finished, score) = Quiz.SubmitAnswer(idx);
            string result = correct ? "✅ Correct! " : "❌ Wrong. ";
            result += explanation;

            if (finished)
            {
                _awaitingQuizAnswer = false;
                string feedback = Quiz.GetFinalFeedback();
                Log.Add($"Quiz completed — score {score}/{Quiz.TotalQuestions}");
                return result + $"\n\n{feedback}";
            }

            Log.Add($"Quiz answer #{score}: {(correct ? "correct" : "incorrect")}");
            return result + "\n\n" + FormatQuizQuestion();
        }
        // method for format quiz question
        private string FormatQuizQuestion()
        {
            var q = Quiz.CurrentQuestion;
            if (q == null) return "No more questions!";
            string text = $"❓ Question {_questionNumber()}:\n{q.Question}";
            if (q.IsTrueFalse)
                text += "\n\n  Type: True  or  False";
            else
                text += "\n\n" + string.Join("\n", q.Options);
            return text;
        }

        private int _questionNumber()
        {
            // Reflect current position from quiz state
            var field = typeof(QuizEngine).GetField("_current",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field != null ? (int)(field.GetValue(Quiz) ?? 1) + 1 : 1;
        }



        private string FormatTaskList()
        {
            var all = Tasks.GetAll();
            if (all.Count == 0) return "📋 You have no tasks yet. Say 'add task' to create one!";
            var sb = new System.Text.StringBuilder("📋 Your Cybersecurity Tasks:\n");
            foreach (var t in all)
                sb.AppendLine($"  #{t.Id} {t.StatusText} {t.Title}\n      {t.Description}\n      {t.ReminderText}");
            return sb.ToString().TrimEnd();
        }

        /// <summary>Simple NLP: returns true if input contains any of the phrases.</summary>
        private static bool NlpMatch(string lower, string[] phrases)
        {
            foreach (var p in phrases)
                if (lower.Contains(p)) return true;
            return false;
        }

        /// <summary>Tries to extract a task title from common sentence patterns.</summary>
        private static string ExtractTaskTitle(string lower, string original)
        {
            // 
            var patterns = new[]
            {
                @"remind me to (.+)",
                @"add (?:a )?task to (.+)",
                @"add (?:a )?task[:\-]?\s*(.+)",
                @"i need to (.+)",
                @"can you remind (?:me )?to (.+)"
            };
            foreach (var p in patterns)
            {
                var m = Regex.Match(lower, p);
                if (m.Success)
                {
                    // Use original casing
                    int start = lower.IndexOf(m.Groups[1].Value);
                    return start >= 0 ? original[start..(start + m.Groups[1].Length)].Trim() : m.Groups[1].Value;
                }
            }
            return "";
        }

        //
        private static DateTime? ParseDate(string raw)
        {
            string lower = raw.ToLower();
            var match = Regex.Match(lower, @"in (\d+) days?");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int days))
                return DateTime.Now.AddDays(days);
            if (DateTime.TryParse(raw, out DateTime dt))
                return dt;
            return null;
        }

        private void AddTopic(string topic) { if (!TopicsDiscussed.Contains(topic)) TopicsDiscussed.Add(topic); }

        private string GetTopicResponse(string topic) => topic switch
        {
            "password" => HandlePassword(),
            "phishing" => HandlePhishing(),
            "browsing" => HandleSafeBrowsing(),
            "virus" => HandleViruses(),
            "privacy" => HandlePrivacy(),
            _ => HandleUnknown()
        };

        // 
        private string DetectSentiment(string lower)
        {
            if (lower.Contains("worried") || lower.Contains("scared") || lower.Contains("anxious"))
                return GetRandomResponse(new[] { "It's understandable to feel that way. Scammers are convincing. Here's a tip:\n", "Cyber threats are real. Here's something to help:\n" });
            if (lower.Contains("curious") || lower.Contains("interested") || lower.Contains("wonder"))
                return GetRandomResponse(new[] { "Great curiosity! Here's what you should know:\n", "Love the enthusiasm! Here's more:\n" });
            if (lower.Contains("frustrated") || lower.Contains("confused") || lower.Contains("don't understand"))
                return GetRandomResponse(new[] { "Let's break it down simply:\n", "Don't worry, here's a clear explanation:\n" });
            return "";
        }

        public override void ShowHelp()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n📚 Topics: passwords, phishing, browsing, viruses, privacy");
            Console.WriteLine("📋 Tasks:  add task | view tasks | complete task | delete task");
            Console.WriteLine("🎮 Quiz:   type 'quiz' to start");
            Console.WriteLine("📜 Log:    type 'activity log'");
            Console.ResetColor();
        }

        public override void SayGoodbye()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nGoodbye {UserName}! Stay safe online! 🛡️");
            Console.ResetColor();
        }

        public void ShowCybersecurityHeader()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n=== CYBERSECURITY AWARENESS BOT ===");
            Console.ResetColor();
        }

        public void ShowPersonalizedWelcome()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nHello, {UserName}! I'm your Cybersecurity Awareness Bot.");
            TypeSlowly("I'm here to help you stay safe online!\n");
            Console.ResetColor();
        }
        // conversation loop
        public void StartConversation()
        {
            while (true)
            {
                DisplayPrompt($"{UserName}: ", ConsoleColor.Cyan);
                string input = Console.ReadLine() ?? "";
                if (input.ToLower() is "exit" or "quit") { SayGoodbye(); break; }
                if (input.ToLower() == "help") { ShowHelp(); continue; }
                if (string.IsNullOrWhiteSpace(input)) { DisplayMessage("Please type something!", ConsoleColor.Red); continue; }
                string response = ProcessInput(input);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("🤖 Bot: ");
                TypeSlowly(response);
                Console.ResetColor();
                Console.WriteLine("---\n");
            }
        }

        // 
        public string HandleGreeting() => GetRandomResponse(new[] { $"Hello {UserName}! How can I help?", $"Hi {UserName}! Ready to learn about cybersecurity?", $"Hey {UserName}! Ask me anything!" });
        public string HandleHowAreYou() => GetRandomResponse(new[] { "All systems secure! 😊", "Operating perfectly! Ready to help!", "Feeling secure and ready!" });
        public string HandlePurpose() => GetRandomResponse(new[] { "I help with passwords, phishing, safe browsing, viruses, privacy — and now tasks and quizzes!", "I'm your cybersecurity guide! Ask me anything or try 'quiz' or 'add task'." });

        public string HandlePassword() => GetRandomResponse(new[]{
            "🔑 Use DIFFERENT passwords for each account, at least 12 characters with symbols.",
            "🔑 Strong example: 'Tr0ub4dor&3#XkQ9' — long, random, mixed characters.",
            "🔑 '123456' is the most common password. Make yours unique and strong!",
            "🔑 Enable 2FA — even if your password leaks, attackers can't get in."});

        public string HandlePhishing() => GetRandomResponse(new[]{
            "🎣 Watch for urgent emails, spelling errors, suspicious senders, and unexpected attachments.",
            "🎣 Real companies NEVER ask for passwords via email. Report suspicious emails!",
            "🎣 In South Africa phishing is very common — always verify the sender's email address.",
            "🎣 Hover over links to see the real URL before clicking."});

        public string HandleSafeBrowsing() => GetRandomResponse(new[]{
            "🌐 Look for 'https://' and the padlock 🔒 before entering personal info.",
            "🌐 Avoid online banking on public Wi-Fi — use a VPN instead.",
            "🌐 Keep your browser updated and use an ad blocker.",
            "🌐 Clear cookies regularly and avoid suspicious pop-up ads."});

        public string HandleViruses() => GetRandomResponse(new[]{
            "🦠 Install antivirus, keep Windows updated, backup files regularly.",
            "🦠 Only download software from official sources.",
            "🦠 If your PC is slow or showing pop-ups, run a virus scan immediately.",
            "🦠 Ransomware encrypts your files — regular backups are your best defence."});

        public string HandlePrivacy() => GetRandomResponse(new[]{
            "🔒 Review privacy settings on all social media and limit what strangers see.",
            "🔒 Use a VPN on public Wi-Fi to encrypt your internet traffic.",
            "🔒 Your ID number, address, and full name can enable identity theft — share carefully."});

        public string HandleThanks() => GetRandomResponse(new[] { $"You're welcome, {UserName}! Stay safe! 😊", $"Glad to help, {UserName}! Think before you click!", $"My pleasure, {UserName}! Keep learning!" });

        public string HandleUnknown() => GetRandomResponse(new[]{
            $"I didn't quite understand, {UserName}. Try: passwords, phishing, quiz, add task, activity log.",
            "I specialise in cybersecurity. Try 'quiz', 'add task', or ask about phishing, passwords, or privacy.",
            "Could you rephrase? I can help with cybersecurity topics, tasks, and quizzes!"});
    }
}
