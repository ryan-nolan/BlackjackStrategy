using BlackjackLogic.Game;
using System;
using System.Linq;

namespace BlackjackLogic
{
    public class BlackjackHandData
    {
        public int AmountOfCardsInDeckBeforeTurn { get; set; }
        public int PlayerStakeForFile { get; set; }
        public int PlayerStartingChips { get; set; }
        public int PlayersStartingHardHandValueForFile { get; set; }
        public int PlayersStartingSoftHandValueForFile { get; set; }
        public string PlayersDecisions { get; set; }
        public string PlayersSplitHandDecisions { get; set; }
        public string DealersDecisions { get; set; }
        public string DoesPlayerSplit { get; set; }
        public string PlayersStartingSplitHandForfile { get; set; }
        public string PlayersStartingSplitHardHandValueForFile { get; set; }
        public string PlayersStartingSplitSoftHandValueForFile { get; set; }
        public string PlayersEndSplitHand { get; set; }
        public string PlayersEndSplitHandValue { get; set; }
        public string PlayersStartingHandPreSplit { get; set; }
        public string GameResult { get; set; }
        public string SplitGameResult { get; set; }
        public int FirstCountBeforeHandForFile { get; set; }
        public int FirstCountAfterHandForFile { get; set; }
        public int CountZeroAtTimeOfBet { get; set; }
        public int CountOneAtTimeOfBet { get; set; }
        public string CurrentTurnDeckHash { get; set; }

        public BlackjackHandData()
        {
        }

        public string GetHandData(Player player, int handsPlayed, Dealer dealer, Deck deck)
        {
            return  $"{handsPlayed + 1},{PlayerStartingChips},{player.Chips},{player.Chips - PlayerStartingChips},{GameResult},{SplitGameResult},{AmountOfCardsInDeckBeforeTurn},{deck.Cards.Count}," +
                    $"{PlayerStakeForFile},{player.hand.cards[0]} {player.hand.cards[1]}," +
                    $"{PlayersStartingHardHandValueForFile},{PlayersStartingSoftHandValueForFile},{player.hand},{player.hand.handValues.Last()},{PlayersDecisions}," +
                    $"{dealer.upCard},{dealer.hand.cards.First().Value},{dealer.hand.cards[0]} {dealer.hand.cards[1]},{dealer.hand},{dealer.hand.handValues.First()},{dealer.hand.handValues.Last()}," +
                    $"{dealer.hand.handValues.Last()},{DealersDecisions},{DoesPlayerSplit},{PlayersStartingSplitHandForfile},{PlayersStartingHardHandValueForFile},{PlayersStartingSplitSoftHandValueForFile}," +
                    $"{PlayersEndSplitHand},{PlayersEndSplitHandValue},{PlayersSplitHandDecisions},{PlayersStartingHandPreSplit}," +
                    $"{FirstCountBeforeHandForFile},{FirstCountAfterHandForFile},{CountZeroAtTimeOfBet},{CountOneAtTimeOfBet},{CurrentTurnDeckHash}";
            //f.WriteLine($"{handsPlayed + 1},{PlayerStartingChips},{player.Chips},{player.Chips - PlayerStartingChips},{GameResult},{SplitGameResult},{AmountOfCardsInDeckBeforeTurn},{deck.Cards.Count}," +
            //        $"{PlayerStakeForFile},{player.hand.cards[0]} {player.hand.cards[1]}," +
            //        $"{PlayersStartingHardHandValueForFile},{PlayersStartingSoftHandValueForFile},{player.hand},{player.hand.handValues.Last()},{PlayersDecisions}," +
            //        $"{dealer.upCard},{dealer.hand.cards.First().Value},{dealer.hand.cards[0]} {dealer.hand.cards[1]},{dealer.hand},{dealer.hand.handValues.First()},{dealer.hand.handValues.Last()}," +
            //        $"{dealer.hand.handValues.Last()},{DealersDecisions},{DoesPlayerSplit},{PlayersStartingSplitHandForfile},{PlayersStartingHardHandValueForFile},{PlayersStartingSplitSoftHandValueForFile}," +
            //        $"{PlayersEndSplitHand},{PlayersEndSplitHandValue},{PlayersSplitHandDecisions},{PlayersStartingHandPreSplit}," +
            //        $"{FirstCountBeforeHandForFile},{FirstCountAfterHandForFile},{CountZeroAtTimeOfBet},{CountOneAtTimeOfBet},{CurrentTurnDeckHash}"
            //        );
            //f.WriteLine(stringToWriteToFile);
            //return stringToWriteToFile;
        }

        public void ClearOnNewTurn()
        {
            throw new NotImplementedException();
        }
    }
}
