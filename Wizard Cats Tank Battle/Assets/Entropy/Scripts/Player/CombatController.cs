using Fusion;
using TanksMP;
using Unity.Mathematics;
using UnityEngine;
using Vashta.Entropy.Network;
using Vashta.Entropy.Player;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.StatusEffects;

namespace Entropy.Scripts.Player
{
    public class CombatController : NetworkBehaviour
    {
        [Header("Modifiers")]
        public int counterDamageMod = 2;
        public int sameClassDamageMod = -1;

        [Header("References")] 
        [SerializeField] private Projectile ProjectilePrefab;
        
        [Header("Controllers")]
        private PlayerController _playerController;
        private DeathController _deathController;
        private ProjectileFactory _projectileFactory;
        private PlayerAnimator _playerAnimator;
        
        private float nextFire;
        public float TimeToNextFire => nextFire - Time.time;
        public float FractionFireReady => Mathf.Min(1-(TimeToNextFire / _playerController.fireRate), 1);
        
        [Header("Cached references")]
        private StatusEffectController _statusEffectController;
        private Transform _shotPos;
        private Transform _turret;
        private GameManager _gameManager;
        
        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _playerAnimator = GetComponent<PlayerAnimator>();
            _deathController = GetComponent<DeathController>();
        }

        private void Start()
        {
            _statusEffectController = _playerController.StatusEffectController;
            _gameManager = GameManager.GetInstance();
            
            _projectileFactory = new ProjectileFactory(gameObject, _statusEffectController);

            _shotPos = _playerController.shotPos;
            _turret = _playerController.turret;
        }
        
        public int CalculateDamageTaken(Projectile projectile, out bool attackerIsCounter, out bool attackerIsSame)
        {
            float calculatedDamage = projectile.GetDamage();

            // Check class modifiers
            if (projectile.ClassDefinition == null)
            {
                Debug.LogWarning("Warning! No class definition assigned to bullet");
            }

            // disable temporarily
            attackerIsCounter = false;
            attackerIsSame = true;
            
            //attackerIsCounter = bullet.ClassDefinition.IsCounter(_player.photonView.GetClassId());
            //attackerIsSame = bullet.ClassDefinition.classId == _player.photonView.GetClassId();
            
            if (attackerIsCounter)
            {
                calculatedDamage += counterDamageMod;
            }
            
            if (attackerIsSame)
            {
                calculatedDamage += sameClassDamageMod;
            }
            
            // Check defense modifier
            calculatedDamage += _statusEffectController.DamageTakenModifier;
            
            // don't allow healing
            return Mathf.Max(0, Mathf.RoundToInt(calculatedDamage));
        }

        //shoots a bullet in the direction passed in
        //we do not rely on the current turret rotation here, because we send the direction
        //along with the shot request to the server to absolutely ensure a synced shot position
        public void AttemptToShoot()
        {
            float fireRateMod = _playerController.fireRate * _statusEffectController.AttackRateModifier;

            if (_statusEffectController.DisableFiring)
            {
                if(_playerController.IsLocal)
                    _gameManager.SfxController.PlayCantShoot(1f);
            }
            else
            {
                //if shot delay is over  
                if (Time.time > nextFire)
                {
                    //set next shot timestamp
                    nextFire = Time.time + fireRateMod;

                    //send current client position and turret rotation along to sync the shot position
                    //also we are sending it as a short array (only x,z - skip y) to save additional bandwidth
                    // short[] pos = new short[] { (short)(_shotPos.position.x * 10), (short)(_shotPos.position.z * 10) };
                    //send shot request with origin to server
                    // Debug.Log(turretRotation);
                    float castDelay = .25f;

                    Vector3 pos = _shotPos.position;
                    short x = (short)Mathf.RoundToInt(pos.x);
                    short z = (short)Mathf.RoundToInt(pos.z);
                    
                    short[] position = new short[] { x,z};
                    short angle = _playerController.turretRotation;
                    Shoot_RPC(position, angle);
                }
            }
        }
        
