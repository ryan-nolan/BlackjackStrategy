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

            int turnsToBePlayed = 200;
            int cardsBeforeShuffling = 26;

            for (int i = 0; i<args.Length;i++)
            {
                switch (args[i])
                {
                    case "--t":
                    case "-hands":
                        turnsToBePlayed = int.Parse(args[i + 1]);
                        break;
                    case "--s":
                    case "-shuffle":
                        cardsBeforeShuffling = int.Parse(args[i + 1]);
                        break;

                    default:
                        break;
                }
            }
            //Game _game = new Game(turnsToBePlayed, cardsBeforeShuffling);

            Game _game = new Game();
        }
    }
}
