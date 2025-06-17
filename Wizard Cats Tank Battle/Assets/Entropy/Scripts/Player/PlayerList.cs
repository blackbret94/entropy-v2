using System.Collections.Generic;
using Fusion;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Player;

namespace Entropy.Scripts.Player
{
    public class PlayerList
    {
        private static List<PlayerController> _players = new ();
        public static List<PlayerController> GetAllPlayers => _players;
        
        private static Dictionary<short, PlayerController> _playersDict = new ();

        public static void Add(PlayerController playerController)
        {
            _playersDict.Add(playerController.NetID, playerController);
            _players.Add(playerController);
        }

        public static short GetValidId()
        {
            int maxTries = 20;

            do
            {
                int netId = Random.Range(0, short.MaxValue);
                if (!_playersDict.ContainsKey((short)netId))
                    return (short)netId;
                
            } while (maxTries-- > 0);

            Debug.LogError("Could not generate valid NetId for player!");
            return 0;
        }

        public static void Sort()
        {
            _playersDict.Clear();
            foreach(PlayerController player in _players)
            {
                _playersDict.Add(player.NetID, player);
            }
        }
        
        public static PlayerController GetPlayer(short netId)
        {
            if (netId == -1)
                return null;
            
            if (!_playersDict.ContainsKey(netId))
                return null;
            
            return _playersDict[netId];
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