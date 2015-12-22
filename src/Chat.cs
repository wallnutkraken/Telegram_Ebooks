using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClutteredMarkov;

namespace TelegramEbooks_Bot
{
    class Chat : IChat
    {
        private int _ChatID;
        public int ChatID
        {
            get
            {
                return _ChatID;
            }
        }
        public Chat(int chatID)
        {
            _ChatID = chatID;
        }
        public Markov Chain { get; set; }
        public void Save()
        {
            Chain.SaveChainState(Convert.ToString(ChatID));
        }

        public void Load()
        {
            Chain.LoadChainState(Convert.ToString(ChatID));
        }

        public override string ToString()
        {
            return Convert.ToString(ChatID);
        }
    }
}
