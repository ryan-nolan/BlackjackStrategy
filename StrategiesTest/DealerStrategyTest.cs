using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackjackLogic;
using BlackjackLogic.Strategies;
using BlackjackLogic.Game;

namespace StrategiesTest
{
    [TestClass]
    public class DealerStrategyTest
    {
        [TestMethod]
        public void StandOnSoft17Test()
        {

            PlayerState expectedState = PlayerState.STAND;
            Player player = new DealerStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Club, Face.Six)
                    }
                }
            };
            PlayerState state = (player.React(dealersUpCard: new Card (Suit.Club, Face.Eight), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void HitOnLessThanHard17Test()
        {

            PlayerState expectedState = PlayerState.HIT;
            Player player = new DealerStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Heart, Face.Five),
                        new Card(Suit.Club, Face.Five)
                    }
                }
            };
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Eight), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void StandOnGreaterThanHard17Test()
        {

            PlayerState expectedState = PlayerState.STAND;
            Player player = new DealerStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Heart, Face.Ten),
                        new Card(Suit.Club, Face.Ten)
                    }
                }
            };
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Eight), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
    }
}
