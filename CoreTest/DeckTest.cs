using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class DeckTest
    {
        [TestMethod]
        public void VerifyDeckTest()
        {
            bool expectedState = true;
            for (int i = 1; i < 50; i++)
            {
                Deck deck = new Deck(52 * i);
                bool state = deck.VerifyDeck(deck.Cards);
                Assert.AreEqual(expectedState, state);
            }
        }

        [TestMethod]
        public void UniqueShuffle()
        {
            bool expectedState = false;
            List<string> deckHashCodes = new List<string>();
            for (int i = 1; i < 10000; i++)
            {
                Deck deck = new Deck(52);
                deck.FisherYatesShuffle();
                string currentDeckHashCode = deck.GetDeckHash();
                bool state = deckHashCodes.Contains(currentDeckHashCode);
                deckHashCodes.Add(currentDeckHashCode);
                
                Assert.AreEqual(expectedState, state);
            }
        }

        [TestMethod]
        public void CheckNon52DecksTest()
        {
            bool expectedState = false;
            Deck deck = new Deck(23);
            bool state = (deck.Cards.Count%52==0) ? false : true;
            Assert.AreEqual(expectedState, state);
        }

        [TestMethod]
        public void NegativeDeckSizeTest()
        {
            bool expectedState = false;
            Deck deck = new Deck(-52);
            bool state = (deck.Cards.Count % 52 == 0) ? false : true;
            Assert.AreEqual(expectedState, state);
        }
    }
}
