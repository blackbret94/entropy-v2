using Fusion;
using TanksMP;
using Unity.Mathematics;
using UnityEngine;
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
                    RPC_Shoot(_playerController.turretRotation);
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
        public void RPC_Shoot(short angle)
        {
            // animate
            _playerAnimator.Attack();
            
            //calculate center between shot position sent and current server position (factor 0.6f = 40% client, 60% server)
            //this is done to compensate network lag and smoothing it out between both client/server positions
            Vector3 shotCenter = _shotPos.position;
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
            // Projectiles.Add(Runner, new ProjectileState(shotCenter, syncedRot.eulerAngles, playerClass.classId), 5);

            // Spray.  Only handles 3 projectiles right now
            if (_statusEffectController.AdditionalProjectilesSpray > 0)
            {
                // shoot left
                Quaternion leftProjectile = Quaternion.Euler(0, angle - 5, 0);
                // Projectiles.Add(Runner, new ProjectileState(shotCenter, leftProjectile.eulerAngles, playerClass.classId, .66f), 0);
                _projectileFactory.SpawnProjectile(shotCenter, leftProjectile, playerClass, .66f);
                
                // shoot right
                Quaternion rightProjectile = Quaternion.Euler(0, angle + 5, 0);
                // Projectiles.Add(Runner, new ProjectileState(shotCenter, rightProjectile.eulerAngles, playerClass.classId, .66f), 0);
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
                // killed the player
                KillPlayer(other, deathFxId);
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
                //bullet killed the player
                KillPlayer(
                    projectile.owner.GetComponent<PlayerController>(), 
                    projectile.DeathFx.SessionId);
            else
            {
                //we didn't die, set health to new value
                _playerController.SetHealth(health);
                _playerController.PlayerViewController.ShowDamageText(damage, attackerIsCounter, attackerIsSame);
            }
        }
        
        // A simple command that ignores the player's health and just kills them.  Useful for respawning on class or team change.
        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        public void RPCKillPlayerForRespawn()
        {
            _playerController.SetHealth(0);
            _playerController.SetShield(0);
            KillPlayer(null);
        }
        
        // The main Kill Player method
        public void KillPlayer(PlayerController other, ushort deathFxId = 0)
        {
            if (HasStateAuthority)
            {
                // Create death struct here
                _playerController.PlayerDeathStruct = new PlayerDeathStruct(other != null ? other.PlayerId : new PlayerRef(), deathFxId, Runner.SimulationTime);
            }

            _gameManager.TeamController.OnePassPlayerCheckToChangeTeams(_playerController, false);
            
            //get killer and increase score for that enemy team
            if (other != null)
            {
                // Reflect damage on killer if blood pact is active
                _statusEffectController.BloodPact(other);
                
                int otherTeam = other.TeamIndex;
                
                // killer is other team
                if (_playerController.TeamIndex != otherTeam)
                {
                    _gameManager.TeamController.AddScore(ScoreType.Kill, otherTeam);
                    other.Kills++;
                }
                
                //the maximum score has been reached now
                if (_gameManager.IsGameOver())
                {
                    //tell all clients the winning team
                    _gameManager.GameOverController.RPCGameOver((byte)otherTeam);
                    // return;
                }
            }
            else if(!_playerController.RespawnIsFreeFromJointime())
            {
                // Killed by environment
                _gameManager.TeamController.RemoveScore(ScoreType.Kill, _playerController.TeamIndex);
            }
            
            // The game is not over
            _playerController.PlayerDeath(other, deathFxId);
        }
    }
}