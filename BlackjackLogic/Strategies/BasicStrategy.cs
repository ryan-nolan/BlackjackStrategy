using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic.Strategies
{
    public class BasicStrategy : Player
    {
        public override int CalculateBet(int minBet, int maxBet)
        {
            throw new NotImplementedException();
        }

        public override PlayerState React(Card dealersUpCard, ref PlayerState stateToChange, Hand hand)
        {
            
        }
    }
}
