using System;
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
