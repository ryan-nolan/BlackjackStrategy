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
    public class TenCountStrategyTest
    {
        [TestMethod]
        public void TestCount()
        {

            int expectedCount = 13;
            Player player = new TenCountStrategy();
            Deck deck = new Deck(52);
            List<Card> burntCards = new List<Card>();
            List<Card> deckToCardsList = deck.Cards.ToList();

            burntCards.Add(new Card(Suit.Club, Face.King));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.King);
            burntCards.Add(new Card(Suit.Club, Face.Ten));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Ten);
            burntCards.Add(new Card(Suit.Club, Face.Queen));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Queen);
            burntCards.Add(new Card(Suit.Club, Face.Nine));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Nine);
            burntCards.Add(new Card(Suit.Club, Face.Five));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Five);
            burntCards.Add(new Card(Suit.Club, Face.Ace));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Ace);

            deck.Cards.Clear();
            foreach (var card in deckToCardsList)
            {
                deck.Cards.Push(card);
            }

            player.hand.SetHandValues();
            int count = player.UpdateCount(deck, burntCards, null).Last();
            Assert.AreEqual(count, expectedCount);
        }
        [TestMethod]
        public void TestIndex()
        {
            float expectedIndex = ((float)33) / ((float)13);
            TenCountStrategy player = new TenCountStrategy();
            Deck deck = new Deck(52);
            List<Card> burntCards = new List<Card>();
            List<Card> deckToCardsList = deck.Cards.ToList();

            burntCards.Add(new Card(Suit.Club, Face.King));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.King);
            burntCards.Add(new Card(Suit.Club, Face.Ten));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Ten);
            burntCards.Add(new Card(Suit.Club, Face.Queen));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Queen);
            burntCards.Add(new Card(Suit.Club, Face.Nine));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Nine);
            burntCards.Add(new Card(Suit.Club, Face.Five));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Five);
            burntCards.Add(new Card(Suit.Club, Face.Ace));
            deckToCardsList.RemoveAll(x => x.Suit == Suit.Club && x.Face == Face.Ace);

            deck.Cards.Clear();
            foreach (var card in deckToCardsList)
            {
                deck.Cards.Push(card);
            }

            player.hand.SetHandValues();
            int count = player.UpdateCount(deck, burntCards, null).First();
            float index = player.GetOthersOverTenRatio();
            Assert.AreEqual(expectedIndex, index);
        }

        [TestMethod]
        //(A,4) against 3 with a count of 1.7 //Should double down if count < 1.9
        public void SoftDoublingLessThanTest()
        {
            PlayerState expectedState = PlayerState.DOUBLE_DOWN;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Club, Face.Four)
                    }
                },
                Count = new List<int>() { 170,100 }
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Three), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(state, expectedState);
        }
        [TestMethod]
        //(A,4) against 3 with a count of 2 //Should not double down if count > 1.9
        public void SoftDoublingGreaterThanTest()
        {
            PlayerState expectedState = PlayerState.DOUBLE_DOWN;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Club, Face.Four)
                    }
                },
                Count = new List<int>() { 200, 100 }
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Three), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreNotEqual(state, expectedState);
        }

        [TestMethod]
        public void HardDoublingLessThanTest()
        {
            PlayerState expectedState = PlayerState.DOUBLE_DOWN;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Three),
                        new Card(Suit.Club, Face.Six)
                    }
                },
                Count = new List<int>() { 150, 100 } //Count = 1.5
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Eight), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(state, expectedState);
        }
        [TestMethod]
        public void HardDoublingGreaterThanTest()
        {
            PlayerState expectedState = PlayerState.DOUBLE_DOWN;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Three),
                        new Card(Suit.Club, Face.Six)
                    }
                },
                Count = new List<int>() { 161, 100 } //Count = 1.61
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Eight), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreNotEqual(state, expectedState);
        }

        [TestMethod]
        public void HardDoublingEqualToTest()
        {
            PlayerState expectedState = PlayerState.DOUBLE_DOWN;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Three),
                        new Card(Suit.Club, Face.Six)
                    }
                },
                Count = new List<int>() { 160, 100 } //Count = 1.6
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Club, Face.Eight), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(state, expectedState);
        }

        [TestMethod]
        public void PairSplittingLessThanTest()
        {
            PlayerState expectedState = PlayerState.SPLIT;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Nine),
                        new Card(Suit.Diamond, Face.Nine)
                    }
                },
                Count = new List<int>() { 410, 100 } //Count = 1.6
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Nine), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(state, expectedState);
        }
        [TestMethod]
        public void PairSplittingGreaterThanTest()
        {
            PlayerState expectedState = PlayerState.SPLIT;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Nine),
                        new Card(Suit.Diamond, Face.Nine)
                    }
                },
                Count = new List<int>() { 430, 100 } //Count = 1.6
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Nine), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreNotEqual(state, expectedState);
        }

        [TestMethod]
        public void SoftStandingLessThanShadedTest()
        {
            PlayerState expectedState = PlayerState.STAND;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ten),
                        new Card(Suit.Diamond, Face.Seven)
                    }
                },
                Count = new List<int>() { 1, 100 } //Count = 0.01
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Two), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(state, expectedState);
        }
        [TestMethod]
        public void SoftStandingNonShadedTest()
        {
            PlayerState expectedState = PlayerState.HIT;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ten),
                        new Card(Suit.Diamond, Face.Six)
                    }
                },
                Count = new List<int>() { 4000, 100 } //Count = 40
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Seven), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(state, expectedState);
        }
        [TestMethod]
        public void SoftStandingLessThanTest()
        {
            PlayerState expectedState = PlayerState.STAND;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Diamond, Face.Seven)
                    }
                },
                Count = new List<int>() { 220, 100 } //Count = 2.2
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Ace), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(expectedState, state);
        }
        [TestMethod]
        public void SoftStandingGreaterThanTest()
        {
            PlayerState expectedState = PlayerState.HIT;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Diamond, Face.Seven)
                    }
                },
                Count = new List<int>() { 221, 100 } //Count = 2.21
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Ace), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(state, expectedState);
        }
        [TestMethod]
        public void Soft19AgainstAceGreaterThanTest()
        {
            PlayerState expectedState = PlayerState.STAND;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ace),
                        new Card(Suit.Diamond, Face.Eight)
                    }
                },
                Count = new List<int>() { 221, 100 } //Count = 2.21
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Ace), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(state, expectedState);
        }

        [TestMethod]
        public void HardStandingLessThanTest()
        {
            PlayerState expectedState = PlayerState.STAND;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ten),
                        new Card(Suit.Diamond, Face.Three)
                    }
                },
                Count = new List<int>() { 260, 100 } //Count = 2.6
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Four), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(state, expectedState);
        }
        [TestMethod]
        public void HardStandingGreaterThanTest()
        {
            PlayerState expectedState = PlayerState.HIT;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ten),
                        new Card(Suit.Diamond, Face.Three)
                    }
                },
                Count = new List<int>() { 500, 100 } //Count = 5
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Four), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(state, expectedState);
        }

        [TestMethod]
        public void HardStandingNonShadedTest()
        {
            PlayerState expectedState = PlayerState.HIT;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ten),
                        new Card(Suit.Diamond, Face.Two)
                    }
                },
                Count = new List<int>() { 4500, 100 } //Count = 45
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Nine), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(state, expectedState);
        }

        [TestMethod]
        public void HardStandingShadedTest()
        {
            PlayerState expectedState = PlayerState.STAND;
            TenCountStrategy player = new TenCountStrategy
            {
                Chips = 500,
                hand = new Hand
                {
                    cards = new List<Card>()
                    {
                        new Card(Suit.Club, Face.Ten),
                        new Card(Suit.Diamond, Face.Seven)
                    }
                },
                Count = new List<int>() { 1, 100 } //Count = 0.01
            };
            player.UpdateOthersOverTenRatio();
            player.hand.SetHandValues();
            PlayerState state = (player.React(dealersUpCard: new Card(Suit.Spade, Face.Three), ref player.CurrentState, player.hand, new List<int>()));
            Assert.AreEqual(state, expectedState);
        }
    }
}
