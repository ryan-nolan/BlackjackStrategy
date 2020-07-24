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
    public class AceToFiveStrategyTest
    {
        [TestMethod]
        public void MaxBetTest()
        {

            int expectedBet = 50;
            Player player = new AceToFiveStrategy
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
            int bet = player.CalculateBet(10, 50, new List<int>() { 2 });
            Assert.AreEqual(bet, expectedBet);
        }
        [TestMethod]
        public void MinBetTest()
        {

            int expectedBet = 10;
            Player player = new AceToFiveStrategy
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
            int bet = player.CalculateBet(10, 50, new List<int>() { 1 });
            Assert.AreEqual(bet, expectedBet);
        }
        [TestMethod]
        public void SoftStandingNumberPlayer18Dealer9()
        {

            PlayerState expectedState = PlayerState.HIT;
            Player player = new AceToFiveStrategy
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
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Nine), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(state, expectedState);
        }
        [TestMethod]
        public void SoftStandingNumberPlayer19()
        {

            PlayerState expectedState = PlayerState.STAND;
            Player player = new AceToFiveStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Club, Face.Eight)
                    }
                }
            };
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Nine), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(state, expectedState);
        }
        [TestMethod]
        public void SoftStandingNumberPlayer18()
        {

            PlayerState expectedState = PlayerState.STAND;
            Player player = new AceToFiveStrategy
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
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Eight), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(state, expectedState);
        }

    }
}
