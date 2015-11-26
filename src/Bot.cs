using System;
using System.Collections.Generic;
using System.Linq;
using TweetSharp;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telebot_Ebooks
{
    static class Bot
    {
        internal static Api Use { get; set; }
        private static int Waiterino { get; set; } = 0;

        public async static void CreateChain()
        {
            string tweet = Twitter.CharacterLimit(Ebookify.Markov());
            if (LaunchArgs.Verbose)
            {
                Console.WriteLine("Created tweet:\n" + tweet + "\n");
            }
            //TweetSharp.SendTweetOptions opts = new TweetSharp.SendTweetOptions();
            //opts.Status = tweet;
            //Twitter.Access.SendTweet(opts);
            await Bot.Use.SendTextMessage(-35276119, tweet);
            Run.LastSent = DateTime.UtcNow;
        }

        public async static void SendMessageToOwner(string message)
        {
            await Bot.Use.SendTextMessage(118857134, message);
        }

        public async static void BotStuff()
        {
            Update[] updates = await Use.GetUpdates(Run.LastMessage + 1);
            
            if (updates.Length > 0)
            {
                foreach (Update status in updates)
                {
                    if (status.Message.Text != null)
                    {
                        if (LaunchArgs.Verbose && status.Message.Text != "")
                        {
                            DateTime msgTime = status.Message.Date;
                            Console.Write(msgTime.ToShortDateString() + " " + msgTime.ToShortTimeString() + " ");
                            Console.Write("Message ID:" + status.Id + ". Chat ID: " + status.Message.Chat.Id + ". ");
                            Console.WriteLine(status.Message.From.FirstName + ": " + status.Message.Text);
                        }
                        Run.LastMessage = status.Id;
                        if (status.Message.Text != "")
                        {
                            Ebookify.AddToMarkov(status.Message.Text);
                        }
                        Run.LastChat = status.Message.From.Id;
                    }
                }
            }
        }

        public static void SendDM(string message)
        {
            SendDirectMessageOptions msg = new SendDirectMessageOptions();
            msg.Text = message;
            msg.ScreenName = "ArgumentNullExc";
            Twitter.Access.SendDirectMessage(msg);
            if (Twitter.Access.Response.Error != null)
            {
                if (LaunchArgs.Verbose || true)
                {
                    Console.WriteLine(Twitter.Access.Response.Error.Code + ": " + Twitter.Access.Response.Error.Message);
                }
            }
        }
    }
}
