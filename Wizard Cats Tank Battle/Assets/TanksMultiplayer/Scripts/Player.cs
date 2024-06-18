/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entropy.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Vashta.Entropy.Character;
using EckTechGames.FloatingCombatText;
using TMPro;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.Spells;
using Vashta.Entropy.StatusEffects;
using Vashta.Entropy.UI;

namespace TanksMP
{
    /// <summary>
    /// Networked player class implementing movement control and shooting.
    /// Contains both server and client logic in an authoritative approach.
    /// </summary>
    [RequireComponent(typeof(StatusEffectController))]
    public class Player : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
    {
        /// <summary>
        /// UI Text displaying the player name.
        /// </summary>    
        public Text label;

        /// <summary>
        /// Maximum health value at game start.
        /// </summary>
        public int maxHealth = 10;
        public int maxShield = 5;

        public int counterDamageMod = 2;
        public int sameClassDamageMod = -1;

        public float acceleration = 30f;

        public float defaultMass = 1;

        /// <summary>
        /// Current turret rotation and shooting direction.
        /// </summary>
        [HideInInspector]
        public short turretRotation;

        /// <summary>
        /// Delay between shots.
        /// </summary>
        public float fireRate = 0.75f;

        /// <summary>
        /// Movement speed in all directions.
        /// </summary>
        public float moveSpeed = 8f;

        /// <summary>
        /// UI Slider visualizing health value.
        /// </summary>
        public Slider healthSlider;

        /// <summary>
        /// UI Slider visualizing shield value.
        /// </summary>
        public Slider shieldSlider;

        /// <summary>
        /// Icon next to the slider displaying the player class
        /// </summary>
        public Image classIcon;
        
        /// <summary>
        /// Object to spawn on shooting.
        /// </summary>
        public GameObject shotFX;
        
        /// <summary>
        /// Turret to rotate with look direction.
        /// </summary>
        public Transform turret;

        /// <summary>
        /// Position to spawn new bullets at.
        /// </summary>
        public Transform shotPos;

        /// <summary>
        /// Array of available bullets for shooting.
        /// </summary>
        public GameObject[] bullets;

        /// <summary>
        /// Character appearance reference, stores information about the model.
        /// </summary>
        public CharacterAppearance CharacterAppearance;

        /// <summary>
        /// Alters some stats to make the player more or less powerful.  Higher means more power.
        /// </summary>
        public float handicapModifier = 1f;

        public PlayerAnimator PlayerAnimator;

        /// <summary>
        /// Shows the amount of health the player has
        /// </summary>
        public TextMeshProUGUI HealthbarText;

        public PlayerHealthbarHUD HealthbarHUD;

        /// <summary>
        /// Last player gameobject that killed this one.
        /// </summary>
        [HideInInspector]
        public GameObject killedBy;

        /// <summary>
        /// Reference to the camera following component.
        /// </summary>
        [HideInInspector]
        public FollowTarget camFollow;
        public StatusEffectController StatusEffectController;
        public Renderer Renderer;

        //timestamp when next shot should happen
        private float nextFire;

        public float TimeToNextFire => nextFire - Time.time;
        public float FractionFireReady => Mathf.Min(1-(TimeToNextFire / fireRate), 1);

        protected PlayerCurrencyRewarder _playerCurrencyRewarder;

        private PlayerInputController InputController;

        //reference to this rigidbody
        #pragma warning disable 0649
		protected Rigidbody rb;
		#pragma warning restore 0649
        
        public bool IsLocal => GameManager.GetInstance().localPlayer == this;
        public ClassDefinition defaultClassDefinition;
        public ClassList classList;

        private Vector3 _lastMousePos;

        private float _lastSecondUpdate;
        private float _secondUpdateTime = 1f;

        private float _initTime;

        private static Dictionary<int, Player> _playersByViewId = new ();
        public static List<Player> GetAllPlayers => _playersByViewId.Values.ToList();

        // Data tables
        public BulletDictionary BulletDictionary;
        public PowerupDirectory PowerupDirectory;
        public VisualEffectDirectory VisualEffectDirectory;

        // Lag compensation
        private Vector3 networkVelocity;
        private Vector3 networkPosition;

        private short networkTurretRotation;
        private float lastTransformUpdate;
        private const float _maxTransformLerp = .15f; 
        
        // Spawn timer
        [HideInInspector]
        public float lastDeathTime = 0f;

        public bool IsAlive => gameObject ? gameObject : gameObject.activeInHierarchy; // Issue might be here
        public bool IsDead => !IsAlive;
        private bool _hasLateInited = false;
        
        public bool IsVisible => Renderer.isVisible;

        public AudioClip CantShootSound;
        public StatusEffectDirectory StatusEffectDirectory;
        public PlayerAimGraphic PlayerAimGraphic;
        
        //initialize server values for this player
        void Awake()
        {
            _playersByViewId.Add(GetId(), this);

            ClassDefinition classDefinition = defaultClassDefinition ? defaultClassDefinition : classList.RandomClass();

            StartCoroutine(RefreshHudCoroutine());
            if (IsLocal)
            {
                GameManager.GetInstance().ui.CastPowerupButton.gameObject.SetActive(false);
            }

            //only let the master do initialization
            if (PhotonNetwork.IsMasterClient)
            {
                _lastSecondUpdate = Time.time + .1f;
                GetView().SetJoinTime(Time.time);
                GetView().SetKills(0);
                GetView().SetDeaths(0);
                GetView().SetIsAlive(true);
                GetView().SetClassId(classDefinition.classId);
                
                GetView().RPC("RpcApplyClass", RpcTarget.All);
            }

            lastTransformUpdate = Time.time;
        }

        private IEnumerator RefreshHudCoroutine()
        {
            yield return new WaitForSeconds(.1f);
            RefreshSlider();
        }

