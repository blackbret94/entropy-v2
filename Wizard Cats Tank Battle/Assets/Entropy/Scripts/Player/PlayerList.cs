using System.Collections.Generic;

namespace Entropy.Scripts.Player
{
    public class PlayerList
    {
        private static List<TanksMP.Player> _players = new ();

        public static List<TanksMP.Player> GetAllPlayers => _players;

        public static void Add(TanksMP.Player player)
        {
            _players.Add(player);
        }

        public static void Remove(TanksMP.Player player)
        {
            _players.Remove(player);
        }

        public static TanksMP.Player GetLocalPlayer()
        {
            foreach (var player in _players)
            {
                if (player != null && player.IsLocal && !player.isBot)
                {
                    return player;
                }
            }

            return null;
        }
    }
}