using Fusion;

namespace Entropy.Scripts.Player
{
    public struct PlayerDeathStruct : INetworkStruct
    {
        public PlayerDeathStruct(short killedByPlayer, ushort visualEffectId, float timeOfDeath)
        {
            this.killedByPlayer = killedByPlayer;
            this.visualEffectId = visualEffectId;
            this.timeOfDeath = timeOfDeath;
            this.expired = false;
        }
        
        public short killedByPlayer { get; set; }
        public ushort visualEffectId { get; set; }
        public float timeOfDeath { get; set; }
        public bool expired { get; set; }
    }
}