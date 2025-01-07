namespace Vashta.Entropy.PhotonExtensions
{
    public struct LocalPlayerInfo
    {
        public string Name;
        public int GameMode;
        public string MapName;
        
        public TanksMP.GameMode GameModeEnum => (TanksMP.GameMode)GameMode;
    }
}