        private void SetMaxHealth()
        {
            //set players current health value after joining
            GetView().SetHealth(maxHealth);
            OnHealthChange(maxHealth);
        }

        /// <summary>
        /// Get the ID of this player
        /// </summary>
        /// <returns></returns>
        public int GetId()
        {
            // Need to verify this is the right way to do it
            return photonView.ViewID;
        }

        private void OnDestroy()
        {
            _playersByViewId.Remove(GetId());
        }

        /// <summary>
        /// Attempt to get a player from an ID.  Returns null if player does not exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Player GetPlayerById(int id)
        {
            if (_playersByViewId.ContainsKey(id))
                return _playersByViewId[id];

            return null;
        }

        public static Player GetLocalPlayer()
        {
            foreach (KeyValuePair<int,Player> keyValuePair in _playersByViewId)
            {
                Player kvpPlayer = keyValuePair.Value;

                if (kvpPlayer != null && kvpPlayer.IsLocal)
                    return kvpPlayer;
            }

            return null;
        }

        public string GetName()
        {
            return GetView().GetName();
        }

        /// <summary>
        /// Initialize synced values on every client.
        /// Initialize camera and input for this local client.
        /// </summary>
        void Start()
        {
            if (GameManager.GetInstance().UsesTeams)
                ColorizePlayerForTeam();

            InputController = GameManager.GetInstance().PlayerInputController;
            
            //set name in label
            label.text = GetView().GetName();
            
            //call hooks manually to update
            OnHealthChange(GetView().GetHealth());
            OnShieldChange(GetView().GetShield());
            ApplyClass();

            _playerCurrencyRewarder = new PlayerCurrencyRewarder();

            //get components and set camera target
            rb = GetComponent<Rigidbody>();
            
            // refresh slider to fix render issues
            RefreshSlider();

            // if (PhotonNetwork.IsMasterClient)
            // {
                // StartCoroutine(ChangeTeamsCoroutine());
            // }
            
            //called only for this client 
            if (!photonView.IsMine)
                return;

			//set a global reference to the local player
            GameManager.GetInstance().localPlayer = this;

            camFollow = Camera.main.GetComponent<FollowTarget>();
            camFollow.target = turret;

			//initialize input controls for mobile devices
			//[0]=left joystick for movement, [1]=right joystick for shooting
            #if !UNITY_STANDALONE && !UNITY_WEBGL
            GameManager.GetInstance().ui.controls[0].onDrag += Move;
            GameManager.GetInstance().ui.controls[0].onDragEnd += MoveEnd;

            // GameManager.GetInstance().ui.controls[1].onClick += Shoot;
            GameManager.GetInstance().ui.controls[1].onDragBegin += ShootBegin;
            GameManager.GetInstance().ui.controls[1].onDrag += RotateTurret;
            GameManager.GetInstance().ui.controls[1].onDrag += Shoot;
            #endif

            GameManager.GetInstance().ui.fireButton.Player = this;
        }

        protected void RefreshSlider()
        {
            healthSlider.gameObject.SetActive(false);
            healthSlider.gameObject.SetActive(true);
        }
        
        private void ColorizePlayerForTeam(Team team = null)
        {
            if(team == null)
                team = GameManager.GetInstance().teams[GetView().GetTeam()];
            
            //get corresponding team and colorize renderers in team color
            CharacterAppearance.Team = team;
            CharacterAppearance.ColorizeCart();
            
            label.color = team.teamDefinition.TeamColorPrim;
            HealthbarHUD.SetTeam(team.teamDefinition);

            if (PlayerAimGraphic)
            {
                PlayerAimGraphic.SetColor(team.teamDefinition.GetPrimaryColorLight());
            }
        }

        public void SetPreferredTeam(int preferredTeamIndex)
        {
            photonView.SetPreferredTeamIndex(preferredTeamIndex);
        }

        /// <summary>
        /// Server only
        /// </summary>
        private void AttemptToChangeTeams(bool respawn)
        {
            int preferredTeamIndex = GetView().GetPreferredTeamIndex();
            int currentTeam = GetView().GetTeam();

            if (preferredTeamIndex == PlayerExtensions.RANDOM_TEAM_INDEX && preferredTeamIndex != currentTeam)
            {
                GetView().SetPreferredTeamIndex(currentTeam);
                return;
            }

            if (preferredTeamIndex == PlayerExtensions.RANDOM_TEAM_INDEX || preferredTeamIndex == GetView().GetTeam() ||
                !GameManager.GetInstance().TeamHasVacancy(preferredTeamIndex))
            {
                return;
            }

            Debug.Log("Changing teams to: " + preferredTeamIndex);

            PhotonNetwork.CurrentRoom.AddSize(GetView().GetTeam(), -1);
            GetView().SetTeam(preferredTeamIndex);
            PhotonNetwork.CurrentRoom.AddSize(GetView().GetTeam(), 1);
            
            // Force respawn
            if(respawn)
                CmdRespawn();
            
            this.photonView.RPC("RpcChangeTeams", RpcTarget.All);
        }

        // Server-only
        protected void OnePassCheckChangeTeams(bool respawn)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            int preferredTeamIndex = GetView().GetPreferredTeamIndex();
            bool prefersDifferentTeam = preferredTeamIndex != GetView().GetTeam();

            if (prefersDifferentTeam)
            {
                Debug.Log("Prefers a different team");
                if (GameManager.GetInstance().TeamHasVacancy(preferredTeamIndex))
                {
                    // Handle game over
                    if (GameManager.GetInstance().IsGameOver())
                        return;

                    AttemptToChangeTeams(respawn);
                }
            }
            
        }

        [PunRPC]
        protected void RpcChangeTeams()
        {
            ColorizePlayerForTeam();
        }
        
