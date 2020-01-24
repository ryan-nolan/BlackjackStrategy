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

        //public List<int> GetHandValue()
        //{
        //    List<int> returnVal = new List<int>();
        //    int noAce = 0;
        //    int anAceMin = 0;
        //    int anAceMax = 0;

        //    foreach (var c in Hand)
        //    {
        //        if (c.Face != Face.Ace && c.Face != Face.Jack && c.Face != Face.Queen && c.Face != Face.King)
        //        {
        //            noAce += c.Value;
        //            anAceMin += c.Value;
        //            anAceMax += c.Value;
        //        }
        //        else if (c.Face == Face.Jack || c.Face == Face.Queen || c.Face == Face.King)
        //        {
        //            noAce += 10;
        //            anAceMin += 10;
        //            anAceMax += 10;
        //        }
        //        else if (c.Face == Face.Ace)
        //        {
        //            noAce += 1;
        //            anAceMin += 1;
        //            anAceMax += 11;
        //        }

        //    }

        //    returnVal.Add(noAce);
        //    returnVal.Add(anAceMin);
        //    returnVal.Add(anAceMax);


        //    return returnVal;
        //}
    }
}
