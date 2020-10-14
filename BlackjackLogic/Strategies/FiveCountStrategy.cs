using System.Collections.Generic;
using System.Linq;
using BlackjackLogic.Game;

namespace BlackjackLogic.Strategies
{
    public class FiveCountStrategy : Player
    {
        public override string StrategyName { get { return "FiveCount"; } }
        public override string CountType { get { return "five"; } }

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
        //TODO Account for optimisations
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
        //Hit on anything <= 17, stand on anything 19 or more
        //This strategy is the same for no fives
        readonly bool[,] SoftHitOrStand = new bool[2, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {true,true,true,true,true,true,true,false,false,true},//18
            {true,true,true,true,true,true,true,true,true,true},//19

        };


        //NO FIVES IN PLAY DECISIONS
        //Pair Splitting when no fives are in the deck
        readonly bool[,] PairSplittingNoFives = new bool[10, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {true,true,true,true,true,true,true,false,false,false },//(2,2)
            {true,true,true,true,true,true,true,false,false,false },//(3,3)
            {false,false,false,false,false,false,false,false,false,false },//(4,4)
            {false,false,false,false,false,false,false,false,false,false },//(5,5)
            {true,true,true,true,true,false,false,false,false,false},//(6,6)
            {true,true,true,true,true,true,true,true,false,false},//(7,7)
            {true,true,true,true,true,true,true,true,true,true },//(8,8)
            {true,true,true,true,true,true,true,true,false,false},//(9,9)
            {false,false,false,false,true,false,false,false,false,false},//(10,10)
            {true,true,true,true,true,true,true,true,true,true },//(A,A)
        };
        readonly bool[,] HardDoubleDownNoFives = new bool[4, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {false,false,true,true,true,false,false,false,false,false},//8
            {true,true,true,true,true,true,false,false,false,false },//9
            {true,true,true,true,true,true,true,true,true,true },//10
            {true,true,true,true,true,true,true,true,true,true },//11
        };
        readonly bool[,] SoftDoubleDownNoFives = new bool[8, 10]
        {
            //2    3     4     5     6    7     8    9    10    A
            {false,true,true,true,true,false,false,false,false,false},//(A,2)
            {false,true,true,true,true,false,false,false,false,false},//(A,3)
            {false,true,true,true,true,false,false,false,false,false},//(A,4)
            {false,true,true,true,true,false,false,false,false,false},//(A,5)
            {true,true,true,true,true,true,false,false,false,false},//(A,6)
            {true,true,true,true,true,false,false,false,false,false},//(A,7)
            {false,true,true,true,true,false,false,false,false,false},//(A,8)
            {false,false,false,false,true,false,false,false,false,false}//(A,9)
        };
        //SoftHitOrStandNoFives = SoftHitOrStand
        //Hard Stand on true, hit on false
        //Account for optimisations
        readonly bool[,] HardHitOrStandNoFives = new bool[6, 10]
        {
            //2   3    4    5    6    7    8    9    10   A
            {true,true,true,true,true,false,false,false,false,false},//12
            {true,true,true,true,true,false,false,false,false,false},//13
            {true,true,true,true,true,false,false,false,false,false},//14
            {true,true,true,true,true,false,false,true,true,false},//15
            {true,true,true,true,true,false,false,true,true,false},//16
            {true,true,true,true,true,true,true,true,true,true},//17
        };
        /// <summary>
        /// Returns max bet when there are no fives in the deck, count = 4
        /// Otherwise returns minBet
        /// </summary>
        /// <param name="minBet"></param>
        /// <param name="maxBet"></param>
        /// <returns>Bet based on fives</returns>
        public override int CalculateBet(int minBet, int maxBet)
        {
            if (Count[0] == Deck.DeckSize / 13)
            {
                return maxBet;
            }
            else
            {
                return minBet;
            }
        }
        /// <summary>
        /// Updates count based on fives seen played
        /// 5 = +1
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="burntCards"></param>
        /// <param name="dealersUpCard"></param>
        /// <returns></returns>
        public override List<int> UpdateCount(Deck deck, List<Card> burntCards, Card dealersUpCard)
        {
            Count[0] = 0;
            if (hand.cards.Count > 0)
            {
                foreach (var c in hand.cards)
                {
                    if (c.Face == Face.Five)
                    {
                        Count[0]++;
                    }
                }
            }
            if (splitHand != null)
            {
                if (splitHand.cards.Count > 0)
                {
                    foreach (var c in splitHand.cards)
                    {
                        if (c.Face == Face.Five)
                        {
                            Count[0]++;
                        }
                    }
                }

            }
            if (dealersUpCard != null)
            {
                if (dealersUpCard.Face == Face.Five)
                {
                    Count[0]++;
                }
            }
            foreach (var c in burntCards)
            {
                if (c.Face == Face.Five)
                {
                    Count[0]++;
                }
            }
            return Count;
        }
        /// <summary>
        /// Reacts to game state with decision matrices
        /// Uses no fives matrices when count = deckSize/13
        /// </summary>
        /// <param name="dealersUpCard"></param>
        /// <param name="stateToChange"></param>
        /// <param name="hand"></param>
        /// <param name="fiveCount"></param>
        /// <returns></returns>
        public override PlayerState React(Card dealersUpCard, ref PlayerState stateToChange, Hand hand, List<int> fiveCount)
        {
            //Play by basic strategy until 5 count is equal to Q(5)
            if (Count[0] != Deck.DeckSize / 13)
            {
                //Bust if hand is over 21
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
                    if (!SoftHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2])
                    {
                        stateToChange = PlayerState.HIT;
                        return PlayerState.HIT;
                    }
                    else if (SoftHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2])
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
                //if hand is hard and is between 12-17, check basic strategy table whether to hit or stand
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
                //STAND HOLDING 7,7 AGAINST A 10
                if (hand.cards.Count == 2 && ((hand.cards.First().Face == Face.Seven) && (hand.cards.Last().Face == Face.Seven)) && dealersUpCard.Value == 10)
                {
                    stateToChange = PlayerState.STAND;
                    return PlayerState.STAND;
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
                //Stand
                stateToChange = PlayerState.STAND;
                return PlayerState.STAND;
            }
            //NO FIVES -----------------------------------------------------------------------------------------------------------
            else
            {
                //Bust if hand is over 21
                if (hand.handValues.First() > 21)
                {
                    stateToChange = PlayerState.BUST;
                    return PlayerState.BUST;
                }

                //Do you have pair
                //yes, split?
                if (((hand.cards.First().Face == hand.cards.Last().Face) && splitHand == null) && hand.cards.Count == 2)
                {
                    if (PairSplittingNoFives[hand.cards.First().Value - 2, dealersUpCard.Value - 2])
                    {
                        if ((dealersUpCard.Value == 4 || dealersUpCard.Value == 5 || dealersUpCard.Value == 6) && (hand.cards.First().Value == 4))
                        {
                            stateToChange = PlayerState.DOUBLE_DOWN;
                            return PlayerState.DOUBLE_DOWN;
                        }
                        stateToChange = PlayerState.SPLIT;
                        return PlayerState.SPLIT;
                    }
                }

                //double down
                //yes
                //stand
                if (hand.cards.Count == 2)
                {
                    //SOFT HAND DOUBLE DOWN? NO FIVES
                    if (hand.handValues.Count > 1)
                    {
                        //Always split aces
                        if (hand.cards.First().Face == Face.Ace && hand.cards.Last().Face == Face.Ace)
                        {
                            stateToChange = PlayerState.DOUBLE_DOWN;
                            return PlayerState.DOUBLE_DOWN;
                        }
                        var cardNotAceInHand = hand.cards.Find(x => x.Face != Face.Ace);
                        //TODO Test This
                        if (cardNotAceInHand.Value < 10)
                        {
                            if (SoftDoubleDownNoFives[cardNotAceInHand.Value - 2, dealersUpCard.Value - 2])
                            {
                                stateToChange = PlayerState.DOUBLE_DOWN;
                                return PlayerState.DOUBLE_DOWN;
                            }
                        }
                    }
                    //HARD HAND DOUBLE DOWN? NO FIVES
                    else
                    {
                        if (hand.handValues.First() >= 8 && hand.handValues.First() <= 11)
                        {
                            if (HardDoubleDownNoFives[hand.handValues.First() - 8, dealersUpCard.Value - 2])
                            {
                                stateToChange = PlayerState.DOUBLE_DOWN;
                                return PlayerState.DOUBLE_DOWN;
                            }
                        }
                    }
                }


                //no 
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
                    if (!SoftHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2])
                    {
                        stateToChange = PlayerState.HIT;
                        return PlayerState.HIT;
                    }
                    else if (SoftHitOrStand[hand.handValues.Max() - 18, dealersUpCard.Value - 2])
                    {
                        stateToChange = PlayerState.STAND;
                        return PlayerState.STAND;
                    }
                }

                //Hard NO FIVES
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
                //if hand is hard and is between 12-17, check basic strategy table whether to hit or stand
                //Hard 16 exception
                if (hand.handValues.First() == 16 && hand.cards.Count > 2)
                {
                    stateToChange = PlayerState.STAND;
                    return PlayerState.STAND;
                }
                if (hand.handValues.First() == 16 && hand.cards.Count == 2)
                {
                    stateToChange = PlayerState.HIT;
                    return PlayerState.HIT;
                }
                if (HardHitOrStandNoFives[hand.handValues.Max() - 12, dealersUpCard.Value - 2])
                {
                    stateToChange = PlayerState.STAND;
                    return PlayerState.STAND;
                }
                else if (!(HardHitOrStandNoFives[hand.handValues.Max() - 12, dealersUpCard.Value - 2]))
                {
                    stateToChange = PlayerState.HIT;
                    return PlayerState.HIT;
                }
                //Stand
                stateToChange = PlayerState.STAND;
                return PlayerState.STAND;
            }
        }
    }
}
