using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic
{
    public class Hand
    {
        public List<Card> cards = new List<Card>();
        public List<int> handValues = new List<int>();

        public Hand()
        {

        }

        //public List<int> GetHandValues()
        //{
        //    List<int> handValues = new List<int>();
        //    return handValues;
        //}
        public List<int> GetHandValues()
        {
            SetHandValues();
            return handValues;
        }


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
    }
}
