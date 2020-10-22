using System;
using System.Collections.Generic;
using System.Linq;
using BlackjackLogic.Game;

namespace BlackjackLogic.Strategies
{
    public class CompletePointCountStrategy : Player
    {
        private float _hiLowIndex = 0;
        public override string StrategyName => "CompletePointCount";
        public override string CountType => "completepointcount";

        //Split if ratio < number in matrix
        //* indicated number should be read in reverse (ratio > num in matrix)
        private readonly int[,] _pairSplitting = new int[10, 10]
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
        private readonly int[,] _hardDoubleDown = new int[7, 10]
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

        private readonly int[,] _softDoubleDown = new int[8, 5]
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
        private readonly int[,] _hardHitOrStand = new int[6, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {14,6,2,-1,0,100,100,100,100,100},//12
            {1,-2,-5,-9,-8,50,100,100,100,100},//13
            {-5,-8,-13,-17,-17,20,38,100,100,100},//14
            {-12,-17,-21,-26,-28,13,15,12,8,16},//15
            {-21,-25,-30,-34,-35,10,11,6,0,14},//16
            { -100,-100,-100,-100,-100,-100,-100,-100,-100,-15 },//17
        };

        //-100 = SHADED = STAND
        //100 = NOT SHADED = HIT
        private readonly int[,] _softHitOrStand = new int[2, 10]
        {
           //2 3 4 5 6 7 8 9 10 A
           {-100,-100,-100,-100,-100,-100,-100,100,12,-6},//18 soft standing number for 18 is any ratio < 2.2
           {-100,-100,-100,-100,-100,-100,-100,-100,-100,-100},//19 STAND ON ANY INDEX 

        };
        /// <summary>
        /// Places bet dependent on the HiLoIndex
        /// </summary>
        /// <param name="minBet"></param>
        /// <param name="maxBet"></param>
        /// <returns></returns>
        public override int CalculateBet(int minBet, int maxBet)
        {
            UpdateIndex();
            if (_hiLowIndex < 2)
            {
                return minBet;
            }
            else if (_hiLowIndex >= 2 && _hiLowIndex < 4)
            {
                return minBet * 2;
            }
            else if (_hiLowIndex >= 4 && _hiLowIndex < 6)
            {
                return minBet * 5;
            }
            else if (_hiLowIndex >= 6 && _hiLowIndex < 8)
            {
                return minBet * 8;
            }
            else if (_hiLowIndex >= 8)
            {
                return minBet * 10;
            }
            return minBet;
        }
        /// <summary>
        /// Returns Index
        /// </summary>
        /// <returns>Index</returns>
        public float GetIndex()
        {
            return _hiLowIndex;
        }
        /// <summary>
        /// Updates the index based of count
        /// </summary>
        public void UpdateIndex()
        {
            _hiLowIndex = ((((float)Count[0]) / ((float)Count[1])) * 100);
            Console.WriteLine($"HiLowIndex:\t{_hiLowIndex}");
        }
        /// <summary>
        /// Updates the count
        /// Ace,J,Q,K,10 = -1
        /// 2,3,4,5,6 = +1
        /// 7,8,9 = 0
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="burntCards"></param>
        /// <param name="dealersUpCard"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Reacts to game state and makes decision according to matrices
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
                //Split eights against 10 if hi low index < 24
                if (hand.cards.First().Face == Face.Eight && dealersUpCard.Value == 10)
                {
                    if (_hiLowIndex < 24)
                    {
                        stateToChange = PlayerState.Split;
                        return PlayerState.Split;
                    }
                }
                if (hand.cards.First().Face == Face.Three && dealersUpCard.Value == 8)
                {
                    if (_hiLowIndex < -2 || _hiLowIndex > 6)
                    {
                        stateToChange = PlayerState.Split;
                        return PlayerState.Split;
                    }
                }
                if (hand.cards.First().Face == Face.Four && dealersUpCard.Face == Face.Six)
                {
                    stateToChange = PlayerState.DoubleDown;
                    return PlayerState.DoubleDown;
                }
                if (_hiLowIndex > _pairSplitting[hand.cards.First().Value - 2, dealersUpCard.Value - 2])
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
                    //Check A,6 Against 2 exception
                    if (cardNotAceInHand.Face == Face.Six && dealersUpCard.Face == Face.Two)
                    {
                        if (_hiLowIndex >= 1 && _hiLowIndex <= 10)
                        {
                            stateToChange = PlayerState.DoubleDown;
                            return PlayerState.DoubleDown;
                        }
                    }
                    if (dealersUpCard.Value < 7 && cardNotAceInHand.Value != 10)
                    {
                        if (_hiLowIndex > _softDoubleDown[cardNotAceInHand.Value - 2, dealersUpCard.Value - 2])
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
                        if ((_hiLowIndex > _hardDoubleDown[hand.handValues.First() - 5, dealersUpCard.Value - 2]))
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
                    if (hand.handValues.Max() == 17 && dealersUpCard.Face == Face.Seven && _hiLowIndex > 29)
                    {
                        stateToChange = PlayerState.Stand;
                        return PlayerState.Stand;
                    }
                    stateToChange = PlayerState.Hit;
                    return PlayerState.Hit;
                }
                if (hand.handValues.Max() > 19)
                {
                    stateToChange = PlayerState.Stand;
                    return PlayerState.Stand;
                }

                if (_hiLowIndex > _softHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2])
                {
                    stateToChange = PlayerState.Stand;
                    return PlayerState.Stand;
                }

                if (_hiLowIndex <= _softHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2])
                {
                    stateToChange = PlayerState.Hit;
                    return PlayerState.Hit;
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
                stateToChange = PlayerState.Stand;
                return PlayerState.Stand;
            }


            if (_hiLowIndex > _hardHitOrStand[hand.handValues.Max() - 12, dealersUpCard.Value - 2])
            {
                stateToChange = PlayerState.Stand;
                return PlayerState.Stand;
            }
            else if (_hiLowIndex <= _hardHitOrStand[hand.handValues.Max() - 12, dealersUpCard.Value - 2])
            {
                stateToChange = PlayerState.Hit;
                return PlayerState.Hit;
            }

            stateToChange = PlayerState.Stand;
            return PlayerState.Stand;
        }
    }
}