        // FOR JOYSTICKS
        //on shot drag start set small delay for first shot
        public void ShootBegin()
        {
            // Add delay to prevent firing before aiming.
            // This check ensures nextFire is not always overridden, which lead to the rapid fire exploit.
            if(Time.time > nextFire)
                nextFire = Time.time + 0.1f;
        }
        
        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        public void Shoot_RPC(short[] casterPosition, short angle)
        {
            // Ignore requests that arrive before the player is set up.  Should improve this later with a proper init
            if(!_shotPos || !_turret)
                return;
         
            // TODO: Interpolate between current position and sent position
            
            // animate
            _playerAnimator.Attack();
            
            //calculate center between shot position sent and current server position (factor 0.4f = 40% client, 60% server)
            //this is done to compensate network lag and smoothing it out between both client/server positions
            Vector3 shotPos = _shotPos.position;
            float xx = Mathf.Lerp(casterPosition[0], shotPos.x, .4f);
            float zz = Mathf.Lerp(casterPosition[1], shotPos.z, .4f);
            
            Vector3 shotCenter = new Vector3(xx, shotPos.y, zz);
            Quaternion syncedRot = _turret.rotation = Quaternion.Euler(0, angle, 0);

            ClassDefinition playerClass = _playerController.GetClass();
            
            // spawn casting vfx
            if (playerClass != null)
            {
                ProjectileData projectileData = playerClass.ProjectileData;
                if (projectileData != null && projectileData)
                {
                    GameObject castFx = projectileData.CastFx;

                    if (castFx != null)
                    {
                        PoolManager.Spawn(castFx, shotCenter + Vector3.up, quaternion.identity);
                    }
                }
            }

            //spawn bullet using pooling
            _projectileFactory.SpawnProjectile(shotCenter, syncedRot, playerClass);

            // Spray.  Only handles 3 projectiles right now
            if (_statusEffectController.AdditionalProjectilesSpray > 0)
            {
                // shoot left
                Quaternion leftProjectile = Quaternion.Euler(0, angle - 5, 0);
                _projectileFactory.SpawnProjectile(shotCenter, leftProjectile, playerClass, .66f);
                
                // shoot right
                Quaternion rightProjectile = Quaternion.Euler(0, angle + 5, 0);
                _projectileFactory.SpawnProjectile(shotCenter, rightProjectile, playerClass, .66f);
            }
        }
        
        /// <summary>
        /// Server only: calculate damage to be taken by the Player,
        /// triggers score increase and respawn workflow on death.
        /// </summary>
        public void TakeDamage(int damage, PlayerController other, bool canKill = true, ushort deathFxId = 0)
        {
            int health = _playerController.Health;
            int shield = _playerController.Shield;

            //reduce shield on hit
            if (shield > 0)
            {
                _playerController.SetShield(shield-1);
                return;
            }
            
            health -= damage;
            
            // Don't kill the player if this only brings them down to 1HP
            if (!canKill)
            {
                if (health <= 0)
                {
                    health = 1;
                }
            }

            if (health <= 0)
            {
                if (HasStateAuthority)
                {
                    // killed the player
                    _deathController.KillPlayerLocalDamage(other, deathFxId);
                }
            }
            else
            {
                //we didn't die, set health to new value
                _playerController.SetHealth(health);
                _playerController.PlayerViewController.ShowDamageText(damage, false, false);
            }
        }

        /// <summary>
        /// Server only: calculate damage to be taken by the Player,
		/// triggers score increase and respawn workflow on death.
        /// </summary>
        public void TakeDamage(Projectile projectile)
        {
            if (projectile == null)
            {
                Debug.LogError("Attempted to take damage from a null projectile");
            }
            
            // ignore damage to team mates
            if (_playerController.TeamIndex == projectile.owner.GetComponent<PlayerController>().TeamIndex)
                return;
            
            //store network variables temporary
            int health = _playerController.Health;
            int startHealth = health;
            int shield = _playerController.Shield;

            //reduce shield on hit
            if (shield > 0)
            {
                _playerController.SetShield(1);
                return;
            }

            //substract health by damage
            //locally for now, to only have one update later on
            int damage = CalculateDamageTaken(projectile, out bool attackerIsCounter, out bool attackerIsSame);
            
            // Debug.Log("Taking damage from bullet: " + damage);
            
            health -= damage;

            if (health <= 0)
            {
                // Only trigger death if local
                if (HasStateAuthority)
                {
                    //bullet killed the player
                    _deathController.KillPlayerLocalDamage(
                        projectile.owner.GetComponent<PlayerController>(),
                        projectile.DeathFx.SessionId);
                }
            }
            else
            {
                //we didn't die, set health to new value
                _playerController.SetHealth(health);

                if (startHealth != health)
                {
                    _playerController.PlayerViewController.ShowDamageText(damage, attackerIsCounter, attackerIsSame);
                }
            }
        }
    }
}