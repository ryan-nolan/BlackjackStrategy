using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using BlackjackStrategy.Core;
using BlackjackStrategy.Core.Strategies;
using BlackjackStrategy.Core.Game;

namespace StrategiesTest
{
    [TestClass]
    public class CompletePointCountStrategyTest
    {
        [TestMethod]
        public void CountTest()
        {

            int expectedCount = 1;
            Player player = new CompletePointCountStrategy();
            Deck deck = new Deck(52);
            List<Card> burntCards = new List<Card>();
            List<Card> deckToCardsList = deck.Cards.ToList();

            burntCards.Add(new Card(Suit.Club, Face.Two));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Two);
            burntCards.Add(new Card(Suit.Club, Face.Ten));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Ten);
            burntCards.Add(new Card(Suit.Club, Face.Ace));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Ace);
            burntCards.Add(new Card(Suit.Club, Face.Nine));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Nine);
            burntCards.Add(new Card(Suit.Club, Face.Five));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Five);
            burntCards.Add(new Card(Suit.Club, Face.Three));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Three);

            deck.Cards.Clear();
            foreach (var card in deckToCardsList)
            {
                deck.Cards.Push(card);
            }

            player.hand.SetHandValues();
            int count = player.UpdateCount(deck, burntCards, null).First();
            Assert.AreEqual(expectedCount, count);
        }
        [TestMethod]
        public void IndexTest()
        {
            float expectedIndex = 2.173913f;
            CompletePointCountStrategy player = new CompletePointCountStrategy();
            Deck deck = new Deck(52);
            List<Card> burntCards = new List<Card>();
            List<Card> deckToCardsList = deck.Cards.ToList();

            burntCards.Add(new Card(Suit.Club, Face.Two));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Two);
            burntCards.Add(new Card(Suit.Club, Face.Ten));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Ten);
            burntCards.Add(new Card(Suit.Club, Face.Ace));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Ace);
            burntCards.Add(new Card(Suit.Club, Face.Nine));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Nine);
            burntCards.Add(new Card(Suit.Club, Face.Five));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Five);
            burntCards.Add(new Card(Suit.Club, Face.Three));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Three);

            deck.Cards.Clear();
            foreach (var card in deckToCardsList)
            {
                deck.Cards.Push(card);
            }

            player.hand.SetHandValues();
            int count = player.UpdateCount(deck, burntCards, null).First();
            float index = player.GetIndex();
            Assert.AreEqual(expectedIndex, index);
        }
        [TestMethod]
        public void BetMinTest()
        {
            int expectedBet = 2;
            Player player = new CompletePointCountStrategy();
            Deck deck = new Deck(52);
            List<Card> burntCards = new List<Card>();
            List<Card> deckToCardsList = deck.Cards.ToList();

            burntCards.Add(new Card(Suit.Club, Face.Ten));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Ten);
            burntCards.Add(new Card(Suit.Spade, Face.Ten));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Spade && x.Face == Face.Ten);
            burntCards.Add(new Card(Suit.Club, Face.Jack));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Jack);
            burntCards.Add(new Card(Suit.Spade, Face.Jack));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Spade && x.Face == Face.Jack);
            burntCards.Add(new Card(Suit.Club, Face.Queen));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Queen);
            burntCards.Add(new Card(Suit.Spade, Face.Queen));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Spade && x.Face == Face.Queen);

            deck.Cards.Clear();
            foreach (var card in deckToCardsList)
            {
                deck.Cards.Push(card);
            }

            player.hand.SetHandValues();
            player.UpdateCount(deck, burntCards, null).First();
            int bet = player.CalculateBet(2, 10);
            Assert.AreEqual(expectedBet, bet);
        }

        [TestMethod]
        public void BetMaxTest()
        {
            int expectedBet = 100;
            Player player = new CompletePointCountStrategy();
            Deck deck = new Deck(52);
            List<Card> burntCards = new List<Card>();
            List<Card> deckToCardsList = deck.Cards.ToList();

            burntCards.Add(new Card(Suit.Club, Face.Two));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Two);
            burntCards.Add(new Card(Suit.Spade, Face.Two));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Spade && x.Face == Face.Two);
            burntCards.Add(new Card(Suit.Club, Face.Three));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Three);
            burntCards.Add(new Card(Suit.Spade, Face.Three));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Spade && x.Face == Face.Three);
            burntCards.Add(new Card(Suit.Club, Face.Four));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Four);
            burntCards.Add(new Card(Suit.Spade, Face.Four));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Spade && x.Face == Face.Four);

            deck.Cards.Clear();
            foreach (var card in deckToCardsList)
            {
                deck.Cards.Push(card);
            }

            player.hand.SetHandValues();
            player.UpdateCount(deck, burntCards, null).First();
            int bet = player.CalculateBet(10, 100);
            Assert.AreEqual(expectedBet, bet);
        }


        [TestMethod]
        //(A,4) against 3 with a count of 1.7 //Should double down if count < 1.9
        public void SoftDoublingGreaterThanTest()
        {
            PlayerState expectedState = PlayerState.DoubleDown;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Club, Face.Six)
                    }
                },
                Count = new List<int>() { -5, 100 } //Count = -5
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Three), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        //(A,4) against 3 with a count of 2 //Should not double down if count > 1.9
        public void SoftDoublingLessThanTest()
        {
            PlayerState expectedState = PlayerState.DoubleDown;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Club, Face.Six)
                    }
                },
                Count = new List<int>() { -7, 100 }
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Three), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreNotEqual(expectedState, state);
        }

        [TestMethod]
        public void HardDoublingGreaterThanTest()
        {
            PlayerState expectedState = PlayerState.DoubleDown;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Six),
                        new Card(Suit.Club, Face.Two)
                    }
                },
                Count = new List<int>() { 23, 100 } //Count = 23
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Three), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void HardDoublingLessThanTest()
        {
            PlayerState expectedState = PlayerState.DoubleDown;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Six),
                        new Card(Suit.Club, Face.Two)
                    }
                },
                Count = new List<int>() { 21, 100 } //Count = 21
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Three), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreNotEqual(expectedState, state);
        }

        [TestMethod]
        public void HardDoublingEqualToTest()
        {
            PlayerState expectedState = PlayerState.DoubleDown;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Six),
                        new Card(Suit.Club, Face.Two)
                    }
                },
                Count = new List<int>() { 22, 100 } //Count = 22
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Three), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreNotEqual(expectedState, state);
        }

        [TestMethod]
        public void PairSplittingGreaterThanTest()
        {
            PlayerState expectedState = PlayerState.Split;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Seven),
                        new Card(Suit.Diamond, Face.Seven)
                    }
                },
                Count = new List<int>() { -20, 100 } //Count = 1.6
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Two), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void PairSplittingLessThanTest()
        {
            PlayerState expectedState = PlayerState.Split;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Seven),
                        new Card(Suit.Diamond, Face.Seven)
                    }
                },
                Count = new List<int>() { -30, 100 } //Count = 1.6
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Two), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreNotEqual(expectedState, state);
        }

        [TestMethod]
        public void PairSplittingShadedTest()
        {
            PlayerState expectedState = PlayerState.Split;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Diamond, Face.Ace)
                    }
                },
                Count = new List<int>() { 50, 100 } //Count = 1.6
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Two), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }

        [TestMethod]
        public void SoftStandingGreaterThanTest()
        {
            PlayerState expectedState = PlayerState.Stand;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Diamond, Face.Six)
                    }
                },
                Count = new List<int>() { 30, 100 } //Count = 0.01
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Seven), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
        
        [TestMethod]
        public void SoftStandingLessThanTest()
        {
            PlayerState expectedState = PlayerState.Hit;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Diamond, Face.Six)
                    }
                },
                Count = new List<int>() { -40, 100 } //Count = 0.01
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Seven), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void SoftStandingShadedTest()
        {
            PlayerState expectedState = PlayerState.Stand;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Diamond, Face.Seven)
                    }
                },
                Count = new List<int>() { -40, 100 } //Count = 0.01
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Eight), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void SoftStandingNonShadedTest()
        {
            PlayerState expectedState = PlayerState.Hit;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Diamond, Face.Seven)
                    }
                },
                Count = new List<int>() { 40, 100 } //Count = 0.01
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Nine), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }

        [TestMethod]
        public void HardStandingGreaterThanTest()
        {
            PlayerState expectedState = PlayerState.Stand;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ten),
                        new Card(Suit.Diamond, Face.Five)
                    }
                },
                Count = new List<int>() { -20, 100 } //Count = 0.01
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Six), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void HardStandingLessThanTest()
        {
            PlayerState expectedState = PlayerState.Hit;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ten),
                        new Card(Suit.Diamond, Face.Five)
                    }
                },
                Count = new List<int>() { -30, 100 } //Count = 30
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Six), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void HardStandingShadedTest()
        {
            PlayerState expectedState = PlayerState.Stand;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ten),
                        new Card(Suit.Diamond, Face.Seven)
                    }
                },
                Count = new List<int>() { -30, 100 } //Count = 30
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Ten), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }

        [TestMethod]
        public void HardStandingNonShadedTest()
        {
            PlayerState expectedState = PlayerState.Hit;
            CompletePointCountStrategy player = new CompletePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ten),
                        new Card(Suit.Diamond, Face.Four)
                    }
                },
                Count = new List<int>() { 50, 100 } //Count = 30
            };
            player.UpdateIndex();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Nine), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
    }
}
