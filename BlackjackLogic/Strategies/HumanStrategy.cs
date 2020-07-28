using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackjackLogic.Strategies
{
    public class HumanStrategy : Player
    {
        /// <summary>
        /// Takes an integer in from console
        /// </summary>
        /// <param name="minBet"></param>
        /// <param name="maxBet"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Player cannot hold a count
        /// All player counting would be done by user
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="burntCards"></param>
        /// <param name="dealersUpCard"></param>
        /// <returns></returns>
        public override List<int> UpdateCount(Deck deck, List<Card> burntCards, Card dealersUpCard)
        {
            return Count;
        }
        /// <summary>
        /// Player reacts to game state using console input
        /// </summary>
        /// <param name="dealersUpCard"></param>
        /// <param name="stateToChange"></param>
        /// <param name="hand"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override PlayerState React(Card dealersUpCard, ref PlayerState stateToChange, Hand hand, List<int> count)
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
