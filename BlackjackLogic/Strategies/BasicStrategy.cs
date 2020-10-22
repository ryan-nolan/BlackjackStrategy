using System.Collections.Generic;
using System.Linq;
using BlackjackLogic.Game;

namespace BlackjackLogic.Strategies
{
    public class BasicStrategy : Player
    {
        public override string StrategyName => "BasicStrategy";
        public override string CountType => "basic";

        //split on true, ignore on false
        private readonly bool[,] _pairSplitting = new bool[10, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {true,true,true,true,true,true,false,false,false,false },//(2,2)
            {true,true,true,true,true,true,false,false,false,false },//(3,3)
            {false,false,false,false,true,false,false,false,false,false },//(4,4)
            {false,false,false,false,false,false,false,false,false,false },//(5,5)
            {true,true,true,true,true,true,false,false,false,false},//(6,6)
            {true,true,true,true,true,true,true,false,false,false },//(7,7)
            {true,true,true,true,true,true,true,true,true,true },//(8,8)
            {true,true,true,true,true,false,true,true,false,false},//(9,9)
            {false,false,false,false,false,false,false,false,false,false},//(10,10)
            {true,true,true,true,true,true,true,true,true,true },//(A,A)
        };
        //Double down on true, ignore on false
        private readonly bool[,] _hardDoubleDown = new bool[4, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {false,false,false,false,false,false,false,false,false,false},//8
            {true,true,true,true,true,false,false,false,false,false },//9
            {true,true,true,true,true,true,true,true,false,false},//10
            {true,true,true,true,true,true,true,true,true,true },//11
        };
        //Double down on true, ignore on false
        private readonly bool[,] _softDoubleDown = new bool[7, 5]
        {
            //2    3     4     5     6    
            {false,false,false,true,true},//(A,A)
            {false,false,true,true,true},//(A,2)
            {false,false,true,true,true},//(A,3)
            {false,false,true,true,true},//(A,4)
            {false,false,true,true,true},//(A,5)
            {true,true,true,true,true},//(A,6)
            {false,true,true,true,true}//(A,7)
        };

