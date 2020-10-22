using BlackjackStrategy.Core.Game;

namespace BlackjackStrategy.Core
{
    public abstract class Actor : IActor
    {
        /// <summary>
        /// Actor has a state hand and isBust bool
        /// </summary>
        public PlayerState CurrentState;
        public Hand hand = new Hand();
        protected bool IsBust;

        public abstract void WriteCurrentState();
    }
}
