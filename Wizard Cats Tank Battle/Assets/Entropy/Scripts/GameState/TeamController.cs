using System;
using System.Collections.Generic;
using Entropy.Scripts.Player;
using Fusion;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Player;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.TanksExtensions;
using Vashta.Entropy.UI.TeamScore;
using Random = UnityEngine.Random;

namespace Vashta.Entropy.GameState
{
    public class TeamController : NetworkBehaviour
    {
        public const int RANDOM_TEAM_INDEX = 100;
        public TeamInstance[] teams; // This is set in the editor
        private GameManager _gameManager;
        private int lastSpawnIndex = -1;
        [Networked]
        public int maxScore { get; set; }
        
        // Networked properties.  Later re-factor into a INetworkStruct
        // Array needs a fixed size set here, so it is always 4
        [Networked, Capacity(4), OnChangedRender(nameof(RefreshDisplay))]
        private NetworkArray<int> ScoreByTeamIndex => default;

        [Networked, Capacity(4), OnChangedRender(nameof(RefreshDisplay))]
        private NetworkArray<int> TeamSize => default;
        public NetworkArray<int> TeamSizes => TeamSize;

        public bool UsesTeams => _gameManager.MatchInfo.GameMode != TanksMP.GameMode.FFA;
        public int TeamCount => teams.Length;
        
        TeamScoreDisplayController TeamScoreDisplayController => TeamScoreDisplayController.GetInstance();
        
        public bool HasSpawned { get; private set; }

        private void Awake()
        {
            _gameManager = GetComponent<GameManager>();
        }

        public override void Spawned()
        {
            base.Spawned();
            HasSpawned = true;
        }
        
        public TeamInstance GetTeamByIndex(int index)
        {
            if (index < teams.Length && index >= 0)
            {
                return teams[index];
            }

            return teams[0];
        }

        public bool TeamsAreEven()
        {
            int teamSize = TeamSize[0];
            int teamCount = TeamCount;

            for (int i = 1; i < TeamCount; i++)
            {
                if (TeamSize[i] != teamSize)
                    return false;
            }

            return true;
        }
        
        public void AddPlayerToTeam(PlayerController playerController, int teamIndex)
        {
            playerController.Team.TeamIndex = teamIndex;
            playerController.Team.PreferredTeamIndex = teamIndex;
            
            if (teamIndex < TeamSize.Length)
            {
                TeamSize.Set(teamIndex, TeamSize[teamIndex]+1);
            }
            else
            {
                Debug.LogError("Tried to add player to invalid team: " + teamIndex);
            }
        }

        public void RemovePlayerFromTeam(PlayerController playerController)
        {
            if (!playerController)
            {
                Debug.LogError("Attempted to remove null player");
                return;
            }
            int teamIndex = playerController.TeamIndex;
            
            if (teamIndex < TeamSize.Length)
            {
                TeamSize.Set(teamIndex, TeamSize[teamIndex]-1);
            }
            else
            {
                Debug.LogError("Tried to remove player with invalid team: " + teamIndex);
            }
        }
        
        public void OnePassPlayerCheckToChangeTeams(PlayerController playerController, bool respawn)
        {
            if (!playerController)
                return;
            
            int preferredTeamIndex = playerController.PreferredTeamIndex;
            bool prefersDifferentTeam = preferredTeamIndex != playerController.TeamIndex;

            if (prefersDifferentTeam)
            {
                if (_gameManager.TeamController.TeamHasVacancy(preferredTeamIndex))
                {
                    // Handle game over. Nested for efficiency
                    if (_gameManager.IsGameOver())
                        return;

                    AttemptToChangePlayerToPreferredTeam(playerController);
                }
            }
        }
        