        //Hard Stand on true, hit on false
        //Always draw on less than or equal to 11
        //Account for optimisations
        private readonly bool[,] _hardHitOrStand = new bool[6, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {false,false,true,true,true,false,false,false,false,false },//12
            {true,true,true,true,true,false,false,false,false,false},//13
            {true,true,true,true,true,false,false,false,false,false},//14
            {true,true,true,true,true,false,false,false,false,false},//15
            {true,true,true,true,true,false,false,false,false,false},//16
            {true,true,true,true,true,true,true,true,true,true},//17

        };
        //Soft Stand on true, hit on false
        //Hit on anything >= 17, stand on anything 19 or more
        private readonly bool[,] _softHitOrStand = new bool[2, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {true,true,true,true,true,true,true,false,false,true},//18
            {true,true,true,true,true,true,true,true,true,true},//19

        };
        /// <summary>
        /// Always returns min bet
        /// </summary>
        /// <param name="minBet"></param>
        /// <param name="maxBet"></param>
        /// <returns></returns>
        public override int CalculateBet(int minBet, int maxBet)
        {
            return minBet;
        }
        /// <summary>
        /// Basic strategy contains no count
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
        /// Reacts to game state based on decision matrices
        /// </summary>
        /// <param name="dealersUpCard"></param>
        /// <param name="stateToChange"></param>
        /// <param name="hand"></param>
        /// <param name="count"></param>
        /// <returns>PlayerState</returns>
        public override PlayerState React(Card dealersUpCard, ref PlayerState stateToChange, Hand hand, List<int> count)
        {
            if (hand.handValues.First() > 21)
            {
                stateToChange = PlayerState.Bust;
                return PlayerState.Bust;
            }

            //Do you have pair
            //yes, split?
            if (((hand.cards.First().Face == hand.cards.Last().Face) && splitHand == null) && hand.cards.Count == 2)
            {
                if (_pairSplitting[hand.cards.First().Value - 2, dealersUpCard.Value - 2])
                {
                    stateToChange = PlayerState.Split;
                    return PlayerState.Split;
                }
            }

            //double down
            //yes
            //stand
            if (hand.cards.Count == 2)
            {
                //SOFT HAND
                if (hand.handValues.Count > 1)
                {
                    //Always split aces
                    if (hand.cards.First().Face == Face.Ace && hand.cards.Last().Face == Face.Ace)
                    {
                        stateToChange = PlayerState.DoubleDown;
                        return PlayerState.DoubleDown;
                    }
                    var cardNotAceInHand = hand.cards.Find(x => x.Face != Face.Ace);
                    if (cardNotAceInHand.Value <= 7 && dealersUpCard.Value <= 6)
                    {
                        if (_softDoubleDown[cardNotAceInHand.Value - 2, dealersUpCard.Value - 2])
                        {
                            stateToChange = PlayerState.DoubleDown;
                            return PlayerState.DoubleDown;
                        }
                    }
                }
                //HARD HAND
                else
                {
                    if (hand.handValues.First() >= 8 && hand.handValues.First() <= 11)
                    {
                        //Check the (6,2) exception
                        if ((hand.handValues.First() == 8) && (dealersUpCard.Value == 5 || dealersUpCard.Value == 6))
                        {
                            if (!(hand.cards.Any(x => x.Value == 6) && hand.cards.Any(x => x.Value == 2)))
                            {
                                stateToChange = PlayerState.DoubleDown;
                                return PlayerState.DoubleDown;
                            }
                        }
                        if (_hardDoubleDown[hand.handValues.First() - 8, dealersUpCard.Value - 2])
                        {
                            stateToChange = PlayerState.DoubleDown;
                            return PlayerState.DoubleDown;
                        }
                    }
                }
            }


            //no 
            //draw? check table

            //Check if hand is hard or soft
            //Soft
            if (hand.handValues.Count > 1)
            {
                if (hand.handValues.Max() <= 17)
                {
                    stateToChange = PlayerState.Hit;
                    return PlayerState.Hit;
                }
                if (hand.handValues.Max() > 19)
                {
                    stateToChange = PlayerState.Stand;
                    return PlayerState.Stand;
                }
                if (!_softHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2])
                {
                    stateToChange = PlayerState.Hit;
                    return PlayerState.Hit;
                }
                else if (_softHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2])
                {
                    stateToChange = PlayerState.Stand;
                    return PlayerState.Stand;
                }
            }

            //Hard
            //Always hit on 11 or less
            if (hand.handValues.First() <= 11)
            {
                stateToChange = PlayerState.Hit;
                return PlayerState.Hit;
            }
            //Always stand on 18 or more
            else if (hand.handValues.First() >= 18)
            {
                stateToChange = PlayerState.Stand;
                return PlayerState.Stand;
            }

            //HARD 16 against a 10 exception
            if (hand.handValues.First() == 16 && hand.cards.Count > 2 && dealersUpCard.Value == 10)
            {
                stateToChange = PlayerState.Stand;
                return PlayerState.Stand;
            }
            if (hand.handValues.First() == 16 && hand.cards.Count == 2 && dealersUpCard.Value == 10)
            {
                stateToChange = PlayerState.Hit;
                return PlayerState.Hit;
            }
            //STAND HOLDING 7,7 AGAINST A 10
            if (hand.cards.Count == 2 && ((hand.cards.First().Face == Face.Seven) && (hand.cards.Last().Face == Face.Seven)) && dealersUpCard.Value == 10)
            {
                stateToChange = PlayerState.Stand;
                return PlayerState.Stand;
            }
            if (_hardHitOrStand[hand.handValues.Max() - 12, dealersUpCard.Value - 2])
            {
                stateToChange = PlayerState.Stand;
                return PlayerState.Stand;
            }
            else if (!(_hardHitOrStand[hand.handValues.Max() - 12, dealersUpCard.Value - 2]))
            {
                stateToChange = PlayerState.Hit;
                return PlayerState.Hit;
            }

            stateToChange = PlayerState.Stand;
            return PlayerState.Stand;
        }
    }
}
