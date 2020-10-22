using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlackjackLogic.Game;

namespace CoreTest
{
    [TestClass]
    public class CardTest
    {
        [TestMethod]
        public void CloneCardTest()
        {
            Card testCard = new Card(Suit.Club, Face.Ace);
            Card compareCard = testCard.Clone();
            bool state = (testCard.Face == compareCard.Face && testCard.Suit == compareCard.Suit && testCard.Value == compareCard.Value);
            Assert.AreEqual(true, state);
        }

        [TestMethod]
        public void FaceCardValueTest()
        {
            int expectedState = 10;
            Card king = new Card(Suit.Club, Face.King);
            Card queen = new Card(Suit.Club, Face.King);
            Card jack = new Card(Suit.Club, Face.King);

            int state = king.Value;
            Assert.AreEqual(expectedState, state);
            state = queen.Value;
            Assert.AreEqual(expectedState, state);
            state = jack.Value;
            Assert.AreEqual(expectedState, state);
        }

        [TestMethod]
        public void AceValueTest()
        {
            int expectedState = 11;
            Card ace = new Card(Suit.Club, Face.Ace);
            int state = ace.Value;
            Assert.AreEqual(expectedState, state);

        }
    }
}
