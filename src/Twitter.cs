using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using TweetSharp;
using System.Diagnostics;

namespace Telebot_Ebooks
{
    static class Twitter
    {
        internal static TwitterService Access { get; set; }
        internal static OAuthAccessToken AppKey { get; set; } = new OAuthAccessToken();
        internal static OAuthAccessToken UserKey { get; set; } = new OAuthAccessToken();
        internal const string FILENAME = "keys.conf";
        internal static string TelegramKey { get; set; }
        public static void GetKeys()
        {
            if (!File.Exists(FILENAME))
            {
                throw new FileNotFoundException("keys.conf does not exist.");
            }
            List<string> readAPIKeys = File.ReadAllLines("keys.conf").ToList();

            int tempIndex = 0;
            while (tempIndex < readAPIKeys.Count) /* Removes empty lines */
            {
                if (readAPIKeys[tempIndex].Trim(' ').CompareTo("") == 0)
                {
                    readAPIKeys.RemoveAt(tempIndex);
                }
                else
                {
                    tempIndex++;
                }
            }

            for (int index = 0; index < readAPIKeys.Count; index++)
            {
                string[] splitter = readAPIKeys[index].Split('=');
                if (splitter[0].ToLower().CompareTo("telegram") == 0)
                {
                    TelegramKey = splitter[1];
                }
                if (splitter[0].ToLower().CompareTo("apptoken") == 0)
                {
                    AppKey.Token = splitter[1];
                }
                if (splitter[0].ToLower().CompareTo("appsecret") == 0)
                {
                    AppKey.TokenSecret = splitter[1];
                }
                if (splitter[0].ToLower().CompareTo("usertoken") == 0)
                {
                    UserKey.Token = splitter[1];
                }
                if (splitter[0].ToLower().CompareTo("usersecret") == 0)
                {
                    UserKey.TokenSecret = splitter[1];
                }
            }
            if (AppKey.Token == null || AppKey.TokenSecret == null)
            {
                Console.WriteLine("Error: keys.conf misconfigured.");
                Console.ReadKey(true);
                Environment.Exit(0);
            }
            if (UserKey.Token == null || UserKey.TokenSecret == null)
            {
                TwitterService service = new TwitterService(AppKey.Token, AppKey.TokenSecret);
                OAuthRequestToken requestToken = service.GetRequestToken();
                Uri uri = service.GetAuthorizationUri(requestToken);
                Process.Start(uri.ToString());
                service.AuthenticateWith(AppKey.Token, AppKey.TokenSecret);

                Console.Write("Please input the authentication number: ");
                string verifier = Console.ReadLine();
                UserKey = service.GetAccessToken(requestToken, verifier);
                SaveFile();
            }

        }
        internal static void SaveFile()
        {
            StreamWriter writer = new StreamWriter(FILENAME);
            writer.WriteLine("telegram=" + TelegramKey);
            writer.WriteLine("appToken=" + AppKey.Token);
            writer.WriteLine("appSecret=" + AppKey.TokenSecret);
            writer.WriteLine("userToken=" + UserKey.Token);
            writer.WriteLine("userSecret=" + UserKey.TokenSecret);
            writer.Close();
        }

        public static string CharacterLimit(string message)
        {
            string newstr = "";
            bool strDone = false;
            string[] words = message.Split(' ');
            for (int index = 0; index < words.Length && strDone == false; index++)
            {
                if (newstr.Length + words[index].Length > 139)
                {
                    strDone = false;
                }
                else
                {
                    newstr = newstr + " " + words[index];
                }
            }
            return newstr;
        }
    }
}
