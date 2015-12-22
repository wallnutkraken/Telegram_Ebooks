﻿using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading;
using ClutteredMarkov;

namespace TelegramEbooks_Bot
{
    static class TgApi
    {
        private static Api TgAccess;
        private static Dictionary<int, Chat> Chats = new Dictionary<int, Chat>();
        private static List<int> ChatKeys = new List<int>();
        private static void ReadChats()
        {
            string[] chatIds;
            chatIds = System.IO.File.ReadAllLines("chat.list");
            if (chatIds.Length > 0)
            {
                foreach (string line in chatIds)
                {
                    try
                    {
                        Chat readChat = new Chat(int.Parse(line));
                        ChatKeys.Add(readChat.ChatID);
                        readChat.Load();
                        Chats.Add(readChat.ChatID, readChat);
                    }
                    catch (Exception)
                    {
                        /* ~~~ */
                    }
                }
            }
        }

        private static void SaveStuff()
        {
            List<string> chatKeys = new List<string>();
            foreach (int key in ChatKeys)
            {
                chatKeys.Add(key + "");
                Chats[key].Save();
            }

            System.IO.File.WriteAllLines("chat.list", chatKeys);
        }

        private async static void RecieveUpdates(object stateInfo)
        {
            Update[] updates = await TgAccess.GetUpdates(
                Properties.Settings.Default.LastReadMessage);
            foreach (Update update in updates)
            {
                if (update.Message.Text != "")
                {
                    if (update.Message.Text.StartsWith("/") == false)
                    {
                        if (ChatKeys.Contains(update.Message.From.Id))
                        {
                            Chats[update.Message.From.Id].Chain.Feed(update.Message.Text);
                        }
                    }
                    else
                    {
                        if (update.Message.Text.ToLower().StartsWith("/subscribe"))
                        {
                            if (ChatKeys.Contains(update.Message.From.Id) == false)
                            {
                                Chat subscriber = new Chat(update.Message.From.Id);
                                Chats.Add(subscriber.ChatID, subscriber);
                                ChatKeys.Add(subscriber.ChatID);
                                await TgAccess.SendTextMessage(update.Message.From.Id, "Congrats! You are now " +
                                    "subscribed to my wisdom!");
                            }
                            else
                            {
                                await TgAccess.SendTextMessage(update.Message.From.Id, "Chat already subscribed.");
                            }
                        }
                        else if (update.Message.Text.ToLower().StartsWith("/unsubscribe"))
                        {
                            if (ChatKeys.Contains(update.Message.From.Id))
                            {
                                ChatKeys.Remove(update.Message.From.Id);
                                Chats.Remove(update.Message.From.Id);
                                await TgAccess.SendTextMessage(update.Message.From.Id, "This chat has successfuly been unsubscribed.");
                            }
                            else
                            {
                                await TgAccess.SendTextMessage(update.Message.From.Id, "Chat is not subsrcibed to begin with!\n" +
                                    "...How dare you!");
                            }
                        }
                    }
                }
            }
            Properties.Settings.Default.LastReadMessage =
                updates[updates.Length - 1].Id;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Posts a markov chain to every chat that is subscribed
        /// </summary>
        private static async void Post(object stateInfo)
        {
            foreach (int key in ChatKeys)
            {
                IChat chat = Chats[key];
                await TgAccess.SendTextMessage(chat.ChatID,
                    MarkovGenerator.Create(chat.Chain));
            }
            if (ChatKeys.Count > 0)
            {
                SaveStuff();
            }
        }

        internal static void InitTelegram()
        {
            TgAccess = new Api(Properties.Settings.Default.APIKey);
            if (System.IO.File.Exists("chat.list") == false)
            {
                System.IO.File.WriteAllLines("chat.list", new string[1]);
            }
            ReadChats();

            TimerCallback updateCall = RecieveUpdates;
            Timer updateThread = new Timer(updateCall, null, 0, 3000);

            Thread.Sleep(1000);

            TimerCallback postCall = Post;
            Timer postThread = new Timer(postCall, null, 0,
                (60 * 1000) * Properties.Settings.Default.PostFrequency);

            char selection;
            Console.WriteLine("Bot running. Please press 'q' to quit");
            do
            {
                selection = char.ToLower(Console.ReadKey(true).KeyChar);
            } while (selection != 'q');
            Exit();
        }


        private static void Exit()
        {
            SaveStuff();
            Environment.Exit(0);
        }
    }
}
