using System.Collections.Generic;
using BlackjackStrategy.Core.Game;

namespace BlackjackStrategy.Core
{
    public interface IPlayer
    {
        int Chips { get; set; }
        public int Stake { get; set; }
        public int SplitHandStake { get; set; }
        public bool IsDoublingDown { get; set; }
        public Hand SplitHand { get; set; }
        public PlayerState SplitHandState { get; set; }
        public List<int> Count { get; set; }
        string CountType { get; set; }
        string StrategyName { get; set; }

        /// <summary>
        /// Adds a bet to a players stake
        /// Removes bet from players chips
        /// </summary>
        /// <param name="bet"></param>
        /// <param name="stake"></param>
        void AddBet(int bet, ref int stake);

        /// <summary>
        /// //Make decision based of current game state and strategy
        /// To be implemented diffrently depending on how strategy makes decisions
        /// </summary>
        /// <param name="dealersUpCard"></param>
        /// <param name="stateToChange"></param>
        /// <param name="hand"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        PlayerState React(Card dealersUpCard, ref PlayerState stateToChange, Hand hand, List<int> count);

        /// <summary>
        /// Calculate Bet 
        /// </summary>
        /// <param name="minBet"></param>
        /// <param name="maxBet"></param>
        /// <returns></returns>
       int CalculateBet(int minBet, int maxBet);
        /// <summary>
        /// Keeps a running count based on state of player, deck, burnt cards and the dealers up card
        /// Dealers up card can be null
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="burntCards"></param>
        /// <param name="dealersUpCard"></param>
        /// <returns></returns>
        List<int> UpdateCount(Deck deck, List<Card> burntCards, Card dealersUpCard);
        /// <summary>
        /// Writes state to console
        /// </summary>
        void WriteCurrentState();
    }
}
