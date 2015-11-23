using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using TextMarkovChains;

namespace Telebot_Ebooks
{
    static class Ebookify
    {
        public static int MessageMin { get; set; } = 6;
        public static int MessageMax { get; set; } = 10;
        public static List<string> TextFile { get; set; }

        private static TextMarkovChain Chain = new TextMarkovChain();

        public static string Markov()
        {
            if (Chain.readyToGenerate())
            {
                return Chain.generateSentence();
            }
            else
            {
                throw new ArgumentException("Markov chain is not ready to be generated");
            }
        }

        public static void AddToMarkov(string line)
        {
            Chain.feed(line);
        }
        public static void AddToMarkov(List<string> lines)
        {
            foreach (string line in lines)
            {
                Chain.feed(line);
            }
        }

        public static void ReadMarkovText()
        {
            const string filename = "markov.text";
            TextFile = File.ReadAllLines(filename).ToList();

            foreach (string line in TextFile)
            {
                Chain.feed(line);
            }
        }

        public static void ReadMarkovXML()
        {
            const string filename = "markov.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            Chain.feed(doc);
        }

        public static void SaveMarkov()
        {
            Chain.save("markov.xml");
        }
    }
}
