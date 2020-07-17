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
            Console.WriteLine($"Chips: {Chips.ToString()}");
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

        public override PlayerState React(Card dealersUpCard, ref PlayerState stateToChange, Hand hand, int count)
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
                            stateToChange = PlayerState.HIT;
                            return PlayerState.HIT;
                        case "STAND":
                            stateToChange = PlayerState.STAND;
                            return PlayerState.STAND;
                        case "SPLIT":
                            if (hand.cards.Count == 2)
                            {
                                if (hand.cards.First().Value == hand.cards.Last().Value)
                                {
                                    stateToChange = PlayerState.SPLIT;
                                    return PlayerState.SPLIT;
                                }
                            }
                            //CurrentState = PlayerState.SPLIT;
                            //return PlayerState.SPLIT;
                            action = null;
                            Console.WriteLine("Invalid Action: Can't split with two different values");
                            break;
                        case "DOUBLE_DOWN":
                            stateToChange = PlayerState.DOUBLE_DOWN;
                            return PlayerState.DOUBLE_DOWN;
                        default:
                            Console.WriteLine("Invalid Action: Possible actions are HIT, STAND, SPLIT and DOUBLE_DOWN");
                            action = null;
                            break;
                    }
                }
                stateToChange = PlayerState.BUST;
                return PlayerState.BUST;
            }
        }

    }
}
