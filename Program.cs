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