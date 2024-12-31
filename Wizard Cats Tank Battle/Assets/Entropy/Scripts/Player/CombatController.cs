using TanksMP;
using UnityEngine;
using Vashta.Entropy.StatusEffects;

namespace Entropy.Scripts.Player
{
    public class CombatController : MonoBehaviour
    {
        [Header("Modifiers")]
        public int counterDamageMod = 2;
        public int sameClassDamageMod = -1;
        
        [Header("Controllers")]
        private TanksMP.Player _player;
        private ProjectileFactory _projectileFactory;
        private PlayerAnimator _playerAnimator;
        
        private float nextFire;
        public float TimeToNextFire => nextFire - Time.time;
        public float FractionFireReady => Mathf.Min(1-(TimeToNextFire / _player.fireRate), 1);
        // death loop protections
        private const float minTimeBetweenDeaths = .5f;
        
        [Header("Cached references")]
        private StatusEffectController _statusEffectController;
        private Transform _shotPos;
        private Transform _turret;
        private GameManager _gameManager;
        
        private void Awake()
        {
            _player = GetComponent<TanksMP.Player>();
            _playerAnimator = GetComponent<PlayerAnimator>();
        }

        private void Start()
        {
            _statusEffectController = _player.StatusEffectController;
            _gameManager = GameManager.GetInstance();
            
            _projectileFactory = new ProjectileFactory(gameObject, _statusEffectController);

            _shotPos = _player.shotPos;
            _turret = _player.turret;
        }
        
        public int CalculateDamageTaken(Bullet bullet, out bool attackerIsCounter, out bool attackerIsSame)
        {
            float calculatedDamage = bullet.GetDamage();

            // Check class modifiers
            if (bullet.ClassDefinition == null)
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
        public void AttemptToShoot(Vector2 direction = default(Vector2))
        {
            float fireRateMod = _player.fireRate * _statusEffectController.AttackRateModifier;

            if (_statusEffectController.DisableFiring)
            {
                if(_player.IsLocal)
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
                    short[] pos = new short[] { (short)(_shotPos.position.x * 10), (short)(_shotPos.position.z * 10) };
                    //send shot request with origin to server
                    // Debug.Log(turretRotation);
                    _player.CombatController.Shoot(pos, _player.turretRotation);
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
        
        public void Shoot(short[] position, short angle)
        {
            //calculate center between shot position sent and current server position (factor 0.6f = 40% client, 60% server)
            //this is done to compensate network lag and smoothing it out between both client/server positions
            Vector3 shotCenter = Vector3.Lerp(_shotPos.position, new Vector3(position[0]/10f, _shotPos.position.y, position[1]/10f), 0.6f);
            Quaternion syncedRot = _turret.rotation = Quaternion.Euler(0, angle, 0);

            ClassDefinition playerClass = _player.GetClass();
            
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
            
            // animate
            _playerAnimator.Attack();
        }
        
        /// <summary>
        /// Server only: calculate damage to be taken by the Player,
        /// triggers score increase and respawn workflow on death.
        /// </summary>
        public void TakeDamage(int damage, TanksMP.Player other, bool canKill = true, string deathFxId = "")
        {
            int health = _player.Health;
            int shield = _player.Shield;

            //reduce shield on hit
            if (shield > 0)
            {
                _player.Shield--;
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
                _player.CombatController.PlayerDeath(other, deathFxId);
            else
            {
                //we didn't die, set health to new value
                _player.Health = health;
                _player.PlayerViewController.ShowDamageText(damage, false, false);
            }
        }

        /// <summary>
        /// Server only: calculate damage to be taken by the Player,
		/// triggers score increase and respawn workflow on death.
        /// </summary>
        public void TakeDamage(Bullet bullet)
        {
            // ignore damage to team mates
            if (_player.TeamIndex == bullet.owner.GetComponent<TanksMP.Player>().TeamIndex)
                return;
            
            //store network variables temporary
            int health = _player.Health;
            int shield = _player.Shield;

            //reduce shield on hit
            if (shield > 0)
            {
                _player.Shield -= 1;
                return;
            }

            //substract health by damage
            //locally for now, to only have one update later on
            int damage = CalculateDamageTaken(bullet, out bool attackerIsCounter, out bool attackerIsSame);
            
            // Debug.Log("Taking damage from bullet: " + damage);
            
            health -= damage;
            
            if (health <= 0)
                //bullet killed the player
                _player.CombatController.PlayerDeath(bullet.owner.GetComponent<TanksMP.Player>(), bullet.deathFxData.Id);
            else
            {
                //we didn't die, set health to new value
                _player.Health = health;
                _player.PlayerViewController.ShowDamageText(damage, attackerIsCounter, attackerIsSame);
            }
        }
        
        /// <summary>
        /// Server-only.  Handles player death
        /// </summary>
        /// <param name="other"></param>
        public void PlayerDeath(TanksMP.Player other, string deathFxId)
        {
            if (_player.lastDeathTime + minTimeBetweenDeaths >= Time.time)
            {
                Debug.LogWarning("Attempted to respawn within the min time between spawns");
                return;
            }
            
            _player.lastDeathTime = Time.time;
            _player.IsAlive = false;

            if (!_player.PlayerCanRespawnFreely())
                _player.Deaths++;
            
            //the game is already over so don't do anything
            if(_gameManager.ScoreController.IsGameOver()) return;

            _gameManager.RoomController.OnePassCheckChangeTeams(_player, false);
            
            //get killer and increase score for that enemy team
            if (other != null)
            {
                // Reflect damage on killer if blood pact is active
                _statusEffectController.BloodPact(other);
                
                int otherTeam = other.TeamIndex;
                
                // killer is other team
                if (_player.TeamIndex != otherTeam)
                {
                    _gameManager.ScoreController.AddScore(ScoreType.Kill, otherTeam);
                    other.Kills++;
                }
                
                //the maximum score has been reached now
                if (_gameManager.ScoreController.IsGameOver())
                {
                    //tell all clients the winning team
                    _gameManager.RoomController.GameOver((byte)otherTeam);
                    return;
                }
            }
            else
            {
                // Killed by environment
                _gameManager.ScoreController.RemoveScore(ScoreType.Kill, _player.TeamIndex);
            }
            
            // The game is not over
            _player.ResetPlayerState();
            _player.DropCollectibles();
            _player.Respawn(other, null);
        }
    }
}