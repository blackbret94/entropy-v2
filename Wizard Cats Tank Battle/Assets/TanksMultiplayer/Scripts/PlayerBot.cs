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
using Vashta.Entropy.SaveLoad;
using Vashta.Entropy.UI;

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

        /// <summary>
        /// Radius in units for detecting other players.
        /// </summary>
        public float range = 6f;

        [Range(0f, 10f)]
        public float accuracyError = 0f;

        public CatNameGenerator CatNameGenerator;
        
        //list of enemy players that are in range of this bot
        private List<GameObject> inRange = new List<GameObject>();

        //reference to the agent component
        private NavMeshAgent agent;

        //current destination on the navigation mesh
        private Vector3 targetPoint;

        //timestamp when next shot should happen
        private float nextShot;

        //toggle for update logic
        private bool isDead = false;


        //called before SyncVar updates
        void Start()
        {
            _playerCurrencyRewarder = new PlayerCurrencyRewarder(false);
            
            //get components and set camera target
            camFollow = Camera.main.GetComponent<FollowTarget>();
            agent = GetComponent<NavMeshAgent>();
            agent.speed = moveSpeed;

            //get corresponding team and colorize renderers in team color
            targetPoint = GameManager.GetInstance().GetSpawnPosition(GetView().GetTeam());
            agent.Warp(targetPoint);

            Team team = GameManager.GetInstance().teams[GetView().GetTeam()];
            CharacterAppearance.Team = team;
            CharacterAppearance.ColorizeCart();

            //set name in label
            label.text = myName = CatNameGenerator.GetRandomName();
            label.color = team.material.color;
            
            rb = GetComponent<Rigidbody>();
           
            
            //call hooks manually to update
            OnHealthChange(GetView().GetHealth());
            OnShieldChange(GetView().GetShield());
            
            // add to player bot list
            GameManager.GetInstance().AddBot(this);

            RefreshSlider();
            
            //start enemy detection routine
            StartCoroutine(DetectPlayers());
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
                inRange.Clear();

                //casts a sphere to detect other player objects within the sphere radius
                Collider[] cols = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Player"));
                //loop over players found within bot radius
                for (int i = 0; i < cols.Length; i++)
                {
                    //get other Player component
                    //only add the player to the list if its not in this team
                    Player p = cols[i].gameObject.GetComponent<Player>();
                    if(p.GetView().GetTeam() != GetView().GetTeam() && !inRange.Contains(cols[i].gameObject))
                    {
                        inRange.Add(cols[i].gameObject);   
                    }
                }
                
                //wait a second before doing the next range check
                yield return new WaitForSeconds(1);
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
        
        protected override void SlowUpdate()
        {
            base.SlowUpdate();
            
            // adjust speed
            float speed = ((moveSpeed + StatusEffectController.MovementSpeedModifier) *
                           StatusEffectController.MovementSpeedMultiplier);
            agent.speed = speed;
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
            if(isDead) return;

            //stat visualization does not update automatically
            OnHealthChange(health);
            OnShieldChange(shield);

            //no enemy players are in range
            if(inRange.Count == 0)
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
                if(Vector3.Distance(transform.position, targetPoint) < agent.stoppingDistance)
                {
                    RandomPoint(inRange[0].transform.position, range * 2, out targetPoint);
                }
                
                //shooting loop 
                for(int i = 0; i < inRange.Count; i++)
                {
                    RaycastHit hit;
                    //raycast to detect visible enemies and shoot at their current position
                    if (Physics.Linecast(transform.position, inRange[i].transform.position, out hit))
                    {
                        //get current enemy position and rotate this turret
                        Vector3 lookPos = inRange[i].transform.position;
                        gameObject.transform.LookAt(lookPos);
                        gameObject.transform.eulerAngles = new Vector3(0, turret.eulerAngles.y, 0);
                        turretRotation = (short)turret.eulerAngles.y;

                        //find shot direction and shoot there
                        Vector3 shotDir = lookPos - transform.position;
                        Vector3 shotDirError = new Vector2(shotDir.x + CalculateAccuracyError(),
                            shotDir.z + CalculateAccuracyError());
                        Shoot(shotDirError);
                        break;
                    }
                }
            }
        }

        private float CalculateAccuracyError()
        {
            return Random.Range(-accuracyError, accuracyError);
        }
        
        /// <summary>
        /// Override of the base method to handle bot respawn separately.
        /// </summary>
        [PunRPC]
        protected override void RpcRespawn(short senderId, int killingBlowBulletId)
        {
            StartCoroutine(Respawn(senderId, killingBlowBulletId));
        }

        //the actual respawn routine
        IEnumerator Respawn(short senderId, int killingBlowBulletId)
        {   
            
            //stop AI updates
            isDead = true;
            inRange.Clear();
            agent.isStopped = true;
            killedBy = null;

            //find original sender game object (killedBy)
            PhotonView senderView = senderId > 0 ? PhotonView.Find(senderId) : null;
            if (senderView != null && senderView.gameObject != null) killedBy = senderView.gameObject;
            
            // Player killedByPlayer = killedBy.GetComponent<Player>();

            //detect whether the current user was responsible for the kill
            //yes, that's my kill: increase local kill counter
            if (killedBy == GameManager.GetInstance().localPlayer.gameObject) // This might be a problem if it is run on EVERY device
            {
                Text[] killCounter = GameManager.GetInstance().ui.killCounter;
                killCounter[0].text = GetView().GetKills().ToString();
                killCounter[0].GetComponent<Animator>().Play("Animation");
                
                RewardCoins();
            }

            Bullet killingBlowBullet = BulletDictionary[killingBlowBulletId];
            SpawnDeathFx(killingBlowBullet);

            //play sound clip on player death
            // if(explosionClip) AudioManager.Play3D(explosionClip, transform.position);
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

            //respawn and continue with pathfinding
            targetPoint = GameManager.GetInstance().GetSpawnPosition(GetView().GetTeam());
            transform.position = targetPoint;
            agent.Warp(targetPoint);
            agent.isStopped = false;
            isDead = false;
        }
        
        private void SpawnDeathFx(Bullet killingBlowBullet)
        {
            GameObject deathFx = null;
            
            if(killingBlowBullet != null)
                deathFx = killingBlowBullet.deathFx;
                
            if (deathFx == null)
                deathFx = StatusEffectController.GetDeathFx();

            if(deathFx != null)
                PoolManager.Spawn(deathFx, transform.position, transform.rotation);
        }


        //disable rendering or blocking components
        void ToggleComponents(bool state)
        {
            GetComponent<Rigidbody>().isKinematic = false; //state;
            GetComponent<Collider>().enabled = state;

            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(state);
        }
    }
}
