using TanksMP;
using UnityEngine;
using Player = Photon.Realtime.Player;
using System.Collections.Generic;
using Photon.Pun;

namespace Vashta.Entropy.TanksExtensions
{
    public class TeamState
    {
        public TeamState(Team team, int score, int teamId)
        {
            Team = team;
            Score = score;
            TeamId = teamId;
            PlayerNames = GetPlayers();
        }

        public string Name => Team.name;
        public Material Material => Team.material;
        public Transform Spawn => Team.spawn;
        public Team Team { get; }
        public int Score { get; }
        public int TeamId { get; }
        public List<string> PlayerNames { get; }
        
        private List<string> GetPlayers()
        {
            List<string> listOfPlayers = new List<string>();
            
            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
            foreach (KeyValuePair<int, Player> kvp in players)
            {
                Player player = kvp.Value;
                if(player.GetTeam() == TeamId)
                    listOfPlayers.Add(player.NickName);
            }

            return listOfPlayers;
        }
    }
}