        public void AttemptToChangePlayerToPreferredTeam(PlayerController playerController)
        {
            int preferredTeamIndex = playerController.PreferredTeamIndex;
            int currentTeam = playerController.TeamIndex;

            if (preferredTeamIndex == RANDOM_TEAM_INDEX)
            {
                preferredTeamIndex = GetTeamFill();
                playerController.Team.SetPlayerPreferredTeam(preferredTeamIndex);
            }

            // Do not continue if already on the preferred team or if the preferred team does not have vacancy
            if (preferredTeamIndex == currentTeam || !TeamHasVacancy(preferredTeamIndex))
            {
                return;
            }

            if(playerController.TeamIndex != -1)
                TeamSize.Set(playerController.TeamIndex, TeamSize[playerController.TeamIndex] - 1);
            
            if(preferredTeamIndex != -1)
                AddPlayerToTeam(playerController, preferredTeamIndex);
            
            playerController.Team.TeamIndex = preferredTeamIndex;

            if (currentTeam != playerController.Team.TeamIndex)
            {
                RefreshDisplay();
            }
        }

        public void RefreshDisplay()
        {
            // ReCalculateTeams();
            TeamScoreDisplayController.UpdateTeamSizes(TeamSize.ToArray());
            TeamScoreDisplayController.UpdateScores(ScoreByTeamIndex.ToArray());
        }

