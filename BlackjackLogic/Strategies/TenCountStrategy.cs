using System;
using System.Collections.Generic;
using System.Linq;
using BlackjackLogic.Game;

namespace BlackjackLogic.Strategies
{
    public class TenCountStrategy : Player
    {
        public float othersOverTenRatio;
        public override string StrategyName { get { return "TenCount"; } }
        public override string CountType { get { return "tencount"; } }

        //Split if ratio < number in matrix
        //* indicated number should be read in reverse (ratio > num in matrix)
        private readonly float[,] _pairSplitting = new float[10, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {3.1f,3.8f,50,50,50,1.1f,3.8f,0,0,0 },//(2,2) //Last 2 numbers are asterisk*
            {50,50,50,50,50,1.1f,2.4f,4.2f,5.3f,0 },//(3,3) //Every number bar 50 & 0 is asterisk*
            {1.3f,1.6f,1.9f,2.4f,2.1f,0,0,0,0,0 },//(4,4) //Always double down 4,4 against 6
            {0,0,0,0,0,0,0,0,0,0 },//(5,5)
            {2.4f,2.6f,3,3.6f,4.1f,3.4f,0,0,0,0},//(6,6)
            {50,50,50,50,50,50,50,0,0,1.4f },//(7,7)
            {50,50,50,50,50,50,50,50,1.6f,4.8f },//(8,8) //1.6*
            {2.4f,2.8f,3.1f,3.7f,3.2f,1.6f,50,4.2f,0,1.5f},//(9,9)
            {1.4f,1.5f,1.7f,1.9f,1.8f,0,0,0,0,0},//(10,10)
            {4.0f,4.1f,4.5f,4.9f,5.0f,3.8f,3.3f,3.1f,3.2f,2.6f },//(A,A)
        };
        //CHECKED
        private readonly float[,] _hardDoubleDown = new float[7, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {0,0,1.0f,1.1f,1.1f,0,0,0,0,0 },//3,2
            {0,0,1.0f,1.2f,1.3f,0,0,0,0,0 },//4,2
            {0.9f,1.1f,1.2f,1.4f,1.4f,0,0,0,0,0 },//7
            {1.3f,1.5f,1.7f,2.0f,2.1f,1.0f,0,0,0,0 },//8
            {2.2f,2.4f,2.8f,3.3f,3.4f,2.0f,1.6f,0,0,0.9f },//9
            {3.7f,4.2f,4.8f,5.6f,5.7f,3.8f,3.0f,2.5f,1.9f,1.8f},//10
            {3.9f,4.2f,4.8f,5.5f,5.5f,3.7f,3.0f,2.6f,2.8f,2.2f } //11
        };
        //CHECKED
        private readonly float[,] _softDoubleDown = new float[8, 6]
        {
            //2    3     4     5     6    
            {1.5f,1.7f,2.1f,2.6f,2.7f,0},//(A,2)
            {1.5f,1.8f,2.3f,2.9f,3.0f,0},//(A,3)
            {1.6f,1.9f,2.4f,3.0f,3.2f,0},//(A,4)
            {1.6f,1.9f,2.5f,3.1f,4.0f,0},//(A,5)
            {2.1f,2.5f,3.2f,4.8f,4.8f,1.1f},//(A,6)
            {2.0f,2.2f,3.3f,3.8f,3.5f,0},//(A,7)
            {1.4f,1.7f,1.8f,2,2,0},//(A,8)
            {1.3f,1.3f,1.5f,1.6f,1.6f,0}//(A,9)

        };

        //Hard Stand on true, hit on false
        //Always draw on less than or equal to 11
        //Account for optimisations
        private readonly float[,] _hardHitOrStand = new float[6, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {2.0f,2.1f,2.2f,2.4f,2.3f,0,0,0,1.1f,1.0f},//12
            {2.3f,2.5f,2.6f,3.0f,2.7f,0,0,0,1.3f,1.1f},//13
            {2.7f,2.9f,3.3f,3.7f,3.4f,0,0,1.1f,1.6f,1.2f},//14
            {3.2f,3.6f,4.1f,4.8f,4.3f,0,0,1.4f,1.9f,1.3f},//15
            {3.9f,4.5f,5.3f,6.5f,4.6f,0,1.2f,1.7f,2.2f,1.4f},//16
            {50,50,50,50,50,50,50,50,50,3.1f},//17
        };                                    //18 against an Ace ratio > 3.1 = stand

