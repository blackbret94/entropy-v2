using System;
using UnityEngine;
using System.Collections.Generic;
using Entropy.Scripts.Player;
using Fusion;
using FusionHelpers;
using UnityEngine.Serialization;
using Vashta.Entropy.GameMode;
using Vashta.Entropy.Player;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.Spells;
using Vashta.Entropy.StatusEffects;
using Random = UnityEngine.Random;

namespace TanksMP
{
    /// <summary>
    /// Projectile script for player shots with collision/hit logic.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        // Eventually break this out into a scriptable object
        [FormerlySerializedAs("bulletId")] public int projectileId = 0;

        public float Speed { get; private set; }

        /// <summary>
        /// Damage to cause on a player that gets hit.
        /// </summary>
        private int _damage = 3;
        [SerializeField][Tooltip("Unmodified damage value, reset upon spawn")]
        private int _damageRaw = 3;

        /// <summary>
        /// Delay until despawned automatically when nothing gets hit.
        /// </summary>
        public float despawnDelay = 1f;

        /// <summary>
        /// Bounce count of walls and other environment obstactles.  How many left until destruction
        /// </summary>
        public int bounce = 0;

        /// <summary>
        /// Maximum amount of Players this bullet can hit on explosion.
        /// </summary>
        [FormerlySerializedAs("maxTargets")] 
        public int maxTargetsBase = 1;

        /// <summary>
        /// Range within the explosion deals damage to other Players.
        /// The area is only checked if maxTargets is greater than 1.
        /// </summary>
        [FormerlySerializedAs("explosionRange")] 
        public float explosionRangeBase = 1;
        public bool bounceInf;

        [Header("Class Mechanics")] // Eventually this will be broken out into specific skills
        public StatusEffectData StatusEffectOnEnemy;
        [Range(0,1)]
        public float StatusEffectOnEnemyChance;
        public StatusEffectData StatusEffectOnAlly;
        [Range(0,1)]
        public float StatusEffectOnAllyChance;

        //reference to rigidbody component
        private Rigidbody myRigidbody;
        //reference to collider component
        private SphereCollider sphereCol;

        //caching last bounce position for calculating next direction. Instead of using
        //the current bullet position on collision, calculating the bounce off the previous
        //bullet position improves the result for high speed bullets which could skip colliders
        private Vector3 lastBouncePos;
        public ProjectileData ProjectileData;
        public ClassDefinition ClassDefinition { set; get; }
        public ClassDirectory classDirectory;
        public VisualEffectData DeathFx => ProjectileData ? ProjectileData.deathFxData : null;

        /// <summary>
        /// Player gameobject that spawned this projectile.
        /// </summary>
        [HideInInspector] public GameObject owner;
        /// <summary>
        /// This missile can buff allies
        /// </summary>
        [HideInInspector] public bool canBuff = true;
        /// <summary>
        /// This missile can debuff enemies
        /// </summary>
        [HideInInspector] public bool canDebuff = true;
        private float _timeCreated;
        private string _lastUUID = "";
        private const float OwnerProtectionTime = 1f;
        
        private float _modifiedDespawnDelay;
        private float _modifiedExplosionRange;
        private int _modifiedMaxBounce;
        private int _modifiedMaxTargets;
        private bool _isPiercing;

        private List<PlayerController> _playerCollidedWith = new List<PlayerController>();

        private bool OwnerIsProtected => Time.time - _timeCreated < OwnerProtectionTime;

        //get component references
        void Awake()
        {
            myRigidbody = GetComponent<Rigidbody>();
            sphereCol = GetComponent<SphereCollider>();
        }

        public int GetRawDamage()
        {
            return _damageRaw;
        }

        public int GetDamage()
        {
            return _damage;
        }

        public void SetDamage(int newDamage)
        {
            _damage = newDamage;
        }

        public float GetBaseSpeed()
        {
            return ProjectileData.BaseSpeed;
        }
        
        public void SetSpeed(float newSpeed)
        {
            myRigidbody.linearVelocity = newSpeed * transform.forward;
        }
        
        public int GetId()
        {
            return projectileId;
        }

        public void SpawnNewBullet()
        {
            _timeCreated = Time.time;
            
            // Reset modifiers
            _modifiedExplosionRange = explosionRangeBase;
            _modifiedDespawnDelay = despawnDelay;
            _modifiedMaxBounce = ProjectileData.BaseBounces;
            _modifiedMaxTargets = maxTargetsBase;
            _damage = _damageRaw;
            _isPiercing = false;
            _playerCollidedWith.Clear();
        }

        //set initial travelling velocity
        //On Host, add automatic despawn coroutine
        void OnSpawn()
        {
            // Check if it is the initial path or if it has bounced
            if (bounce == _modifiedMaxBounce)
            {
                Vector3 pos = transform.position;
                
                //for bouncing bullets, save current position only on first spawn (turret position)
                lastBouncePos = pos;
                
                //play cast sound
                if (ProjectileData != null)
                {
                    AudioManager.Play3D(ProjectileData.CastSfx.GetRandomClip(), pos);
                }
            }
            else
            {
                if (ProjectileData != null)
                {
                    if(ProjectileData.BounceSfx) 
                        AudioManager.Play3D(ProjectileData.BounceSfx.GetRandomClip(), transform.position);
                }
            }

            myRigidbody.linearVelocity = ProjectileData.BaseSpeed * transform.forward;
        }
        
