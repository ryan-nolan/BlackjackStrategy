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

        readonly bool[,] PairSplitting = new bool[10, 10]
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
        readonly bool[,] HardDoubleDown = new bool[4, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {false,false,false,false,false,false,false,false,false,false},//8
            {true,true,true,true,true,false,false,false,false,false },//9
            {true,true,true,true,true,true,true,true,false,false},//10
            {true,true,true,true,true,true,true,true,true,true },//11
        };

        readonly bool[,] SoftDoubleDown = new bool[7, 5]
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
                if (PairSplitting[hand.cards.First().Value - 2, dealersUpCard.Value - 2])
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
                //SOFT HAND
                if (hand.handValues.Count > 1)
                {
                    //Always split aces
                    if (hand.cards.First().Face == Face.Ace && hand.cards.Last().Face == Face.Ace)
                    {
                        stateToChange = PlayerState.DOUBLE_DOWN;
                        return PlayerState.DOUBLE_DOWN;
                    }
                    var cardNotAceInHand = hand.cards.Find(x => x.Face != Face.Ace);
                    if (cardNotAceInHand.Value <= 7 && dealersUpCard.Value <= 6)
                    {
                        if (SoftDoubleDown[cardNotAceInHand.Value - 2, dealersUpCard.Value - 2])
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
