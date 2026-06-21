using System.Collections.Generic;

namespace CybersecurityChatbot
{
    public class QuizQuestion
    {
        public string Question { get; set; } = "";
        public string[] Options { get; set; } = [];   // empty = True/False
        public int CorrectIndex { get; set; }
        public string Explanation { get; set; } = "";
        public bool IsTrueFalse => Options.Length == 0;
    }

    public class QuizEngine
    {
        private readonly List<QuizQuestion> _questions = new()
        {    // W3schools.com
            //Quiz questions with explanations for each answer nbbbbbbbb
            new QuizQuestion
            {
                Question     = "What should you do if you receive an email asking for your password?",
                Options      = new[]{"A) Reply with your password","B) Delete the email","C) Report it as phishing","D) Ignore it"},
                CorrectIndex = 2,
                Explanation  = "Correct! Always report phishing emails. Legitimate companies NEVER ask for your password via email."
            },
            new QuizQuestion
            {
                Question     = "Which of these is the strongest password?",
                Options      = new[]{"A) password123","B) MyName1990","C) P@ssw0rd!","D) Tr0ub4dor&3#XkQ9"},
                CorrectIndex = 3,
                Explanation  = "D is correct! Long, random passwords with symbols are the strongest. Length beats complexity."
            },
            new QuizQuestion
            {
                Question     = "What does 'https' in a website URL mean?",
                Options      = new[]{"A) The site is fast","B) The connection is encrypted","C) The site is government-owned","D) Nothing important"},
                CorrectIndex = 1,
                Explanation  = "B is correct! The 'S' stands for Secure. It means your data is encrypted in transit."
            },
            new QuizQuestion
            {
                Question     = "What is two-factor authentication (2FA)?",
                Options      = new[]{"A) Using two different passwords","B) Logging in from two devices","C) A second verification step beyond your password","D) Having two email accounts"},
                CorrectIndex = 2,
                Explanation  = "C is correct! 2FA adds a second check (like an SMS code) so even if your password is stolen, attackers can't get in."
            },
            new QuizQuestion
            {
                Question     = "Which action puts you most at risk on public Wi-Fi?",
                Options      = new[]{"A) Browsing news websites","B) Online banking","C) Watching YouTube","D) Checking the weather"},
                CorrectIndex = 1,
                Explanation  = "B is correct! Public Wi-Fi is unencrypted. Avoid banking or any sensitive transactions on public networks."
            },
            new QuizQuestion
            {
                Question     = "What is ransomware?",
                Options      = new[]{"A) Software that speeds up your PC","B) Malware that encrypts your files and demands payment","C) A type of antivirus","D) A secure messaging app"},
                CorrectIndex = 1,
                Explanation  = "B is correct! Ransomware locks your files and demands money. Regular backups are your best defence."
            },
            new QuizQuestion
            {
                Question     = "What is social engineering in cybersecurity?",
                Options      = new[]{"A) Building social media apps","B) Manipulating people into revealing confidential info","C) Networking at tech events","D) A type of firewall"},
                CorrectIndex = 1,
                Explanation  = "B is correct! Social engineering tricks people psychologically — it's often easier to hack a person than a system."
            },

            new QuizQuestion
            {
                Question     = "TRUE or FALSE: You should use the same password for multiple accounts to make it easier to remember.",
                CorrectIndex = 1,   // 0=True, 1=False
                Explanation  = "FALSE! Using the same password everywhere means one breach exposes ALL your accounts."
            },
            new QuizQuestion
            {
                Question     = "TRUE or FALSE: A padlock icon in your browser always means a website is completely safe.",
                CorrectIndex = 1,
                Explanation  = "FALSE! HTTPS only means the connection is encrypted, not that the site itself is trustworthy. Phishing sites can also use HTTPS."
            },
            new QuizQuestion
            {
                Question     = "TRUE or FALSE: Antivirus software alone is enough to protect your computer from all threats.",
                CorrectIndex = 1,
                Explanation  = "FALSE! Antivirus is one layer of defence. Safe browsing habits, updates, and backups are equally important."
            },
            new QuizQuestion
            {
                Question     = "TRUE or FALSE: You should keep your operating system and apps updated to stay secure.",
                CorrectIndex = 0,   // True
                Explanation  = "TRUE! Updates patch security vulnerabilities. Outdated software is one of the most common ways attackers get in."
            },
            new QuizQuestion
            {
                Question     = "TRUE or FALSE: It is safe to click a link in an email if the sender's name looks familiar.",
                CorrectIndex = 1,
                Explanation  = "FALSE! Attackers can spoof familiar names. Always verify the actual email address and hover over links before clicking."
            },
        };

        public int TotalQuestions => _questions.Count;

        private int _current = 0;
        private int _score = 0;
        public bool IsActive { get; private set; } = false;
        public bool IsFinished => _current >= _questions.Count;

        public void Start()
        {
            _current = 0;
            _score = 0;
            IsActive = true;
        }
        // Stop the quiz and mark it as inactive 
        public void Stop() { IsActive = false; }

        public QuizQuestion? CurrentQuestion =>
            _current < _questions.Count ? _questions[_current] : null;


        public (bool correct, string explanation, bool finished, int score) SubmitAnswer(int answerIndex)
        {
            var q = _questions[_current];
            bool correct = answerIndex == q.CorrectIndex;
            if (correct) _score++;
            _current++;
            bool finished = _current >= _questions.Count;
            if (finished) IsActive = false;
            return (correct, q.Explanation, finished, _score);
        }

        public string GetFinalFeedback()
        {
            double pct = (double)_score / _questions.Count * 100;
            return pct >= 80
                ? $"🏆 Amazing! {_score}/{_questions.Count} — You're a cybersecurity pro!"
                : pct >= 60
                ? $"👍 Good effort! {_score}/{_questions.Count} — Keep learning to stay safe!"
                : $"📚 {_score}/{_questions.Count} — Keep studying, cybersecurity is important!";
        }
    }
}
