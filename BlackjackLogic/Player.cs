using System;
using System.Collections.Generic;
using System.Linq;
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
        public abstract PlayerState React(Card dealersUpCard, ref PlayerState stateToChange, Hand hand, int count);
        //public abstract PlayerState React(Card DealersUpCard, Hand hand);
        public abstract int CalculateBet(int minBet, int maxBet, int count);

        public override void WriteCurrentState() { Console.WriteLine($"PLAYER REACTS: {CurrentState.ToString()}"); }
    }

}
