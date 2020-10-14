using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackjackLogic;
using BlackjackLogic.Game;

namespace BlackjackTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Deck deck = new Deck(104);
            Console.WriteLine(deck.GetDeckHash());
            deck.TrueShuffle();
            Console.WriteLine(deck.GetDeckHash());
        }
    }
}
