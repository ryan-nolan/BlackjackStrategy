using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackjackLogic;

namespace BlackjackStrategy.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Parse args
            //init game loop and pass parsed args

            int handsToBePlayed = 1000;
            int cardsBeforeShuffling = 13;
            int maxBet = 100;
            int minBet = 10;
            int startChips = 10000;
            string strategyName = "knockoutcount";
            int deckSize = 52;
            string path = @".\Data\";

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
                            int numOfDecks = int.Parse(args[i + 1]);
                            deckSize = 52*numOfDecks;
                        }
                        catch (Exception e)
                        {

                            throw e;
                        }
                        break;


                    default:
                        break;
                }
            }
            //Game _game = new Game(turnsToBePlayed, cardsBeforeShuffling);
            SimulatorGameOptions options = new SimulatorGameOptions
            {
                HandsToBePlayed = handsToBePlayed,
                CardCountWhenToShuffle = cardsBeforeShuffling,
                MinBet = minBet,
                MaxBet = maxBet,
                StrategyName = strategyName,
                StartChips = startChips,
                DeckSize = deckSize,
                FilePath = path
            };
            Simulator sim = new Simulator(options);
            sim.RunGame();
        }
    }
}
