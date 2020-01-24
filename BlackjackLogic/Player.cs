using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic
{
    public class Player
    {
        public Hand Hand;
        public int Chips;
        public int Stake;

        public void AddBet(int bet)
        {
            Stake += bet;
            Chips -= bet;
        }
    }
}
