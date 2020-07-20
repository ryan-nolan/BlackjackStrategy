using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic
{
    public class Deck
    {
        public Stack<Card> Cards = new Stack<Card>();
        public static int DeckSize = 52;
        static Random rand = new Random();
        public Deck(int deckSize)
        {
            if (deckSize%52 != 0)
            {
                deckSize = 52;
                Console.WriteLine("InvalidDeckValue: 52 Card Deck Used");
                BuildDeck();
            }
            else
            {
                DeckSize = deckSize;
                BuildDeck(DeckSize);
            }

        }

        private void BuildDeck(int deckSize)
        {
            for (int i = 0; i < deckSize/52; i++)
            {
                foreach (Suit s in Enum.GetValues(typeof(Suit)))
                {
                    foreach (Face f in Enum.GetValues(typeof(Face)))
                    {
                        Cards.Push(new Card(s, f));
                    }
                }
            }
            Shuffle();

            VerifyDeck(Cards);
        }

        //public void Shuffle()
        //{
        //    var stackToArray = Cards.ToArray();
        //    var rnd = new Random();
        //    Cards.Clear();
        //    foreach (var card in stackToArray.OrderBy(x => rnd.Next()))
        //    {
        //        Cards.Push(card);
        //    }
        //}

        public void Shuffle()
        {
            Card[] deck = Cards.ToArray();
            for (int n = deck.Length - 1; n > 0; --n)
            {
                int k = rand.Next(n + 1);
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

        static public int[] GetTrueRandNumsIntArray()
        {
            string html = string.Empty;
            string url = @"https://www.random.org/sequences/?min=0&max=51&col=1&format=html&rnd=new";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //Console.WriteLine(req);

            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            //Console.WriteLine(html);

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            HtmlNode data = htmlDocument.DocumentNode.SelectSingleNode("//pre");
            //Console.WriteLine(data.InnerText);
            int[] trueRandomNumbers = new int[52];
            string[] randNumsStringArray = data.InnerText.Split('\n');
            for (int i = 0; i < trueRandomNumbers.Length; i++)
            {
                trueRandomNumbers[i] = Int32.Parse(randNumsStringArray[i + 1].Trim());
            }
            return trueRandomNumbers;
        }

        private bool VerifyDeck(Stack<Card> cards)
        {
            if (cards.Count % cards.Distinct().Count() != 0)
            {
                Console.WriteLine("FALSE DECK IS INVALID");
                throw new Exception { };
                return false;
            }
            if (cards.Count % 52 != 0)
            {
                Console.WriteLine("FALSE");
                throw new Exception { };
                return false;
            }
            Console.WriteLine("DECK IS VALID");
            return true;
        }

        public void BuildDeck()
        {

            foreach (Suit s in Enum.GetValues(typeof(Suit)))
            {
                foreach (Face f in Enum.GetValues(typeof(Face)))
                {
                    Cards.Push(new Card(s, f));
                }
            }
            //try
            //{
            //    TrueShuffle();
            //}
            //catch (Exception e)
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(e);
            //    Shuffle();
            //    Console.ResetColor();
            //}
            Shuffle();

            VerifyDeck(Cards);
        }

        public override string ToString()
        {
            string returnStr = string.Empty;
            foreach (var c in Cards)
            {
                returnStr += c.ToString();
            }
            return returnStr;
        }

        public string GetDeckHash()
        {
            string returnStr = ToString();
            var bytes = Encoding.UTF8.GetBytes(returnStr);
            //Could do byte conversion to check if deck combination has been used
            return Convert.ToBase64String(bytes);
        }
    }
}
