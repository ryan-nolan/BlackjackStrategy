using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic.Strategies
{
    public class DealerStrategy : Player
    {
        public PlayerState React(Card DealersUpCard)
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
            else if (hand.handValues.First() < 16)
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
    }
}
