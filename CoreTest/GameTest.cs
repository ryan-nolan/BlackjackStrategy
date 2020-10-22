using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using BlackjackStrategy.Core;
using BlackjackStrategy.Core.Strategies;
using BlackjackStrategy.Core.Game;

namespace CoreTest
{
    [TestClass]
    public class GameTest
    {
        [TestMethod]
        public void DealHandTest()
        {
            Simulator game = new Simulator();
            game.Player = new DealerStrategy();
            game.Dealer = new Dealer();
            game.Deck = new Deck(52);

            game.DealHand();
            int expectedState = 2;
            Assert.AreEqual(expectedState, game.Player.hand.cards.Count);
            Assert.AreEqual(expectedState, game.Dealer.hand.cards.Count);

        }
        [TestMethod]
        public void HitPlayerActorTest()
        {
            Simulator game = new Simulator();
            game.Player = new DealerStrategy();
            game.Dealer = new Dealer();
            game.Deck = new Deck(52);

            game.DealHand();
            game.HitPlayer(game.Dealer);
            game.HitPlayer(game.Dealer);
            game.HitPlayer(game.Player);
            game.HitPlayer(game.Player);
            game.HitPlayer(game.Player);
            Assert.AreEqual(5, game.Player.hand.cards.Count);
            Assert.AreEqual(4, game.Dealer.hand.cards.Count);
        }
        [TestMethod]
        public void HitPlayerHandTest()
        {
            Simulator game = new Simulator();
            game.Player = new DealerStrategy();
            game.Dealer = new Dealer();
            game.Deck = new Deck(52);

            game.DealHand();
            game.HitPlayer(game.Dealer.hand);
            game.HitPlayer(game.Dealer.hand);
            game.HitPlayer(game.Dealer.hand);
            game.HitPlayer(game.Player.hand);
            game.HitPlayer(game.Player.hand);
            Assert.AreEqual(4, game.Player.hand.cards.Count);
            Assert.AreEqual(5, game.Dealer.hand.cards.Count);
        }
        [TestMethod]
        public void UpdateHandValuesPlayerSoftTest()
        {
            int expectedState = 13;
            Simulator game = new Simulator();
            game.Player = new BasicStrategy();
            game.Dealer = new Dealer();
            game.Player.hand.cards.Add(new Card(Suit.Club, Face.Ace));
            game.Player.hand.cards.Add(new Card(Suit.Club, Face.Two));
            game.UpdateHandValues();

            Assert.AreEqual(expectedState, game.Player.hand.handValues.Last());
            Assert.AreEqual(2, game.Player.hand.handValues.Count);
        }
        [TestMethod]
        public void UpdateHandValuesDealerSoftTest()
        {
            int expectedState = 13;
            Simulator game = new Simulator();
            game.Player = new BasicStrategy();
            game.Dealer = new Dealer();
            game.Dealer.hand.cards.Add(new Card(Suit.Club, Face.Ace));
            game.Dealer.hand.cards.Add(new Card(Suit.Club, Face.Two));
            game.UpdateHandValues();

            Assert.AreEqual(expectedState, game.Dealer.hand.handValues.Last());
            Assert.AreEqual(2, game.Dealer.hand.handValues.Count);
        }
        [TestMethod]
        public void UpdateHandValuesPlayerHardTest()
        {
            int expectedState = 14;
            Simulator game = new Simulator();
            game.Player = new BasicStrategy();
            game.Dealer = new Dealer();
            game.Player.hand.cards.Add(new Card(Suit.Club, Face.Ten));
            game.Player.hand.cards.Add(new Card(Suit.Club, Face.Four));
            game.UpdateHandValues();

            Assert.AreEqual(expectedState, game.Player.hand.handValues.Last());
            Assert.AreEqual(1, game.Player.hand.handValues.Count);
        }
        [TestMethod]
        public void UpdateHandValuesDealerHardTest()
        {
            int expectedState = 16;
            Simulator game = new Simulator();
            game.Player = new BasicStrategy();
            game.Dealer = new Dealer();
            game.Dealer.hand.cards.Add(new Card(Suit.Club, Face.Ten));
            game.Dealer.hand.cards.Add(new Card(Suit.Club, Face.Six));
            game.UpdateHandValues();

            Assert.AreEqual(expectedState, game.Dealer.hand.handValues.Last());
            Assert.AreEqual(1, game.Dealer.hand.handValues.Count);
        }
    }
}
