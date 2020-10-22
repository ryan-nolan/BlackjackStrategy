using System;

namespace BlackjackLogic
{
    public class SimulatorGameOptions
    {
        public int HandsToBePlayed { get; set; } = 50000;             //The amount of hands wanted to be played
        public int CardCountWhenToShuffle { get; set; } = 13;         //Shuffle when val < this numner
        public bool HumanPlayer { get; set; } = false;                //True if game is played by human
        public int MinBet { get; set; } = 2;                          //Min bet that can be placed
        public int MaxBet { get; set; } = 50;                         //Max bet that can be placed
        public string StrategyName { get; set; } = "simplepointcount";//The strategy name for documnetation
        public int StartChips { get; set; } = 500;                    //Chips player should start on
        public int DeckSize { get; set; } = 52;                       //Size of deck, must be multiple of 52
        public string FilePath { get; set; } = @".\Data\";

        public SimulatorGameOptions()
        {

        }
        public SimulatorGameOptions(int handsToBePlayed, int cardCountWhenToShuffle, bool humanPlayer, int minBet, int maxBet, string strategyName, int startChips, int deckSize, string path)
        {
            HandsToBePlayed = handsToBePlayed;
            CardCountWhenToShuffle = cardCountWhenToShuffle;
            HumanPlayer = humanPlayer;
            MinBet = minBet;
            MaxBet = maxBet;
            StrategyName = strategyName;
            StartChips = startChips;
            DeckSize = deckSize;
            FilePath = path;
        }
        public SimulatorGameOptions(int handsToBePlayed, int cardCountWhenToShuffle, int minBet, int maxBet, string strategyName, int startChips, int deckSize)
        {
            HandsToBePlayed = handsToBePlayed;
            CardCountWhenToShuffle = cardCountWhenToShuffle;
            MinBet = minBet;
            MaxBet = maxBet;
            StrategyName = (String.IsNullOrEmpty(strategyName)) ? strategyName : "StringWasNull";
            StartChips = startChips;
            DeckSize = deckSize;
        }

        public override string ToString()
        {
            return $"Hands To Be Played:\t{HandsToBePlayed}\n" +
                $"Card Count When To Shuffle:\t{CardCountWhenToShuffle}\n" +
                $"Min Bet:\t{MinBet}\n" +
                $"Max Bet:\t{MaxBet}\n" +
                $"Start Chips {StartChips}\n" +
                $"Strategy Name:\t{StrategyName}\n" +
                $"Deck Size:\t{DeckSize}\n";
        }
    }

}
