using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackjackLogic;

namespace BlackjackStrategy
{
    class Program
    {
        static void Main(string[] args)
        {
            //Parse args
            //init game loop and pass parsed args

            int handsToBePlayed = 100000;
            int cardsBeforeShuffling = 13;
            int maxBet = 50;
            int minBet = 2;
            int startChips = 500;
            string strategyName = "acetofive";
            int deckSize = 52;

            for (int i = 0; i<args.Length;i++)
            {
                switch (args[i])
                {
                    case "--h":
                    case "-hands":
                        try
                        {
                            handsToBePlayed = int.Parse(args[i + 1]);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw e;
                        }
                        break;
                    case "--s":
                    case "-shuffleFreq":
                        try
                        {
                            cardsBeforeShuffling = int.Parse(args[i + 1]);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw e;
                        }
                        break;
                    case "--min":
                    case "-MinBet":
                        try
                        {
                            minBet = int.Parse(args[i + 1]);
                        }
                        catch (Exception e)
                        {

                            throw e;
                        }
                        break;
                    case "--max":
                    case "-MaxBet":
                        try
                        {
                            maxBet = int.Parse(args[i + 1]);
                        }
                        catch (Exception e)
                        {

                            throw e;
                        }
                        break;
                    case "--strat":
                    case "-StrategyName":
                        try
                        {
                            strategyName = args[i + 1].ToLower();
                        }
                        catch (Exception e)
                        {

                            throw e;
                        }
                        break;
                    case "--c":
                    case "-StartChips":
                        try
                        {
                            startChips = int.Parse(args[i + 1]);
                        }
                        catch (Exception e)
                        {

                            throw e;
                        }
                        break;
                    case "--ds":
                    case "-DeckSize":
                        try
                        {
                            deckSize = int.Parse(args[i + 1]);
                        }
                        catch (Exception e)
                        {

                            throw e;
                        }
                        break;
                    case "--sh":
                    case "-startingHand":
                        try
                        {
                            //
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        break;

                    default:
                        break;
                }
            }
            //Game _game = new Game(turnsToBePlayed, cardsBeforeShuffling);

            Game _game = new Game
            {
                HandsToBePlayed = handsToBePlayed,
                CardCountWhenToShuffle = cardsBeforeShuffling,
                MinBet = minBet,
                MaxBet = maxBet,
                StrategyName = strategyName,
                StartChips = startChips,
                DeckSize = deckSize
        };
            _game.RunGame();
        }
    }
}
