using Fusion;

namespace Vashta.Entropy.GameState
{
    // Network struct storing important match information
    public struct MatchInfo : INetworkStruct
    {
        public TanksMP.GameMode GameMode;
        public int MaxNumberOfPlayers;
        public NetworkBool BotFilling;
        public NetworkString<_32> MapName;
        public int MaxScoreToWin;
        public float MaxTime;
    }
}