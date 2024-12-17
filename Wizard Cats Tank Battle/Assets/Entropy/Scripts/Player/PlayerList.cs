using System.Collections.Generic;
using System.Linq;

namespace Entropy.Scripts.Player
{
    public class PlayerList
    {
        private static Dictionary<int, TanksMP.Player> _playersByViewId = new ();
        
        public static List<TanksMP.Player> GetAllPlayers => _playersByViewId.Values.ToList();

        public static void Add(int viewId, TanksMP.Player player)
        {
            _playersByViewId[viewId] = player;
        }

        public static void Remove(int viewId)
        {
            _playersByViewId.Remove(viewId);
        }
        
        /// <summary>
        /// Attempt to get a player from an ID.  Returns null if player does not exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static TanksMP.Player GetPlayerById(int id)
        {
            if (_playersByViewId.ContainsKey(id))
                return _playersByViewId[id];

            return null;
        }

        public static TanksMP.Player GetLocalPlayer()
        {
            foreach (KeyValuePair<int, TanksMP.Player> keyValuePair in _playersByViewId)
            {
                TanksMP.Player kvpPlayer = keyValuePair.Value;

                if (kvpPlayer != null && kvpPlayer.IsLocal && !kvpPlayer.isBot)
                    return kvpPlayer;
            }

            return null;
        }

        public static void Clear()
        {
            _playersByViewId.Clear();
        }
    }
}