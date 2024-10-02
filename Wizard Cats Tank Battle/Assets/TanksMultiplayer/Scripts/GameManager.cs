/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using System.Collections;
using System.Collections.Generic;
using Entropy.Scripts.Audio;
using Entropy.Scripts.Player;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vashta.Entropy.GameMode;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.UI.ClassSelectionPanel;
using Vashta.Entropy.UI.MapSelection;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

namespace TanksMP
{
    /// <summary>
    /// Manages game workflow and provides high-level access to networked logic during a game.
    /// It manages functions such as team fill, scores and ending a game, but also video ad results.
    /// </summary>
    [RequireComponent(typeof(MatchTimer))]
    public class GameManager : MonoBehaviourPun
    {
        //reference to this script instance
        private static GameManager instance;

        /// <summary>
        /// The local player instance spawned for this client.
        /// </summary>
        [HideInInspector]
        public Player localPlayer;

        /// <summary>
        /// Active game mode played in the current scene.
        /// </summary>
        public GameMode gameMode = GameMode.TDM;

        /// <summary>
        /// Does this game mode use teams?
        /// </summary>
        public bool UsesTeams => gameMode != GameMode.FFA;

        /// <summary>
        /// Reference to the UI script displaying game stats.
        /// </summary>
        public UIGame ui;

        /// <summary>
        /// Definition of playing teams with additional properties.
        /// </summary>
        public Team[] teams;

        /// <summary>
        /// The maximum amount of kills to reach before ending the game.
        /// </summary>
        public int maxScore { get; private set; }= 30;
        
        /// <summary>
        /// The delay in seconds before respawning a player after it got killed.
        /// </summary>
        public int respawnTime = 5;

        /// <summary>
        /// Enable or disable friendly fire. This is verified in the Bullet script on collision.
        /// </summary>
        public bool friendlyFire = false;

        public List<GameObject> BotTargetList;
        public MusicController MusicController;
        public PlayerInputController PlayerInputController;
        public MatchTimer MatchTimer;
        
        [FormerlySerializedAs("_mapDefinition")] [SerializeField]
        private MapDefinition mapDefinition;
        
        public GameModeDictionary GameModeDictionary;
        private GameModeDefinition _gameModeDefinition;
        
        private int lastSpawnIndex = -1;
        private RoomOptionsReader _roomOptionsReader;

        private List<PlayerBot> _botList;

        //initialize variables
        void Awake()
        {
            instance = this;
            _botList = new List<PlayerBot>();

            _roomOptionsReader = new RoomOptionsReader();
            gameMode = _roomOptionsReader.GetGameMode();
            _gameModeDefinition = GameModeDictionary[gameMode];

            // string mapName = _roomOptionsReader.GetMapName();
            // _mapDefinition = MapDefinitionDictionary.GetByName(mapName);

            maxScore = _gameModeDefinition.ScoreToWin;

            MatchTimer.InitTimer();            

            //if Unity Ads is enabled, hook up its result callback
#if UNITY_ADS
                UnityAdsManager.adResultEvent += HandleAdResult;
#endif
        }
        
        /// <summary>
        /// Returns a reference to this script instance.
        /// </summary>
        public static GameManager GetInstance()
        {
            return instance;
        }

        public void AddBot(PlayerBot bot)
        {
            _botList.Add(bot);
        }

        public void ClearBots()
        {
            _botList.Clear();
        }

        public List<PlayerBot> GetBotList()
        {
            return _botList;
        }
        
        /// <summary>
        /// Global check whether this client is the match master or not.
        /// </summary>
        public static bool isMaster()
        {
            return PhotonNetwork.IsMasterClient;
        }

        public GameModeDefinition GetGameModeDefinition()
        {
            if (_gameModeDefinition == null)
            {
                Debug.LogError("Missing game mode definition!");
            }
            
            return _gameModeDefinition;
        }

        public MapDefinition GetMap()
        {
            return mapDefinition;
        }
        
        /// <summary>
        /// Returns the next team index a player should be assigned to.
        /// </summary>
        public int GetTeamFill()
        {
            //init variables
            int[] size = PhotonNetwork.CurrentRoom.GetSize();
            int teamNo = 0;

            int min = size[0];
            //loop over teams to find the lowest fill
            for (int i = 0; i < teams.Length; i++)
            {
                //if fill is lower than the previous value
                //store new fill and team for next iteration
                if (size[i] < min)
                {
                    min = size[i];
                    teamNo = i;
                }
            }

            //return index of lowest team
            return teamNo;
        }

        public bool TeamHasVacancy(int teamIndex)
        {
            return true;
            // TODO: Revisit this later
            // int teamNo = teamIndex - 1;
            int maxTeamSize = 3; // This should NOT be hardcoded here
            
            int[] size = PhotonNetwork.CurrentRoom.GetSize();

            return size[teamIndex] < maxTeamSize;
        }
        

        /// <summary>
        /// Returns a random spawn position within the team's spawn area.
        /// </summary>
        public Vector3 GetSpawnPosition(int teamIndex)
        {
            return UsesTeams ? GetSpawnTeams(teamIndex) : GetSpawnFFA();
        }

