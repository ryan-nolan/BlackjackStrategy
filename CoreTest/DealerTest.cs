﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackjackLogic;
using BlackjackLogic.Strategies;

namespace CoreTest
{
    [TestClass]
    public class DealerTest
    {
        [TestMethod]
        public void StandOnSoft17Test()
        {

            PlayerState expectedState = PlayerState.STAND;
            Dealer dealer = new Dealer
            {
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Club, Face.Six)
                    }
                }
            };
            PlayerState state = (dealer.React());
            Assert.AreEqual(state, expectedState);
        }
        [TestMethod]
        public void HitOnLessThanHard17Test()
        {

            PlayerState expectedState = PlayerState.HIT;
            Dealer dealer = new Dealer
            {
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Heart, Face.Five),
                        new Card(Suit.Club, Face.Five)
                    }
                }
            };
            PlayerState state = (dealer.React());
            Assert.AreEqual(state, expectedState);
        }
        [TestMethod]
        public void StandOnGreaterThanHard17Test()
        {

            PlayerState expectedState = PlayerState.STAND;
            Dealer dealer = new Dealer
            {
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Heart, Face.Ten),
                        new Card(Suit.Club, Face.Ten)
                    }
                }
            };
            PlayerState state = (dealer.React());
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void SetUpCardTest()
        {

            int expectedState = 2;
            Dealer dealer = new Dealer
            {
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Heart, Face.Two),
                        new Card(Suit.Club, Face.Five)
                    }
                }
            };
            dealer.SetUpCard();
            Assert.AreEqual(expectedState, dealer.upCard.Value);
        }
    }
}
