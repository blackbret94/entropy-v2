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
        // This many less bots than max players
        public const int MAX_PLAYER_BUFFER = 2;
        
        /// <summary>
        /// Selection of bot prefabs to choose from.
        /// </summary>
        public GameObject prefab;
        
        public List<GameObject> BotTargetList;
        private List<PlayerControllerBot> _botList;
        private GameManager _gameManager;
        private bool _hasSpawned;
        private float _waitBetweenBots = .33f;
        private Coroutine _spawnBotCR;

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
                yield return new WaitForSeconds(_waitBetweenBots);
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

            if (_spawnBotCR != null)
            {
                StopCoroutine(_spawnBotCR);
            }
            
            _spawnBotCR = StartCoroutine(FillWithBotsCR());
        }

        protected IEnumerator FillWithBotsCR()
        {
            yield return new WaitForSeconds(1);

            while (Runner.ActivePlayers.Count() + _botList.Count() < maxPlayers-MAX_PLAYER_BUFFER)
            {
                SpawnBot();
                yield return new WaitForSeconds(_waitBetweenBots);
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

            _botList.Remove(botToRemove);
            
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

                List<PlayerControllerBot> bots = GetComponents<PlayerControllerBot>().ToList();

                foreach (PlayerControllerBot bot in bots)
                {
                    if (bot.JoinTime < oldestBotSpawnTime)
                    {
                        oldestBot = bot;
                        oldestBotSpawnTime = bot.JoinTime;
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
                
                // iterate over teams ordered by size, return first bot, ideally from the largest team
                List<PlayerControllerBot> bots = GetComponents<PlayerControllerBot>().ToList();
                
                for (int i = 0; i < teams.Count; i++)
                {
                    int teamIndex = teams[i].Item1;
                    
                    foreach (PlayerControllerBot bot in bots)
                    {
                        if (bot.TeamIndex == teamIndex)
                            return bot;
                    }
                }
            }

            return null;
        }
    }
}