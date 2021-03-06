﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramEbooks_Bot
{
    static class UI
    {
        /// <summary>
        /// Waits for user input until either a Y or N is pressed
        /// </summary>
        private static char YesNo()
        {
            char selection;
            do
            {
                selection = char.ToLower(Console.ReadKey(true).KeyChar);
            } while (selection != 'y' && selection != 'n');
            return selection;
        }

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
                Console.WriteLine("Welcome to TUSK Telegram Ebooks bot.");
                Console.WriteLine("====================================\n");
                int selection = MainMenu();

                if (selection == 1)
                {
                    TgApi.InitTelegram();
                }
                else if (selection == 2)
                {
                    ResetSettings();
                }
            }
        }

        private static int MainMenu()
        {
            List<string> Menu = new List<string>();
            Menu.Add("Start bot");
            Menu.Add("Reset bot");

            for (int index = 0; index < Menu.Count; index++)
            {
                Console.WriteLine((index + 1) + ". " + Menu[index]);
            }
            int selection = -1;
            do
            {
                try
                {
                    selection = int.Parse(Console.ReadKey(true).KeyChar + "");
                }
                catch (Exception)
                {
                    selection = int.MinValue;
                }
                if (selection < 1 || selection > Menu.Count)
                {
                    selection = int.MinValue;
                }
            } while (selection == int.MinValue);
            return selection;
        }

        private static void ResetSettings()
        {
            Console.Clear();
            Console.WriteLine("Are you sure you want to reset all settings? [Y/N]");

            char selection = YesNo();
            if (selection == 'y')
            {
                Properties.Settings.Default.Reset();
                Properties.Settings.Default.Save();
                if (System.IO.File.Exists("chat.list"))
                {
                    System.IO.File.Delete("chat.list");
                }
            }
            Environment.Exit(0);
        }

        private static void SetApiKey()
        {
            string apiKey;
            do
            {
                Console.WriteLine("Please write your Telegram bot API key that you " +
                    "have recieved from the Botfather");
                apiKey = Console.ReadLine();
                Console.Clear();
                Console.WriteLine("Is this key correct? [Y/N]");
                Console.WriteLine(apiKey);

                char selection = YesNo();

                if (selection == 'n')
                {
                    apiKey = null;
                    Console.Clear();
                }
            } while (apiKey == null);
            Properties.Settings.Default.APIKey = apiKey;
        }

        /// <summary>
        /// Sets the bot's posting frequency with the user's help
        /// </summary>
        private static void SetPostFrequency()
        {
            int postFreq = 0;
            do
            {
                do
                {
                    Console.Clear();
                    Console.WriteLine("Please set the post frequency for all chats.");
                    Console.WriteLine("In minutes.");
                    try
                    {
                        postFreq = int.Parse(Console.ReadLine());
                    }
                    catch (Exception)
                    {
                        postFreq = 0;
                    }
                } while (postFreq <= 0);
                Console.Clear();
                Console.WriteLine("You chose " + postFreq + " minutes as the post frequency");
                Console.WriteLine("Is this okay?");
                char selection;
                do
                {
                    selection = char.ToLower(Console.ReadKey(true).KeyChar);
                } while (selection != 'y' && selection != 'n');
                if (selection == 'n')
                {
                    postFreq = 0;
                }
            } while (postFreq <= 0);
            Properties.Settings.Default.PostFrequency = postFreq;
        }

        private static void SetSleepSetting()
        {
            Console.WriteLine("Do you want the bot to sleep? [Y/N]");
            char selection = YesNo();

            if (selection == 'n')
            {
                Properties.Settings.Default.UseSleepMode = false;
            }
            else
            {
                Properties.Settings.Default.UseSleepMode = true;
                int sleepStart, sleepDur;
                do
                {
                    Console.Clear();
                    Console.WriteLine("At what hour do you want the bot to start sleeping? " +
                        "(0-23)");
                    try
                    {
                        sleepStart = int.Parse(Console.ReadLine());
                    }
                    catch (Exception)
                    {
                        sleepStart = 0;
                    }
                } while (sleepStart < 1 || sleepStart > 23);

                do
                {
                    Console.Clear();
                    Console.WriteLine("How many hours do you want the bot to sleep? " +
                        "(0-23)");
                    try
                    {
                        sleepDur = int.Parse(Console.ReadLine());
                    }
                    catch (Exception)
                    {
                        sleepDur = 0;
                    }
                } while (sleepDur < 1 || sleepDur > 23);

                Properties.Settings.Default.SleepStartHour = sleepStart;
                Properties.Settings.Default.SleepDuration = sleepDur;
            }
        }

        private static void FirstRun()
        {
            Properties.Settings.Default.FirstRun = false;
            SetApiKey();
            SetPostFrequency();
            SetSleepSetting();
            Properties.Settings.Default.Save();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Bot setup complete!\n" +
                "Press any key to show main menu.");
            Console.ResetColor();
            Console.ReadKey(true);

            Console.Clear();
            MainScreen();
        }
    }
}
