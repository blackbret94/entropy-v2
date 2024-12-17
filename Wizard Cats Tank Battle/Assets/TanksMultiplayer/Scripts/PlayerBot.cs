/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using System.Collections;
using System.Collections.Generic;
using Entropy.Scripts.Player;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using UnityEngine.UI;
using Vashta.Entropy.UI;
using Vashta.Entropy.World;

namespace TanksMP
{          
    /// <summary>
    /// Implementation of AI bots by overriding methods of the Player class.
    /// </summary>
	public class PlayerBot : Player
    {
        //custom properties per PhotonPlayer do not work in offline mode
        //(actually they do, but for objects spawned by the master client,
        //PhotonPlayer is always the local master client. This means that
        //setting custom player properties would apply to all objects)
        [HideInInspector] public string myName;
        [HideInInspector] public int teamIndex;
        [HideInInspector] public int health;
        [HideInInspector] public int shield;
        [HideInInspector] public int ammo;
        [HideInInspector] public int currentBullet;
        [HideInInspector] public int kills;
        [HideInInspector] public int deaths;
        [HideInInspector] public float joinTime;
        [HideInInspector] public int classId;
        [HideInInspector] public int classIdQueued;
        [HideInInspector] public int ultimate;
        [HideInInspector] public int powerup;
        
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
        private NavMeshAgent agent;

        //current destination on the navigation mesh
        private Vector3 targetPoint;

        //timestamp when next shot should happen
        private float nextShot;

        private float _slowUpdateRate = .5f;
        private float _lastUpdateTime;


        //called before SyncVar updates
        void Start()
        {
            isBot = true;
            _playerCurrencyRewarder = new PlayerCurrencyRewarder();
            GameManager = GameManager.GetInstance();
   
            agent = GetComponent<NavMeshAgent>();
            agent.speed = moveSpeed;

            //get corresponding team and colorize renderers in team color
            targetPoint = GameManager.GetInstance().GetSpawnPosition(GetView().GetTeam());
            agent.Warp(targetPoint);

            Team team = GameManager.GetInstance().teams[GetView().GetTeam()];
            CharacterAppearance.Team = team;
            CharacterAppearance.ColorizeCart();
            
            myName = CatNameGenerator.GetRandomName();
            PlayerViewController.SetName(myName);
            
            PlayerViewController.SetTeam(team.teamDefinition);
            
            GameManager.ui.GameLogPanel.EventPlayerJoined(GetName());
            rb = GetComponent<Rigidbody>();
            
            //call hooks manually to update
            OnHealthChange(GetView().GetHealth());
            OnShieldChange(GetView().GetShield());
            
            // add to player bot list
            GameManager.GetInstance().AddBot(this);

            PlayerViewController.RefreshHealthSlider();
            
            //start enemy detection routine
            StartCoroutine(DetectPlayers());

            _slowUpdateRate += Random.Range(0f, .25f);
            
            if (PhotonNetwork.IsMasterClient)
            {
                // Apply status effect
                if (StatusEffectApplyOnSpawn)
                {
                    StatusEffectController.AddStatusEffect(StatusEffectApplyOnSpawn.Id, this);
                }
            }
        }
        
        
        //sets inRange list for player detection
        IEnumerator DetectPlayers()
        {
            //wait for initialization
            yield return new WaitForEndOfFrame();
            
            //detection logic
            while(true)
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
                    Player p = cols[i].gameObject.GetComponent<Player>();
                    
                    // Add enemies to the list
                    if(p.GetView().GetTeam() != GetView().GetTeam() && !_enemiesInRange.Contains(cols[i].gameObject))
                    {
                        _enemiesInRange.Add(cols[i].gameObject);   
                    // Add allies to the list
                    } else if (p.GetView().GetTeam() == GetView().GetTeam() && p != this)
                    {
                        _alliesInRange.Add(cols[i].gameObject);
                    }
                }
                
