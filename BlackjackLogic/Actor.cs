using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum PlayerState
{
    HIT,
    STAND,
    SPLIT,
    DOUBLE_DOWN,
    BUST,

}

namespace BlackjackLogic
{
    public abstract class Actor : IActor
    {
        public PlayerState CurrentState;
        public Hand hand = new Hand();
        public bool IsBust;


        public abstract void WriteCurrentState();
    }
}
