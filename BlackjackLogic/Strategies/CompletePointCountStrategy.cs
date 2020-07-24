using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic.Strategies
{
    public class CompletePointCountStrategy : Player
    {
        private float HiLowIndex = 0;
        public override string StrategyName { get { return "CompletePointCount"; } }
        public override string CountType { get { return "completepointcount"; } }

        //Split if ratio < number in matrix
        //* indicated number should be read in reverse (ratio > num in matrix)
        readonly int[,] PairSplitting = new int[10, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {-9,-15,-22,-30,-100,-100,100,100,100,100 },//(2,2) //Last 2 numbers are asterisk*
            {-21,-34,-100,-100,-100,-100,6,100,100,100 },//(3,3) //Every number bar 50 & 0 is asterisk*
            {100,18,8,0,5,100,100,100,100,100 },//(4,4) //Always double down 4,4 against 6
            {100,100,100,100,100,100,100,100,100,100 },//(5,5)
            {0,-3,-8,-13,-16,-8,100,100,100,100},//(6,6)
            {-22,-29,-35,-100,-100,-100,-100,100,100,100 },//(7,7)
            {-100,-100,-100,-100,-100,-100,-100,-100,24,-18},//(8,8)*
            {-3,-8,-10,-15,-14,8,-16,-22,100,10},//(9,9)
            {25,17,10,6,7,19,100,100,100,100},//(10,10)
            {-100,-100,-100,-100,-100,-33,-24,-22,-20,-17 },//(A,A)
        };
        //DOUBLE CHECK ALL VALUES
        //100 = NOT SHADED = DON'T DOUBLE DOWN
        readonly int[,] HardDoubleDown = new int[7, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {100,100,100,20,26,100,100,100,100,100 },//5
            {100,100,27,18,24,100,100,100,100,100 },//6
            {100,45,21,14,17,100,100,100,100,100 },//7
            {100,22,11,5,5,22,100,100,100,100 },//8
            {3,0,-5,-10,-12,4,14,100,100,100 },//9
            {-15,-17,-21,-24,-26,-17,-9,-3,7,6},//10
            {-23,-26,-29,-33,-35,-26,-16,-10,-9,-3 } //11
        };

        readonly int[,] SoftDoubleDown = new int[8, 5]
        {
            //2  3     4     5     6   
            {100,10,2,-19,-13},//(A,2)
            {100,11,-3,-13,-19},//(A,3)
            {100,19,-7,-16,-23},//(A,4)
            {100,21,-6,-16,-32},//(A,5)
            {100,-6,-14,-28,-30},//(A,6)
            {100,-2,-15,-18,-23},//(A,7)
            {100,9,5,1,0},//(A,8)
            {100,20,12,8,8}//(A,9)

        };

        //Always draw on less than or equal to 11
        //Stand on 18 or more
        //-100 = SHADED = STAND
        //100 = NOT SHADED = HIT
        readonly int[,] HardHitOrStand = new int[6, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {14,6,2,-1,0,100,100,100,100,100},//12
            {1,-2,-5,-9,-8,50,100,100,100,100},//13
            {-5,-8,-13,-17,-17,-17,20,38,100,100},//14
            {-12,-17,-21,-26,-28,13,15,12,8,16},//15
            {-21,-25,-30,-34,-35,10,11,6,0,14},//16
            { -100,-100,-100,-100,-100,-100,-100,-100,-100,-15 },//17
        };

        //-100 = SHADED = STAND
        //100 = NOT SHADED = HIT
        readonly int[,] SoftHitOrStand = new int[2, 10]
        {
           //2 3 4 5 6 7 8 9 10 A
           {-100,-100,-100,-100,-100,-100,-100,100,12,-6},//18 soft standing number for 18 is any ratio < 2.2
           {100,100,100,100,100,29,100,100,100,100},//19 soft standing number for 19 is any ratio > 2.2

        };

        //Consider returning index/2*minBet
        public override int CalculateBet(int minBet, int maxBet, List<int> count)
        {
            UpdateIndex();
            if (HiLowIndex < 2)
            {
                return minBet;
            }
            else if (HiLowIndex >= 2 && HiLowIndex < 4)
            {
                return minBet * 2;
            }
            else if (HiLowIndex >= 4 && HiLowIndex < 6)
            {
                return minBet * 3;
            }
            else if (HiLowIndex >= 6 && HiLowIndex < 8)
            {
                return minBet * 4;
            }
            else if (HiLowIndex >= 8)
            {
                return minBet * 5;
            }
            return minBet;
        }
        private void UpdateIndex()
        {
            HiLowIndex = ((((float)Count[0]) / ((float)Count[1]))*100);
            Console.WriteLine($"HiLowIndex:\t{HiLowIndex}");
        }

        public override List<int> UpdateCount(Deck deck, List<Card> burntCards, Card dealersUpCard)
        {
            Count[0] = 0; Count[1] = deck.Cards.Count;
            if (hand.cards.Count > 0)
            {
                foreach (var c in hand.cards)
                {
                    if (c.Face == Face.Two || c.Face == Face.Three || c.Face == Face.Four || c.Face == Face.Five || c.Face == Face.Six)
                    {
                        Count[0]++;
                    }
                    if (c.Face == Face.Ace || c.Value == 10)
                    {
                        Count[0]--;
                    }
                }
            }
            if (splitHand != null)
            {
                if (splitHand.cards.Count > 0)
                {
                    foreach (var c in splitHand.cards)
                    {
                        if (c.Face == Face.Two || c.Face == Face.Three || c.Face == Face.Four || c.Face == Face.Five || c.Face == Face.Six)
                        {
                            Count[0]++;
                        }
                        if (c.Face == Face.Ace || c.Value == 10)
                        {
                            Count[0]--;
                        }
                    }
                }

            }
            if (dealersUpCard != null)
            {
                if (dealersUpCard.Face == Face.Two || dealersUpCard.Face == Face.Three || dealersUpCard.Face == Face.Four || dealersUpCard.Face == Face.Five || dealersUpCard.Face == Face.Six)
                {
                    Count[0]++;
                }
                if (dealersUpCard.Face == Face.Ace || dealersUpCard.Value == 10)
                {
                    Count[0]--;
                }

            }
            foreach (var c in burntCards)
            {
                if (c.Face == Face.Two || c.Face == Face.Three || c.Face == Face.Four || c.Face == Face.Five || c.Face == Face.Six)
                {
                    Count[0]++;
                }
                if (c.Face == Face.Ace || c.Value == 10)
                {
                    Count[0]--;
                }
            }
            UpdateIndex();
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
                //Split eights against 10 if hi low index < 24
                if (hand.cards.First().Face == Face.Eight && dealersUpCard.Value == 10)
                {
                    if (HiLowIndex < 24)
                    {
                        stateToChange = PlayerState.SPLIT;
                        return PlayerState.SPLIT;
                    }
                }
                if (hand.cards.First().Face == Face.Three && dealersUpCard.Value == 8)
                {
                    if (HiLowIndex < -2 || HiLowIndex > 6)
                    {
                        stateToChange = PlayerState.SPLIT;
                        return PlayerState.SPLIT;
                    }
                }
                if (HiLowIndex > PairSplitting[hand.cards.First().Value - 2, dealersUpCard.Value - 2])
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
                    if (cardNotAceInHand.Face == Face.Six && dealersUpCard.Face == Face.Two)
                    {
                        if (HiLowIndex >= 1 && HiLowIndex <= 10)
                        {
                            stateToChange = PlayerState.DOUBLE_DOWN;
                            return PlayerState.DOUBLE_DOWN;
                        }
                    }
                    if (dealersUpCard.Value < 7 && cardNotAceInHand.Value != 10)
                    {
                        if (HiLowIndex > SoftDoubleDown[cardNotAceInHand.Value - 2, dealersUpCard.Value - 2])
                        {
                            stateToChange = PlayerState.DOUBLE_DOWN;
                            return PlayerState.DOUBLE_DOWN;
                        }
                    }
                }
                //HARD HAND
                else
                {
                    if (hand.handValues.First() >= 5 && hand.handValues.First() <= 11)
                    {
                        if ((HiLowIndex > HardDoubleDown[hand.handValues.First() - 5, dealersUpCard.Value - 2]))
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

                if (HiLowIndex > SoftHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2])
                {
                    stateToChange = PlayerState.STAND;
                    return PlayerState.STAND;
                }

                if (HiLowIndex <= SoftHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2])
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
            //Always stand on 18 or more, unless you have 18 vs Ace up and ratio < 3.1
            else if (hand.handValues.First() >= 18)
            {
                stateToChange = PlayerState.STAND;
                return PlayerState.STAND;
            }


            if (HiLowIndex > HardHitOrStand[hand.handValues.Max() - 12, dealersUpCard.Value - 2])
            {
                stateToChange = PlayerState.STAND;
                return PlayerState.STAND;
            }
            else if (HiLowIndex <= HardHitOrStand[hand.handValues.Max() - 12, dealersUpCard.Value - 2])
            {
                stateToChange = PlayerState.HIT;
                return PlayerState.HIT;
            }

            stateToChange = PlayerState.STAND;
            return PlayerState.STAND;
        }
    }
}
