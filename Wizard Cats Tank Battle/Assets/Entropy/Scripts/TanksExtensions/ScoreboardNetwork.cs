using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Vashta.Entropy.TanksExtensions
{
    /// <summary>
    /// Extension methods for Photon.Realtime.Room
    /// </summary>
    public static class ScoreboardNetwork
    {
        private const string scoreboard = "scoreboardOffline";
        
        public static void AddOfflinePlayerToScoreboard(this Room room, Player player)
        {
            ScoreboardRowDataSerializableList offlinePlayerList = ReadScoreboard(room);
            
            ScoreboardRowData data = new ScoreboardRowData(player, null, false);
            offlinePlayerList.Add(data.OfflinePlayerSerializable());
            string offlinePlayerListJson = JsonUtility.ToJson(offlinePlayerList);
            
            Debug.Log("Encoding: " + offlinePlayerList);
            
            room.SetCustomProperties(new Hashtable() {{scoreboard, offlinePlayerListJson}});
        }

        public static ScoreboardRowDataSerializableList ReadScoreboard(this Room room)
        {
            string encodedPlayerList = (string)room.CustomProperties[scoreboard];

            Debug.Log("Encoded score: " + encodedPlayerList);
            
            if (encodedPlayerList == null)
                return new ScoreboardRowDataSerializableList();

            return JsonUtility.FromJson<ScoreboardRowDataSerializableList>(encodedPlayerList);
            
            // return (List<ScoreboardRowDataSerializable>)room.CustomProperties[scoreboard] ?? new List<ScoreboardRowDataSerializable>();
        }
    }
}