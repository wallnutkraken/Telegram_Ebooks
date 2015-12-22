using System;
using ClutteredMarkov;

namespace TelegramEbooks_Bot
{
    interface IChat
    {
        int ChatID { get; }
        Markov Chain { get; set; }
    }
}
