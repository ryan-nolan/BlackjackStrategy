using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic
{


    public abstract class Player : Actor
    {
        //public Hand Hand;
        public int Chips;
        public int Stake;
        public int SplitHandStake;
        public bool isDoublingDown;
        public Hand splitHand = null;
        public PlayerState splitHandState;
        public List<int> Count = new List<int> { 0, 0 };

        public virtual string CountType { get; protected set; }
        public virtual string StrategyName { get; protected set; }

        public Player()
        {
            IsBust = false;
            Chips = 500;
        }
        public Player(int chips)
        {
            IsBust = false;
            Chips = chips;
        }

        public void AddBet(int bet, ref int stake)
        {
            stake += bet;
            Chips -= bet;
        }

        //Make decision based of current game state and strategy
        public abstract PlayerState React(Card dealersUpCard, ref PlayerState stateToChange, Hand hand, List<int> count);
        //public abstract PlayerState React(Card DealersUpCard, Hand hand);
        public abstract int CalculateBet(int minBet, int maxBet);

        public abstract List<int> UpdateCount(Deck deck, List<Card> burntCards, Card dealersUpCard);

        public override void WriteCurrentState() { Console.WriteLine($"PLAYER REACTS: {CurrentState.ToString()}"); }
    }

}
