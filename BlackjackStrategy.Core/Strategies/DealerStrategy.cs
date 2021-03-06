﻿using System.Collections.Generic;
using System.Linq;
using BlackjackStrategy.Core.Game;

namespace BlackjackStrategy.Core.Strategies
{
    public class DealerStrategy : Player
    {
        public override string StrategyName => "DealerStrategy";

        /// <summary>
        /// Returns min bet
        /// </summary>
        /// <param name="minBet"></param>
        /// <param name="maxBet"></param>
        /// <returns>minBet</returns>
        public override int CalculateBet(int minBet, int maxBet)
        {
            return minBet;
        }
        /// <summary>
        /// Dealer strategy holds no count
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="burntCards"></param>
        /// <param name="dealersUpCard"></param>
        /// <returns></returns>
        public override List<int> UpdateCount(Deck deck, List<Card> burntCards, Card dealersUpCard)
        {
            return Count;
        }
        /// <summary>
        /// Mimics dealer decisions
        /// </summary>
        /// <param name="dealersUpCard"></param>
        /// <param name="stateToChange"></param>
        /// <param name="hand"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override PlayerState React(Card dealersUpCard, ref PlayerState stateToChange, Hand hand, List<int> count)
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
            else if (hand.handValues.First() < 16)
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
    }
}
