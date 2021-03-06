﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using BlackjackStrategy.Core;
using BlackjackStrategy.Core.Strategies;
using BlackjackStrategy.Core.Game;

namespace StrategiesTest
{
    [TestClass]
    public class FiveCountStrategyTest
    {
        [TestMethod]
        public void MaxBetTest()
        {

            int expectedBet = 50;
            Player player = new FiveCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Club, Face.Seven)
                    }
                },
                Count = new List<int>() { 4 }

            };
            int bet = player.CalculateBet(10, 50);
            Assert.AreEqual(expectedBet, bet);
        }
        [TestMethod]
        public void MinBetTest()
        {

            int expectedBet = 10;
            Player player = new FiveCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Club, Face.Seven)
                    }
                }

            };
            int bet = player.CalculateBet(10, 50);
            Assert.AreEqual(expectedBet, bet);
        }

        [TestMethod]
        public void CountTest()
        {
            int expectedCount = 3;
            Player player = new FiveCountStrategy();
            Deck deck = new Deck(52);
            List<Card> burntCards = new List<Card>();
            List<Card> deckToCardsList = deck.Cards.ToList();

            burntCards.Add(new Card(Suit.Spade, Face.Five));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Spade && x.Face == Face.Five);
            burntCards.Add(new Card(Suit.Club, Face.Ten));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Ten);
            burntCards.Add(new Card(Suit.Heart, Face.Five));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Heart && x.Face == Face.Five);
            burntCards.Add(new Card(Suit.Club, Face.Nine));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Nine);
            burntCards.Add(new Card(Suit.Club, Face.Five));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Five);
            burntCards.Add(new Card(Suit.Club, Face.Three));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Seven);

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
        public void SoftStandingNumberPlayer18Dealer9()
        {

            PlayerState expectedState = PlayerState.Hit;
            //Deck deck = new Deck(52);
            Player player = new FiveCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Club, Face.Seven)
                    },

                },
                Count = new List<int>() { 0 }
                
            };
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Nine), ref player.CurrentState, player.hand, player.Count));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void SoftStandingNumberPlayer19()
        {

            PlayerState expectedState = PlayerState.Stand;
            Player player = new FiveCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Club, Face.Eight)
                    }
                },
                Count = new List<int>() { 0 }
            };
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Nine), ref player.CurrentState, player.hand, player.Count));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void SoftStandingNumberPlayer18()
        {

            PlayerState expectedState = PlayerState.Stand;
            Player player = new FiveCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Club, Face.Seven)
                    }
                },
                Count = new List<int>() { 0 }
            };
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Eight), ref player.CurrentState, player.hand, player.Count));
            Assert.AreEqual(expectedState, state);
        }

        [TestMethod]
        public void NoFivesSplit10Dealer6Test()
        {

            PlayerState expectedState = PlayerState.Split;
            //Deck deck = new Deck(52);
            Player player = new FiveCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ten),
                        new Card(Suit.Heart, Face.Ten)
                    },

                },
                Count = new List<int>() { 4 }

            };
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Six), ref player.CurrentState, player.hand, player.Count));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void NoFivesSoftDoubleTest()
        {

            PlayerState expectedState = PlayerState.DoubleDown;
            //Deck deck = new Deck(52);
            Player player = new FiveCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Heart, Face.Six)
                    },

                },
                Count = new List<int>() { 4 }

            };
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Seven), ref player.CurrentState, player.hand, player.Count));
            Assert.AreEqual(expectedState, state);
        }
        public void HardDoubleTest()
        {

            PlayerState expectedState = PlayerState.DoubleDown;
            //Deck deck = new Deck(52);
            Player player = new FiveCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Two),
                        new Card(Suit.Heart, Face.Six)
                    },

                },
                Count = new List<int>() { 4 }

            };
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Four), ref player.CurrentState, player.hand, player.Count));
            Assert.AreEqual(expectedState, state);
        }
        public void HardStandTest()
        {

            PlayerState expectedState = PlayerState.DoubleDown;
            //Deck deck = new Deck(52);
            Player player = new FiveCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ten),
                        new Card(Suit.Heart, Face.Five)
                    },

                },
                Count = new List<int>() { 4 }

            };
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Nine), ref player.CurrentState, player.hand, player.Count));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void DoubleOnFourFourAgainstFourTest()
        {

            PlayerState expectedState = PlayerState.DoubleDown;
            //Deck deck = new Deck(52);
            Player player = new FiveCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Four),
                        new Card(Suit.Heart, Face.Four)
                    },

                },
                Count = new List<int>() { 4 }

            };
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Four), ref player.CurrentState, player.hand, player.Count));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void DoubleDownExceptSixTwoTest()
        {
            PlayerState expectedState = PlayerState.DoubleDown;
            Player player = new FiveCountStrategy
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
                Count = new List<int>() { 0 }
            };
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Five), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreNotEqual(expectedState, state);
        }
        [TestMethod]
        public void StandHoldingSevenSevenAgainstTenTest()
        {
            PlayerState expectedState = PlayerState.Stand;
            Player player = new FiveCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Seven),
                        new Card(Suit.Club, Face.Seven)
                    }
                },
                Count = new List<int>() { 0 }
            };
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Ten), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void HoldingHard16Against10Test()
        {
            PlayerState expectedState = PlayerState.Hit;
            Player player = new FiveCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ten),
                        new Card(Suit.Club, Face.Six)
                    }
                },
                Count = new List<int>() { 0 }
            };
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Ten), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
    }
}
