namespace Vashta.Entropy.TanksExtensions
{
    public class PlayerScore
    {
        public PlayerScore(string name, int teamIndex, int kills, int deaths)
        {
            Name = name;
            TeamIndex = teamIndex;
            Kills = kills;
            Deaths = deaths;
        }

        public string Name { get; }
        public int TeamIndex { get; }
        public int Kills { get; }
        public int Deaths { get; }
    }
}