namespace Game.UI
{
    // UniRx MessageBroker routes messages by type, so each game event is its own type.
    // Publish(new GameOverMessage()) and Receive<GameOverMessage>() are checked by the compiler.
    // A server payload carries enum names as strings (ToString / Enum.TryParse), never their numbers.

    public enum WaveType
    {
        Normal,
        Safe,
        Super
    }

    public readonly struct WaveTypeChangedMessage
    {
        public readonly WaveType WaveType;

        public WaveTypeChangedMessage(WaveType waveType)
        {
            WaveType = waveType;
        }
    }

    public readonly struct ProgressFirstStopMessage { }

    public readonly struct RewardPopupClosedMessage { }

    public readonly struct BombPopupClosedMessage { }

    public readonly struct PlayerRevivedMessage { }

    public readonly struct GameOverMessage { }

    public readonly struct ExitButtonClickedMessage { }

    public readonly struct ExitConfirmedMessage { }
}
