﻿namespace BlackjackStrategy.Core
{
    /// <summary>
    /// States an actor can hold
    /// All possible blackjack states
    /// </summary>
    public enum PlayerState
    {
        Hit,
        Stand,
        Split,
        DoubleDown,
        Bust,
    }
}
