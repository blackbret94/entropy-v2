/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
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
        public const string buffIndex = "buffIndex";
        public const string buffSeconds = "buffSeconds";

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
            Debug.Log("Setting team index: " + teamIndex);
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
            
            if(player.IsLocal)
                GameManager.GetInstance().ui.bulletIcon.SetPowerup(index, value);
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

        public static float GetBuffSeconds(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.buffSeconds;
                }
            }

            return player.Owner.GetBuffSeconds();
        }

        /// <summary>
        /// Online: returns the networked buffseconds value of the player out of properties.
        /// </summary>
        public static float GetBuffSeconds(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[buffSeconds]);
        }

        public static void DecBuffSeconds(this PhotonView player, float delta)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    bot.buffSeconds -= delta;

                    if (bot.buffSeconds <= 0)
                    {
                        player.SetBuff(0, 0);
                    }

                    return;
                }
            }

            player.Owner.DecBuffSeconds(player, delta);
        }

        private static void DecBuffSeconds(this Photon.Realtime.Player player, PhotonView playerPhotonView, float delta)
        {
            float newBuffValue = player.GetBuffSeconds() - delta;
            
            Hashtable hash = new Hashtable();
            hash.Add(buffSeconds, (byte)newBuffValue);

            player.SetCustomProperties(hash);
            
            if(newBuffValue <= 0f)
                player.SetBuff(playerPhotonView, 0, 0);
            
            // if(player.IsLocal)
            //     GameManager.GetInstance().ui.buffIcon.SetSeconds(newBuffValue);
        }
        
        /// <summary>
        /// Offline: returns the buff index of a bot stored in PlayerBot.
        /// Fallback to online mode for the master or in case offline mode was turned off.
        /// </summary>
        public static int GetBuffIndex(this PhotonView player)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    return bot.buffIndex;
                }
            }

            return player.Owner.GetBuffIndex();
        }

        /// <summary>
        /// Online: returns the networked buff index of the player out of properties.
        /// </summary>
        public static int GetBuffIndex(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[buffIndex]);
        }
        
        /// <summary>
        /// Offline: synchronizes the buff seconds of a PlayerBot locally.
        /// Provides an optional index parameter for setting a new buff and timer together.
        /// Fallback to online mode for the master or in case offline mode was turned off.
        /// </summary>
        public static void SetBuff(this PhotonView player, float value, int index = -1)
        {
            if (PhotonNetwork.OfflineMode == true)
            {
                PlayerBot bot = player.GetComponent<PlayerBot>();
                if (bot != null)
                {
                    bot.buffSeconds = value;
                    if (index >= 0)
                    {
                        bot.buffIndex = index;
                    }

                    return;
                }
            }

            player.Owner.SetBuff(player, value, index);
        }

        /// <summary>
        /// Online: synchronizes the buff seconds of the player for all players via properties.
        /// Provides an optional index parameter for setting a new buff and timer together.
        /// </summary>
        public static void SetBuff(this Photon.Realtime.Player player, PhotonView playerPhotonView, float value, int index = -1)
        {
            Hashtable hash = new Hashtable();
            hash.Add(buffSeconds, (byte)value);
            if (index >= 0)
            {
                hash.Add(buffIndex, (byte)index);
            }

            player.SetCustomProperties(hash);
            
            // update UI
            // if(player.IsLocal)
            //     GameManager.GetInstance().ui.buffIcon.SetPowerup(index, value);
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
                    bot.buffIndex = 0;
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
                                                         {PlayerExtensions.buffIndex, (byte)0 }
            });
        }
    }
}
