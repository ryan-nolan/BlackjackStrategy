using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic.Strategies
{
    public class HumanStrategy : Player
    {
        public override int CalculateBet(int minBet, int maxBet)
        {

            int stake = 0;
            Console.WriteLine(Chips.ToString());
            //Console.Write("Enter Amount you want to bet: ");
            while (stake == 0)
            {
                Console.Write("Enter Amount you want to bet: ");
                try
                {
                    stake = int.Parse(Console.ReadLine());
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid Stake");
                    //throw;
                }
            }

            return stake;
        }

        public override PlayerState React(Card DealersUpCard)
        {
            Console.Write("Player's Cards: ");
            hand.WriteHandAndHandValue();
            //Console.Write("Enter an action: ");
            if (hand.handValues.First() > 21)
            {
                CurrentState = PlayerState.BUST;
                return PlayerState.BUST;
            }
            else
            {
                
                string action = null;
                while (action == null)
                {
                    Console.Write("Enter an action: ");
                    action = Console.ReadLine().ToUpper();
                    switch (action)
                    {
                        case "HIT":
                            CurrentState = PlayerState.HIT;
                            return PlayerState.HIT;
                        case "STAND":
                            CurrentState = PlayerState.STAND;
                            return PlayerState.STAND;
                        case "SPLIT":
                            CurrentState = PlayerState.SPLIT;
                            return PlayerState.SPLIT;
                        case "DOUBLE_DOWN":
                            CurrentState = PlayerState.DOUBLE_DOWN;
                            return PlayerState.DOUBLE_DOWN;
                        default:
                            Console.WriteLine("Invalid Action: Possible actions are HIT, STAND, SPLIT and DOUBLE_DOWN");
                            action = null;
                            break;
                    }
                }
                CurrentState = PlayerState.BUST;
                return PlayerState.BUST;
            }
        }
    }
}