                //wait a second before doing the next range check
                yield return new WaitForSeconds(1);
            }
        }

        public override string GetName()
        {
            return myName;
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
                    if (!PathfindingZone.PointIsInRestrictedZone(randomPoint, teamIndex))
                    {
                        result = hit.position;
                        break;
                    }
                }
            }
            
            //set the target point as the new destination
            agent.SetDestination(result);
        }
        
        protected override void StatusEffectTick()
        {
            base.StatusEffectTick();
            
            // adjust speed
            float speed = ((moveSpeed + StatusEffectController.MovementSpeedModifier) *
                           StatusEffectController.MovementSpeedMultiplier);
            agent.speed = speed;
        }

        protected override void Update()
        {
            base.Update();

            if (Time.time >= _lastUpdateTime + _slowUpdateRate)
            {
                SlowUpdate();
            }
        }

        // Cast ultimates
        void SlowUpdate()
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
                Player p = cols[i].gameObject.GetComponent<Player>();
                    
                // Add enemies to the list
                if(p.GetView().GetTeam() != GetView().GetTeam() && !_enemiesInRange.Contains(cols[i].gameObject))
                {
                    _enemiesInRange.Add(cols[i].gameObject);   
                    // Add allies to the list
                } else if (p.GetView().GetTeam() == GetView().GetTeam() && p != this)
                {
                    _alliesInRange.Add(cols[i].gameObject);
                }
            }

            if (_enemiesInRange.Count > 0)
            {
                TryCastUltimate();
            }

            _lastUpdateTime = Time.time;
        }

        void FixedUpdate()
        {
            //don't execute anything if the game is over already,
            //but termine the agent and path finding routines
            if(GameManager.GetInstance().IsGameOver())
            {
                agent.isStopped = true;
                StopAllCoroutines();
                enabled = false;
                return;
            }
            
            //don't continue if this bot is marked as dead
            if(!IsAlive) return;

            //stat visualization does not update automatically
            OnHealthChange(health);
            OnShieldChange(shield);

            //no enemy players are in range
            if(_enemiesInRange.Count == 0)
            {
                //if this bot reached the the random point on the navigation mesh,
                //then calculate another random point on the navmesh on continue moving around
                //with no other players in range, the AI wanders from team spawn to team spawn
                // EXPERIMENTAL UPDATE to seek out specific spots instead
                if(Vector3.Distance(transform.position, targetPoint) < agent.stoppingDistance)
                {
                    List<GameObject> possibleTargets = GameManager.GetInstance().BotTargetList;
                    RandomPoint(possibleTargets[Random.Range(0, possibleTargets.Count)].transform.position, range, out targetPoint);
                    // int teamCount = GameManager.GetInstance().teams.Length;
                    // RandomPoint(GameManager.GetInstance().teams[Random.Range(0, teamCount)].spawn.position, range, out targetPoint);
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
                        Shoot();
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
                        Shoot();
                        _lastBuffS = Time.time;
                        return;
                    }
                }
            }
        }

        private bool CanBuff()
        {
            return Time.time - _lastBuffS > buffFrequencyS;
        }

        private float CalculateAccuracyError()
        {
            return Random.Range(-accuracyError, accuracyError);
        }
        
        /// <summary>
        /// Override of the base method to handle bot respawn separately.
        /// </summary>
        [PunRPC]
        protected override void RpcRespawn(short senderId, string deathFxId)
        {
            StartCoroutine(Respawn(senderId, deathFxId));
        }

        //the actual respawn routine
        IEnumerator Respawn(short senderId, string deathFxId)
        {   
            
            //stop AI updates
            IsAlive = false;
            _enemiesInRange.Clear();
            agent.isStopped = true;
            killedBy = null;

            //find original sender game object (killedBy)
            PhotonView senderView = senderId > 0 ? PhotonView.Find(senderId) : null;
            if (senderView != null && senderView.gameObject != null) killedBy = senderView.gameObject;
            
            Player killedByPlayer = killedBy.GetComponent<Player>();

            if (killedByPlayer)
            {
                killedByPlayer.RewardUltimateForKill();
                GameManager.ui.GameLogPanel.EventPlayerKilled(GetName(), GetTeamDefinition(), killedByPlayer.GetName(), killedByPlayer.GetTeamDefinition());
            }

            //detect whether the current user was responsible for the kill
            //yes, that's my kill: increase local kill counter
            if (killedBy == PlayerList.GetLocalPlayer().gameObject) // This might be a problem if it is run on EVERY device
            {
                RewardCoinsForKill();
            }
            
            SpawnDeathFx(deathFxId);
            
            // Mark dead on minimap
            if (MinimapEntityControllerPlayer)
            {
                MinimapEntityControllerPlayer.RenderAsDead();
            }

            //play sound clip on player death
            if (killedBy != null)
            {
                Player player = killedBy.GetComponent<Player>();
                
                if (player != null)
                {
                    AudioManager.Play3D(player.CharacterAppearance.Meow.AudioClip, transform.position);
                }
            }

            //toggle visibility for all rendering parts (off)
            ToggleComponents(false);
            //wait global respawn delay until reactivation
            yield return new WaitForSeconds(GameManager.GetInstance().respawnTime);
            //toggle visibility again (on)
            ToggleComponents(true);
            
            // Mark alive on minimap
            if (MinimapEntityControllerPlayer)
            {
                MinimapEntityControllerPlayer.RenderAsAlive();
            }

            //respawn and continue with pathfinding
            targetPoint = GameManager.GetInstance().GetSpawnPosition(GetView().GetTeam());
            transform.position = targetPoint;
            agent.Warp(targetPoint);
            agent.isStopped = false;
            IsAlive = true;

            if (PhotonNetwork.IsMasterClient)
            {
                // Apply status effect
                if (StatusEffectApplyOnSpawn)
                {
                    StatusEffectController.AddStatusEffect(StatusEffectApplyOnSpawn.Id, this);
                }
            }
        }

        //disable rendering or blocking components
        void ToggleComponents(bool state)
        {
            GetComponent<Rigidbody>().isKinematic = false; //state;
            GetComponent<Collider>().enabled = state;

            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(state);
        }

        private void OnDestroy()
        {
            GameManager.ui.GameLogPanel.EventPlayerLeft(GetName());
        }
    }
}
