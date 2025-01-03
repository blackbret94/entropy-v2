﻿/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using Photon.Pun;
using UnityEngine;
using Vashta.Entropy.Character;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace TanksMP
{
    /// <summary>
    /// This class extends Photon's PhotonPlayer object by custom properties.
    /// Provides several methods for setting and getting variables out of them.
    /// </summary>
    public static class PlayerExtensions
    {
        //keys for saving and accessing values in custom properties Hashtable
        public const string team = "team";
        public const string health = "health";
        public const string shield = "shield";
        public const string ammo = "ammo";
        public const string bullet = "bullet";
        public const string kills = "kills";
        public const string deaths = "deaths";
        public const string joinTime = "joinTime";
        public const string classId = "classId";
        public const string classIdQueued = "classIdQueued";
        public const string preferredTeam = "preferredTeam"; // for changing teams
        public const string isAlive = "isAlive";
        public const string ultimate = "ultimate";
        public const string powerup = "powerup";

        public const int RANDOM_TEAM_INDEX = 100;

        /// <summary>
        /// Returns the networked player nick name.
        /// Offline: bot name. Online: PhotonPlayer name.
        /// </summary>
        public static string GetName(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.myName;
                }
            }

            return player.Owner.NickName;
        }

        /// <summary>
        /// Offline: returns the team number of a bot stored in PlayerBot.
        /// Fallback to online mode for the master or in case offline mode was turned off.
        /// </summary>
        public static int GetTeam(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.teamIndex;
                }
            }

            return player.Owner.GetTeam();
        }

        /// <summary>
        /// Online: returns the networked team number of the player out of properties.
        /// </summary>
        public static int GetTeam(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[team]);
        }

        /// <summary>
        /// Offline: synchronizes the team number of a PlayerBot locally.
        /// Fallback to online mode for the master or in case offline mode was turned off.
        /// </summary>
        public static void SetTeam(this PhotonView player, int teamIndex)
        {
            // Debug.Log("Setting team index: " + teamIndex);
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    bot.teamIndex = teamIndex;
                    return;
                }
            }

            player.Owner.SetTeam(teamIndex);
            player.Owner.SetPreferredTeamIndex(teamIndex);
        }

        /// <summary>
        /// Online: synchronizes the team number of the player for all players via properties.
        /// </summary>
        public static void SetTeam(this Photon.Realtime.Player player, int teamIndex)
        {
            player.SetCustomProperties(new Hashtable() { { team, (byte)teamIndex } });
        }

        /// <summary>
        /// Offline: returns the health value of a bot stored in PlayerBot.
        /// Fallback to online mode for the master or in case offline mode was turned off.
        /// </summary>
        public static int GetHealth(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.health;
                }
            }

            return player.Owner.GetHealth();
        }

        /// <summary>
        /// Online: returns the networked health value of the player out of properties.
        /// </summary>
        public static int GetHealth(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[health]);
        }
        
        /// <summary>
        /// Offline: synchronizes the team number of a PlayerBot locally.
        /// Fallback to online mode for the master or in case offline mode was turned off.
        /// </summary>
        public static void SetIsAlive(this PhotonView player, bool bIsAlive)
        {
            // Debug.Log("Setting team index: " + teamIndex);
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    // Bots have isAlive
                    return;
                }
            }

            player.Owner.SetIsAlive(bIsAlive);
        }

        /// <summary>
        /// Online: synchronizes the team number of the player for all players via properties.
        /// </summary>
        public static void SetIsAlive(this Photon.Realtime.Player player, bool bIsAlive)
        {
            player.SetCustomProperties(new Hashtable() { { isAlive, bIsAlive } });
        }

        /// <summary>
        /// Offline: returns the health value of a bot stored in PlayerBot.
        /// Fallback to online mode for the master or in case offline mode was turned off.
        /// </summary>
        public static bool GetIsAlive(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.IsAlive;
                }
            }

            return player.Owner.GetIsAlive();
        }

        /// <summary>
        /// Online: returns the networked health value of the player out of properties.
        /// </summary>
        public static bool GetIsAlive(this Photon.Realtime.Player player)
        {
            return System.Convert.ToBoolean(player.CustomProperties[isAlive]);
        }

        /// <summary>
        /// Offline: synchronizes the health value of a PlayerBot locally.
        /// Fallback to online mode for the master or in case offline mode was turned off.
        /// </summary>
        public static void SetHealth(this PhotonView player, int value)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    bot.health = value;
                    return;
                }
            }

            player.Owner.SetHealth(value);
        }

        /// <summary>
        /// Online: synchronizes the health value of the player for all players via properties.
        /// </summary>
        public static void SetHealth(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { health, (byte)value } });
        }

        /// <summary>
        /// Offline: returns the shield value of a bot stored in PlayerBot.
        /// Fallback to online mode for the master or in case offline mode was turned off.
        /// </summary>
        public static int GetShield(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.shield;
                }
            }

            return player.Owner.GetShield();
        }

        /// <summary>
        /// Online: returns the networked shield value of the player out of properties.
        /// </summary>
        public static int GetShield(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[shield]);
        }

        /// <summary>
        /// Offline: synchronizes the shield value of a PlayerBot locally.
        /// Fallback to online mode for the master or in case offline mode was turned off.
        /// </summary>
        public static void SetShield(this PhotonView player, int value)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    bot.shield = value;
                    return;
                }
            }

            player.Owner.SetShield(value);
        }

        /// <summary>
        /// Online: synchronizes the shield value of the player for all players via properties.
        /// </summary>
        public static void SetShield(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { shield, (byte)value } });
        }

        /// <summary>
        /// Decreases the networked shield value of the player or bot by the amount passed in.
        /// </summary>
        public static int DecreaseShield(this PhotonView player, int value)
        {
            int newShield = player.GetShield();
            newShield -= value;

            player.SetShield(newShield);
            return newShield;
        }

        /// <summary>
        /// Offline: returns the ammo value of a bot stored in PlayerBot.
        /// Fallback to online mode for the master or in case offline mode was turned off.
        /// </summary>
        public static int GetAmmo(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.ammo;
                }
            }

            return player.Owner.GetAmmo();
        }

        /// <summary>
        /// Online: returns the networked ammo value of the player out of properties.
        /// </summary>
        public static int GetAmmo(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[ammo]);
        }

        /// <summary>
        /// Offline: synchronizes the ammo count of a PlayerBot locally.
        /// Provides an optional index parameter for setting a new bullet and ammo together.
        /// Fallback to online mode for the master or in case offline mode was turned off.
        /// </summary>
        public static void SetAmmo(this PhotonView player, int value, int index = -1)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    bot.ammo = value;
                    if (index >= 0)
                        bot.currentBullet = index;
                    return;
                }
            }

            player.Owner.SetAmmo(value, index);
        }

        /// <summary>
        /// Online: synchronizes the ammo count of the player for all players via properties.
        /// Provides an optional index parameter for setting a new bullet and ammo together.
        /// </summary>
        public static void SetAmmo(this Photon.Realtime.Player player, int value, int index = -1)
        {
            Hashtable hash = new Hashtable();
            hash.Add(ammo, (byte)value);
            if (index >= 0)
                hash.Add(bullet, (byte)index);

            player.SetCustomProperties(hash);
        }

        /// <summary>
        /// Decreases the networked ammo value of the player or bot by the amount passed in.
        /// If the player runs out of ammo, the bullet index is set to the default automatically.
        /// </summary>
        public static int DecreaseAmmo(this PhotonView player, int value)
        {
            int newAmmo = player.GetAmmo();
            newAmmo -= value;

            if (newAmmo <= 0)
                player.SetAmmo(newAmmo, 0);
            else
                player.SetAmmo(newAmmo);

            return newAmmo;
        }

        /// <summary>
        /// Offline: returns the bullet index of a bot stored in PlayerBot.
        /// Fallback to online mode for the master or in case offline mode was turned off.
        /// </summary>
        public static int GetBullet(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.currentBullet;
                }
            }

            return player.Owner.GetBullet();
        }

        /// <summary>
        /// Online: returns the networked bullet index of the player out of properties.
        /// </summary>
        public static int GetBullet(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[bullet]);
        }

        /// <summary>
        /// Offline: synchronizes the currently selected bullet of a PlayerBot locally.
        /// Fallback to online mode for the master or in case offline mode was turned off.
        /// </summary>
        public static void SetBullet(this PhotonView player, int value)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    bot.currentBullet = value;
                    return;
                }
            }

            player.Owner.SetBullet(value);
        }

        /// <summary>
        /// Online: Synchronizes the currently selected bullet of the player for all players via properties.
        /// </summary>
        public static void SetBullet(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { bullet, (byte)value } });
        }

        public static int GetKills(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.kills;
                }
            }

            return player.Owner.GetKills();
        }
        
        public static int GetKills(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[kills]);
        }

        public static void SetKills(this PhotonView player, int value)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    bot.kills = value;
                    return;
                }
            }

            player.Owner.SetKills(value);
        }
        
        public static void SetKills(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { kills, (byte)value } });
        }

        public static void IncrementKills(this PhotonView player, int value = 1)
        {
            SetKills(player, GetKills(player) + value);
        }

        public static int GetDeaths(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.deaths;
                }
            }

            return player.Owner.GetDeaths();
        }
        
        public static int GetDeaths(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[deaths]);
        }

        public static void SetDeaths(this PhotonView player, int value)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    bot.deaths = value;
                    return;
                }
            }

            player.Owner.SetDeaths(value);
        }
        
        public static void SetDeaths(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { deaths, (byte)value } });
        }

        public static void IncrementDeaths(this PhotonView player, int value = 1)
        {
            SetDeaths(player, GetDeaths(player) + value);
        }

        // Join Time
        public static float GetJoinTime(this Photon.Realtime.Player player)
        {
            return System.Convert.ToSingle(player.CustomProperties[joinTime]);
        }

        public static float GetJoinTime(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.joinTime;
                }
            }

            return player.Owner.GetJoinTime();
        }
        
        public static void SetJoinTime(this PhotonView player, float value)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    bot.joinTime = value;
                    return;
                }
            }

            player.Owner.SetJoinTime(value);
        }

        public static void SetJoinTime(this Photon.Realtime.Player player, float value)
        {
            player.SetCustomProperties(new Hashtable() { { joinTime, (byte)value } });
        }
        
        // Class ID
        public static int GetClassId(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[classId]);
        }

        public static int GetClassId(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.classId;
                }
            }

            return player.Owner.GetClassId();
        }
        
        public static void SetClassId(this PhotonView player, int value)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    bot.classId = value;
                    return;
                }
            }

            player.Owner.SetClassId(value);
        }

        public static void SetClassId(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { classId, (byte)value } });
        }
        
        // ClassIdQueued
        public static int GetClassIdQueued(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[classIdQueued]);
        }

        public static int GetClassIdQueued(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.classIdQueued;
                }
            }

            return player.Owner.GetClassIdQueued();
        }
        
        public static void SetClassIdQueued(this PhotonView player, int value)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    bot.classIdQueued = value;
                    return;
                }
            }

            player.Owner.SetClassIdQueued(value);
        }

        public static void SetClassIdQueued(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { classIdQueued, (byte)value } });
        }
        
        // Preferred Team Index
        public static int GetPreferredTeamIndex(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[preferredTeam]);
        }

        public static int GetPreferredTeamIndex(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.teamIndex;
                }
            }

            return player.Owner.GetPreferredTeamIndex();
        }
        
        public static void SetPreferredTeamIndex(this PhotonView player, int value)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return;
                }
            }

            player.Owner.SetPreferredTeamIndex(value);
        }

        public static void SetPreferredTeamIndex(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { preferredTeam, (byte)value } });
        }
        
        // Ultimate
        public static int GetUltimate(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[ultimate]);
        }

        public static int GetUltimate(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.ultimate;
                }
            }

            return player.Owner.GetUltimate();
        }
        
        public static void SetUltimate(this PhotonView player, int value)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    bot.ultimate = value;
                    return;
                }
            }

            player.Owner.SetUltimate(value);
        }

        public static void SetUltimate(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { ultimate, (byte)value } });
        }

        public static int GetPowerup(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[powerup]);
        }
        
        public static int GetPowerup(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.powerup;
                }
            }

            return player.Owner.GetPowerup();
        }
        
        public static void SetPowerup(this PhotonView player, int value)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    bot.powerup = value;
                    
                    // Always use powerup immediately if bot
                    if(value > 0)
                        bot.TryCastPowerup();
                    
                    return;
                }
            }

            player.Owner.SetPowerup(value);
        }

        public static void SetPowerup(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { powerup, (byte)value } });
        }

        /// <summary>
        /// Offline: clears all properties of a PlayerBot locally.
        /// Fallback to online mode for the master or in case offline mode was turned off.
        /// </summary>
        public static void Clear(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    bot.currentBullet = 0;
                    bot.health = 0;
                    bot.shield = 0;
                    bot.joinTime = 0;
                    bot.classId = 0;
                    bot.ultimate = 0;
                    bot.powerup = 0;
                    return;
                }
            }

            player.Owner.Clear();
        }

        /// <summary>
        /// Online: Clears all networked variables of the player via properties in one instruction.
        /// </summary>
        public static void Clear(this Photon.Realtime.Player player)
        {
            player.SetCustomProperties(new Hashtable() { { PlayerExtensions.bullet, (byte)0 },
                                                         { PlayerExtensions.health, (byte)0 },
                                                         { PlayerExtensions.shield, (byte)0 },
                                                         {PlayerExtensions.joinTime, (byte)0 },
                                                         {PlayerExtensions.classId, (byte)1 },
                                                         {PlayerExtensions.preferredTeam, (byte) RANDOM_TEAM_INDEX},
                                                         {PlayerExtensions.ultimate, (byte)0 },
                                                         {PlayerExtensions.powerup, (byte)0}
            });
        }
    }
}
