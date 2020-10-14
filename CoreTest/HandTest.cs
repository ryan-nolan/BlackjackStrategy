using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackjackLogic;
using BlackjackLogic.Strategies;
using BlackjackLogic.Game;

namespace CoreTest
{
    [TestClass]
    public class HandTest
    {
        private const int V = 2;

        [TestMethod]
        public void HardHandValueTest()
        {
            int expectedState = 18;
            Hand hand = new Hand();
            hand.cards.Add(new Card(Suit.Club, Face.Eight));
            hand.cards.Add(new Card(Suit.Club, Face.Ten));
            hand.SetHandValues();
            int state = hand.handValues.First();
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void HardHandSingleHandValueTest()
        {
            int expectedState = 1;
            Hand hand = new Hand();
            hand.cards.Add(new Card(Suit.Club, Face.Eight));
            hand.cards.Add(new Card(Suit.Club, Face.Ten));
            hand.SetHandValues();
            int state = hand.handValues.Count;
            Assert.AreEqual(expectedState, state);
        }

        [TestMethod]
        public void SoftHandValueTest()
        {
            int expectedState = 4;
            Hand hand = new Hand();
            hand.cards.Add(new Card(Suit.Club, Face.Ace));
            hand.cards.Add(new Card(Suit.Club, Face.Three));
            hand.SetHandValues();
            int state = hand.handValues.First();
            Assert.AreEqual(expectedState, state);

            expectedState = 14;
            state = hand.handValues.Last();
            Assert.AreEqual(expectedState, state);
        }

        [TestMethod]
        public void SoftHandTwoHandValueTest()
        {
            int expectedState = V;
            Hand hand = new Hand();
            hand.cards.Add(new Card(Suit.Club, Face.Ace));
            hand.cards.Add(new Card(Suit.Club, Face.Three));
            hand.SetHandValues();
            int state = hand.handValues.Count;
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void FiveCardHandValueTest()
        {
            int expectedState = 37;
            Hand hand = new Hand();
            hand.cards.Add(new Card(Suit.Club, Face.Eight));
            hand.cards.Add(new Card(Suit.Club, Face.Ten));
            hand.cards.Add(new Card(Suit.Club, Face.Three));
            hand.cards.Add(new Card(Suit.Club, Face.Six));
            hand.cards.Add(new Card(Suit.Club, Face.Jack));
            hand.SetHandValues();
            int state = hand.handValues.First();
            Assert.AreEqual(expectedState, state);
        }
    }
}
