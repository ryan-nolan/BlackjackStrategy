using System;
using System.Linq;

namespace BlackjackStrategy.Core.Game
{
    public class Dealer : Actor
    {
        //Dealers card revealed to player
        public Card upCard;
        /// <summary>
        /// Sets the up card
        /// </summary>
        public void SetUpCard()
        {
            upCard = hand.cards.First();
        }

        /// <summary>
        /// Reacts to a given game state
        /// Stands on > hard 17
        /// Hits on < 17
        /// Stands on hard 17
        /// </summary>
        /// <returns></returns>
        public PlayerState React()
        {
            hand.SetHandValues();
            if (hand.handValues.Count > 1)
            {
                if (hand.handValues[1] >= 17)
                {
                    CurrentState = PlayerState.Stand;
                    return PlayerState.Stand;
                }
            }
            if (hand.handValues.First() > 21)
            {
                CurrentState = PlayerState.Bust;
                return PlayerState.Bust;
            }
            else if (hand.handValues.First() < 17)
            {
                CurrentState = PlayerState.Hit;
                return PlayerState.Hit;
            }
            else
            {
                CurrentState = PlayerState.Stand;
                return PlayerState.Stand;
            }
        }
        /// <summary>
        /// Write current state to console
        /// </summary>
        public override void WriteCurrentState() { Console.WriteLine($"DEALER REACTS: {CurrentState.ToString()}"); }

    }
}
