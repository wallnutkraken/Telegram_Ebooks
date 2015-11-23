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
        private static DateTime LastSent { get; set; }
        public static void Main(string[] args)
        {
            Twitter.GetKeys();
            Bot.Use = new Api(Twitter.TelegramKey);
            Random dice = new Random();

            /* Only for testing */
            LaunchArgs.Verbose = true;
            
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
            LastSent = DateTime.Now;
            while (true)
            {
                Thread.Sleep(1000);
                Int32 time = 5; /* Default */
                if (DateTime.Now.Hour > 22 || DateTime.Now.Hour < 7)
                {
                    time = 60;
                }
                Bot.BotStuff();
                if (DateTime.Now.Subtract(LastSent).Minutes >= dice.Next(10, time))
                {
                    try
                    {
                        string tweet = Twitter.CharacterLimit(Ebookify.Markov());
                        if (LaunchArgs.Verbose)
                        {
                            Console.WriteLine("Created tweet:\n" + tweet + "\n");
                        }
                        TweetSharp.SendTweetOptions opts = new TweetSharp.SendTweetOptions();
                        opts.Status = tweet;
                        Twitter.Access.SendTweet(opts);
                        Console.ReadKey(true);
                        Bot.Use.SendTextMessage(LastChat, tweet);
                        LastSent = DateTime.Now;
                    }
                    catch (ArgumentException)
                    {
                        /* Do nothing! */
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
