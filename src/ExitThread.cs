using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Telebot_Ebooks
{
    static class ExitThread
    {
        public static void Save()
        {
            Ebookify.SaveMarkov();
            string msg = Run.LastMessage + "";
            string[] wr = { msg };
            File.WriteAllLines("lastreadmessage", wr);
        }
        public static void Check()
        {
            while (true)
            {
                char ch = Console.ReadKey(true).KeyChar;
                if (ch == 'q')
                {
                    Save();
                    Environment.Exit(0);
                }
            }
        }
    }
}
