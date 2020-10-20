namespace BlackjackLogic.Game
{
    /// <summary>
    /// Suit enum
    /// </summary>
    public enum Suit
    {
        Heart,
        Diamond,
        Spade,
        Club
    }
    /// <summary>
    /// Face enum
    /// </summary>
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
        /// <summary>
        /// Card has a suit face and value
        /// </summary>
        public Suit Suit { get; set; }
        public Face Face { get; set; }
        public int Value { get; set; }

        /// <summary>
        /// Assign card suit face and value on construction
        /// Aces have value 11
        /// J,Q,K have value 10
        /// </summary>
        /// <param name="suit"></param>
        /// <param name="face"></param>
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
        /// <summary>
        /// Clones a card, doesn't return a reference
        /// </summary>
        /// <returns>Non reference Card</returns>
        public Card Clone()
        {
            return new Card(Suit, Face);
        }
        /// <summary>
        /// Writes card as human readable string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Face} of {Suit}s";
        }
    }
}
