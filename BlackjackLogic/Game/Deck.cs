using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace BlackjackLogic.Game
{
    public class Deck
    {
        //Deck constains a stack of cards
        public Stack<Card> Cards = new Stack<Card>();
        //Static deck size so deck sizing can be check without instantiated object
        public static int DeckSize = 52;
        //PRNG for shuffle
        static Random _rand = new Random();
        /// <summary>
        /// Initialises deck of size deckSize
        /// Defaults to 52 if invalid deckSize is given
        /// </summary>
        /// <param name="deckSize"></param>
        public Deck(int deckSize)
        {
            if (deckSize % 52 != 0 || deckSize < 0)
            {
                DeckSize = 52;
                Console.WriteLine("InvalidDeckValue: 52 Card Deck Used");
                BuildDeck();
            }
            else
            {
                DeckSize = deckSize;
                BuildDeck(DeckSize);
            }

        }
        /// <summary>
        /// Builds a deck of deckSize
        /// </summary>
        /// <param name="deckSize"></param>
        private void BuildDeck(int deckSize)
        {
            for (int i = 0; i < deckSize / 52; i++)
            {
                foreach (Suit s in Enum.GetValues(typeof(Suit)))
                {
                    foreach (Face f in Enum.GetValues(typeof(Face)))
                    {
                        Cards.Push(new Card(s, f));
                    }
                }
            }
            FisherYatesShuffle();

            VerifyDeck(Cards);
        }
        /// <summary>
        /// Fisher-Yates Shuffle
        /// Capable of > 54! deck permutations
        /// https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        /// </summary>
        public void FisherYatesShuffle()
        {
            Card[] deck = Cards.ToArray();
            for (int n = deck.Length - 1; n > 0; --n)
            {
                int k = _rand.Next(n + 1);
                Card temp = deck[n];
                deck[n] = deck[k];
                deck[k] = temp;
            }
            Cards.Clear();
            foreach (var card in deck)
            {
                Cards.Push(card);
            }
        }
        /// <summary>
        /// Creates a truly random non biased shuffle
        /// based of a 1-52 sequence gathered from https://www.random.org/
        /// unused because of cost
        /// </summary>
        public void TrueShuffle()
        {
            int[] randNums = GetTrueRandNumsIntArray();
            var array = Cards.ToArray();
            int n = array.Length;
            for (int i = 0; i < (n - 1); i++)
            {
                // Use Next on random instance with an argument.
                // ... The argument is an exclusive bound.
                //     So we will not go past the end of the array.
                int r = randNums[i];
                Card t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
            Cards.Clear();
            foreach (var card in array)
            {
                Cards.Push(card);
            }
            //Console.WriteLine(ToString());
        }
        /// <summary>
        /// Get sequence form https://www.random.org/
        /// Called in true shuffle algorithm
        /// </summary>
        /// <returns></returns>
        static public int[] GetTrueRandNumsIntArray()
        {
            string html = string.Empty;
            string url = @"https://www.random.org/sequences/?min=0&max=51&col=1&format=html&rnd=new";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            HtmlNode data = htmlDocument.DocumentNode.SelectSingleNode("//pre");

            int[] trueRandomNumbers = new int[52];
            string[] randNumsStringArray = data.InnerText.Split('\n');
            for (int i = 0; i < trueRandomNumbers.Length; i++)
            {
                trueRandomNumbers[i] = Int32.Parse(randNumsStringArray[i + 1].Trim());
            }
            return trueRandomNumbers;
        }
        /// <summary>
        /// Verify the deck contains correct amount of cards on generation
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public bool VerifyDeck(Stack<Card> cards)
        {
            if (cards.Count % cards.Distinct().Count() != 0)
            {
                Console.WriteLine("FALSE DECK IS INVALID");
                //throw new Exception { };
                return false;
            }
            if (cards.Count % 52 != 0)
            {
                Console.WriteLine("FALSE");
                //throw new Exception { };
                return false;
            }
            Console.WriteLine("DECK IS VALID");
            return true;
        }
        /// <summary>
        /// Build deck with no constructor
        /// Defaults to size 52
        /// </summary>
        private void BuildDeck()
        {

            foreach (Suit s in Enum.GetValues(typeof(Suit)))
            {
                foreach (Face f in Enum.GetValues(typeof(Face)))
                {
                    Cards.Push(new Card(s, f));
                }
            }
            FisherYatesShuffle();

            VerifyDeck(Cards);
        }
        /// <summary>
        /// Outputs deck as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string returnStr = string.Empty;
            foreach (var c in Cards)
            {
                returnStr += c.ToString();
            }
            return returnStr;
        }
        /// <summary>
        /// Turns deck string into a base64 string ready for storage
        /// </summary>
        /// <returns></returns>
        public string GetDeckHash()
        {
            string returnStr = ToString();
            var bytes = Encoding.UTF8.GetBytes(returnStr);
            //Could do byte conversion to check if deck combination has been used
            return Convert.ToBase64String(bytes);
        }
    }
}
