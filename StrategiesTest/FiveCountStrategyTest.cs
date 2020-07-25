using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackjackLogic;
using BlackjackLogic.Strategies;


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
        public void SoftStandingNumberPlayer18Dealer9()
        {

            PlayerState expectedState = PlayerState.HIT;
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

            PlayerState expectedState = PlayerState.STAND;
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

            PlayerState expectedState = PlayerState.STAND;
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

            PlayerState expectedState = PlayerState.SPLIT;
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

            PlayerState expectedState = PlayerState.DOUBLE_DOWN;
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

            PlayerState expectedState = PlayerState.DOUBLE_DOWN;
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

            PlayerState expectedState = PlayerState.DOUBLE_DOWN;
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

            PlayerState expectedState = PlayerState.DOUBLE_DOWN;
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
            PlayerState expectedState = PlayerState.DOUBLE_DOWN;
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
            PlayerState expectedState = PlayerState.STAND;
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
            PlayerState expectedState = PlayerState.HIT;
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
