using Game.Boxes;
using Game.Collectable;
using UniRx;

namespace Game.UI.Wheel
{
    // One run of the wheel: the current wave, which slot a spin lands on and what that result means.
    // Plain C# so the rules can be tested without a scene; WheelManager only adapts it to Unity.
    public class WheelSession
    {
        private readonly Box box;
        private readonly IMessagePublisher publisher;

        public int CurrentWave { get; private set; }

        public BoxContent CurrentContent => box.contents[CurrentWave];

        public WheelSession(Box box, IMessagePublisher publisher)
        {
            this.box = box;
            this.publisher = publisher;
        }

        public WheelSlot PickSlot()
        {
            var slot = box.GetReward(CurrentWave);
            return slot != null && slot.item != null ? slot : null;
        }

        public void CompleteSpin(WheelSlot slot)
        {
            if (slot.item.ItemType == ItemType.Bomb)
                publisher.Publish(new BombGivenMessage(slot.item, CurrentWave));
            else
                publisher.Publish(new RewardGivenMessage(slot, CurrentWave));

            CurrentWave++;
            publisher.Publish(new WaveTypeChangedMessage(CurrentContent.WaveType));
        }

        public void Reset()
        {
            CurrentWave = 0;
            publisher.Publish(new WaveTypeChangedMessage(CurrentContent.WaveType));
        }
    }
}
