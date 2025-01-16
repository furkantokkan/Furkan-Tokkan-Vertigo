namespace Game.Utilities
{
    public static class GameConst
    {
        public const int PROGRESS_BAR_POOL_CAPACITY = 13;

        public static class Events
        {
            public const string PROGRESS_FIRST_STOP = "FirstStopPointEvent";
            public const string WAVE_SUPER = "SuperWaveEvent";
            public const string WAVE_SAFE = "SafeWaveEvent";
            public const string WAVE_NORMAL = "NormalWaveEvent";
            public const string WAVE_FINISH = "WaveFinishEvent";
            public const string REWARD_POPUP_CLOSED = "RewardPopupClosedEvent";
            public const string BOMB_POPUP_CLOSED = "BombPopupClosedEvent";
            public const string PLAYER_REVIVED = "PlayerRevivedEvent";
            public const string GAME_OVER = "GameOverEvent";
        }
    }
}