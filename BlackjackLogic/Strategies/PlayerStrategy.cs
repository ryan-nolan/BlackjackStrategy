﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic.Strategies
{
    public class PlayerStrategy : Player
    {
        public override int CalculateBet(int minBet, int maxBet)
        {
            throw new NotImplementedException();
        }

        public override PlayerState React(Card DealersUpCard, ref PlayerState stateToChange, Hand hand)
        {
            throw new NotImplementedException();
        }
    }
}