using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using FusionHelpers;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.Player;

namespace Vashta.Entropy.GameState
{
    public class BotController : NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        /// <summary>
        /// Amount of bots to spawn across all teams.  For singleplayer
        /// </summary>
        public int maxBots;
        /// <summary>
        /// Total number of players.  For multiplayer
        /// </summary>
        public int maxPlayers;
        
        /// <summary>
        /// Selection of bot prefabs to choose from.
        /// </summary>
        public GameObject prefab;
        
        public List<GameObject> BotTargetList;
        private List<PlayerControllerBot> _botList;
        private GameManager _gameManager;
        private bool _hasSpawned;

        protected int CurrentPlayerCount()
        {
            int numberOfRealPlayers = 0;
            IEnumerable<PlayerRef> it = Runner.ActivePlayers;

            foreach (PlayerRef playerRef in it)
            {
                if (Runner.TryGetPlayerObject(playerRef, out var plObject))
                {
                    PlayerController player = plObject.GetComponent<PlayerController>();
                    if(!player.isBot)
                        numberOfRealPlayers++;
                }
            }

            return numberOfRealPlayers;
        }

        protected int CurrentBotCount()
        {
            int numberOfRealPlayers = 0;
            IEnumerable<PlayerRef> it = Runner.ActivePlayers;

            foreach (PlayerRef playerRef in it)
            {
                if (Runner.TryGetPlayerObject(playerRef, out var plObject))
                {
                    PlayerController player = plObject.GetComponent<PlayerController>();
                    if(player.isBot)
                        numberOfRealPlayers++;
                }
            }

            return numberOfRealPlayers;
        }

        public override void Spawned()
        {
            base.Spawned();
            
            _gameManager = GameManager.GetInstance();
            _botList = new List<PlayerControllerBot>();

            if (!_gameManager.HasStateAuthority)
                return;
            
            
            if(Runner.GameMode == Fusion.GameMode.Single)
            {
                StartCoroutine(SpawnBots());
            }
            else
            {
                // Eventually, this should instead use a "maxBotCount" property
                maxPlayers = Runner.SessionInfo.MaxPlayers;
            }

            _hasSpawned = true;
        }

        public IEnumerator InitialSpawnCR()
        {
            while(!_hasSpawned)
                yield return null;
            
            FillWithBots();
        }
        
        /// <summary>
        /// CR to spawn bots for singleplayer games
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnBots()
        {
            //wait a second for all script to initialize
            yield return new WaitForSeconds(1);

            //loop over bot count
            for (int i = 0; i < maxBots; i++)
            {
                SpawnBot();

                //let the local host determine the team assignment
                // PlayerController p = obj.GetComponent<PlayerController>();
                // int teamIndex = GameManager.GetInstance().TeamController.GetTeamFill();
                // p.PlayerTeam.SetPlayerPreferredTeam(teamIndex);
                // p.PlayerTeam.TryChangeTeams(true);

                //increase corresponding team size
                // _gameManager.TeamController.AddPlayerTeamTeam(p, p.TeamIndex);

                yield return new WaitForSeconds(0.33f);
            }
        }

        protected void SpawnBot()
        {
            if (!_gameManager.HasStateAuthority)
                return;
            
            NetworkObject obj = Runner.Spawn(prefab, Vector3.zero, Quaternion.identity, null, (runner, o) =>
            {
                FusionPlayer player = o.GetComponent<FusionPlayer>();
                if (player != null)
                {
                    player.InitNetworkState();
                }
            });
        }

        /// <summary>
        /// Fills any open spots in multiplayer games
        /// </summary>
        protected void FillWithBots()
        {
            if (!_gameManager.HasStateAuthority)
                return;
            
            int numberOfPlayers = Runner.ActivePlayers.Count();
            int botsToAdd = maxBots - numberOfPlayers;

            for (int i = 0; i < botsToAdd; i++)
            {
                SpawnBot();
            }
        }

        // Should be something that is called regularly when players change teams
        protected void BalanceBots()
        {
            if (!_gameManager.HasStateAuthority)
                return;
            
            TeamController teamController = _gameManager.TeamController;

            if (teamController.TeamsAreEven())
                return;
            
            // int totalPlayers
            // TODO: Come back to this after bots are working
        }

        protected void RemoveBot()
        {
            if (!_gameManager.HasStateAuthority)
                return;
            
            PlayerControllerBot botToRemove = GetBotToRemove();
            
            if(botToRemove != null)
                Runner.Despawn(botToRemove.Object);
        }
        
        public void AddBot(PlayerControllerBot controllerBot)
        {
            _botList.Add(controllerBot);
        }

        public List<PlayerControllerBot> GetBotList()
        {
            return _botList;
        }

        public void PlayerJoined(PlayerRef player)
        {
            if (!_gameManager.HasStateAuthority)
                return;
            
            // remove a bot
            RemoveBot();
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (!_gameManager.HasStateAuthority)
                return;
            
            // add bots
            FillWithBots();
            BalanceBots();
        }

        public PlayerControllerBot GetBotToRemove()
        {
            if (_gameManager.TeamController.TeamsAreEven())
            {
                // If teams are even, remove the oldest bot
                PlayerControllerBot oldestBot = null;
                float oldestBotSpawnTime = float.MaxValue;

                IEnumerable<PlayerRef> it = Runner.ActivePlayers;

                foreach (PlayerRef playerRef in it)
                {
                    if (Runner.TryGetPlayerObject(playerRef, out var plObject))
                    {
                        PlayerControllerBot bot = plObject.GetComponent<PlayerControllerBot>();
                        if (bot && bot.isBot)
                        {
                            if (bot.JoinTime < oldestBotSpawnTime)
                            {
                                oldestBot = bot;
                                oldestBotSpawnTime = bot.JoinTime;
                            }
                        }
                    }
                }

                return oldestBot;
            }
            else
            {
                // if teams are uneven find a bot from the largest team
                // sort team indecies by size
                int numberOfTeams = _gameManager.TeamController.TeamCount;

                NetworkArray<int> teamSizes = _gameManager.TeamController.TeamSizes;
                List<Tuple<int, int>> teams = new List<Tuple<int, int>>();

                for (int i = 0; i < numberOfTeams; i++)
                {
                    teams.Add(new Tuple<int, int>(i, teamSizes[i]));
                }

                teams = teams.OrderByDescending(x => x.Item2).ToList();
                
                // iterate over teams, return first bot
                for (int i = 0; i < teams.Count; i++)
                {
                    int teamIndex = teams[i].Item1;
                    
                    IEnumerable<PlayerRef> it = Runner.ActivePlayers;

                    foreach (PlayerRef playerRef in it)
                    {
                        if (Runner.TryGetPlayerObject(playerRef, out var plObject))
                        {
                            PlayerControllerBot bot = plObject.GetComponent<PlayerControllerBot>();
                            if (bot && bot.isBot && bot.TeamIndex == teamIndex)
                            {
                                return bot;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}