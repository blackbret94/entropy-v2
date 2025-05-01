/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Vashta.Entropy.Player;
using Vashta.Entropy.UI;
using Vashta.Entropy.Util;
using Vashta.Entropy.World;

namespace TanksMP
{          
    /// <summary>
    /// Implementation of AI bots by overriding methods of the Player class.
    /// </summary>
	public class PlayerControllerBot : PlayerController
    {
        /// <summary>
        /// Radius in units for detecting other players.
        /// </summary>
        public float range = 6f;

        public float buffFrequencyS = 1f;
        private float _lastBuffS = 0f;

        [Range(0f, 10f)]
        public float accuracyError = 0f;

        public CatNameGenerator CatNameGenerator;
        
        //list of enemy players that are in range of this bot
        private List<GameObject> _enemiesInRange = new List<GameObject>();
        
        // List of allies that are in range of this bot
        private List<GameObject> _alliesInRange = new List<GameObject>();

        //reference to the agent component
        public NavMeshAgent agent;

        //current destination on the navigation mesh
        private Vector3 targetPoint;

        //timestamp when next shot should happen
        private float nextShot;

        private readonly float _slowUpdateRate = .5f;
        private readonly float _pathfindingRate = 1f;
        private Timer _timerSlowUpdate;
        private Timer _timerPathfinding;
        
        private float _maxTimeWithoutMovement = 10f;
        private float _lastMovementTime = 0f;
        private Vector3 _lastPosition;
        private float _minMovementDistance = .5f;

        private void Awake()
        {
            isBot = true;
            defaultClassDefinition = classDirectory.RandomClass();
        }

        protected override void PostSpawn()
        {
            _timerSlowUpdate = new Timer(_slowUpdateRate, true);
            _timerPathfinding = new Timer(_pathfindingRate, true);
   
            agent = GetComponent<NavMeshAgent>();
            agent.speed = moveSpeed;

            //get corresponding team and colorize renderers in team color
            targetPoint = GameManager.GetInstance().TeamController.GetSpawnPosition(TeamIndex);
            agent.Warp(targetPoint);
            SnapToNavMesh(targetPoint);
            bool success = agent.SetDestination(targetPoint);
            
            // add to player bot list
            GameManager.GetInstance().BotController.AddBot(this);
            PlayerViewController.ColorizePlayerForTeam();

            _timerPathfinding.Run();
            _timerSlowUpdate.Run();
        }

        protected override void SetName()
        {
            PlayerName = CatNameGenerator.GetRandomName();
        }
        
        //sets inRange list for player detection
        private void DetectPlayers()
        {
            //empty list on each iteration
            _enemiesInRange.Clear();
            _alliesInRange.Clear();

            //casts a sphere to detect other player objects within the sphere radius
            Collider[] cols = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Player"));
            //loop over players found within bot radius
            for (int i = 0; i < cols.Length; i++)
            {
                //get other Player component
                PlayerController p = cols[i].gameObject.GetComponent<PlayerController>();
                
                // Add enemies to the list
                if(p.TeamIndex != TeamIndex && !_enemiesInRange.Contains(cols[i].gameObject))
                {
                    _enemiesInRange.Add(cols[i].gameObject);   
                // Add allies to the list
                } else if (p.TeamIndex == TeamIndex && p != this)
                {
                    _alliesInRange.Add(cols[i].gameObject);
                }
            }
        }
        
        //calculate random point for movement on navigation mesh
        private void RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            //clear previous target point
            result = Vector3.zero;
            
