using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot;

namespace TelegramEbooks_Bot
{
    static class TgApi
    {
        private static Api Telegram;

        internal static void InitTelegram()
        {
            Telegram = new Api(Properties.Settings.Default.APIKey);
        }
    }
}
