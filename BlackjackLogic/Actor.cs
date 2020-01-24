using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic
{
    public abstract class Actor : IActor
    {
        public Hand hand = new Hand();
        public bool IsBust;
        public int HandValue { get; set; }

    }
}
