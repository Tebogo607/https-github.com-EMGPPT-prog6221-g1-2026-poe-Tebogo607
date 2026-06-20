using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CybersecurityChatbot
{
    public partial class MainWindow : Window
    {
        private readonly CybersecurityBot _bot = new CybersecurityBot();
        private bool _nameConfirmed = false;

        private static readonly SolidColorBrush BotBg    = new(Color.FromRgb(0x1C,0x3A,0x4A));
        private static readonly SolidColorBrush UserBg   = new(Color.FromRgb(0x1A,0x3A,0x1A));
        private static readonly SolidColorBrush BotFg    = new(Color.FromRgb(0xB3,0xE5,0xFC));
        private static readonly SolidColorBrush UserFg   = new(Color.FromRgb(0xA5,0xD6,0xA7));
        private static readonly SolidColorBrush MutedFg  = new(Color.FromRgb(0x8B,0x94,0x9E));
        private static readonly SolidColorBrush QuizBg   = new(Color.FromRgb(0x2D,0x2A,0x10));
        private static readonly SolidColorBrush QuizFg   = new(Color.FromRgb(0xFF,0xD7,0x66));

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _bot.PlayGreeting();
            AddSystem("🛡️  Welcome to the Cybersecurity Awareness Bot — Part 3!");
            AddSystem("New in Part 3: Task Assistant 📋 | Mini Quiz 🎮 | NLP 🧠 | Activity Log 📜");

            // Show database connection status (Part 3 Task 1 - Database Integration)
            if (_bot.Tasks.UsingDatabase)
            {
                DbStatusLabel.Text = "● DB: Connected (MySQL)";
                DbStatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(0x3F, 0xB9, 0x50));
                AddSystem("✅ Connected to MySQL database 'PoePart3' — tasks will be saved permanently.");
            }
            else
            {
                DbStatusLabel.Text = "● DB: Offline (using memory)";
                DbStatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(0xF8, 0x51, 0x49));
                AddSystem("⚠️ Could not reach MySQL — tasks will only be kept for this session.");
            }

            AddSystem("Enter your name above to get started.");
            NameBox.Focus();
        }

        // ── Name ─────────────────────────────────────────────────────────────────
        private void ConfirmName_Click(object s, RoutedEventArgs e) => ConfirmName();
        private void NameBox_KeyDown(object s, KeyEventArgs e) { if (e.Key == Key.Enter) ConfirmName(); }

        private void ConfirmName()
        {
            string name = NameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name)) { Shake(NameBox); return; }

            _bot.UserName = name;
            _nameConfirmed = true;
            NameBanner.Visibility = Visibility.Collapsed;
            InputBox.IsEnabled = true;
            SendBtn.IsEnabled  = true;
            InputBox.Focus();
            RefreshMemory();

            AddBot($"Hello, {name}! 👋 I'm your Cybersecurity Awareness Bot — now with Part 3 features!");
            AddBot("Try the sidebar buttons to add tasks, start the quiz, or view your activity log.");
            AddBot("Or just chat — I understand natural language like: \"Remind me to enable 2FA\" or \"I need to review my passwords\".");
        }

        // ── Send ──────────────────────────────────────────────────────────────────
        private void Send_Click(object s, RoutedEventArgs e) => SendMessage();
        private void Input_KeyDown(object s, KeyEventArgs e) { if (e.Key == Key.Enter) SendMessage(); }

        private void Chip_Click(object sender, RoutedEventArgs e)
        {
            if (!_nameConfirmed) { Shake(NameBanner); return; }
            if (sender is Button btn && btn.Tag is string q)
            { InputBox.Text = q; SendMessage(); }
        }

        private void SendMessage()
        {
            string input = InputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;
            if (!_nameConfirmed) { Shake(NameBanner); return; }

            // Quiz in progress — render answer buttons as yellow bubbles
            bool wasQuiz = _bot.Quiz.IsActive;

            AddUser(input);
            InputBox.Clear();

            string lower = input.ToLower();
            if (lower is "exit" or "quit" or "goodbye")
            {
                AddBot($"Goodbye, {_bot.UserName}! Stay safe online. 🛡️");
                AddSystem("Session ended.");
                InputBox.IsEnabled = false;
                SendBtn.IsEnabled  = false;
                return;
            }
            if (lower == "help")
            {
                AddBot("📚 Cybersecurity topics: passwords, phishing, browsing, viruses, privacy\n" +
                       "📋 Tasks: add task | view tasks | complete task | delete task\n" +
                       "🎮 Quiz: type 'quiz'\n📜 Log: type 'activity log'");
                return;
            }

            ShowTyping();
            string captured = input;
            var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(380) };
            timer.Tick += (_, __) =>
            {
                timer.Stop();
                HideTyping();
                string response = _bot.ProcessInput(captured);

                // Choose bubble style based on context
                bool isQuizMsg = _bot.Quiz.IsActive || wasQuiz;
                if (isQuizMsg)
                    AddQuizBubble(response);
                else
                    AddBot(response);

                RefreshMemory();
            };
            timer.Start();
        }

        // ── Bubble builders ───────────────────────────────────────────────────────
        private void AddBot(string text)
        {
            ChatPanel.Children.Add(Bubble("🤖  CyberSec Bot", text, BotBg, BotFg, false));
            ScrollBottom();
        }
        private void AddUser(string text)
        {
            ChatPanel.Children.Add(Bubble($"👤  {_bot.UserName}", text, UserBg, UserFg, true));
            ScrollBottom();
        }
        private void AddQuizBubble(string text)
        {
            ChatPanel.Children.Add(Bubble("🎮  Quiz", text, QuizBg, QuizFg, false));
            ScrollBottom();
        }
        private void AddSystem(string text)
        {
            ChatPanel.Children.Add(new TextBlock
            {
                Text = text, Foreground = MutedFg, FontSize = 11,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0,5,0,5)
            });
            ScrollBottom();
        }

        private FrameworkElement Bubble(string sender, string text,
            SolidColorBrush bg, SolidColorBrush fg, bool right)
        {
            var stack = new StackPanel();
            stack.Children.Add(new TextBlock { Text=sender, FontSize=10, FontWeight=FontWeights.Bold, Foreground=MutedFg, Margin=new Thickness(0,0,0,3) });
            stack.Children.Add(new TextBlock { Text=text, Foreground=fg, FontSize=13, TextWrapping=TextWrapping.Wrap, LineHeight=20 });
            stack.Children.Add(new TextBlock { Text=DateTime.Now.ToString("HH:mm"), FontSize=9, Foreground=MutedFg,
                HorizontalAlignment=right?HorizontalAlignment.Right:HorizontalAlignment.Left, Margin=new Thickness(0,4,0,0) });

            var border = new Border
            {
                Background = bg,
                CornerRadius = new CornerRadius(right?12:4, 12, 12, right?4:12),
                Padding = new Thickness(13,9,13,9), MaxWidth=600,
                Child = stack, Margin = new Thickness(8,3,8,3),
                HorizontalAlignment = right ? HorizontalAlignment.Right : HorizontalAlignment.Left
            };
            border.BeginAnimation(OpacityProperty, new DoubleAnimation(0,1,TimeSpan.FromMilliseconds(180)));
            return border;
        }

        // ── Typing indicator ──────────────────────────────────────────────────────
        private Border? _typing;
        private void ShowTyping()
        {
            HideTyping();
            _typing = new Border { Background=BotBg, CornerRadius=new CornerRadius(4,12,12,12),
                Padding=new Thickness(13,9,13,9), Margin=new Thickness(8,3,8,3),
                HorizontalAlignment=HorizontalAlignment.Left,
                Child=new TextBlock{Text="🤖  Bot is typing...",Foreground=MutedFg,FontStyle=FontStyles.Italic,FontSize=12}};
            ChatPanel.Children.Add(_typing);
            ScrollBottom();
        }
        private void HideTyping() { if (_typing!=null && ChatPanel.Children.Contains(_typing)) ChatPanel.Children.Remove(_typing); _typing=null; }

        // ── Memory panel ──────────────────────────────────────────────────────────
        private void RefreshMemory()
        {
            MemoryName.Text  = $"Name: {(string.IsNullOrEmpty(_bot.UserName)?"—":_bot.UserName)}";
            MemoryTopic.Text = $"Favourite: {(string.IsNullOrEmpty(_bot.FavouriteTopic)?"—":_bot.FavouriteTopic)}";
            MemoryCount.Text = $"Topics explored: {_bot.TopicsDiscussed.Count}";
        }

        // ── Utility ───────────────────────────────────────────────────────────────
        private void ClearChat_Click(object s, RoutedEventArgs e)
        { ChatPanel.Children.Clear(); AddSystem("Chat cleared. 🛡️"); }

        private void ScrollBottom() { Scroller.UpdateLayout(); Scroller.ScrollToEnd(); }

        private static void Shake(UIElement el)
        {
            var t = new TranslateTransform(); el.RenderTransform = t;
            var a = new DoubleAnimationUsingKeyFrames();
            foreach (var (v,ms) in new[]{(0.0,0),(-8.0,60),(8.0,120),(-5.0,180),(5.0,240),(0.0,300)})
                a.KeyFrames.Add(new LinearDoubleKeyFrame(v, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(ms))));
            t.BeginAnimation(TranslateTransform.XProperty, a);
        }
    }
}
