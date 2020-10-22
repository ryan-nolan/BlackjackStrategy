using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackStrategy.Core
{
    public class BlackjackData
    {
        public List<BlackjackHandData> blackjackHandDataCollection;
        public double WinRate => CalculateWinRate();
        public BlackjackData()
        {
            blackjackHandDataCollection = new List<BlackjackHandData>();
        }

        private double CalculateWinRate()
        {
            int wins = 0, losses = 0;
            foreach (var data in blackjackHandDataCollection)
            {
                switch (data.GameResult)
                {
                    case "W":
                    case "W_N":
                        wins++;
                        break;
                    case "L":
                    case "L_N":
                        losses++;
                        break;
                }
            }

            return ((double) wins) / ((double) (wins + losses)) * 100;
        }
    }
}
