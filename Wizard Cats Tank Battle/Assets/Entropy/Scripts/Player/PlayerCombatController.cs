using Photon.Pun;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.StatusEffects;

namespace Entropy.Scripts.Player
{
    public class PlayerCombatController : MonoBehaviour
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
        
        [Header("Cached references")]
        private StatusEffectController _statusEffectController;
        private Transform _shotPos;
        private Transform _turret;
        private PhotonView _photonView;
        private GameManager _gameManager;
        
        private void Awake()
        {
            _player = GetComponent<TanksMP.Player>();
            _playerAnimator = GetComponent<PlayerAnimator>();
            _photonView = GetComponent<PhotonView>();
        }

        private void Start()
        {
            _statusEffectController = _player.StatusEffectController;
            GameManager.GetInstance();
            
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
                    _photonView.RPC("CmdShoot", RpcTarget.AllViaServer, pos, _player.turretRotation);
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
            int health = _photonView.GetHealth();
            int shield = _photonView.GetShield();

            //reduce shield on hit
            if (shield > 0)
            {
                _photonView.DecreaseShield(1);
                return;
            }
            
            health -= damage;
            health = _player.CapHealth(health);
            
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
                _player.PlayerDeath(other, deathFxId);
            else
            {
                //we didn't die, set health to new value
                _photonView.SetHealth(health);
                _photonView.RPC("RpcTakeDamage", RpcTarget.AllViaServer, damage, false, false);
            }
        }

        /// <summary>
        /// Server only: calculate damage to be taken by the Player,
		/// triggers score increase and respawn workflow on death.
        /// </summary>
        public void TakeDamage(Bullet bullet)
        {
            // ignore damage to team mates
            if (_photonView.GetTeam() == bullet.owner.GetComponent<TanksMP.Player>().GetView().GetTeam())
                return;
            
            //store network variables temporary
            int health = _photonView.GetHealth();
            int shield = _photonView.GetShield();

            //reduce shield on hit
            if (shield > 0)
            {
                _photonView.DecreaseShield(1);
                return;
            }

            //substract health by damage
            //locally for now, to only have one update later on
            int damage = CalculateDamageTaken(bullet, out bool attackerIsCounter, out bool attackerIsSame);
            
            // Debug.Log("Taking damage from bullet: " + damage);
            
            health -= damage;
            health = _player.CapHealth(health);
            
            if (health <= 0)
                //bullet killed the player
                _player.PlayerDeath(bullet.owner.GetComponent<TanksMP.Player>(), bullet.deathFxData.Id);
            else
            {
                //we didn't die, set health to new value
                _photonView.SetHealth(health);
                _photonView.RPC("RpcTakeDamage", RpcTarget.AllViaServer, damage, attackerIsCounter, attackerIsSame);
            }
        }
    }
}