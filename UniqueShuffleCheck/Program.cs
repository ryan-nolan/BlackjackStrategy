using BlackjackLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniqueShuffleCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            Deck deck = new Deck(52);
            deck.TrueShuffle();
            List<string> shuffleHashes = new List<string>();
            while(true)
            {
                deck.TrueShuffle();
                string currentHash = deck.GetDeckHash();
                var duplicate = shuffleHashes
                    .FirstOrDefault(stringCheck => stringCheck.Contains(currentHash));
                if (duplicate == null)
                {
                    shuffleHashes.Add(currentHash);
                    Console.WriteLine($"Unique Hashes: {shuffleHashes.Count}");
                }
                else
                {
                    break;
                }
            }
        }
    }
}
