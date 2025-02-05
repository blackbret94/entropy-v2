using System.Collections.Generic;
using TanksMP;
using Vashta.Entropy.Player;

namespace Entropy.Scripts.Player
{
    public class PlayerList
    {
        private static List<PlayerController> _players = new ();
        public static List<PlayerController> GetAllPlayers => _players;

        public static void Add(PlayerController playerController)
        {
            _players.Add(playerController);
        }

        public static void Remove(PlayerController playerController)
        {
            _players.Remove(playerController);
        }

        public static PlayerController GetLocalPlayer()
        {
            PlayerController localPlayer = GameManager.GetInstance().localPlayerController;
            
            if(localPlayer)
               return localPlayer;
            
            if(GameManager.GetInstance().localPlayerController)
            
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