        /// <summary>
        /// This method gets called whenever player properties have been changed on the network.
        /// </summary>
        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player player, ExitGames.Client.Photon.Hashtable playerAndUpdatedProps)
        {
            //only react on property changes for this player
            if(player != photonView.Owner)
                return;

            //update values that could change any time for visualization to stay up to date
            OnHealthChange(player.GetHealth());
            OnShieldChange(player.GetShield());
            
            OnAmmoChange(player.GetBullet(), player.GetAmmo());
        }

        
        //this method gets called multiple times per second, at least 10 times or more
        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {        
            if (stream.IsWriting)
            {             
                if (rb == null)
                    return;
                
                //here we send the turret rotation angle to other clients
                stream.SendNext(turretRotation);
                
                // lag compensation
                stream.SendNext(rb.position);
                stream.SendNext(rb.velocity);
            }
            else
            {
                //here we receive the turret rotation angle from others and apply it
                networkTurretRotation = (short)stream.ReceiveNext();
                OnTurretRotation();
                
                // lag compensation
                networkPosition = (Vector3)stream.ReceiveNext();
                networkVelocity = (Vector3)stream.ReceiveNext();
                
                if (rb == null)
                    return;
                
                rb.velocity = networkVelocity;
                
                float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.timestamp));

                float lagMultiplier = 2f;
                networkPosition += (networkVelocity * lag * lagMultiplier);
                lastTransformUpdate = Time.time;
            }
        }
        
        // This needs to be on a second timer
        protected virtual void Update()
        {
            // Delayed update
            if (Time.time >= _lastSecondUpdate + _secondUpdateTime)
            {
                LateInit();

                if (PhotonNetwork.IsMasterClient)
                {
                    StatusEffectTick();
                    // OnePassCheckChangeTeams();
                }
            }
        }
        
        

        protected virtual void StatusEffectTick()
        {
            // leech
            StatusEffectController.Leech();
            
            // handle health changes from DoTs/HoTs
            int health = GetView().GetHealth();
            int healthPerSecond = Mathf.RoundToInt(StatusEffectController.HealthPerSecond);
            
            if (healthPerSecond != 0)
            {
                int shield = GetView().GetShield();
                if (shield > 0 && healthPerSecond < 0)
                {
                    GetView().DecreaseShield(1);
                }
                else
                {
                    health += healthPerSecond;
                    health = CapHealth(health);
                    GetView().SetHealth(health);
                }
            }
            
            if(healthPerSecond != 0 && (healthPerSecond < 0 || health < maxHealth))
                this.photonView.RPC("CmdTakeDamage", RpcTarget.AllViaServer, -healthPerSecond, false, false);

            if (health <= 0)
            {
                string deathFx = StatusEffectController.GetDeathFx();
                
                // killed the player
                PlayerDeath(StatusEffectController.LastDotAppliedBy, deathFx);
            }

            _lastSecondUpdate = Time.time;
        }

        private void LateInit()
        {
            if (_hasLateInited)
                return;
            
            _hasLateInited = true;

            if(rb)
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }

        // Overriden in PlayerBot
        void FixedUpdate()
		{
            UpdateMass();
            
			//skip further calls for remote clients    
            if (!photonView.IsMine)
            {
                // lag compensation
                // movement
                rb.position = Vector3.MoveTowards(rb.position, networkPosition, Time.fixedDeltaTime);
                rb.velocity = networkVelocity;

                // rotation
                short targetRotation = networkTurretRotation;

                float diff = Mathf.Abs(turretRotation - targetRotation); 
                
                if (diff > 1f)
                {
                    // normalize direction
                    if (turretRotation < 90 && targetRotation > 270)
                        targetRotation -= 360;

                    if (turretRotation > 270 && targetRotation < 90)
                    {
                        targetRotation += 360;
                    }
                    
                    // rotate
                    float maxLerpTime = _maxTransformLerp * Mathf.Ceil(diff / 90);
                    
                    float time = (Time.time - lastTransformUpdate) / maxLerpTime;
                    turretRotation = (short)(Mathf.RoundToInt(Mathf.Lerp(turretRotation, targetRotation, time)));

                    if (turretRotation > targetRotation)
                        turretRotation = targetRotation;
                }
                else
                {
                    turretRotation = networkTurretRotation;
                }

                OnTurretRotation();

                return;
            }
            
            //continously check for input on desktop platforms
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            
            //movement variables
            Vector2 moveDir = InputController.GetAdapter().GetMovementVector(out bool isMoving);
            Vector2 turnDir = InputController.GetAdapter().GetTurretRotation(transform.position);

            if (isMoving)
            {
                Move(moveDir);
            }
            else
            {
                // Stop network velocity if not moving
                networkVelocity = Vector3.zero;
            }
            
            //rotate turret to look at the mouse direction
            RotateTurret(turnDir);
            
            //shoot bullet on left mouse click
            if(InputController.GetAdapter().ShouldShoot())
                Shoot();

			//replicate input to mobile controls for illustration purposes
			#if UNITY_EDITOR
				GameManager.GetInstance().ui.controls[0].position = moveDir;
				GameManager.GetInstance().ui.controls[1].position = turnDir;
			#endif
#endif
        }

        private void UpdateMass()
        {
            rb.mass = defaultMass * StatusEffectController.MassMultiplier;
        }
      
        /// <summary>
        /// Helper method for getting the current object owner.
        /// </summary>
        public PhotonView GetView()
        {
            return this.photonView;
        }


        //moves rigidbody in the direction passed in
        void Move(Vector2 direction = default(Vector2))
        {
            //if direction is not zero, rotate player in the moving direction relative to camera
            if (direction != Vector2.zero)
                transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y))
                                     * Quaternion.Euler(0, camFollow.camTransform.eulerAngles.y, 0);

            //create movement vector based on current rotation and speed
            float movementSpeed = ((moveSpeed + StatusEffectController.MovementSpeedModifier) *
                                   StatusEffectController.MovementSpeedMultiplier);
            Vector3 velocity = transform.forward * movementSpeed;

            //apply vector to rigidbody position
            rb.velocity = Vector3.MoveTowards(rb.velocity, velocity, acceleration);
        }


        //on movement drag ended
        void MoveEnd()
        {
            //reset rigidbody physics values
            // rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }


        //rotates turret to the direction passed in
        // Never called in PlayerBot
        private void RotateTurret(Vector2 direction = default(Vector2))
        {
            //don't rotate without values
            if (direction == Vector2.zero)
                return;

            //get rotation value as angle out of the direction we received
            turretRotation = (short)Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y)).eulerAngles.y;
            OnTurretRotation();
        }


        //on shot drag start set small delay for first shot
        void ShootBegin()
        {
            // Add delay to prevent firing before aiming.
            // This check ensures nextFire is not always overridden, which lead to the rapid fire exploit.
            if(Time.time > nextFire)
                nextFire = Time.time + 0.1f;
        }


        //shoots a bullet in the direction passed in
        //we do not rely on the current turret rotation here, because we send the direction
        //along with the shot request to the server to absolutely ensure a synced shot position
        protected void Shoot(Vector2 direction = default(Vector2))
        {
            float fireRateMod = fireRate * StatusEffectController.AttackRateModifier;

            if (StatusEffectController.DisableFiring)
            {
                // Play sound if shooting is disabled
                if (CantShootSound)
                {
                    AudioManager.Play2D(CantShootSound);
                }
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
                    short[] pos = new short[] { (short)(shotPos.position.x * 10), (short)(shotPos.position.z * 10) };
                    //send shot request with origin to server
                    // Debug.Log(turretRotation);
                    this.photonView.RPC("CmdShoot", RpcTarget.AllViaServer, pos, turretRotation);
                }
            }
        }

        public void CmdTryChangeTeams(bool respawn)
        {
            if (PlayerCanRespawnFreely() || !IsAlive)
            {
                photonView.RPC("RpcTryChangeTeams", RpcTarget.MasterClient, respawn);
            }
        }
        
        [PunRPC]
        protected void RpcTryChangeTeams(bool respawn)
        {
            OnePassCheckChangeTeams(respawn);
        }
        

        //called on the server first but forwarded to all clients
        [PunRPC]
        protected void CmdTakeDamage(int damage, bool attackerIsCounter, bool attackerIsSame)
        {
            if (damage == 0)
                return;
            
            // Show damage
            if (damage > 0)
            {
                if (attackerIsCounter)
                {
                    OverlayCanvasController.instance.ShowCombatText(gameObject, CombatTextType.CriticalHit,
                        damage);
                }
                else if (attackerIsSame)
                {
                    OverlayCanvasController.instance.ShowCombatText(gameObject, CombatTextType.Miss, damage);
                }
                else
                {
                    OverlayCanvasController.instance.ShowCombatText(gameObject, CombatTextType.Hit, damage);
                }
            }
            else
            {
                // Show heals
                OverlayCanvasController.instance.ShowCombatText(gameObject, CombatTextType.Heal, Mathf.Abs(damage));
            }

            // animate
            if (damage > 0)
                PlayerAnimator.TakeDamage();
            else
                PlayerAnimator.Heal();
        }

        //called on the server first but forwarded to all clients
        [PunRPC]
        protected void CmdShoot(short[] position, short angle)
        {   
            //calculate center between shot position sent and current server position (factor 0.6f = 40% client, 60% server)
            //this is done to compensate network lag and smoothing it out between both client/server positions
            Vector3 shotCenter = Vector3.Lerp(shotPos.position, new Vector3(position[0]/10f, shotPos.position.y, position[1]/10f), 0.6f);
            Quaternion syncedRot = turret.rotation = Quaternion.Euler(0, angle, 0);

            //spawn bullet using pooling
            SpawnProjectile(shotCenter, syncedRot);

            // Spray.  Only handles 3 projectiles right now
            if (StatusEffectController.AdditionalProjectilesSpray > 0)
            {
                // shoot left
                Quaternion leftProjectile = Quaternion.Euler(0, angle - 5, 0);
                SpawnProjectile(shotCenter, leftProjectile, .66f);
                
                // shoot right
                Quaternion rightProjectile = Quaternion.Euler(0, angle + 5, 0);
                SpawnProjectile(shotCenter, rightProjectile, .66f);
            }
            
            // animate
            PlayerAnimator.Attack();

            //send event to all clients for spawning effects
            if (shotFX)
                RpcOnShot();
        }


        //called on all clients after bullet spawn
        //spawn effects or sounds locally, if set
        protected void RpcOnShot()
        {
            if (shotFX) PoolManager.Spawn(shotFX, shotPos.position, Quaternion.identity);
        }

        private void SpawnProjectile(Vector3 shotCenter, Quaternion syncedRot, float DamageModifier = 1)
        {
            GameObject obj = PoolManager.Spawn(bullets[0], shotCenter, syncedRot);
            Bullet bullet = obj.GetComponent<Bullet>();
            
            bullet.SpawnNewBullet();
            bullet.owner = gameObject;
            bullet.ClassDefinition = classList[photonView.GetClassId()];
            bullet.SetDamage(Mathf.CeilToInt(bullet.GetRawDamage() * StatusEffectController.DamageOutputModifier * DamageModifier));
            bullet.canBuff = !StatusEffectController.BlocksCastingBuffs;
            bullet.canDebuff = !StatusEffectController.BlocksCastingDebuffs;

            if (StatusEffectController.ProjectileExplodes)
            {
                bullet.SetExplosionRange(3);
                bullet.SetMaxTargets(3);
            }

            if (StatusEffectController.ProjectileReflects)
            {
                bullet.SetMaxBounce(10);
            }

            if (StatusEffectController.ProjectileLifeExtended > 0)
            {
                bullet.IncreaseDespawnDelay(StatusEffectController.ProjectileLifeExtended);
            }

            if (StatusEffectController.Pierces)
            {
                bullet.SetPiercing(true);
            }
        }


        //hook for updating turret rotation locally
        //never called in PlayerBot
        void OnTurretRotation()
        {
            //we don't need to check for local ownership when setting the turretRotation,
            //because OnPhotonSerializeView PhotonStream.isWriting == true only applies to the owner
            turret.rotation = Quaternion.Euler(0, turretRotation, 0);
        }


        //hook for updating health locally
        //(the actual value updates via player properties)
        protected void OnHealthChange(int value)
        {
            healthSlider.value = Mathf.Max(0f,(float)value / maxHealth);
            HealthbarText.text = $"{value} / {maxHealth}";
        }


        //hook for updating shield locally
        //(the actual value updates via player properties)
        protected void OnShieldChange(int value)
        {
            float val = Mathf.Max(0f, (float)value / maxShield);
            
            shieldSlider.value = val;
            shieldSlider.gameObject.SetActive(val > .001f);

        }

        /// <summary>
        /// Server only.  Heal the player a specified amount
        /// </summary>
        public void Heal(int healAmount)
        {
            // handle health changes from DoTs/HoTs
            int health = GetView().GetHealth();

            if (healAmount == 0)
                return;
            
            health += healAmount;

            health = CapHealth(health);
            GetView().SetHealth(health);
            
            if(healAmount < 0 || health < maxHealth)
                this.photonView.RPC("CmdTakeDamage", RpcTarget.AllViaServer, -healAmount, false, false);
        }

        /// <summary>
        /// Makes sure health never goes over the max value
        /// </summary>
        /// <param name="health"></param>
        /// <returns></returns>
        private int CapHealth(int health)
        {
            return Mathf.Min(health, maxHealth);
        }
        
        /// <summary>
        /// Server only: calculate damage to be taken by the Player,
        /// triggers score increase and respawn workflow on death.
        /// </summary>
        public void TakeDamage(int damage, Player other, bool canKill = true, string deathFxId = "")
        {
            int health = GetView().GetHealth();
            int shield = GetView().GetShield();

            //reduce shield on hit
            if (shield > 0)
            {
                GetView().DecreaseShield(1);
                return;
            }
            
            health -= damage;
            health = CapHealth(health);
            
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
                PlayerDeath(other, deathFxId);
            else
            {
                //we didn't die, set health to new value
                GetView().SetHealth(health);
                this.photonView.RPC("CmdTakeDamage", RpcTarget.AllViaServer, damage, false, false);
            }
        }

        /// <summary>
        /// Server only: calculate damage to be taken by the Player,
		/// triggers score increase and respawn workflow on death.
        /// </summary>
        public void TakeDamage(Bullet bullet)
        {
            // ignore damage to team mates
            if (GetView().GetTeam() == bullet.owner.GetComponent<Player>().GetView().GetTeam())
                return;
            
            //store network variables temporary
            int health = GetView().GetHealth();
            int shield = GetView().GetShield();

            //reduce shield on hit
            if (shield > 0)
            {
                GetView().DecreaseShield(1);
                return;
            }

            //substract health by damage
            //locally for now, to only have one update later on
            int damage = CalculateDamageTaken(bullet, out bool attackerIsCounter, out bool attackerIsSame);
            
            // Debug.Log("Taking damage from bullet: " + damage);
            
            health -= damage;
            health = CapHealth(health);
            
            if (health <= 0)
                //bullet killed the player
                PlayerDeath(bullet.owner.GetComponent<Player>(), bullet.deathFxData.Id);
            else
            {
                //we didn't die, set health to new value
                GetView().SetHealth(health);
                this.photonView.RPC("CmdTakeDamage", RpcTarget.AllViaServer, damage, attackerIsCounter, attackerIsSame);
            }
        }

        private int CalculateDamageTaken(Bullet bullet, out bool attackerIsCounter, out bool attackerIsSame)
        {
            float calculatedDamage = bullet.GetDamage();

            // Check class modifiers
            if (bullet.ClassDefinition == null)
            {
                Debug.LogWarning("Warning! No class definition assigned to bullet");
            }

            // Disable counters for now
            attackerIsCounter = false;
            attackerIsSame = false;

            attackerIsCounter = bullet.ClassDefinition.IsCounter(photonView.GetClassId());
            attackerIsSame = bullet.ClassDefinition.classId == photonView.GetClassId();
            
            if (attackerIsCounter)
            {
                calculatedDamage += counterDamageMod;
            }
            
            if (attackerIsSame)
            {
                calculatedDamage += sameClassDamageMod;
            }
            
            // Check defense modifier
            calculatedDamage += StatusEffectController.DamageTakenModifier;
            
            // don't allow healing
            return Mathf.Max(0, Mathf.RoundToInt(calculatedDamage));
        }

        /// <summary>
        /// Commands the server to kill this player
        /// </summary>
        public void CmdKillPlayer()
        {
            if (!IsAlive || !IsLocal)
                return;
            
            this.photonView.RPC("RpcKillPlayer", RpcTarget.MasterClient);
        }

        /// <summary>
        /// Server only, force the death of hte player
        /// </summary>
        [PunRPC]
        protected void RpcKillPlayer()
        {
            // PlayerDeath
            PlayerDeath(this, null);
        }

        /// <summary>
        /// Server-only.  Handles player death
        /// </summary>
        /// <param name="other"></param>
        private void PlayerDeath(Player other, string deathFxId)
        {
            bool canRespawnFreely = PlayerCanRespawnFreely();
            lastDeathTime = Time.time;
            
            GetView().SetIsAlive(false);
            
            if(!canRespawnFreely)
                GetView().IncrementDeaths();
            
            //the game is already over so don't do anything
            if(GameManager.GetInstance().IsGameOver()) return;

            OnePassCheckChangeTeams(false);
            
            //get killer and increase score for that enemy team
            if (other != null)
            {
                // Reflect damage on killer if blood pact is active
                StatusEffectController.BloodPact(other);
                
                int otherTeam = other.GetView().GetTeam();
                
                // killer is other team
                if (GetView().GetTeam() != otherTeam)
                {
                    GameManager.GetInstance().AddScore(ScoreType.Kill, otherTeam);
                    other.GetView().IncrementKills();
                }
                // else
                // killer is self (respawn)
                // {
                    // if (!canRespawnFreely)
                    // {
                    //     GameManager.GetInstance().RemoveScore(ScoreType.Kill, otherTeam);
                    // }
                // }

                //the maximum score has been reached now
                if (GameManager.GetInstance().IsGameOver())
                {
                    //close room for joining players
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    //tell all clients the winning team
                    this.photonView.RPC("RpcGameOver", RpcTarget.All, (byte)otherTeam);
                    return;
                }
            }
            else
            {
                // Killed by environment
                GameManager.GetInstance().RemoveScore(ScoreType.Kill, GetView().GetTeam());
            }

            //the game is not over yet, reset runtime values
            //also tell all clients to despawn this player
            GetView().SetHealth(maxHealth);
            GetView().SetShield(0);
            GetView().SetBullet(0);
            GetView().SetUltimate(0);

            DropCollectibles();

            //tell the dead player who killed them (owner of the bullet)
            short senderId = 0;
            if (other != null)
                senderId = (short)other.GetComponent<PhotonView>().ViewID;

            this.photonView.RPC("RpcRespawn", RpcTarget.All, senderId, deathFxId);
        }

        public void CommandDropCollectibles()
        {
            photonView.RPC("DropCollectibles", RpcTarget.MasterClient);
        }
        
        /// Server only
        [PunRPC]
        private void DropCollectibles()
        {
            //clean up collectibles on this player by letting them drop down
            Collectible[] collectibles = GetComponentsInChildren<Collectible>(true);
            for (int i = 0; i < collectibles.Length; i++)
            {
                PhotonNetwork.RemoveRPCs(collectibles[i].spawner.photonView);
                collectibles[i].spawner.photonView.RPC("Drop", RpcTarget.AllBuffered, transform.position);
            }
        }

        public bool PlayerCanRespawnFreely()
        {
            // Check timer
            // float countdownMax = ClassSelectionPanel.Instance.TimerLength;

            // if (Time.time <= photonView.GetJoinTime() + countdownMax)
            // {
                // return true;
            // }

            // Check all potential team colliders
            GameManager gameManager = GameManager.GetInstance();
            foreach (var team in gameManager.teams)
            {
                Collider col = team.freeClassChange.GetComponent<Collider>();
                    
                if (col == null)
                {
                    Debug.LogError("Team is missing a free respawn collider! " + photonView.GetTeam());
                }
                else
                {
                    if (col.bounds.Contains(transform.position))
                        return true;
                }
            }
            
            return false;
        }


        //called on all clients on both player death and respawn
        //only difference is that on respawn, the client sends the request
        [PunRPC]
        protected virtual void RpcRespawn(short senderId, string deathFxId)
        {
            
            //toggle visibility for player gameobject (on/off)
            gameObject.SetActive(!gameObject.activeInHierarchy);
            bool isActive = gameObject.activeInHierarchy;
            killedBy = null;

            //the player has been killed
            if (!isActive)
            {
                if (IsLocal)
                {
                    // Hide "Drop Flag" button if local player
                    GameManager.GetInstance().ui.DropCollectiblesButton.gameObject.SetActive(false);
                    GameManager.GetInstance().ui.CastUltimateButton.gameObject.SetActive(false);
                    GameManager.GetInstance().ui.CastPowerupButton.gameObject.SetActive(false);
                }
                
                //find original sender game object (killedBy)
                PhotonView senderView = senderId > 0 ? PhotonView.Find(senderId) : null;
                if (senderView != null && senderView.gameObject != null) killedBy = senderView.gameObject;
                
                //detect whether the current user was responsible for the kill, but not for suicide
                //yes, that's my kill: increase local kill counter
                Player localPlayer = GameManager.GetInstance().localPlayer;
                if (this != localPlayer && killedBy == GameManager.GetInstance().localPlayer.gameObject)
                {
                    Text[] killCounter = GameManager.GetInstance().ui.killCounter;
                    killCounter[0].text = GetView().GetKills().ToString();
                    killCounter[0].GetComponent<Animator>().Play("Animation");

                    // RewardUltimateForKill();
                }
                
                SpawnDeathFx(deathFxId);
                StatusEffectController.ClearStatusEffects();

                //play sound clip on player death
                // play killer's death cry
                if (killedBy != null)
                {
                    Player player = killedBy.GetComponent<Player>();
                    
                    player.RewardUltimateForKill();
                
                    if (player != null && player != this)
                    {
                        AudioManager.Play3D(player.CharacterAppearance.Meow.AudioClip, transform.position);
                    }
                }
            }

            if (PhotonNetwork.IsMasterClient)
            {
                //send player back to the team area, this will get overwritten by the exact position from the client itself later on
                //we just do this to avoid players "popping up" from the position they died and then teleporting to the team area instantly
                //this is manipulating the internal PhotonTransformView cache to update the networkPosition variable
                GetComponent<PhotonTransformView>().OnPhotonSerializeView(new PhotonStream(false, new object[] { GameManager.GetInstance().GetSpawnPosition(GetView().GetTeam()),
                                                                                                                 Vector3.zero, Quaternion.identity }), new PhotonMessageInfo());
            }

            // Player is alive
            if (isActive)
            {
                MovePlayerToSpawn();

                // apply class
                StatusEffectController.RefreshCache();
                ApplyClass();
                ColorizePlayerForTeam();
                
                // Show ultimates button
                if(IsLocal)
                    GameManager.GetInstance().ui.CastUltimateButton.gameObject.SetActive(true);

                if (PhotonNetwork.IsMasterClient)
                {
                    GetView().SetIsAlive(true);
                    // StartCoroutine(ChangeTeamsCoroutine());
                }
            }

            //further changes only affect the local client
            if (!photonView.IsMine)
                return;

            //local player got respawned so reset states
            if (isActive == true)
            {
                ResetTransform();
            }
            else
            {
                //local player was killed, set camera to follow the killer
                if (killedBy != null)
                {
                    camFollow.target = killedBy.transform;
                    camFollow.SetDeathCam();
                }
                //hide input controls and other HUD elements
                camFollow.HideMask(true);
                //display respawn window (only for local player)
                GameManager.GetInstance().DisplayDeath();
            }
        }

        protected void SpawnDeathFx(string killingBlowDeathFx)
        {
            string deathFx = "";
            
            if(killingBlowDeathFx != "")
                deathFx = killingBlowDeathFx;
                
            if (deathFx == "")
                deathFx = StatusEffectController.GetDeathFx();

            if (deathFx != null)
            {
                VisualEffect deathFxData = VisualEffectDirectory[deathFx];
                PoolManager.Spawn(deathFxData.VisualEffectPrefab, transform.position, transform.rotation);
            }
        }

        protected void RewardCoinsForKill()
        {
            // reward coins if the player is on a different team
            RewardCoins(_playerCurrencyRewarder.RewardForKill());
            
        }

        private void RewardCoins(int amount)
        {
            OverlayCanvasController.instance.ShowCombatText(gameObject, CombatTextType.CoinReward, "+"+amount);

            // play coin reward sound
            GameManager.GetInstance().ui.SfxController.PlayCoinEarnedSound();
        }

        public void CmdRewardForCapture()
        {
            GetView().IncrementKills(10);
            photonView.RPC("RpcRewardForCapture", RpcTarget.All);
        }
        
        [PunRPC]
        protected virtual void RpcRewardForCapture()
        {
            if (!IsLocal)
                return;
            
            GameManager.GetInstance().ui.DropCollectiblesButton.gameObject.SetActive(false);

            RewardCoins(_playerCurrencyRewarder.RewardForFlagCapture());
        }

        /// <summary>
        /// Command telling the server and all others that this client is ready for respawn.
        /// This is when the respawn delay is over or a video ad has been watched.
        /// </summary>
        public void CmdRespawn()
        {
            this.photonView.RPC("RpcRespawn", RpcTarget.AllViaServer, (short)0, null);
        }
        
        /// <summary>
        /// Repositions in team area and resets camera & input variables.
        /// This should only be called for the local player.
        /// </summary>
        private void ResetTransform()
        {
            //start following the local player again
            camFollow.target = turret;
            camFollow.SetNormalCam();
            camFollow.HideMask(false);

            //get team area and reposition it there
            // transform.position = GameManager.GetInstance().GetSpawnPosition(GetView().GetTeam());

            //reset forces modified by input
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
            //reset input left over
            GameManager.GetInstance().ui.controls[0].OnEndDrag(null);
            GameManager.GetInstance().ui.controls[1].OnEndDrag(null);
        }

        /// <summary>
        /// Repositions the player in the team area, called by all clients to prevent "ghost" cats
        /// </summary>
        private void MovePlayerToSpawn()
        {
            transform.position = GameManager.GetInstance().GetSpawnPosition(GetView().GetTeam());
        }


        //called on all clients on game end providing the winning team
        [PunRPC]
        protected void RpcGameOver(byte teamIndex)
        {
            //display game over window
            GameManager.GetInstance().DisplayGameOver(teamIndex);
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            CharacterAppearanceSerializable characterAppearanceSerializable = null;
            
            try
            {
                characterAppearanceSerializable = CharacterAppearanceSerializable.Decrypt(
                        (string)info.Sender.CustomProperties[Vashta.Entropy.SaveLoad.PrefsKeys.characterAppearance]);
            }
            catch (Exception e)
            {
                characterAppearanceSerializable = new CharacterAppearanceSerializable();
                Debug.LogWarning("Warning!  Could not load character from Custom Properties. " + e);
            }

            CharacterAppearance characterAppearance = GetComponentInChildren<CharacterAppearance>();
            characterAppearance.LoadFromSerialized(characterAppearanceSerializable);
        }

        public void SetClass(ClassDefinition newClassDefinition, bool respawnPlayer, bool applyInstantly)
        {
            photonView.SetClassId(newClassDefinition.classId);
            UIGame.GetInstance().ClassSelectionButton.UpdateIcon();
            
            if(applyInstantly)
                this.photonView.RPC("RpcApplyClass", RpcTarget.All);

            if(respawnPlayer && !PlayerCanRespawnFreely())
                CmdKillPlayer();
        }

        public int GetClassId()
        {
            return GetView().GetClassId();
        }

        /// <summary>
        /// Run on all clients, ensures classes are updated after being set
        /// </summary>
        [PunRPC]
        protected void RpcApplyClass()
        {
            ApplyClass();
        }
        
        private void ApplyClass()
        {
            PlayerCollisionHandler playerCollisionHandler = GetComponent<PlayerCollisionHandler>();

            if (!playerCollisionHandler)
            {
                Debug.LogError("Player is missing a collision handler!  Can not apply class.");
                return;
            }

            ClassDefinition classDefinition = classList[photonView.GetClassId()];

            if (classDefinition == null)
            {
                Debug.LogError("Could not find class definition for class " + GetView().GetClassId());
            }

            ClassApplier.ApplyClass(this, playerCollisionHandler, classDefinition, handicapModifier);
            SetMaxHealth();
            ReplaceClassMissile();
            
            if(IsLocal)
                GameManager.GetInstance().ui.CastUltimateButton.UpdateSpellIcon(classDefinition.ultimateIcon);
        }

        private void ReplaceClassMissile()
        {
            ClassDefinition classDefinition = classList[photonView.GetClassId()];
            
            if (classDefinition.Missile == null)
                return;

            bullets[0] = classDefinition.Missile;
        }

        public void ApplyStatusEffect(string statusEffectId, int ownerId)
        {
            Player owner = GetPlayerById(ownerId);

            if (owner == null)
            {
                Debug.Log("Player is null!");
                return;
            }

            StatusEffectController.AddStatusEffect(statusEffectId, owner);
        }

        public void OnAmmoChange(int bulletId, int ammoValue)
        {
            if (!IsLocal)
                return;

            UIGame uiGame = GameManager.GetInstance().ui;
            uiGame.bulletIcon.SetLoadout(bulletId, ammoValue);
        }

        /// <summary>
        /// Shows, or updates, the powerup icon in the bottom-right corner
        /// </summary>
        /// <param name="powerupSessionId"></param>
        public void CmdShowPowerupIcon(int powerupSessionId)
        {
            photonView.RPC("RpcShowPowerupIcon", photonView.Owner, powerupSessionId);
        }

        [PunRPC]
        public void RpcShowPowerupIcon(int powerupSessionId)
        {
            if (!IsLocal)
                return;
            
            StatusEffectData statusEffectData = StatusEffectDirectory.GetBySessionId(powerupSessionId);

            if (statusEffectData)
            {
                UIGame.GetInstance().CastPowerupButton.UpdateIcon(statusEffectData.EffectIcon);
            }
            else
            {
                Debug.LogError("Could not show powerup icon for powerup with session ID: " + powerupSessionId);
            }
        }

        /// <summary>
        /// Shows UI overlay announcing powerup
        /// </summary>
        /// <param name="powerupId"></param>
        public void CmdShowPowerupUI(int powerupId)
        {
            photonView.RPC("RpcShowPowerupUI", photonView.Owner, powerupId);
        }

        [PunRPC]
        public void RpcShowPowerupUI(int powerupId)
        {
            Powerup powerup = PowerupDirectory[powerupId];

            if (!IsLocal || powerup == null)
                return;

            UIGame uiGame = GameManager.GetInstance().ui;
            uiGame.PowerUpPanel.SetText(powerup.DisplayText,powerup.DisplaySubtext, powerup.Color, powerup.Icon);
        }

        /// Section: ULTIMATES

        // Server only
        public void IncreaseUltimate()
        {
            if(!PhotonNetwork.IsMasterClient)
                return;

            // Only give to living players
            if (!IsAlive)
                return;

            int ultimateCost = GetClass().ultimateCost;
            if(GetView().GetUltimate() < ultimateCost)
                GetView().SetUltimate(GetView().GetUltimate()+1);
        }

        public void RewardUltimateForKill()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            // Only give to living players
            if (!IsAlive)
                return;

            int ultimateIncrease = 5;
            int ultimateCost = GetClass().ultimateCost;
            int currentUltimate = GetView().GetUltimate();

            if (currentUltimate < ultimateCost)
            {
                GetView().SetUltimate(currentUltimate + ultimateIncrease);
            }
        }

        // Server only
        [PunRPC]
        public void RpcClearUltimate()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            GetView().SetUltimate(0);
        }
        
        public int GetUltimate()
        {
            return GetView().GetUltimate();
        }

        public float GetUltimatePerun()
        {
            int ultimateCost = GetClass().ultimateCost;
            return (float)GetView().GetUltimate() / ultimateCost;
        }

        /// <summary>
        /// Called by local player
        /// </summary>
        /// <returns></returns>
        public bool TryCastUltimate()
        {
            int ultimateCost = GetClass().ultimateCost;
            
            if (GetView().GetUltimate() >= ultimateCost)
            {
                GetView().RPC("RpcClearUltimate", RpcTarget.MasterClient);
                GetView().RPC("RpcCastUltimate", RpcTarget.All);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Create ultimate effect, run on all clients
        /// </summary>
        [PunRPC]
        public void RpcCastUltimate()
        {
            ClassDefinition classDefinition = GetClass();
            SpellData ultimateSpell = classDefinition.ultimateSpell;

            if (!ultimateSpell)
            {
                Debug.LogError("Class with ID " + photonView.GetClassId() + " is missing an ultimate spell!");
                return;
            }

            ultimateSpell.Cast(this);
        }

        /// <summary>
        /// Called by local player
        /// </summary>
        public void TryCastPowerup()
        {
            if (GetView().GetPowerup() > 0)
            {
                GetView().RPC("RpcCastPowerup", RpcTarget.All);
            }
            else
            {
                Debug.LogWarning("Tried to cast powerup with ID <=0: "+ GetView().GetPowerup());
            }
        }

        [PunRPC]
        public void RpcCastPowerup()
        {
            int sessionId = GetView().GetPowerup();

            if (sessionId < 1)
            {
                Debug.LogError("Could not cast powerup, session ID: " + sessionId);
            }
            
            StatusEffectData data = StatusEffectDirectory.GetBySessionId(sessionId);

            if (!data)
            {
                Debug.LogError("Could not find powerup, session ID: " + sessionId);
            }
            
            StatusEffectController.AddStatusEffect(data.Id, this);
            
            RpcClearPowerup();
        }

        [PunRPC]
        public void RpcClearPowerup()
        {
            if (IsLocal)
            {
                UIGame.GetInstance().CastPowerupButton.ClosePanel();
            }
            
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            GetView().SetPowerup(0);
        }

        public ClassDefinition GetClass()
        {
            return classList[photonView.GetClassId()];
        }

        public int GetTeam()
        {
            return photonView.GetTeam();
        }
    }
}