        //50 = SHADED = STAND
        //0 = NOT SHADED = HIT
        private readonly float[,] _softHitOrStand = new float[2, 10]
        {
           //2 3 4 5 6 7 8 9 10 A
            {50,50,50,50,50,50,50,0,0,2.2f},//18 soft standing number for 18 is any ratio < 2.2
            {0,0,0,0,0,0,0,0,0,2.2f},//19 soft standing number for 19 is any ratio > 2.2

        };
        /// <summary>
        /// Getter for others over ten ratio
        /// </summary>
        /// <returns></returns>
        public float GetOthersOverTenRatio()
        {
            return othersOverTenRatio;
        }
        /// <summary>
        /// Returns bet size based on ten count
        /// </summary>
        /// <param name="minBet"></param>
        /// <param name="maxBet"></param>
        /// <returns></returns>
        public override int CalculateBet(int minBet, int maxBet)
        {
            UpdateOthersOverTenRatio();
            if (othersOverTenRatio >= 2)
            {
                return minBet;
            }
            else if (othersOverTenRatio < 2 && othersOverTenRatio >= 1.75)
            {
                return minBet * 5;
            }
            else if (othersOverTenRatio < 1.75 && othersOverTenRatio > 1.65)
            {
                return minBet * 8;
            }
            else if (othersOverTenRatio <= 1.65)
            {
                return minBet * 10;
            }
            return minBet;
        }
        /// <summary>
        /// Calculates others over ten ratio used to make decisions
        /// Should be called before every decision for running count
        /// </summary>
        public void UpdateOthersOverTenRatio()
        {
            othersOverTenRatio = ((float)Count[0]) / ((float)Count[1]); //Others / Tens
        }
        /// <summary>
        /// Count 1 = Tens left in deck
        /// Count 2 = Non Tens left in deck
        /// Updates both count values accordingly
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="burntCards"></param>
        /// <param name="dealersUpCard"></param>
        /// <returns></returns>
        public override List<int> UpdateCount(Deck deck, List<Card> burntCards, Card dealersUpCard)
        {
            Count[0] = 0; Count[1] = 0;
            foreach (var c in deck.Cards)
            {
                if (c.Value == 10)
                {
                    Count[1]++;//Tens 
                }
                else
                {
                    Count[0]++;//Others
                }
            }
            UpdateOthersOverTenRatio();
            Console.WriteLine($"The Others / Ten ratio is now:\t{othersOverTenRatio}");
            return Count;
        }
        /// <summary>
        /// Reacts to game state based on count and float decision matrices
        /// </summary>
        /// <param name="dealersUpCard"></param>
        /// <param name="stateToChange"></param>
        /// <param name="hand"></param>
        /// <param name="count"></param>
        /// <returns></returns>
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
                //4,4 against 6 double down
                if (hand.cards.First().Face == Face.Four && dealersUpCard.Face == Face.Six)
                {
                    stateToChange = PlayerState.DoubleDown;
                    return PlayerState.DoubleDown;
                }
                //8 against 10 greater than exception
                if ((hand.cards.First().Face == Face.Eight && dealersUpCard.Value == 10))
                {
                    if (othersOverTenRatio >= _pairSplitting[hand.cards.First().Value - 2, dealersUpCard.Value - 2])
                    {
                        stateToChange = PlayerState.Split;
                        return PlayerState.Split;
                    }
                }
                //3 against 7 or 8 greater than exception
                if ((hand.cards.First().Face == Face.Three && (dealersUpCard.Value == 7 || dealersUpCard.Value == 8)))
                {
                    if (othersOverTenRatio >= _pairSplitting[hand.cards.First().Value - 2, dealersUpCard.Value - 2])
                    {
                        stateToChange = PlayerState.Split;
                        return PlayerState.Split;
                    }
                }
                //2 against 7,8,9 or 10 greater than exception
                if ((hand.cards.First().Face == Face.Two && (dealersUpCard.Value == 7 || dealersUpCard.Value == 8 || dealersUpCard.Value == 9 || dealersUpCard.Value == 10)))
                {
                    if (othersOverTenRatio >= _pairSplitting[hand.cards.First().Value - 2, dealersUpCard.Value - 2])
                    {
                        stateToChange = PlayerState.Split;
                        return PlayerState.Split;
                    }
                }
                else if (othersOverTenRatio <= _pairSplitting[hand.cards.First().Value - 2, dealersUpCard.Value - 2])
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
                //SOFT HAND DOUBLE
                if (hand.handValues.Count > 1)
                {
                    //Always split aces
                    if (hand.cards.First().Face == Face.Ace && hand.cards.Last().Face == Face.Ace)
                    {
                        stateToChange = PlayerState.DoubleDown;
                        return PlayerState.DoubleDown;
                    }
                    var cardNotAceInHand = hand.cards.Find(x => x.Face != Face.Ace);
                    if (dealersUpCard.Value < 8 && cardNotAceInHand.Value != 10)
                    {
                        if (othersOverTenRatio <= _softDoubleDown[cardNotAceInHand.Value - 2, dealersUpCard.Value - 2])
                        {
                            stateToChange = PlayerState.DoubleDown;
                            return PlayerState.DoubleDown;
                        }
                    }
                }
                //HARD HAND
                else
                {
                    if (hand.handValues.First() >= 5 && hand.handValues.First() <= 11)
                    {
                        if ((hand.handValues.First() == 6 || hand.handValues.First() == 5) && (hand.cards.First().Value != hand.cards.Last().Value))
                        {
                            if (othersOverTenRatio <= _hardDoubleDown[hand.handValues.First() - 5, dealersUpCard.Value - 2])
                            {
                                stateToChange = PlayerState.DoubleDown;
                                return PlayerState.DoubleDown;
                            }
                        }
                        if ((othersOverTenRatio <= _hardDoubleDown[hand.handValues.First() - 5, dealersUpCard.Value - 2]) && hand.handValues.First() > 6)
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
                //Hit on less than 17
                if (hand.handValues.Max() <= 17)
                {
                    stateToChange = PlayerState.Hit;
                    return PlayerState.Hit;
                }
                //Stand on greater than 19
                if (hand.handValues.Max() > 19)
                {
                    stateToChange = PlayerState.Stand;
                    return PlayerState.Stand;
                }
                if (hand.handValues.Max() == 18)
                {
                    if (othersOverTenRatio <= _softHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2])
                    {
                        stateToChange = PlayerState.Stand;
                        return PlayerState.Stand;
                    }
                    else if (othersOverTenRatio > (_softHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2]))
                    {
                        stateToChange = PlayerState.Hit;
                        return PlayerState.Hit;
                    }
                }
                else if (hand.handValues.Max() == 19)
                {
                    if (dealersUpCard.Face == Face.Ace && othersOverTenRatio > 2.2)
                    {
                        stateToChange = PlayerState.Stand;
                        return PlayerState.Stand;
                    }
                    if (othersOverTenRatio > _softHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2])
                    {
                        stateToChange = PlayerState.Stand;
                        return PlayerState.Stand;
                    }
                    else if (othersOverTenRatio < (_softHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2]))
                    {
                        stateToChange = PlayerState.Hit;
                        return PlayerState.Hit;
                    }
                }
            }

            //Hard
            //Always hit on 11 or less
            if (hand.handValues.First() <= 11)
            {
                stateToChange = PlayerState.Hit;
                return PlayerState.Hit;
            }
            //Always stand on 18 or more, unless you have 18 vs Ace up and ratio < 3.1
            else if (hand.handValues.First() >= 18)
            {
                if (hand.handValues.First() == 18 && dealersUpCard.Face == Face.Ace)
                {
                    if (othersOverTenRatio < 3.1)
                    {
                        stateToChange = PlayerState.Hit;
                        return PlayerState.Hit;
                    }
                }
                stateToChange = PlayerState.Stand;
                return PlayerState.Stand;
            }


            if (othersOverTenRatio <= _hardHitOrStand[hand.handValues.Max() - 12, dealersUpCard.Value - 2])
            {
                stateToChange = PlayerState.Stand;
                return PlayerState.Stand;
            }
            else if (othersOverTenRatio > _hardHitOrStand[hand.handValues.Max() - 12, dealersUpCard.Value - 2])
            {
                stateToChange = PlayerState.Hit;
                return PlayerState.Hit;
            }

            stateToChange = PlayerState.Stand;
            return PlayerState.Stand;
        }
    }
}
