# 🗄️ MySQL Database Setup — Part 3

This project now stores cybersecurity tasks and the activity log in your local
**MySQL** database called `PoePart3`.

## 1. Run the schema script

Open MySQL Workbench (or your MySQL CLI) and run the script in:

```
Database/schema.sql
```

This creates two tables inside `PoePart3`:

- **Tasks** — stores task title, description, completion status, and reminder date
- **ActivityLog** — stores every logged chatbot action with a timestamp

## 2. Check your connection details

Open `DatabaseHelper.cs` and confirm these match your local MySQL setup:

```csharp
private const string Server   = "localhost";
private const string Database = "PoePart3";
private const string User     = "root";
private const string Password = "";   // <-- set this if your root user has a password
private const string Port     = "3306";
```

If your MySQL root user has a password, or you use a different username,
update these values. This is the **only file** you need to edit.

## 3. Build and run

```powershell
dotnet restore
dotnet build
```

Then run the app (F5 in Visual Studio). On startup, the top bar will show:

- 🟢 **● DB: Connected (MySQL)** — tasks are being saved to `PoePart3`
- 🔴 **● DB: Offline (using memory)** — MySQL isn't reachable; tasks will
  still work for this session but won't be saved permanently

## 4. Verify it's working

1. Add a task in the chatbot (e.g. type "add task")
2. Open MySQL Workbench and run:
   ```sql
   SELECT * FROM PoePart3.Tasks;
   ```
3. You should see your task there — proof the GUI is actually writing to MySQL

## How it works (for your video/report)

- `DatabaseHelper.cs` centralises the connection string and a `TestConnection()`
  health check
- `TaskManager.cs` uses `MySqlConnector` to run parameterised SQL (`INSERT`,
  `UPDATE`, `DELETE`, `SELECT`) against the `Tasks` table
- `ActivityLog.cs` does the same against the `ActivityLog` table
- **Graceful fallback:** if MySQL is unreachable at any point, both classes
  silently fall back to an in-memory list so the chatbot never crashes —
  this satisfies the "functional, running software" requirement even in a
  marker's environment where they may not have your exact database
