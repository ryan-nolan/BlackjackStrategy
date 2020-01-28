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
        public Hand splitHand = new Hand();

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

        public void AddBet(int bet)
        {
            Stake += bet;
            Chips -= bet;
        }

        //Make decision based of current game state and strategy
        public abstract PlayerState React(Card DealersUpCard);
        public abstract int CalculateBet(int minBet, int maxBet);

        public override void WriteCurrentState() { Console.WriteLine($"PLAYER REACTS: {CurrentState.ToString()}"); }
    }

}
