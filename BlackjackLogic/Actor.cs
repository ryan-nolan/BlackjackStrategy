using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic
{
    public abstract class Actor : IActor
    {
        public List<Card> Hand = new List<Card>();
        public int HandValue { get; set; }

    }
}
