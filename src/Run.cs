using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using Telegram.Bot;

namespace Telebot_Ebooks
{
    class Run
    {
        public static Int32 LastChat { get; set; }
        public static Int32 LastMessage { get; set; }
        public static DateTime LastSent { get; set; }
        public static void Main(string[] args)
        {
            Twitter.GetKeys();
            Bot.Use = new Api(Twitter.TelegramKey);
            Random dice = new Random();

            if (args.Contains("-v"))
            {
                LaunchArgs.Verbose = true;
            }
            if (args.Contains("-d"))
            {
                LaunchArgs.Debug = true;
            }
            
            Thread exitWaitThread = new Thread(new ThreadStart(ExitThread.Check));
            exitWaitThread.Start();

            if (File.Exists("markov.xml"))
            {
                Ebookify.ReadMarkovXML();
            }
            else
            {
                Ebookify.ReadMarkovText();
            }
            Int32 loops = 0;

            
            Twitter.Access = new TweetSharp.TwitterService(Twitter.AppKey.Token, Twitter.AppKey.TokenSecret);
            Twitter.Access.AuthenticateWith(Twitter.UserKey.Token, Twitter.UserKey.TokenSecret);
            LastSent = DateTime.UtcNow;
            int lup = 0;
            while (true)
            {
                Thread.Sleep(1000);
                Int32 time = 10; /* Default */
                if (DateTime.Now.Hour > 22 || DateTime.Now.Hour < 7)
                {
                    time = 60;
                }
                Bot.BotStuff();
                if (DateTime.UtcNow.Subtract(LastSent).Minutes >= time)
                {
                    if (LaunchArgs.Debug)
                    {
                        Console.WriteLine(DateTime.UtcNow.Subtract(LastSent).Minutes + "  out of " + time);
                    }
                    try
                    {
                        Bot.CreateChain();
                        lup = 0;
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Data + "\n";
                        message = message + ex.Message;
                        Bot.SendMessageToOwner(message);
                        lup = time - 1;
                    }
                }
                if (loops >= 60)
                {
                    loops = 0;
                    ExitThread.Save();
                }
                else
                {
                    loops++;
                }
            }
        }
    }
}
