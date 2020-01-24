using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic
{
    public class Player : Actor
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
    }
}
