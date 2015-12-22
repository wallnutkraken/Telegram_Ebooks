using System;
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
        private static Dictionary<int, IChat> Chats = new Dictionary<int, IChat>();
        private static List<int> ChatKeys = new List<int>();
        private static void ReadChats()
        {
            string[] chatIds;
            if (System.IO.File.Exists("chat.list"))
            {
                chatIds = System.IO.File.ReadAllLines("chat.list");
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

        private async static void RecieveUpdates(object stateInfo)
        {
            Update[] updates = await TgAccess.GetUpdates(
                Properties.Settings.Default.LastReadMessage);
            foreach (Update update in updates)
            {
                if (update.Message.Text != "")
                {
                    if (Chats.ContainsKey(update.Message.From.Id))
                    {
                        Chats[update.Message.From.Id].Chain.Feed(update.Message.Text);
                    }
                    else
                    {
                        Chat newChat = new Chat(update.Message.From.Id);
                        newChat.Chain.Feed(update.Message.Text);
                        Chats.Add(newChat.ChatID, newChat);
                        ChatKeys.Add(newChat.ChatID);
                    }
                }
            }
        }

        private static async void Post(object stateInfo)
        {
            foreach (int key in ChatKeys)
            {
                IChat chat = Chats[key];
                await TgAccess.SendTextMessage(chat.ChatID,
                    MarkovGenerator.Create(chat.Chain));
            }
        }

        internal static void InitTelegram()
        {
            TgAccess = new Api(Properties.Settings.Default.APIKey);
            ReadChats();
            TimerCallback updateCall = RecieveUpdates;
            Timer updateThread = new Timer(updateCall, null, 0, 3000);

            TimerCallback postCall = 
        }
    }
}
