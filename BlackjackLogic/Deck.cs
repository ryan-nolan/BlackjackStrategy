using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic
{
    public class Deck
    {
        public Stack<Card> Cards = new Stack<Card>();

        public Deck()
        {
            BuildDeck();
        }

        public void Shuffle()
        {
            var stackToArray = Cards.ToArray();
            var rnd = new Random();
            Cards.Clear();
            foreach (var card in stackToArray.OrderBy(x => rnd.Next()))
            {
                Cards.Push(card);
            }
        }

        private bool VerifyDeck(Stack<Card> cards)
        {
            if (cards.Count != cards.Distinct().Count())
            {
                Console.WriteLine("FALSE");
                return false;
            }
            if (cards.Count != 52)
            {
                Console.WriteLine("FALSE");
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
            Shuffle();

            VerifyDeck(Cards);
        }
    }
}