        private Vector3 GetSpawnTeams(int teamIndex)
        {
            //init variables
            Vector3 pos = teams[teamIndex].spawn.position;
            BoxCollider col = teams[teamIndex].spawn.GetComponent<BoxCollider>();

            if(col != null)
            {
                //find a position within the box collider range, first set fixed y position
                //the counter determines how often we are calculating a new position if out of range
                pos.y = col.transform.position.y;
                int counter = 10;
                
                //try to get random position within collider bounds
                //if it's not within bounds, do another iteration
                do
                {
                    pos.x = UnityEngine.Random.Range(col.bounds.min.x, col.bounds.max.x);
                    pos.z = UnityEngine.Random.Range(col.bounds.min.z, col.bounds.max.z);
                    counter--;
                }
                while(!col.bounds.Contains(pos) && counter > 0);
            }
            
            return pos;
        }

        private Vector3 GetSpawnFFA()
        {
            int counter = 10;
            int spawnIndex;
            do
            {
                spawnIndex = Random.Range(0, teams.Length);
            } while (lastSpawnIndex == spawnIndex && counter-- > 0);

            return GetSpawnTeams(spawnIndex);
        }
        
        //implements what to do when an ad view completes
        #if UNITY_ADS
        void HandleAdResult(ShowResult result)
        {
            switch (result)
            {
                //in case the player successfully watched an ad,
                //it sends a request for it be respawned
                case ShowResult.Finished:
                case ShowResult.Skipped:
                    localPlayer.CmdRespawn();
                    break;
                
                //in case the ad can't be shown, just handle it
                //like we wouldn't have tried showing a video ad
                //with the regular death countdown (force ad skip)
                case ShowResult.Failed:
                    DisplayDeath(true);
                    break;
            }
        }
        #endif


        /// <summary>
        /// Adds points to the target team depending on matching game mode and score type.
        /// This allows us for granting different amount of points on different score actions.
        /// </summary>
        public void AddScore(ScoreType scoreType, int teamIndex)
        {
            //distinguish between game mode
            switch(gameMode)
            {
                //in TDM, we only grant points for killing
                case GameMode.TDM: case GameMode.FFA:
                    switch(scoreType)
                    {
                        case ScoreType.Kill:
                            PhotonNetwork.CurrentRoom.AddScore(teamIndex, 1);
                            break;
                    }
                break;

                //in CTF, we grant points for both killing and flag capture
                case GameMode.CTF: case GameMode.CTFS:
                    switch(scoreType)
                    {
                        case ScoreType.Kill:
                            PhotonNetwork.CurrentRoom.AddScore(teamIndex, 1);
                            break;

                        case ScoreType.Capture:
                            PhotonNetwork.CurrentRoom.AddScore(teamIndex, 10);
                            break;
                    }
                break;
                
                case GameMode.KOTH:
                    switch (scoreType)
                    {
                        case ScoreType.Kill:
                            PhotonNetwork.CurrentRoom.AddScore(teamIndex, 1);
                            break;

                        case ScoreType.Capture:
                            PhotonNetwork.CurrentRoom.AddScore(teamIndex, 10);
                            break;
                        
                        case ScoreType.HoldPoint:
                            PhotonNetwork.CurrentRoom.AddScore(teamIndex, 5);
                            break;
                    }
                    break;
            }
        }

        public void RemoveScore(ScoreType scoreType, int teamIndex)
        {
            PhotonNetwork.CurrentRoom.AddScore(teamIndex, -1);
        }

        /// <summary>
        /// Returns whether a team reached the maximum game score.
        /// </summary>
        public bool IsGameOver()
        {
            if (!MatchTimer.MatchTimeIsRunning())
                return true;
            
            //init variables
            bool isOver = false;
            int[] score = PhotonNetwork.CurrentRoom.GetScore();
            
            //loop over teams to find the highest score
            for(int i = 0; i < teams.Length; i++)
            {
                //score is greater or equal to max score,
                //which means the game is finished
                if(score[i] >= maxScore)
                {
                    isOver = true;
                    break;
                }
            }
            
            //return the result
            return isOver;
        }

