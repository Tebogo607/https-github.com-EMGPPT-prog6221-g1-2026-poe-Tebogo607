using System;
using System.Windows;

namespace CybersecurityChatbot
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].ToLower() == "console")
            {
               
                CybersecurityBot myBot = new CybersecurityBot();
                myBot.Start();
            }
            else
            {
                
                App app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }
    }
}
// 
// REFERENCE LIST (Harvard style)

// Farrell, J. (2018) Microsoft Visual C#: An Introduction to Object-Oriented
//     Programming. 7th edn. Boston: Cengage Learning.

// GeeksforGeeks (no date) C# Programming Language. Available at:
//     https://www.geeksforgeeks.org/c-sharp-programming-language/
//     (Accessed: 20 June 2026).

// Microsoft (no date) C# documentation. Available at:
//     https://learn.microsoft.com/en-us/dotnet/csharp/
//     (Accessed: 20 June 2026).

// Microsoft (no date) Windows Presentation Foundation (WPF) documentation.
//     Available at: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/
//     (Accessed: 20 June 2026).

// Oracle Corporation (no date) MySQL 8.0 Reference Manual. Available at:
//     https://dev.mysql.com/doc/refman/8.0/en/
//     (Accessed: 20 June 2026).

// W3Schools (no date) C# Tutorial. Available at:
//     https://www.w3schools.com/cs/
//     (Accessed: 20 June 2026).
