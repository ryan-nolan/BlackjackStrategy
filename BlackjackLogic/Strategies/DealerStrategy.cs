﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic.Strategies
{
    public class DealerStrategy : Player
    {
        public override string StrategyName { get { return "DealerStrategy"; } }
        public override int CalculateBet(int minBet, int maxBet)
        {
            return minBet;
        }
        public override List<int> UpdateCount(Deck deck, List<Card> burntCards, Card dealersUpCard)
        {
            return Count;
        }
        public override PlayerState React(Card DealersUpCard, ref PlayerState stateToChange, Hand hand, List<int> count)
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
