using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic.Strategies
{
    public class HumanStrategy : Player
    {
        public override int CalculateBet()
        {
            int stake = 0;

            Console.WriteLine("Enter Amount you want to bet");
            while (stake == 0)
            {
                try
                {
                    stake = int.Parse(Console.ReadLine());
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid Stake");
                    throw;
                }
            }

            return stake;
        }

        public override PlayerState React(Card DealersUpCard)
        {
            hand.WriteHandAndHandValue();
            Console.Write("Enter an action: ");
            if (hand.handValues.First() > 21)
            {
                return PlayerState.BUST;
            }
            else
            {
                string action = null;
                while (action != null)
                {
                    action = Console.ReadLine().ToUpper();
                    switch (action)
                    {
                        case "HIT":
                            return PlayerState.HIT;
                        case "STAND":
                            return PlayerState.STAND;
                        case "SPLIT":
                            return PlayerState.SPLIT;
                        case "DOUBLE_DOWN":
                            return PlayerState.DOUBLE_DOWN;
                        default:
                            action = null;
                            break;
                    }
                }
                return PlayerState.BUST;
            }
        }
    }
}
