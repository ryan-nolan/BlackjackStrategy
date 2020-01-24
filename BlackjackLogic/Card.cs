using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static BlackjackLogic.Face

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
        Ace = 1,
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
            switch (Face)
            {
                case Face.Ten:
                case Face.Jack:
                case Face.Queen:
                case Face.King:
                    Value = 10;
                    break;
                case Face.Ace:
                    Value = 11;
                    break;
                default:
                    Value = (int)Face;
                    break;
            }
        }

        public override string ToString()
        {
            return $"{Face} of {Suit}s";
        }
    }
}
