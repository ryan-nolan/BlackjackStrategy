using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic.Strategies
{
    public class TenCountStrategy : Player
    {
        public float othersOverTenRatio;
        public override string StrategyName { get { return "TenCount"; } }
        public override string CountType { get { return "tencount"; } }

        //Split if ratio < number in matrix
        //* indicated number should be read in reverse (ratio > num in matrix)
        readonly float[,] PairSplitting = new float[10, 10]
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
        readonly bool[,] HardDoubleDown = new bool[4, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {false,false,false,false,false,false,false,false,false,false},//8
            {true,true,true,true,true,false,false,false,false,false },//9
            {true,true,true,true,true,true,true,true,false,false},//10
            {true,true,true,true,true,true,true,true,true,true },//11
        };

        readonly float[,] SoftDoubleDown = new float[8, 6]
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
        readonly bool[,] HardHitOrStand = new bool[6, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {false,false,true,true,true,false,false,false,false,false },//12
            {true,true,true,true,true,false,false,false,false,false},//13
            {true,true,true,true,true,false,false,false,false,false},//14
            {true,true,true,true,true,false,false,false,false,false},//15
            {true,true,true,true,true,false,false,false,false,false},//16
            {true,true,true,true,true,true,true,true,true,true},//17

        };
        ////50 = SHADED = STAND
        ////0 = NOT SHADED = HIT
        //readonly float[,] SoftHitOrStand = new float[2, 10]
        //{
        //   //2 3 4 5 6 7 8 9 10 A
        //    {50,50,50,50,50,50,50,0,0,2.2f},//18 soft standing number for 18 is any ratio < 2.2
        //    {50,50,50,50,50,50,50,50,50,2.2f},//19 soft standing number for 19 is any ratio > 2.2

        //};
        //Soft Stand on true, hit on false
        //Hit on anything >= 17, stand on anything 19 or more
        readonly bool[,] SoftHitOrStand = new bool[2, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {true,true,true,true,true,true,true,false,false,true},//18
            {true,true,true,true,true,true,true,true,true,true},//19

        };


        public override int CalculateBet(int minBet, int maxBet, List<int> count)
        {
            UpdateOthersOverTenRatio();
            if (othersOverTenRatio>=2)
            {
                return minBet;
            }
            else if (othersOverTenRatio < 2 && othersOverTenRatio >= 1.75)
            {
                return minBet * 2;
            }
            else if (othersOverTenRatio < 1.75 && othersOverTenRatio > 1.65)
            {
                return minBet * 4;
            }
            else if (othersOverTenRatio <= 1.65)
            {
                return minBet * 5;
            }
            return minBet;
        }
        public void UpdateOthersOverTenRatio()
        {
            othersOverTenRatio = ((float)Count[0]) / ((float)Count[1]);
        }

        public override List<int> UpdateCount(Deck deck, List<Card> burntCards, Card dealersUpCard)
        {
            Count[0] = 0; Count[1] = 0;
            foreach (var c in deck.Cards)
            {
                if (c.Value == 10)
                {
                    Count[1]++;
                }
                else
                {
                    Count[0]++;
                }
            }
            UpdateOthersOverTenRatio();
            Console.WriteLine($"The Others / Ten ratio is now:\t{othersOverTenRatio}");
            return Count;
        }

        public override PlayerState React(Card dealersUpCard, ref PlayerState stateToChange, Hand hand, List<int> count)
        {
            if (hand.handValues.First() > 21)
            {
                stateToChange = PlayerState.BUST;
                return PlayerState.BUST;
            }
            //Do you have pair
            //yes, split?
            if (((hand.cards.First().Face == hand.cards.Last().Face) && splitHand == null) && hand.cards.Count == 2)
            {
                if ((hand.cards.First().Face == Face.Eight && dealersUpCard.Value == 10))
                {
                    if (othersOverTenRatio >= PairSplitting[hand.cards.First().Value - 2, dealersUpCard.Value - 2])
                    {
                        stateToChange = PlayerState.SPLIT;
                        return PlayerState.SPLIT;
                    }
                }
                if ((hand.cards.First().Face == Face.Three && (dealersUpCard.Value == 7 || dealersUpCard.Value == 8 )))
                {
                    if (othersOverTenRatio >= PairSplitting[hand.cards.First().Value - 2, dealersUpCard.Value - 2])
                    {
                        stateToChange = PlayerState.SPLIT;
                        return PlayerState.SPLIT;
                    }
                }
                if ((hand.cards.First().Face == Face.Two && (dealersUpCard.Value == 7 || dealersUpCard.Value == 8 || dealersUpCard.Value == 9 || dealersUpCard.Value == 10)))
                {
                    if (othersOverTenRatio >= PairSplitting[hand.cards.First().Value - 2, dealersUpCard.Value - 2])
                    {
                        stateToChange = PlayerState.SPLIT;
                        return PlayerState.SPLIT;
                    }
                }
                else if(othersOverTenRatio <= PairSplitting[hand.cards.First().Value - 2, dealersUpCard.Value - 2])
                {
                    stateToChange = PlayerState.SPLIT;
                    return PlayerState.SPLIT;
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
                        stateToChange = PlayerState.DOUBLE_DOWN;
                        return PlayerState.DOUBLE_DOWN;
                    }
                    var cardNotAceInHand = hand.cards.Find(x => x.Face != Face.Ace);
                    if (dealersUpCard.Value < 8 && cardNotAceInHand.Value != 10)
                    {
                        if (othersOverTenRatio <= SoftDoubleDown[cardNotAceInHand.Value - 2, dealersUpCard.Value - 2])
                        {
                            stateToChange = PlayerState.DOUBLE_DOWN;
                            return PlayerState.DOUBLE_DOWN;
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
                                stateToChange = PlayerState.DOUBLE_DOWN;
                                return PlayerState.DOUBLE_DOWN;
                            }
                        }
                        if (HardDoubleDown[hand.handValues.First() - 8, dealersUpCard.Value - 2])
                        {
                            stateToChange = PlayerState.DOUBLE_DOWN;
                            return PlayerState.DOUBLE_DOWN;
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
                    stateToChange = PlayerState.HIT;
                    return PlayerState.HIT;
                }
                if (hand.handValues.Max() > 19)
                {
                    stateToChange = PlayerState.STAND;
                    return PlayerState.STAND;
                }
                if (SoftHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2])
                {
                    stateToChange = PlayerState.HIT;
                    return PlayerState.HIT;
                }
                else if (!(SoftHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2]))
                {
                    stateToChange = PlayerState.STAND;
                    return PlayerState.STAND;
                }
            }

            //Hard
            //Always hit on 11 or less
            if (hand.handValues.First() <= 11)
            {
                stateToChange = PlayerState.HIT;
                return PlayerState.HIT;
            }
            //Always stand on 18 or more
            else if (hand.handValues.First() >= 18)
            {
                stateToChange = PlayerState.STAND;
                return PlayerState.STAND;
            }

            //HARD 16 against a 10 exception
            if (hand.handValues.First() == 16 && hand.cards.Count > 2 && dealersUpCard.Value == 10)
            {
                stateToChange = PlayerState.STAND;
                return PlayerState.STAND;
            }
            if (hand.handValues.First() == 16 && hand.cards.Count == 2 && dealersUpCard.Value == 10)
            {
                stateToChange = PlayerState.HIT;
                return PlayerState.HIT;
            }

            if (HardHitOrStand[hand.handValues.Max() - 12, dealersUpCard.Value - 2])
            {
                stateToChange = PlayerState.STAND;
                return PlayerState.STAND;
            }
            else if (!(HardHitOrStand[hand.handValues.Max() - 12, dealersUpCard.Value - 2]))
            {
                stateToChange = PlayerState.HIT;
                return PlayerState.HIT;
            }

            stateToChange = PlayerState.STAND;
            return PlayerState.STAND;
        }
    }
}
