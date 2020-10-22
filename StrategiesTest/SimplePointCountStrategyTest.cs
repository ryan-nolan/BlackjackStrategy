using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using BlackjackLogic;
using BlackjackLogic.Strategies;
using BlackjackLogic.Game;

namespace StrategiesTest
{
    [TestClass]
    public class SimplePointCountStrategyTest
    {
        [TestMethod]
        public void CountTest()
        {
            int expectedCount = 1;
            Player player = new KnockoutCountStrategy();
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
            Player player = new SimplePointCountStrategy
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
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void SoftStandingNumberPlayer19()
        {

            PlayerState expectedState = PlayerState.Stand;
            Player player = new SimplePointCountStrategy
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
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void SoftStandingNumberPlayer18()
        {

            PlayerState expectedState = PlayerState.Stand;
            Player player = new SimplePointCountStrategy
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
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void DoubleDownExceptSixTwoTest()
        {
            PlayerState expectedState = PlayerState.DoubleDown;
            Player player = new SimplePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Six),
                        new Card(Suit.Club, Face.Two)
                    }
                }
            };
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Five), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreNotEqual(expectedState, state);
        }
        [TestMethod]
        public void StandHoldingSevenSevenAgainstTenTest()
        {
            PlayerState expectedState = PlayerState.Stand;
            Player player = new SimplePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Seven),
                        new Card(Suit.Club, Face.Seven)
                    }
                }
            };
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Ten), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void HoldingHard16Against10Test()
        {
            PlayerState expectedState = PlayerState.Hit;
            Player player = new SimplePointCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ten),
                        new Card(Suit.Club, Face.Six)
                    }
                }
            };
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Ten), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }




    }
}