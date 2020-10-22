using BlackjackLogic.Game;
using System;
using System.Linq;

namespace BlackjackLogic
{
    public class BlackjackHandData
    {
        public int AmountOfCardsInDeckBeforeTurn { get; set; }
        public int PlayerStake { get; set; }
        public int PlayerStartingChips { get; set; }
        public int PlayersStartingHardHandValue { get; set; }
        public int PlayersStartingSoftHandValue { get; set; }
        public string PlayersDecisions { get; set; }
        public string PlayersSplitHandDecisions { get; set; }
        public string DealersDecisions { get; set; }
        public string DoesPlayerSplit { get; set; }
        public string PlayersStartingSplitHand { get; set; }
        public string PlayersStartingSplitHardHandValue { get; set; }
        public string PlayersStartingSplitSoftHandValue { get; set; }
        public string PlayersEndSplitHand { get; set; }
        public string PlayersEndSplitHandValue { get; set; }
        public string PlayersStartingHandPreSplit { get; set; }
        public string GameResult { get; set; }
        public string SplitGameResult { get; set; }
        public int FirstCountBeforeHand { get; set; }
        public int FirstCountAfterHand { get; set; }
        public int CountZeroAtTimeOfBet { get; set; }
        public int CountOneAtTimeOfBet { get; set; }
        public string CurrentTurnDeckHash { get; set; }

        public BlackjackHandData()
        {
        }

        public string GetHandDataAsString(Player player, int handsPlayed, Dealer dealer, Deck deck)
        {
            return  $"{handsPlayed + 1},{PlayerStartingChips},{player.Chips},{player.Chips - PlayerStartingChips},{GameResult},{SplitGameResult},{AmountOfCardsInDeckBeforeTurn},{deck.Cards.Count}," +
                    $"{PlayerStake},{player.hand.cards[0]} {player.hand.cards[1]}," +
                    $"{PlayersStartingHardHandValue},{PlayersStartingSoftHandValue},{player.hand},{player.hand.handValues.Last()},{PlayersDecisions}," +
                    $"{dealer.upCard},{dealer.hand.cards.First().Value},{dealer.hand.cards[0]} {dealer.hand.cards[1]},{dealer.hand},{dealer.hand.handValues.First()},{dealer.hand.handValues.Last()}," +
                    $"{dealer.hand.handValues.Last()},{DealersDecisions},{DoesPlayerSplit},{PlayersStartingSplitHand},{PlayersStartingHardHandValue},{PlayersStartingSplitSoftHandValue}," +
                    $"{PlayersEndSplitHand},{PlayersEndSplitHandValue},{PlayersSplitHandDecisions},{PlayersStartingHandPreSplit}," +
                    $"{FirstCountBeforeHand},{FirstCountAfterHand},{CountZeroAtTimeOfBet},{CountOneAtTimeOfBet},{CurrentTurnDeckHash}";
        }

        public void ClearOnNewTurn()
        {
            throw new NotImplementedException();
        }
    }
}
