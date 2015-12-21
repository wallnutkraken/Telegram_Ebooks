using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramEbooks_Bot
{
    static class Arguments
    {
        public static void SetUpArgs(string[] args)
        {
            Properties.Settings.Default.RunOnly = false;
            Properties.Settings.Default.Verbose = false; /* In case it is saved with true, */
            Properties.Settings.Default.Save();          /* which is not desireable */
            for (int index = 0; index < args.Length; index++)
            {
                if (args[index].ToLower() == "-h")
                {
                    Help();
                }
                else if (args[index].ToLower() == "-v")
                {
                    Properties.Settings.Default.Verbose = true;
                }
                else if (args[index].ToLower() == "-r")
                {
                    Properties.Settings.Default.RunOnly = true;
                }
            }
        }
        
        private static void WriteAtEdge(string message)
        {
            Console.SetCursorPosition((Console.WindowWidth - message.Length) / 2, Console.CursorTop);
            Console.Write(message);
        }

        private static void Help()
        {
            Console.Write("\nShows the help");
            WriteAtEdge("-h\n");
            Console.Write("Runs the bot in verbose mode");
            WriteAtEdge("-v\n");
            Console.Write("Runs the bot without showing any menus");
            WriteAtEdge("-r\n");
            Environment.Exit(0);
        }
    }
}
