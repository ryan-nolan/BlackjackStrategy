using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic
{
    public class Dealer : Actor
    {
        public Card upCard;

        public void SetUpCard()
        {
            upCard = hand.cards.First();
        }


        public PlayerState React()
        {
            if (hand.handValues.First() > 21)
            {
                return PlayerState.BUST;
            }
            else if (hand.handValues.First() < 16)
            {
                return PlayerState.HIT;
            }
            else
            {
                return PlayerState.STAND;
            }
        }

    }
}
