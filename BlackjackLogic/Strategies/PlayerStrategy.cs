using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic.Strategies
{
    public class PlayerStrategy : Player
    {
        public override int CalculateBet(int minBet, int maxBet, int count)
        {
            throw new NotImplementedException();
        }

        public override PlayerState React(Card DealersUpCard, ref PlayerState stateToChange, Hand hand, int count)
        {
            throw new NotImplementedException();
        }
    }
}