            //try to find a valid point on the navmesh with an upper limit (10 times)
            for (int i = 0; i < 10; i++)
            {
                //find a point in the movement radius
                Vector3 randomPoint = center + (Vector3)Random.insideUnitCircle * range;
                randomPoint.y = transform.position.y;
                //randomPoint.y = 0;
                NavMeshHit hit;

                //if the point found is a valid target point, set it and continue
                if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas)) 
                {
                    // Check for collision with PathfindingZone
                    if (!PathfindingZone.PointIsInRestrictedZone(randomPoint, Team.TeamIndex))
                    {
                        result = hit.position;
                        break;
                    }
                }
            }
            
            //set the target point as the new destination
            bool success = agent.SetDestination(result);
        }
        //
        // private bool CheckForStuckInPosition()
        // {
        //     if (_lastMovementTime + _maxTimeWithoutMovement > Time.time)
        //     {
        //         Vector3 pos = transform.position;
        //         if ((_lastPosition - pos).magnitude < _minMovementDistance)
        //         {
        //             SnapToNavMesh();
        //             PickRandomLocation();
        //             _lastMovementTime = Time.time;
        //             return true;
        //         }
        //         else
        //         {
        //             _lastPosition = pos;
        //             _lastMovementTime = Time.time;
        //             return false;
        //         }
        //     }
        //
        //     return false;
        // }
        
        private void PickRandomLocation()
        {
            List<GameObject> possibleTargets = GameManager.BotController.BotTargetList;
            RandomPoint(possibleTargets[Random.Range(0, possibleTargets.Count)].transform.position, range, out targetPoint);
        }

        public override void Render()
        {
            base.Render();

            if(_timerSlowUpdate.Run())
            {
                SlowUpdate();
            }

            if (_timerPathfinding.Run())
            {
                DetectPlayers();
            }
        }

        private void SlowUpdate()
        {
            //empty list on each iteration
            _enemiesInRange.Clear();
            _alliesInRange.Clear();
            float detectionRange = 4;

            //casts a sphere to detect other player objects within the sphere radius
            Collider[] cols = Physics.OverlapSphere(transform.position, detectionRange, LayerMask.GetMask("Player"));
            //loop over players found within bot radius
            for (int i = 0; i < cols.Length; i++)
            {
                //get other Player component
                PlayerController p = cols[i].gameObject.GetComponent<PlayerController>();
                    
                // Add enemies to the list
                if(p.TeamIndex != TeamIndex && !_enemiesInRange.Contains(cols[i].gameObject))
                {
                    _enemiesInRange.Add(cols[i].gameObject);   
                    // Add allies to the list
                } else if (p.TeamIndex == TeamIndex && p != this)
                {
                    _alliesInRange.Add(cols[i].gameObject);
                }
            }

            // Cast ultimates
            if (_enemiesInRange.Count > 0)
            {
                UltimateController.TryCastUltimate();
            }
        }

        public override void FixedUpdateNetwork()
        {
            //don't execute anything if the game is over already,
            //but termine the agent and path finding routines
            if(GameManager.IsGameOver())
            {
                agent.isStopped = true;
                StopAllCoroutines();
                enabled = false;
                return;
            }
            
            //don't continue if this bot is marked as dead
            if(!IsAlive) return;

            // CheckForStuckInPosition();

            //no enemy players are in range
            if(_enemiesInRange.Count == 0)
            {
                //if this bot reached the the random point on the navigation mesh,
                //then calculate another random point on the navmesh on continue moving around
                //with no other players in range, the AI wanders from team spawn to team spawn
                // EXPERIMENTAL UPDATE to seek out specific spots instead
                if(Vector3.Distance(transform.position, targetPoint) < agent.stoppingDistance)
                {
                    PickRandomLocation();
                }
            }
            else
            {
                //if we reached the targeted point, calculate a new point around the enemy
                //this simulates more fluent "dancing" movement to avoid being shot easily
                if(Vector3.Distance(shotPos.position, targetPoint) < agent.stoppingDistance)
                {
                    RandomPoint(_enemiesInRange[0].transform.position, range * 2, out targetPoint);
                }
                
                //shooting loop 
                for(int i = 0; i < _enemiesInRange.Count; i++)
                {
                    RaycastHit hit;
                    //raycast to detect visible enemies and shoot at their current position
                    if (Physics.Linecast(shotPos.position, _enemiesInRange[i].transform.position, out hit))
                    {
                        //get current enemy position and rotate this turret
                        Vector3 lookPos = _enemiesInRange[i].transform.position;
                        gameObject.transform.LookAt(lookPos);
                        gameObject.transform.eulerAngles = new Vector3(0, turret.eulerAngles.y, 0);
                        turretRotation = (short)turret.eulerAngles.y;

                        //find shot direction and shoot there
                        // Vector3 shotDir = lookPos - shotPos.position;
                        // Vector3 shotDirError = new Vector2(shotDir.x /*+ CalculateAccuracyError()*/,
                        //     shotDir.z/* + CalculateAccuracyError()*/);
                        CombatController.AttemptToShoot();
                        return;
                    }
                }
            }
            
            // Shoot at an ally 
            if (_alliesInRange.Count > 0 && CanBuff())
            {
                for(int i = 0; i < _alliesInRange.Count; i++)
                {
                    RaycastHit hit;
                    //raycast to detect visible allies and shoot at their current position
                    if (Physics.Linecast(shotPos.position, _alliesInRange[i].transform.position, out hit))
                    {
                        //get current ally position and rotate this turret
                        Vector3 lookPos = _alliesInRange[i].transform.position;
                        gameObject.transform.LookAt(lookPos);
                        gameObject.transform.eulerAngles = new Vector3(0, turret.eulerAngles.y, 0);
                        turretRotation = (short)turret.eulerAngles.y;

                        //find shot direction and shoot there
                        // Vector3 shotDir = lookPos - shotPos.position;
                        // Vector3 shotDirError = new Vector2(shotDir.x + CalculateAccuracyError(),
                        //     shotDir.z + CalculateAccuracyError());
                        CombatController.AttemptToShoot();
                        _lastBuffS = Runner.SimulationTime;
                        return;
                    }
                }
            }
        }

        private bool CanBuff()
        {
            return Runner.SimulationTime - _lastBuffS > buffFrequencyS;
        }

        private float CalculateAccuracyError()
        {
            return Random.Range(-accuracyError, accuracyError);
        }

        //disable rendering or blocking components
        void ToggleComponents(bool state)
        {
            GetComponent<Rigidbody>().isKinematic = false; //state;
            GetComponent<Collider>().enabled = state;

            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(state);
        }
        
        // Draw navmesh path
        private void OnDrawGizmos()
        {
            if (agent == null || agent.path == null) return;

            var corners = agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(corners[i], corners[i + 1]);
            }
        }
    }
}
