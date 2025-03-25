using Fusion;

namespace Entropy.Scripts.Player
{
    public struct PlayerDeathStruct : INetworkStruct
    {
        public PlayerDeathStruct(PlayerRef killedByPlayer, ushort visualEffectId, float timeOfDeath)
        {
            this.killedByPlayer = killedByPlayer;
            this.visualEffectId = visualEffectId;
            this.timeOfDeath = timeOfDeath;
        }
        
        public PlayerRef killedByPlayer { get; set; }
        public ushort visualEffectId { get; set; }
        public float timeOfDeath { get; set; }
    }
}