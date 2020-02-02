using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic.Strategies
{
    public class BasicStrategy : Player
    {
        bool[,] PairSplitting = new bool[10, 10]
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

        bool[,] HardDoubleDown = new bool[4, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {false,false,false,false,false,false,false,false,false,false},//8
            {true,true,true,true,true,false,false,false,false,false },//9
            {true,true,true,true,true,true,true,true,false,false},//10
            {true,true,true,true,true,true,true,true,true,true },//11
        };

        //Dictionary<Face, bool[]> PairSplit2 = new Dictionary<Face, bool[]>()
        //{
        //    {
        //        { Face.Two, new bool[] {true,true,true,true,true,true,false,false,false,false } },
        //        { Face.Three, new bool[] {true,true,true,true,true,true,false,false,false,false } }
        //    }
        //};

        //Dictionary<Face, Dictionary<Face, bool>> PairSplit = new Dictionary<Face, Dictionary<Face, bool>>()
        //{
        //    {
        //        Face.Two,
        //        new Dictionary<Face, bool>
        //        {
        //            { Face.Two, true },{ Face.Three, true },{ Face.Four, true },
        //            { Face.Five, true }, { Face.Six, true },{ Face.Seven, true },
        //            { Face.Eight, false},{ Face.Nine, false },{ Face.Ten, false },{ Face.Ace, false }
        //        }
        //    },
        //    {
        //        Face.Three,
        //        new Dictionary<Face, bool>
        //        {
        //            { Face.Two, true },{ Face.Three, true },{ Face.Four, true },
        //            { Face.Five, true }, { Face.Six, true },{ Face.Seven, true },
        //            { Face.Eight, false},{ Face.Nine, false },{ Face.Ten, false },{ Face.Ace, false }
        //        }
        //    },
        //    {
        //        Face.Four,
        //        new Dictionary<Face, bool>
        //        {
        //            { Face.Two, true },{ Face.Three, true },{ Face.Four, true },
        //            { Face.Five, true }, { Face.Six, true },{ Face.Seven, true },
        //            { Face.Eight, false},{ Face.Nine, false },{ Face.Ten, false },{ Face.Ace, false }
        //        }
        //    }

        //};

        public override int CalculateBet(int minBet, int maxBet)
        {
            return minBet;
        }

        public override PlayerState React(Card dealersUpCard, ref PlayerState stateToChange, Hand hand)
        {
            //Do you have pair
            //yes, split?
            if ((hand.cards.First().Face == hand.cards.Last().Face) && splitHand == null)
            {
                if(PairSplitting[hand.cards.First().Value - 2,dealersUpCard.Value - 2])
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
                        if (HardDoubleDown[hand.handValues.First()-8,dealersUpCard.Value - 2])
                        {
                            stateToChange = PlayerState.DOUBLE_DOWN;
                            return PlayerState.DOUBLE_DOWN;
                        }
                    }
                }
            }
            

            //no 
            //draw? check table
            stateToChange = PlayerState.STAND;
            return PlayerState.STAND;
        }
    }
}
