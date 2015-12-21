using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramEbooks_Bot
{
    static class TgApi
    {
        private static Api Telegram;
        private static List<Chat> Chats = new List<Chat>();
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
                        Chat readChat = new Chat();
                        readChat.ChatID = int.Parse(line);
                        readChat.Load();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        private static void Update()
        {
            throw new NotImplementedException();
        }
        internal static void InitTelegram()
        {
            Telegram = new Api(Properties.Settings.Default.APIKey);
            ReadChats();
        }
    }
}
