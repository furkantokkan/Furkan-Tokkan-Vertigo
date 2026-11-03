using Game.Boxes;
using Game.Collectable;

namespace Game.UI
{
    // UniRx MessageBroker routes messages by type, so each game event is its own type.
    // Publish(new GameOverMessage()) and Receive<GameOverMessage>() are checked by the compiler.
    // Messages carry data only; whoever shows them decides the text.
    // A server payload carries enum names as strings (ToString / Enum.TryParse), never their numbers.

    public readonly struct RewardGivenMessage
    {
        public readonly WheelSlot Slot;
        public readonly int CurrentWave;

        public RewardGivenMessage(WheelSlot slot, int currentWave)
        {
            Slot = slot;
            CurrentWave = currentWave;
        }
    }

    public readonly struct BombGivenMessage
    {
        public readonly WheelItem Item;
        public readonly int CurrentWave;

        public BombGivenMessage(WheelItem item, int currentWave)
        {
            Item = item;
            CurrentWave = currentWave;
        }
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
