using System;
using System.Collections.Generic;
using BlackjackLogic.Game;

namespace BlackjackLogic
{


    public abstract class Player : Actor
    {
        public int Chips;
        public int Stake;
        public int SplitHandStake;
        public bool isDoublingDown;
        public Hand splitHand = null;
        public PlayerState splitHandState;
        public List<int> Count = new List<int> { 0, 0 };

        public virtual string CountType { get; protected set; }
        public virtual string StrategyName { get; protected set; }
        /// <summary>
        /// Default constructor for player, to be overwritten
        /// </summary>
        public Player()
        {
            IsBust = false;
            Chips = 500;
        }
        /// <summary>
        /// Constructs a player with chips amount
        /// </summary>
        /// <param name="chips"></param>
        public Player(int chips)
        {
            IsBust = false;
            Chips = chips;
        }
        /// <summary>
        /// Adds a bet to a players stake
        /// Removes bet from players chips
        /// </summary>
        /// <param name="bet"></param>
        /// <param name="stake"></param>
        public void AddBet(int bet, ref int stake)
        {
            stake += bet;
            Chips -= bet;
        }

        /// <summary>
        /// //Make decision based of current game state and strategy
        /// To be implemented diffrently depending on how strategy makes decisions
        /// </summary>
        /// <param name="dealersUpCard"></param>
        /// <param name="stateToChange"></param>
        /// <param name="hand"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public abstract PlayerState React(Card dealersUpCard, ref PlayerState stateToChange, Hand hand, List<int> count);

        /// <summary>
        /// Calculate Bet 
        /// </summary>
        /// <param name="minBet"></param>
        /// <param name="maxBet"></param>
        /// <returns></returns>
        public abstract int CalculateBet(int minBet, int maxBet);
        /// <summary>
        /// Keeps a running count based on state of player, deck, burnt cards and the dealers up card
        /// Dealers up card can be null
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="burntCards"></param>
        /// <param name="dealersUpCard"></param>
        /// <returns></returns>
        public abstract List<int> UpdateCount(Deck deck, List<Card> burntCards, Card dealersUpCard);
        /// <summary>
        /// Writes state to console
        /// </summary>
        public override void WriteCurrentState() { Console.WriteLine($"PLAYER REACTS: {CurrentState.ToString()}"); }
    }

}
