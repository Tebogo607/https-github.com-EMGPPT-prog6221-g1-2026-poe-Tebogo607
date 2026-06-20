# 🛡️ Cybersecurity Awareness Chatbot — Part 3 / POE

A C# WPF chatbot that educates South African citizens on cybersecurity, now
with a Task Assistant, Mini-Quiz, NLP simulation, and Activity Log — all on
top of the original Part 1 (console) and Part 2 (GUI) chatbot.

---

## 🆕 What's new in this version: MySQL Database Integration

Part 3 Task 1 requires the Task Assistant to persist data in a real database.
This project now connects to a local **MySQL** database called `PoePart3`.

👉 **See `Database/SETUP.md` for full setup instructions.**

Quick start:
1. Run `Database/schema.sql` once in MySQL Workbench against `PoePart3`
2. Check `DatabaseHelper.cs` matches your local MySQL username/password
3. Build and run — the top bar shows 🟢 Connected or 🔴 Offline

If MySQL isn't running, the chatbot **does not crash** — `TaskManager` and
`ActivityLog` automatically fall back to in-memory storage for that session.
This means the app is always demoable, even on a marker's machine without
your exact database.

---

## 🗂️ Project Structure

```
Part3/
├── .github/workflows/ci.yml      ← GitHub Actions CI
├── Database/
│   ├── schema.sql                ← Run this once to create Tasks/ActivityLog tables
│   └── SETUP.md                  ← Step-by-step DB setup guide
├── Chatbot.cs                    ← Part 1: parent class (greeting, ASCII art, name)
├── cybersecuritybot.cs           ← Core bot logic — NLP routing, all features
├── DatabaseHelper.cs             ← MySQL connection string + health check
├── TaskItem.cs                   ← Task model (title, description, reminder)
├── TaskManager.cs                ← CRUD against MySQL "Tasks" table (+ fallback)
├── QuizEngine.cs                 ← 12-question cybersecurity quiz
├── ActivityLog.cs                ← Persists actions to MySQL "ActivityLog" table
├── App.xaml / App.xaml.cs        ← WPF application + global dark theme
├── MainWindow.xaml               ← GUI layout (sidebar, chat, task/quiz/log buttons)
├── MainWindow.xaml.cs            ← GUI code-behind
├── Program.cs                    ← Entry point (console or GUI mode)
└── Assignment1_Prog.csproj
```

---

## 🚀 How to Run

```powershell
dotnet restore
dotnet build
```

Then press **F5** in Visual Studio (or `dotnet run`) for the GUI.
Console mode (Part 1): `dotnet run -- console`

---

## 🎯 Part 3 Features

| Task | Feature | Where |
|---|---|---|
| 1 | **Task Assistant + MySQL** | `TaskManager.cs`, `Database/schema.sql` |
| 2 | **Cybersecurity Mini-Quiz** (12 Qs, MC + T/F) | `QuizEngine.cs` |
| 3 | **NLP Simulation** (keyword + regex matching) | `cybersecuritybot.cs` → `NlpMatch()`, `ExtractTaskTitle()` |
| 4 | **Activity Log** (persisted to MySQL) | `ActivityLog.cs` |

All Part 1 (console, voice greeting, ASCII art) and Part 2 (keyword
recognition, sentiment detection, memory & recall) features are preserved.

---

## 💬 Example Interactions

```
User:  Remind me to update my password tomorrow
Bot:   Got it! Adding task: "update my password tomorrow"
       Please give a short description for this task:

User:  quiz
Bot:   ❓ Question 1:
       What should you do if you receive an email asking for your password?
       A) Reply with your password
       B) Delete the email
       C) Report it as phishing
       D) Ignore it

User:  C
Bot:   ✅ Correct! Reporting phishing emails helps prevent scams.
       ❓ Question 2: ...

User:  activity log
Bot:   📋 Here's a summary of recent actions:
       1. [14:02:10] Task added: 'update my password tomorrow'
       2. [14:03:45] Quiz started
       3. [14:03:52] Quiz answer #1: correct
```

---

## 📚 References

Pieterse, H. 2021. The Cyber Threat Landscape in South Africa: A 10-Year Review.
*The African Journal of Information and Communication*, 28(28).
doi: https://doi.org/10.23962/10539/32213

---

*© The Independent Institute of Education (Pty) Ltd 2026*
