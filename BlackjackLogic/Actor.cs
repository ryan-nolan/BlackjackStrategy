using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// States an actor can hold
/// All possible blackjack states
/// </summary>
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
        /// <summary>
        /// Actor has a state hand and isBust bool
        /// </summary>
        public PlayerState CurrentState;
        public Hand hand = new Hand();
        public bool IsBust;


        public abstract void WriteCurrentState();
    }
}
