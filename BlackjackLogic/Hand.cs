using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic
{
    public class Hand
    {
        //Hand contains a list of cards and values for the lists of cards
        public List<Card> cards = new List<Card>();
        public List<int> handValues = new List<int>();
        /// <summary>
        /// Constructor makes a blank hand
        /// </summary>
        public Hand()
        {

        }
        /// <summary>
        /// Returns hand values
        /// </summary>
        /// <returns>Hand Values as List Int</returns>
        public List<int> GetHandValues()
        {
            SetHandValues();
            return handValues;
        }

        /// <summary>
        /// Sets hand value to values of the cards in hand
        /// </summary>
        public void SetHandValues()
        {
            if(handValues.Count != 0) handValues.Clear();
            List<int> returnHandValues = new List<int>();
            int noAce = 0;
            int anAceMin = 0;
            int anAceMax = 0;
            bool hasAce = false;

            foreach (var c in cards)
            {
                if(c.Face == Face.Ace)
                {
                    hasAce = true;
                }
            }
            if (!hasAce)
            {
                foreach (var c in cards)
                {
                    if (c.Face != Face.Ace && c.Face != Face.Jack && c.Face != Face.Queen && c.Face != Face.King)
                    {
                        noAce += c.Value;
                    }
                    else if (c.Face == Face.Jack || c.Face == Face.Queen || c.Face == Face.King)
                    {
                        noAce += 10;
                    }

                }
                returnHandValues.Add(noAce);
            }
            else
            {
                foreach (var c in cards)
                {
                    if (c.Face != Face.Ace && c.Face != Face.Jack && c.Face != Face.Queen && c.Face != Face.King)
                    {
                        anAceMin += c.Value;
                        anAceMax += c.Value;
                    }
                    else if (c.Face == Face.Jack || c.Face == Face.Queen || c.Face == Face.King)
                    {
                        anAceMin += 10;
                        anAceMax += 10;
                    }
                    else if (c.Face == Face.Ace)
                    {
                        anAceMin += 1;
                        anAceMax += 11;
                    }
                }
                returnHandValues.Add(anAceMin);
                if (anAceMax <= 21) returnHandValues.Add(anAceMax);
            }
            handValues = returnHandValues;
        }
        /// <summary>
        /// Converts a hand into a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string returnStr = "";
            foreach (var c in cards)
            {
                returnStr += $"{c.ToString()} ";
            }
            return returnStr;
        }
        /// <summary>
        /// Converts hand into a string but includes values
        /// </summary>
        /// <returns></returns>
        public string HandValuesAsString()
        {
            SetHandValues();
            string returnStr = "";
            if (handValues.Count == 1)
            {
                returnStr += handValues.First().ToString();
            }
            else if (handValues.Count > 1)
            {
                foreach (var handVal in handValues)
                {
                    returnStr += $"{handVal.ToString()}, ";
                }
            }
            return returnStr;
        }
        /// <summary>
        /// Writes hand values to console
        /// </summary>
        public void WriteHandAndHandValue()
        {
            Console.Write(ToString());
            Console.WriteLine($"\t{HandValuesAsString()}");
        }
    }
}
