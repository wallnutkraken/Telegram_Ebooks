using System;
using ClutteredMarkov;

namespace TelegramEbooks_Bot
{
    interface IChat
    {
        int ChatID { get; set; }
        Markov Chain { get; set; }
    }
}
