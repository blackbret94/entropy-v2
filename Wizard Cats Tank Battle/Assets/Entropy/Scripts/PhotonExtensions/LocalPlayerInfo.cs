namespace Vashta.Entropy.PhotonExtensions
{
    public struct LocalPlayerInfo
    {
        public string Name;
        public int GameMode;
        public string MapName;

        public LocalPlayerInfo(string name, int gameMode = (int)TanksMP.GameMode.RAND, string mapName = "random")
        {
            Name = name;
            GameMode = gameMode;
            MapName = mapName;
        }
        
        public TanksMP.GameMode GameModeEnum => (TanksMP.GameMode)GameMode;
    }
}