﻿using System;
using System.Collections.Generic;
using TeleBotDotNet;
using TeleBotDotNet.Requests.Methods;
using TeleBotDotNet.Responses.Methods;
using TeleBotDotNet.Responses.Types;
using System.Threading;
using ClutteredMarkov;

namespace TelegramEbooks_Bot
{
    static class TgApi
    {
        private static TeleBot ApiAccess;
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
                        try
                        {
                            readChat.Load();
                        }
                        catch (Exception)
                        {
                            readChat.Chain = new Markov();
                        }
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

        private static void SendTgMessage(int chatId, string text)
        {
            SendMessageRequest message = new SendMessageRequest();
            message.ChatId = chatId;
            message.Text = text;
            ApiAccess.SendMessage(message);
        }

        private static void RecieveUpdates(object stateInfo)
        {
            GetUpdatesRequest updateOpts = new GetUpdatesRequest();
            updateOpts.Offset = Properties.Settings.Default.LastReadMessage;
            GetUpdatesResponse response = ApiAccess.GetUpdates(updateOpts);
            List<UpdateResponse> updates = response.Result;

            if (response.Ok)
            {
                if (updates.Count > 0)
                {
                    foreach (UpdateResponse update in updates)
                    {
                        if (update.Message.Text != "" && update.Message.Text != null)
                        {
                            if (Properties.Settings.Default.Verbose)
                            {
                                string date = "";
                                if (update.Message.Date.HasValue)
                                {
                                    date = update.Message.Date.Value.ToShortTimeString() + " ";
                                }
                                Console.WriteLine(date +
                                    update.Message.From.FirstName + ":" +
                                    update.Message.Text);
                            }
                            if (update.Message.Text.StartsWith("/") == false)
                            {
                                if (ChatKeys.Contains(update.Message.Chat.Id))
                                {
                                    Chats[update.Message.Chat.Id].Chain.Feed(update.Message.Text);
                                }
                            }
                            else
                            {
                                if (update.Message.Text.ToLower().StartsWith("/subscribe"))
                                {
                                    if (ChatKeys.Contains(update.Message.Chat.Id) == false)
                                    {
                                        Chat subscriber = new Chat(update.Message.Chat.Id);
                                        try
                                        {
                                            subscriber.Load();
                                        }
                                        catch (Exception)
                                        {
                                            subscriber.Chain = new Markov();
                                        }
                                        Chats.Add(subscriber.ChatID, subscriber);
                                        ChatKeys.Add(subscriber.ChatID);

                                        SendTgMessage(update.Message.Chat.Id, "Congrats! You are now " +
                                            "subscribed to my wisdom!");
                                    }
                                    else
                                    {
                                        SendTgMessage(update.Message.Chat.Id, "Chat already subscribed.");
                                    }
                                }
                                else if (update.Message.Text.ToLower().StartsWith("/unsubscribe"))
                                {
                                    if (ChatKeys.Contains(update.Message.Chat.Id))
                                    {
                                        ChatKeys.Remove(update.Message.Chat.Id);
                                        Chats.Remove(update.Message.Chat.Id);
                                        SendTgMessage(update.Message.Chat.Id, "This chat has successfuly been unsubscribed.");
                                    }
                                    else
                                    {
                                        SendTgMessage(update.Message.Chat.Id, "Chat is not subsrcibed to begin with!\n" +
                                            "...How dare you!");
                                    }
                                }
                            }
                        }
                    }
                    Properties.Settings.Default.LastReadMessage =
                    updates[updates.Count - 1].UpdateId + 1;
                    Properties.Settings.Default.Save();
                }
            }
        }

        /// <summary>
        /// Posts a markov chain to every chat that is subscribed
        /// </summary>
        private static void Post(object stateInfo)
        {
            bool isTimeToPost = true;
            if (Properties.Settings.Default.UseSleepMode)
            {
                int endhour = GetEndHour();
                if ((DateTime.UtcNow.Hour >= Properties.Settings.Default.SleepStartHour && 
                    DateTime.UtcNow.Hour <= endhour) ||
                    (DateTime.UtcNow.Hour <= endhour && DateTime.UtcNow.Hour >= 
                    Properties.Settings.Default.SleepStartHour))
                {
                    isTimeToPost = false;
                }
            }
            if (isTimeToPost)
            {
                foreach (int key in ChatKeys)
                {
                    IChat chat = Chats[key];
                    string chain = MarkovGenerator.Create(chat.Chain);
                    SendTgMessage(chat.ChatID, chain);

                    if (Properties.Settings.Default.Verbose)
                    {
                        Console.WriteLine("Posted to chat: " + chat.ChatID + ". " + chain);
                    }
                }
            }
            if (ChatKeys.Count > 0)
            {
                SaveStuff();
            }
        }

        private static int GetEndHour()
        {
            int hour = Properties.Settings.Default.SleepStartHour;
            hour += Properties.Settings.Default.SleepDuration;
            if (hour > 23)
            {
                hour = hour - 24;
            }
            return hour;
        }

        internal static void InitTelegram()
        {
            ApiAccess = new TeleBot(Properties.Settings.Default.APIKey);
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
