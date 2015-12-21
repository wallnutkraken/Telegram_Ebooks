using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramEbooks_Bot
{
    static class UI
    {
        public static void Screen()
        {
            if (Properties.Settings.Default.FirstRun)
            {
                FirstRun();
            }
            else
            {
                MainScreen();
            }
        }

        private static void MainScreen()
        {
            if (Properties.Settings.Default.RunOnly && Properties.Settings.Default.FirstRun == false)
            {
                TgApi.InitTelegram();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        private static void FirstRun()
        {
            throw new NotImplementedException();
        }
    }
}
