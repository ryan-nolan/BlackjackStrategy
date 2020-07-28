using System.Collections.Generic;
using System.Linq;

namespace BlackjackLogic.Strategies
{
    public class DealerStrategy : Player
    {
        public override string StrategyName { get { return "DealerStrategy"; } }
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
        /// <param name="DealersUpCard"></param>
        /// <param name="stateToChange"></param>
        /// <param name="hand"></param>
        /// <param name="count"></param>
        /// <returns></returns>
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
