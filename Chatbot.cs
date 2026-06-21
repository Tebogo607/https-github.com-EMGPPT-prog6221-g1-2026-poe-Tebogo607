using System;
using System.Media;
using System.IO;
using System.Threading;

namespace CybersecurityChatbot
{
    // This is the PARENT class
    // It contains methods that can be called and overridden
    class Chatbot
    {
        // Properties (variables)
        public string Name = "Generic Chatbot";
        public string UserName = "";
        public Random RandomGenerator = new Random();

        // Constructor runs when we create a new chatbot
        public Chatbot()
        {
            Name = "Generic Chatbot";
            RandomGenerator = new Random();


            try
            {
                Console.Title = "Cybersecurity Chatbot";
            }
            catch (IOException)
            {
                // No console attached (running as WPF GUI) — safe to ignore
            }
        }

        // METHODS THAT CAN BE CALLED 

        // Method 1: Start the chatbot
        public virtual void Start()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Chatbot is starting...");
            Console.ResetColor();

            // Call other methods
            PlayGreeting();
            ShowLogo();
            AskForName();
        }

        // Method 2 Play voice greeting
        public void PlayGreeting()
        {
            try
            {
                if (File.Exists("greeting.wav"))
                {
                    SoundPlayer player = new SoundPlayer("greeting.wav");
                    player.PlaySync();
                }
                else
                {
                    Console.WriteLine("(Welcome greeting would play here)");
                    Console.Beep(440, 500);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("(Voice greeting not available)");
            }
        }

        // Method 3  Show logo can be overridden
        public virtual void ShowLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════╗");
            Console.WriteLine("║                                  ║");
            Console.WriteLine("║        BASIC CHATBOT             ║");
            Console.WriteLine("║         [💬] [🤖]               ║");
            Console.WriteLine("║                                  ║");
            Console.WriteLine("╚══════════════════════════════════╝");
            Console.ResetColor();
        }

        // Method 4  Ask for user name
        public void AskForName()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("\nWhat is your name? ");
                Console.ResetColor();

                UserName = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(UserName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Please enter a valid name.");
                    Console.ResetColor();
                }
                else
                {
                    break;
                }
            }
        }

        // Method 5 Type slowly

        public void TypeSlowly(string text)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(30);
            }
            Console.WriteLine();
        }

        // Method 6 Get random response
        public string GetRandomResponse(string[] responses)
        {
            int index = RandomGenerator.Next(responses.Length);
            return responses[index];
        }

        // Method 7 Process user input can be overridden
        public virtual string ProcessInput(string input)
        {
            return "I'm a basic chatbot. Ask me something!";
        }

        // Method 8 Show help can be overridden
        public virtual void ShowHelp()
        {
            Console.WriteLine("I'm a basic chatbot. Type 'exit' to quit.");
        }

        // Method 9  Say goodbye can be overridden
        public virtual void SayGoodbye()
        {
            Console.WriteLine($"Goodbye, {UserName}!");
        }

        // Method 10 Display message with color
        public void DisplayMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        // Method 11 Display message without new line
        public void DisplayPrompt(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }
    }
}