        public void IncreaseDespawnDelay(float delay)
        {
            _modifiedDespawnDelay = despawnDelay + delay;
        }

        public void SetExplosionRange(int newRange)
        {
            _modifiedExplosionRange = newRange;
        }

        public void SetMaxBounce(int newMaxBounce)
        {
            _modifiedMaxBounce = newMaxBounce;
            bounce = newMaxBounce;
        }

        public void SetMaxTargets(int newMaxTargets)
        {
            _modifiedMaxTargets = newMaxTargets;
        }

        public void SetPiercing(bool pierces)
        {
            _isPiercing = pierces;
        }

        private void CheckRespawn()
        {
            float despawnTime = _timeCreated + _modifiedDespawnDelay;

            if (Time.time >= despawnTime)
            {
                PoolManager.Despawn(gameObject);
            }
        }

        private void Update()
        {
            CheckRespawn();
        }

        ///check what was hit on collisions. Only do non-critical client work here,
        //not even accessing player variables or anything like that. The server side is separate below
        void OnTriggerEnter(Collider col)
        {
            //cache corresponding gameobject that was hit
            GameObject obj = col.gameObject;
            Collide(obj);
        }

        public void Collide(GameObject obj)
        {
            // see if we hit a spell field
            SpellField spellField = obj.GetComponent<SpellField>();
            if (spellField)
                return;
            
            // see if we hit a control point
            ControlPoint controlPoint = obj.GetComponent<ControlPoint>();
            if (controlPoint)
                return;
            
            //try to get a player component out of the collided gameobject
            PlayerController playerController = obj.GetComponent<PlayerController>();

            //we actually hit a player
            //do further checks
            if (playerController != null)
            {
                if (HasHitProtectedOwner(playerController))
                {
                    return;
                }

                // Handle reflection
                if (playerController.StatusEffectController.IsReflective)
                {
                    BounceOffReflectivePlayer(playerController);
                    return;
                }

                // Handle piercing
                if (_isPiercing)
                {
                    if (HasAlreadyHitPlayer(playerController))
                    {
                        return;
                    }
                    else
                    {
                        _playerCollidedWith.Add(playerController);
                    }
                }
                else
                {
                    //despawn gameobject
                    PoolManager.Despawn(gameObject);
                }

                //create clips and particles on hit
                if (ProjectileData && ProjectileData.HitFx) 
                    PoolManager.Spawn(ProjectileData.HitFx, transform.position, Quaternion.identity);

                if (ProjectileData != null)
                {
                    AudioManager.Play3D(ProjectileData.HitSfx.GetRandomClip(), transform.position);
                }
            }
            else
            {
                if (bounce > 0 || bounceInf)
                {
                    //a player was not hit but something else, and we still have some bounces left
                    //create a ray that points in the direction this bullet is currently flying to
                    Ray ray = new Ray(lastBouncePos - transform.forward * 0.5f, transform.forward);
                    RaycastHit hit;

                    //perform spherecast in the flying direction, on the default layer
                    if (Physics.SphereCast(ray, sphereCol.radius, out hit, Mathf.Infinity, 1 << 0))
                    {
                        //ignore multiple collisions i.e. inside colliders
                        if (Vector3.Distance(transform.position, lastBouncePos) < 0.05f)
                        {
                            return;
                        }

                        //cache latest collision point
                        lastBouncePos = hit.point;
                        //substract bouncing count by one
                        bounce--;

                        //something was hit in the direction this projectile is flying to
                        //get new reflected (bounced off) direction of the colliding object
                        Vector3 dir = Vector3.Reflect(ray.direction, hit.normal);
                        //rotate bullet to face the new direction
                        transform.rotation = Quaternion.LookRotation(dir);
                        //reassign velocity with the new direction in mind
                        OnSpawn();

                        //play clip at the collided position
                        if (ProjectileData != null)
                        {
                            AudioManager.Play3D(ProjectileData.HitSfx.GetRandomClip(), transform.position);
                        }

                        //exit execution until next collision
                        return;
                    }
                }

                //despawn gameobject
                PoolManager.Despawn(gameObject);
            }
            
            //create list for affected players by this bullet and add the collided player immediately,
            //we have done validation & friendly fire checks above already
            List<PlayerController> targets = new List<PlayerController>();
            if(playerController != null) targets.Add(playerController);

            //in case this bullet can hit more than 1 target, perform the additional physics area check
            if (_modifiedMaxTargets > 1)
            {
                // Debug.Log("Checking for targets: " + _modifiedMaxTargets + " with range " + _modifiedExplosionRange);
                //find all colliders in the specified range around this bullet, on the Player layer
                Collider[] others = Physics.OverlapSphere(transform.position, _modifiedExplosionRange, 1 << 8);

                //loop over all player collisions found
                for (int i = 0; i < others.Length; i++)
                {
                    //get Player component from that collision
                    PlayerController other = others[i].GetComponent<PlayerController>();
                    if (other == null || targets.Contains(other)) continue;

                    //add this Player component to the list
                    //cancel in case we do reach the maximum count now
                    targets.Add(other);

                    if (ProjectileData != null)
                    {
                        PoolManager.Spawn(ProjectileData.ExplosionFx, other.transform.position, transform.rotation);
                    }

                    if (targets.Count == _modifiedMaxTargets)
                        break;
                }
            }

            //apply damage and effects to the collided players
            if (owner != null)
            {
                PlayerController origin = owner.GetComponent<PlayerController>();
                for (int i = 0; i < targets.Count; i++)
                {
                    PlayerController target = targets[i];
                    if (HasHitProtectedOwner(target) || target.gameObject == null)
                        continue;

                    if (IsFriendlyFire(origin, target))
                    {
                        // Buff allies
                        if(canBuff)
                            AttemptApplyEffectAlly(origin, target);
                    }
                    else
                    {
                        // Debuff enemies
                        if(canDebuff)
                            AttemptApplyEffectEnemy(origin, target);
                    }

                    // apply damage
                    target.CombatController.TakeDamage(this);
                    
                    // increase ultimate for player
                    origin.UltimateController.IncreaseUltimate();
                }
            }
        }

