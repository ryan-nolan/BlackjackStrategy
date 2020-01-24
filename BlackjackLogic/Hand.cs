using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic
{
    public class Hand
    {
        public List<Card> hand = new List<Card>();
        public List<int> handValues;

        public Hand()
        {

        }

        public List<int> GetHandValues()
        {
            List<int> handValues = new List<int>();
            return handValues;
        }
    }
}
