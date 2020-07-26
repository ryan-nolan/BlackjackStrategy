using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic
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
                    CurrentState = PlayerState.STAND;
                    return PlayerState.STAND;
                }
            }
            if (hand.handValues.First() > 21)
            {
                CurrentState = PlayerState.BUST;
                return PlayerState.BUST;
            }
            else if (hand.handValues.First() < 17)
            {
                CurrentState = PlayerState.HIT;
                return PlayerState.HIT;
            }
            else
            {
                CurrentState = PlayerState.STAND;
                return PlayerState.STAND;
            }
        }
        /// <summary>
        /// Write current state to console
        /// </summary>
        public override void WriteCurrentState() { Console.WriteLine($"DEALER REACTS: {CurrentState.ToString()}"); }

    }
}