        public int GetTeam()
        {
            PlayerController origin = owner.GetComponent<PlayerController>();
            if (!origin)
                return -1;

            return origin.TeamIndex;
        }

        private void AttemptApplyEffectAlly(PlayerController playerController, PlayerController target)
        {
            if(Random.Range(0f, 1f) < StatusEffectOnAllyChance)
                target.ApplyStatusEffect(StatusEffectOnAlly.Id, playerController);
        }

        private void AttemptApplyEffectEnemy(PlayerController playerController, PlayerController target)
        {
            if(Random.Range(0f, 1f) < StatusEffectOnEnemyChance)
                target.ApplyStatusEffect(StatusEffectOnEnemy.Id, playerController);
        }
        
        private void BounceOffReflectivePlayer(PlayerController playerController)
        {
            //a player was not hit but something else, and we still have some bounces left
            //create a ray that points in the direction this bullet is currently flying to
            Ray ray = new Ray(lastBouncePos - transform.forward * 0.5f, transform.forward);
            RaycastHit hit;

            //perform spherecast in the flying direction, on the default layer
            if (Physics.SphereCast(ray, sphereCol.radius, out hit, Mathf.Infinity, 1 << 0))
            {
                //ignore multiple collisions i.e. inside colliders
                if (Vector3.Distance(transform.position, lastBouncePos) < 0.05f)
                {
                    return;
                }

                owner = playerController.gameObject;

                //cache latest collision point
                lastBouncePos = hit.point;
                //substract bouncing count by one
                // bounce--;

                //something was hit in the direction this projectile is flying to
                //get new reflected (bounced off) direction of the colliding object
                Vector3 dir = Vector3.Reflect(ray.direction, hit.normal);
                //rotate bullet to face the new direction
                transform.rotation = Quaternion.LookRotation(dir);
                //reassign velocity with the new direction in mind
                OnSpawn();

                //play clip at the collided position
                if (ProjectileData != null)
                {
                    AudioManager.Play3D(ProjectileData.HitSfx.GetRandomClip(), transform.position);
                }
                //exit execution until next collision
            }
        }


        //set despawn effects and reset variables
        void OnDespawn()
        {
            //create clips and particles on despawn
            if (ProjectileData && ProjectileData.ExplosionFxLarge && _modifiedExplosionRange > 1)
            {
                PoolManager.Spawn(ProjectileData.ExplosionFxLarge, transform.position, transform.rotation);
            } else if (ProjectileData && ProjectileData.ExplosionFx)
            {
                PoolManager.Spawn(ProjectileData.ExplosionFx, transform.position, transform.rotation);
            }

            //reset modified variables to the initial state
            myRigidbody.linearVelocity = Vector3.zero;
            myRigidbody.angularVelocity = Vector3.zero;
            bounce = ProjectileData.BaseBounces;
        }
        
        //method to check for friendly fire (same team index).
        private bool IsFriendlyFire(PlayerController origin, PlayerController target)
        {
            //do not trigger damage for colliding with our own bullet
            if (target.gameObject == owner || target.gameObject == null) return true;
            
            //perform the actual friendly fire check on both team indices and see if they match
            if (origin.TeamIndex == target.TeamIndex) return true;
            
            return false;
        }

        private bool HasHitProtectedOwner(PlayerController target)
        {
            return target.gameObject == owner && OwnerIsProtected;
        }

        private bool HasAlreadyHitPlayer(PlayerController playerController)
        {
            return _playerCollidedWith.Contains(playerController);
        }
    }
}
