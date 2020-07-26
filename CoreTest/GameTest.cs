using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackjackLogic;
using BlackjackLogic.Strategies;
using System.IO;

namespace CoreTest
{
    [TestClass]
    public class GameTest
    {
        [TestMethod]
        public void DealHandTest()
        {
            Game game = new Game();
            game.player = new DealerStrategy();
            game.dealer = new Dealer();
            game.deck = new Deck(52);

            game.DealHand();
            int expectedState = 2;
            Assert.AreEqual(expectedState, game.player.hand.cards.Count);
            Assert.AreEqual(expectedState, game.dealer.hand.cards.Count);

        }
        [TestMethod]
        public void HitPlayerActorTest()
        {
            Game game = new Game();
            game.player = new DealerStrategy();
            game.dealer = new Dealer();
            game.deck = new Deck(52);

            game.DealHand();
            game.HitPlayer(game.dealer);
            game.HitPlayer(game.dealer);
            game.HitPlayer(game.player);
            game.HitPlayer(game.player);
            game.HitPlayer(game.player);
            Assert.AreEqual(5, game.player.hand.cards.Count);
            Assert.AreEqual(4, game.dealer.hand.cards.Count);
        }
        [TestMethod]
        public void HitPlayerHandTest()
        {
            Game game = new Game();
            game.player = new DealerStrategy();
            game.dealer = new Dealer();
            game.deck = new Deck(52);

            game.DealHand();
            game.HitPlayer(game.dealer.hand);
            game.HitPlayer(game.dealer.hand);
            game.HitPlayer(game.dealer.hand);
            game.HitPlayer(game.player.hand);
            game.HitPlayer(game.player.hand);
            Assert.AreEqual(4, game.player.hand.cards.Count);
            Assert.AreEqual(5, game.dealer.hand.cards.Count);
        }
        [TestMethod]
        public void UpdateHandValuesPlayerSoftTest()
        {
            int expectedState = 13;
            Game game = new Game();
            game.player = new BasicStrategy();
            game.dealer = new Dealer();
            game.player.hand.cards.Add(new Card(Suit.Club, Face.Ace));
            game.player.hand.cards.Add(new Card(Suit.Club, Face.Two));
            game.UpdateHandValues();

            Assert.AreEqual(expectedState, game.player.hand.handValues.Last());
            Assert.AreEqual(2, game.player.hand.handValues.Count);
        }
        [TestMethod]
        public void UpdateHandValuesDealerSoftTest()
        {
            int expectedState = 13;
            Game game = new Game();
            game.player = new BasicStrategy();
            game.dealer = new Dealer();
            game.dealer.hand.cards.Add(new Card(Suit.Club, Face.Ace));
            game.dealer.hand.cards.Add(new Card(Suit.Club, Face.Two));
            game.UpdateHandValues();

            Assert.AreEqual(expectedState, game.dealer.hand.handValues.Last());
            Assert.AreEqual(2, game.dealer.hand.handValues.Count);
        }
        [TestMethod]
        public void UpdateHandValuesPlayerHardTest()
        {
            int expectedState = 14;
            Game game = new Game();
            game.player = new BasicStrategy();
            game.dealer = new Dealer();
            game.player.hand.cards.Add(new Card(Suit.Club, Face.Ten));
            game.player.hand.cards.Add(new Card(Suit.Club, Face.Four));
            game.UpdateHandValues();

            Assert.AreEqual(expectedState, game.player.hand.handValues.Last());
            Assert.AreEqual(1, game.player.hand.handValues.Count);
        }
        [TestMethod]
        public void UpdateHandValuesDealerHardTest()
        {
            int expectedState = 16;
            Game game = new Game();
            game.player = new BasicStrategy();
            game.dealer = new Dealer();
            game.dealer.hand.cards.Add(new Card(Suit.Club, Face.Ten));
            game.dealer.hand.cards.Add(new Card(Suit.Club, Face.Six));
            game.UpdateHandValues();

            Assert.AreEqual(expectedState, game.dealer.hand.handValues.Last());
            Assert.AreEqual(1, game.dealer.hand.handValues.Count);
        }
    }
}
