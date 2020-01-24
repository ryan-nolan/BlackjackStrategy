using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic
{
    public enum Suit
    {
        Heart,
        Diamond,
        Spade,
        Club
    }

    public enum Face
    {
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
    }


    public class Card
    {
        public Suit Suit { get; set; }
        public Face Face { get; set; }
        public int Value { get; set; }

        public Card(Suit suit, Face face)
        {
            Suit = suit;
            Face = face;
            switch (face)
            {
                case Face.Ace:
                    Value = 1; // 1 or 11
                    break;
                case Face.Two:
                    Value = 2;
                    break;
                case Face.Three:
                    Value = 3;
                    break;
                case Face.Four:
                    Value = 4;
                    break;
                case Face.Five:
                    Value = 5;
                    break;
                case Face.Six:
                    Value = 6;
                    break;
                case Face.Seven:
                    Value = 7;
                    break;
                case Face.Eight:
                    Value = 8;
                    break;
                case Face.Nine:
                    Value = 9;
                    break;
                case Face.Ten:
                    Value = 10;
                    break;
                case Face.Jack:
                    Value = 11; // 10 in Blackjack
                    break;
                case Face.Queen:
                    Value = 12; // 10 in Blackjack
                    break;
                case Face.King:
                    Value = 13; // 10 in Blackjack
                    break;
                default:
                    break;
            }
        }

        public override string ToString()
        {
            return $"{Face} of {Suit}s";
        }
    }
}