        public void ReCalculateTeams()
        {
            if (!Runner)
                return;
            
            List<int> teamScores = new List<int> { 0, 0, 0, 0 };

            // Iterate over players
            List<PlayerController> players = PlayerList.GetAllPlayers;

            foreach (PlayerController player in players)
            {
                int teamIndex = player.TeamIndex;

                if (teamIndex != -1 && teamIndex != RANDOM_TEAM_INDEX)
                {
                    teamScores[teamIndex]++;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                TeamSize.Set(i, teamScores[i]);
            }
        }
        
        /// <summary>
        /// Returns the next team index a player should be assigned to.
        /// </summary>
        public int GetTeamFill()
        {
            if (!_gameManager.MatchInfo.BotFilling || Runner.GameMode == Fusion.GameMode.Single)
            {
                return GetTeamFillNoBots();
            }

            return GetTeamFillWithBots();
        }

        // More efficient team fill when there are no bots to consider
        private int GetTeamFillNoBots()
        {
            //init variables
            int teamNo = 0;

            int min = TeamSize[0];
            //loop over teams to find the lowest fill
            for (int i = 0; i < teams.Length; i++)
            {
                //if fill is lower than the previous value
                //store new fill and team for next iteration
                if (TeamSize[i] < min)
                {
                    min = TeamSize[i];
                    teamNo = i;
                }
            }

            //return index of lowest team
            return teamNo;
        }

        private int GetTeamFillWithBots()
        {
            // init
            List<int> teamSizesNoBots = new();

            for (int i = 0; i < TeamCount; i++)
            {
                teamSizesNoBots.Add(0);
            }
            
            // count
            IEnumerable<PlayerRef> it = Runner.ActivePlayers;
            
            foreach (PlayerRef playerRef in it)
            {
                if (Runner.TryGetPlayerObject(playerRef, out var plObject))
                {
                    PlayerController player = plObject.GetComponent<PlayerController>();
                    if (!player.isBot)
                    {
                        if(player.TeamIndex == -1)
                            continue;
                        
                        teamSizesNoBots[player.TeamIndex]++;
                    }
                }
            }
            
            // get smallest team
            int smallestTeamIndex = -1;
            int smallestTeamSize = Int32.MaxValue;

            for (int i = 0; i < teamSizesNoBots.Count; i++)
            {
                if (teamSizesNoBots[i] < smallestTeamSize)
                {
                    smallestTeamSize = teamSizesNoBots[i];
                    smallestTeamIndex = i;
                }
            }

            if (smallestTeamIndex == -1)
                return 0;

            return smallestTeamIndex;
        }

        public bool TeamHasVacancy(int teamIndex)
        {
            return true;
            // TODO: Revisit this later
        }

        #region Spawning
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
            Vector3 pos = teams[teamIndex].spawnArea.position;
            BoxCollider col = teams[teamIndex].spawnArea.GetComponent<BoxCollider>();

            if(col != null)
            {
                //find a position within the box collider range, first set fixed y position
                //the counter determines how often we are calculating a new position if out of range
                pos.y = col.transform.position.y;
                int counter = 30;
                
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
        #endregion

        #region Score
/// <summary>
        /// Adds points to the target team depending on matching game mode and score type.
        /// This allows us for granting different amount of points on different score actions.
        /// </summary>
        public void AddScore(ScoreType scoreType, int teamIndex)
        {
            if (!HasStateAuthority)
                return;
            
            GameModeDefinition gameMode = _gameManager.GameModeDefinition;
            
            switch(scoreType)
            {
                case ScoreType.Kill:
                    ScoreByTeamIndex.Set(teamIndex, ScoreByTeamIndex[teamIndex] + gameMode.KillPoints);
                    break;
                
                case ScoreType.Capture:
                    ScoreByTeamIndex.Set(teamIndex, ScoreByTeamIndex[teamIndex] + gameMode.CapturePoints);
                    break;
                
                case ScoreType.HoldPoint:
                    ScoreByTeamIndex.Set(teamIndex, ScoreByTeamIndex[teamIndex] + gameMode.HoldPointPoints);
                    break;
            }
            
            TeamScoreDisplayController.UpdateScores(ScoreByTeamIndex.ToArray());
        }

        public void RemoveScore(ScoreType scoreType, int teamIndex)
        {
            if (!HasStateAuthority)
                return;
            
            ScoreByTeamIndex.Set(teamIndex, ScoreByTeamIndex[teamIndex]-1);
            TeamScoreDisplayController.UpdateScores(ScoreByTeamIndex.ToArray());
        }
        
        /// <summary>
        /// Returns whether a team reached the maximum game score.
        /// </summary>
        public bool MaxScoreIsReached()
        {
            //init variables
            bool isOver = false;
            // int[] score = PhotonNetwork.CurrentRoom.GetScore();
            
            //loop over teams to find the highest score
            foreach (int score in ScoreByTeamIndex)
            {
                //score is greater or equal to max score,
                //which means the game is finished
                if(score >= maxScore)
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
            int teamWithHighestScore = -1;
            int highestScoreFound = 0;
            
            //loop over teams to find the highest score
            for (var index = 0; index < ScoreByTeamIndex.Length; index++)
            {
                var score = ScoreByTeamIndex[index];
                if (score > highestScoreFound)
                {
                    highestScoreFound = score;
                    teamWithHighestScore = index;
                }
                else if (score == highestScoreFound)
                {
                    teamWithHighestScore = -1;
                }
            }

            return teamWithHighestScore;
        }
        
        #endregion

        #region TeamStateSnapshot

        public List<TeamStateSnapshot> GetTeamStates(bool includeLocalPlayer = true)
        {
            List<TeamStateSnapshot> teamStates = new List<TeamStateSnapshot>();
            
            for (int i = 0; i < teams.Length && i < ScoreByTeamIndex.Length; i++)
            {
                TeamStateSnapshot stateSnapshot = new TeamStateSnapshot(teams[i], ScoreByTeamIndex[i], i, includeLocalPlayer);
                teamStates.Add(stateSnapshot);
            }

            return teamStates;
        }

        public TeamStateSnapshot GetTeamState(int teamIndex, bool includeLocalPlayer = true)
        {
            if (teamIndex >= teams.Length || teamIndex >= ScoreByTeamIndex.Length)
            {
                Debug.LogWarning($"Team index {teamIndex} was too high!");
            }

            return new TeamStateSnapshot(teams[teamIndex], ScoreByTeamIndex[teamIndex], teamIndex, includeLocalPlayer);
        }

        #endregion
    }
}