        public int GetTeamWithHighestScore()
        {
            int[] score = PhotonNetwork.CurrentRoom.GetScore();
            int teamWithHighestScore = -1;
            int highestScoreFound = 0;
            
            //loop over teams to find the highest score
            for(int i = 0; i < teams.Length; i++)
            {
                if(score[i] > highestScoreFound)
                {
                    highestScoreFound = score[i];
                    teamWithHighestScore = i;
                    
                } else if (score[i] == highestScoreFound)
                {
                    teamWithHighestScore = -1;
                }
            }

            return teamWithHighestScore;
        }
        
        
        /// <summary>
        /// Only for this player: sets the death text stating the killer on death.
        /// If Unity Ads is enabled, tries to show an ad during the respawn delay.
        /// By using the 'skipAd' parameter is it possible to force skipping ads.
        /// </summary>
        public void DisplayDeath(bool skipAd = false)
        {
            if (!ClassSelectionPanel.Instance.CountdownIsActive())
            {
                //get the player component that killed us
                Player other = localPlayer;
                string killedByName = "YOURSELF";
                if (localPlayer.killedBy != null)
                    other = localPlayer.killedBy.GetComponent<Player>();

                //suicide or regular kill?
                if (other != localPlayer)
                {
                    killedByName = other.GetView().GetName();
                }

                Text[] killCounter = ui.killCounter;
                killCounter[1].text = localPlayer.GetView().GetDeaths().ToString();
                killCounter[1].GetComponent<Animator>().Play("Animation");

                //calculate if we should show a video ad
#if UNITY_ADS
            if (!skipAd && UnityAdsManager.ShowAd())
                return;
#endif

                //when no ad is being shown, set the death text
                //and start waiting for the respawn delay immediately
                ui.SetDeathText(killedByName, teams[other.GetView().GetTeam()]);
            }

            StartCoroutine(SpawnRoutine());
        }


        //coroutine spawning the player after a respawn delay
        IEnumerator SpawnRoutine()
        {
            //calculate point in time for respawn
            float targetTime = 0f;
            if (!ClassSelectionPanel.Instance.CountdownIsActive())
            {
                targetTime = Time.time + respawnTime;
            }
            
            //wait for the respawn to be over,
            //while waiting update the respawn countdown
            while (targetTime - Time.time > 0)
            {
                float timeToSpawn = targetTime - Time.time;
                ui.SetSpawnDelay(timeToSpawn);
                yield return null;
            }

            //respawn now: send request to the server
            ui.DisableDeath();
            localPlayer.CmdRespawn();
        }
        
        
        /// <summary>
        /// Only for this player: sets game over text stating the winning team.
        /// Disables player movement so no updates are sent through the network.
        /// </summary>
        public void DisplayGameOver(int teamIndex)
        {
            Debug.Log("Team Index: " + teamIndex);
            
            
            MatchTimer.StopTimer();
            //PhotonNetwork.isMessageQueueRunning = false;
            localPlayer.enabled = false;
            localPlayer.camFollow.HideMask(true);
            
            if (teamIndex != -1)
            {
                Team winningTeam = teams[teamIndex];
                ui.SetGameOverText(winningTeam);
            }
            else
            {
                ui.SetGameOverText(null);
            }

            //starts coroutine for displaying the game over window
            StopCoroutine(SpawnRoutine());
            StartCoroutine(DisplayGameOverCR(teamIndex));
        }

        //displays game over window after short delay
        IEnumerator DisplayGameOverCR(int teamIndex)
        {
            //give the user a chance to read which team won the game
            //before enabling the game over screen
            yield return new WaitForSeconds(3);

            //show game over window (still connected at that point)
            if (teamIndex != -1)
            {
                // Handle victory
                Team winningTeam = teams[teamIndex];
                ui.ShowGameOver(teamIndex, winningTeam.name, winningTeam.material.color);

                int playerTeamIndex = localPlayer.GetView().GetTeam();
                if (playerTeamIndex == teamIndex)
                    MusicController.PlayVictoryMusic();
                else
                    MusicController.PlayDefeatMusic();
            }
            else
            {
                // Handle tie
                ui.ShowGameOver(teamIndex, "NO ONE", Color.white);
                MusicController.PlayDefeatMusic();
            }
        }


        //clean up callbacks on scene switches
        void OnDestroy()
        {
            #if UNITY_ADS
                UnityAdsManager.adResultEvent -= HandleAdResult;
            #endif
        }
    }


    /// <summary>
    /// Defines properties of a team.
    /// </summary>
    [System.Serializable]
    public class Team
    {
        /// <summary>
        /// The name of the team shown on game over.
        /// </summary>
        public string name;

        /// <summary>
        /// The color of a team for UI and player prefabs.
        /// </summary>   
        public Material material;

        /// <summary>
        /// The spawn point of a team in the scene. In case it has a BoxCollider
        /// component attached, a point within the collider bounds will be used.
        /// </summary>
        public Transform spawn;

        public Transform freeClassChange;

        public TeamDefinition teamDefinition;
    }


    /// <summary>
    /// Defines the types that could grant points to players or teams.
    /// Used in the AddScore() method for filtering.
    /// </summary>
    public enum ScoreType
    {
        Kill,
        Capture,
        HoldPoint,
        TargetDestroyed
    }


    /// <summary>
    /// Available game modes selected per scene.
    /// Used in the AddScore() method for filtering.
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// Team Deathmatch
        /// </summary>
        TDM,

        /// <summary>
        /// Capture The Flag
        /// </summary>
        CTF,
        
        /// <summary>
        /// Free For All
        /// </summary>
        FFA,
        
        /// <summary>
        /// King of the Hill
        /// </summary>
        KOTH,
        
        /// <summary>
        /// Mouse Hunt
        /// </summary>
        MH,
        
        /// <summary>
        /// Capture The Flags
        /// </summary>
        CTFS
    }
}