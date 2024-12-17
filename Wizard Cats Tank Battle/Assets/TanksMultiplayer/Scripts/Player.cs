/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using System;
using System.Collections;
using Entropy.Scripts.Player;
using UnityEngine;
using Photon.Pun;
using Vashta.Entropy.Character;
using EckTechGames.FloatingCombatText;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.Spells;
using Vashta.Entropy.StatusEffects;
using Vashta.Entropy.UI;
using Vashta.Entropy.UI.Minimap;

namespace TanksMP
{
    /// <summary>
    /// Networked player class implementing movement control and shooting.
    /// Contains both server and client logic in an authoritative approach.
    /// </summary>
    [RequireComponent(typeof(StatusEffectController))]
    [RequireComponent(typeof(PlayerCameraController))]
    [RequireComponent(typeof(PlayerViewController))]
    [RequireComponent(typeof(PlayerCombatController))]
    [RequireComponent(typeof(UltimateController))]
    public class Player : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
    {
        [Header("Stats")]
        public int maxHealth = 10;
        public int maxShield = 5;
        public float acceleration = 30f;
        public float fireRate = 0.75f;
        public float moveSpeed = 8f;
        public float defaultMass = 1;

        /// <summary>
        /// Current turret rotation and shooting direction.
        /// </summary>
        [HideInInspector]
        public short turretRotation;
        
        /// <summary>
        /// Turret to rotate with look direction.
        /// </summary>
        public Transform turret;

        /// <summary>
        /// Position to spawn new projectiles.
        /// </summary>
        public Transform shotPos;

        /// <summary>
        /// Character appearance reference, stores information about the model.
        /// </summary>
        public CharacterAppearance CharacterAppearance;

        /// <summary>
        /// Alters some stats to make the player more or less powerful.  Higher means more power.
        /// </summary>
        public float handicapModifier = 1f;

        /// <summary>
        /// Last player gameobject that killed this one.
        /// </summary>
        [HideInInspector]
        public GameObject killedBy;
        
        [HideInInspector]
        public StatusEffectController StatusEffectController;

        protected PlayerCurrencyRewarder _playerCurrencyRewarder;

        public PlayerInputController InputController { get; private set; }
        public PlayerCameraController CameraController { get; private set; }
        public PlayerViewController PlayerViewController { get; private set; }
        public PlayerCombatController PlayerCombatController { get; private set; }
        public UltimateController UltimateController { get; private set; }

        //reference to this rigidbody
        #pragma warning disable 0649
		protected Rigidbody rb;
		#pragma warning restore 0649
        
        public bool IsLocal => (GameManager.localPlayer == this && !isBot);
        public ClassDefinition defaultClassDefinition;

        private Vector3 _lastMousePos;

        private float _lastSecondUpdate;
        private float _secondUpdateTime = 1f;

        private float _initTime;
        
        public MinimapEntityControllerPlayer MinimapEntityControllerPlayer;

        // Lag compensation
        private Vector3 networkVelocity;
        private Vector3 networkPosition;

        private short networkTurretRotation;
        private float lastTransformUpdate;
        private const float _maxTransformLerp = .15f; 
        
        // death loop protections
        private const float minTimeBetweenDeaths = .5f;
        
        // Spawn timer
        [HideInInspector]
        public float lastDeathTime = 0f;

        public bool IsAlive = true;
        private bool _hasLateInited = false;
        
        [Header("Data")]
        public ClassList classList;
        public StatusEffectDirectory StatusEffectDirectory;
        public StatusEffectData StatusEffectApplyOnSpawn;

        public GameManager GameManager;
        
        public bool isBot = false;
        
        //initialize server values for this player
        // Called on both Player and PlayerBot
        void Awake()
        {
            GameManager = GameManager.GetInstance();
            CameraController = GetComponent<PlayerCameraController>();
            PlayerViewController = GetComponent<PlayerViewController>();
            PlayerCombatController = GetComponent<PlayerCombatController>();
            UltimateController = GetComponent<UltimateController>();
            
            InputController = GameManager.PlayerInputController;
            rb = GetComponent<Rigidbody>();
            _playerCurrencyRewarder = new PlayerCurrencyRewarder();
            
            PlayerList.Add(GetId(), this);

            ClassDefinition classDefinition = defaultClassDefinition ? defaultClassDefinition : classList.RandomClass();

            StartCoroutine(RefreshHudCoroutine());
            
            if (IsLocal)
            {
                GameManager.ui.CastPowerupButton.gameObject.SetActive(false);
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

        // TODO: Check if this does anything
        private IEnumerator RefreshHudCoroutine()
        {
            yield return new WaitForSeconds(.1f);
            PlayerViewController.RefreshHealthSlider();
        }

        private void SetMaxHealth()
        {
            //set players current health value after joining
            GetView().SetHealth(maxHealth);
            OnHealthChange(maxHealth);
        }

        private void OnDestroy()
        {
            PlayerList.Remove(GetId());
            GameManager.ui.GameLogPanel.EventPlayerLeft(GetName());
        }

        /// <summary>
        /// Initialize synced values on every client.
        /// Initialize camera and input for this local client.
        /// </summary>
        void Start()
        {
            if (photonView.IsMine && !isBot)
            {
                //set a global reference to the local player
                GameManager.localPlayer = this;
                // InputController.PlayerFired += OnFire;
            }

            if (GameManager.UsesTeams)
            {
                PlayerViewController.ColorizePlayerForTeam();
                GameManager.ui.GameLogPanel.EventPlayerChangedTeam(GetName(), GetTeamDefinition());
            }
            
            PlayerViewController.SetName(GetName());
            
            GameManager.ui.GameLogPanel.EventPlayerJoined(GetName());
            
            //call hooks manually to update
            OnHealthChange(GetView().GetHealth());
            OnShieldChange(GetView().GetShield());
            ApplyClass();
            
            // refresh slider to fix render issues
            PlayerViewController.RefreshHealthSlider();
            
            //called only for this client 
            if (photonView.IsMine)
            {
                CameraController.SetTarget(turret);

                //initialize input controls for mobile devices
                //[0]=left joystick for movement, [1]=right joystick for shooting
#if !UNITY_STANDALONE && !UNITY_WEBGL
            GameManager.ui.controls[0].onDrag += Move;
            GameManager.ui.controls[0].onDragEnd += MoveEnd;

            // GameManager.ui.controls[1].onClick += Shoot;
            GameManager.ui.controls[1].onDragBegin += PlayerCombatController.ShootBegin;
            GameManager.ui.controls[1].onDrag += RotateTurret;
            GameManager.ui.controls[1].onDrag += Shoot;
#endif

                GameManager.ui.fireButton.Player = this;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                // Apply status effect
                if (StatusEffectApplyOnSpawn)
                {
                    StatusEffectController.AddStatusEffect(StatusEffectApplyOnSpawn.Id, this);
                }
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
                !GameManager.TeamHasVacancy(preferredTeamIndex))
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
                if (GameManager.TeamHasVacancy(preferredTeamIndex))
                {
                    // Handle game over
                    if (GameManager.IsGameOver())
                        return;

                    AttemptToChangeTeams(respawn);
                }
            }
        }

        [PunRPC]
        protected void RpcChangeTeams()
        {
            PlayerViewController.ColorizePlayerForTeam();
            
            // Notify
            GameManager.ui.GameLogPanel.EventPlayerChangedTeam(GetName(), GetTeamDefinition());
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
        
        protected virtual void Update()
        {
            // Delayed update
            if (Time.time >= _lastSecondUpdate + _secondUpdateTime)
            {
                LateInit();

                if (PhotonNetwork.IsMasterClient)
                {
                    StatusEffectTick();
                }
            }
        }

        /// <summary>
        /// Server only, one status effect tick
        /// </summary>
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
                this.photonView.RPC("RpcTakeDamage", RpcTarget.AllViaServer, -healthPerSecond, false, false);

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
                PlayerCombatController.AttemptToShoot();

			//replicate input to mobile controls for illustration purposes
			#if UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
				GameManager.ui.controls[0].position = moveDir;
				GameManager.ui.controls[1].position = turnDir;
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
            {
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y))
                                            * Quaternion.Euler(0, CameraController.camTransform.eulerAngles.y, 0);

                float rotationSpeed = Time.deltaTime * 450;
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, targetRotation,
                    rotationSpeed);
            }

            //create movement vector based on current rotation and speed
            float movementSpeed = ((moveSpeed + StatusEffectController.MovementSpeedModifier) *
                                   StatusEffectController.MovementSpeedMultiplier);
            // Vector3 velocity = transform.forward * movementSpeed;
            Vector3 velocity = new Vector3(direction.x, 0, direction.y) * movementSpeed;

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
        protected void RpcTakeDamage(int damage, bool attackerIsCounter, bool attackerIsSame)
        {
            PlayerViewController.ShowDamageText(damage, attackerIsCounter, attackerIsSame);
        }

        //called on the server first but forwarded to all clients
        [PunRPC]
        protected void CmdShoot(short[] position, short angle)
        {   
            PlayerCombatController.Shoot(position, angle);
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
            PlayerViewController.SetHealth(value, maxHealth);
        }
        
        //hook for updating shield locally
        //(the actual value updates via player properties)
        protected void OnShieldChange(int value)
        {
            PlayerViewController.SetOvershield(value, maxShield);
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
                this.photonView.RPC("RpcTakeDamage", RpcTarget.AllViaServer, -healAmount, false, false);
        }

        /// <summary>
        /// Makes sure health never goes over the max value
        /// </summary>
        /// <param name="health"></param>
        /// <returns></returns>
        public int CapHealth(int health)
        {
            return Mathf.Min(health, maxHealth);
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
        public void PlayerDeath(Player other, string deathFxId)
        {
            if (lastDeathTime + minTimeBetweenDeaths >= Time.time)
            {
                Debug.LogWarning("Attempted to respawn within the min time between spawns");
                return;
            }
            
            bool canRespawnFreely = PlayerCanRespawnFreely();
            lastDeathTime = Time.time;
            
            GetView().SetIsAlive(false);
            
            if(!canRespawnFreely)
                GetView().IncrementDeaths();
            
            //the game is already over so don't do anything
            if(GameManager.IsGameOver()) return;

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
                    GameManager.AddScore(ScoreType.Kill, otherTeam);
                    other.GetView().IncrementKills();
                }
                
                //the maximum score has been reached now
                if (GameManager.IsGameOver())
                {
                    //close room for joining players
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    //tell all clients the winning team
                    GameManager.GetInstance().photonView.RPC("RpcGameOver", RpcTarget.All, (byte)otherTeam);
                    return;
                }
            }
            else
            {
                // Killed by environment
                GameManager.RemoveScore(ScoreType.Kill, GetView().GetTeam());
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
            // Check all potential team colliders
            GameManager gameManager = GameManager;
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
            if (PhotonNetwork.IsMasterClient)
            {
                lastDeathTime = Time.time;
            }
            
            //toggle visibility for player gameobject (on/off)
            gameObject.SetActive(!gameObject.activeInHierarchy);
            bool isActive = gameObject.activeInHierarchy;
            killedBy = null;

            //the player has been killed
            if (!isActive)
            {
                IsAlive = false;
                
                if (IsLocal)
                {
                    // Hide "Drop Flag" button if local player
                    GameManager.ui.DropCollectiblesButton.gameObject.SetActive(false);
                    GameManager.ui.CastUltimateButton.gameObject.SetActive(false);
                    GameManager.ui.CastPowerupButton.gameObject.SetActive(false);
                }
                
                //find original sender game object (killedBy)
                PhotonView senderView = senderId > 0 ? PhotonView.Find(senderId) : null;
                if (senderView != null && senderView.gameObject != null) killedBy = senderView.gameObject;
                
                PlayerViewController.SpawnDeathFx(deathFxId);
                
                // Mark as grey on minimap
                if (MinimapEntityControllerPlayer)
                {
                    MinimapEntityControllerPlayer.RenderAsDead();
                }
                
                StatusEffectController.ClearStatusEffects();

                //play sound clip on player death
                // play killer's death cry
                if (killedBy != null)
                {
                    Player otherPlayer = killedBy.GetComponent<Player>();
                    
                    otherPlayer.UltimateController.RewardUltimateForKill();
                    
                    // log
                    GameManager.ui.GameLogPanel.EventPlayerKilled(GetName(), GetTeamDefinition(), otherPlayer.GetName(), otherPlayer.GetTeamDefinition());
                
                    if (otherPlayer != null && otherPlayer != this)
                    {
                        AudioManager.Play3D(otherPlayer.CharacterAppearance.Meow.AudioClip, transform.position);
                    }
                }
            }

            if (PhotonNetwork.IsMasterClient)
            {
                //send player back to the team area, this will get overwritten by the exact position from the client itself later on
                //we just do this to avoid players "popping up" from the position they died and then teleporting to the team area instantly
                //this is manipulating the internal PhotonTransformView cache to update the networkPosition variable
                GetComponent<PhotonTransformView>().OnPhotonSerializeView(new PhotonStream(false, new object[] { GameManager.GetSpawnPosition(GetView().GetTeam()),
                                                                                                                 Vector3.zero, Quaternion.identity }), new PhotonMessageInfo());
            }

            // Player is alive
            if (isActive)
            {
                IsAlive = true;
                
                MovePlayerToSpawn();

                // apply class
                StatusEffectController.RefreshCache();
                ApplyClass();
                PlayerViewController.ColorizePlayerForTeam();
                
                // Render as alive
                if (MinimapEntityControllerPlayer)
                {
                    MinimapEntityControllerPlayer.RenderAsAlive();
                }
                
                // Show ultimates button
                if(IsLocal)
                    GameManager.ui.CastUltimateButton.gameObject.SetActive(true);

                // Server only
                if (PhotonNetwork.IsMasterClient)
                {
                    GetView().SetIsAlive(true);
                    
                    // Apply status effect
                    if (StatusEffectApplyOnSpawn)
                    {
                        StatusEffectController.AddStatusEffect(StatusEffectApplyOnSpawn.Id, this);
                    }
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
                CameraController.FollowKiller(killedBy);
                
                //display respawn window (only for local player)
                GameManager.DisplayDeath();
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
            GameManager.ui.SfxController.PlayCoinEarnedSound();
        }

        public void CmdRewardForFlagCapture()
        {
            GetView().IncrementKills(10);
            photonView.RPC("RpcRewardForFlagCapture", RpcTarget.All);
        }
        
        [PunRPC]
        protected virtual void RpcRewardForFlagCapture()
        {
            if (!IsLocal)
                return;
            
            GameManager.ui.DropCollectiblesButton.gameObject.SetActive(false);

            RewardCoins(_playerCurrencyRewarder.RewardForFlagCapture());
        }

        public void CmdRewardForControlPointCapture()
        {
            GetView().IncrementKills(10);
            photonView.RPC("RpcRewardForControlPointCapture", RpcTarget.All);
        }

        [PunRPC]
        protected virtual void RpcRewardForControlPointCapture()
        {
            if (!IsLocal)
                return;

            RewardCoins(_playerCurrencyRewarder.RewardForPointCapture());
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
            CameraController.SetTarget(turret);
            CameraController.SetNormalCam();
            CameraController.HideMask(false);
            
            //get team area and reposition it there
            // transform.position = GameManager.GetSpawnPosition(GetView().GetTeam());

            //reset forces modified by input
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
            //reset input left over
            GameManager.ui.controls[0].OnEndDrag(null);
            GameManager.ui.controls[1].OnEndDrag(null);
        }

        /// <summary>
        /// Repositions the player in the team area, called by all clients to prevent "ghost" cats
        /// </summary>
        private void MovePlayerToSpawn()
        {
            transform.position = GameManager.GetSpawnPosition(GetView().GetTeam());
        }
        
        //called on all clients on game end providing the winning team
        [PunRPC]
        protected void RpcGameOver(byte teamIndex)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            
            int teamIndexInt = teamIndex;
            if (teamIndexInt == 255)
                teamIndexInt = -1;
            
            //display game over window
            GameManager.DisplayGameOver(teamIndexInt);
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
            
            if(IsLocal)
                GameManager.ui.CastUltimateButton.UpdateSpellIcon(classDefinition.ultimateIcon);
        }
        
        public void ApplyStatusEffect(string statusEffectId, int ownerId)
        {
            Player owner = PlayerList.GetPlayerById(ownerId);

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

            UIGame uiGame = GameManager.ui;
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
            
            HUDPanel.Get().ShowPowerupIcon(powerupSessionId);
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
            if (!IsLocal)
                return;
            
            HUDPanel.Get().ShowPowerupUI(powerupId);
        }

        /// Section: ULTIMATES
        // Server only
        [PunRPC]
        public void RpcClearUltimate()
        {
            UltimateController.ClearUltimate();
        }

        /// <summary>
        /// Create ultimate effect, run on all clients
        /// </summary>
        [PunRPC]
        public void RpcCastUltimate()
        {
            SpellData ultimateSpell = GetClass().ultimateSpell;

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

        /// SECTION: HELPERS
        public virtual string GetName()
        {
            return GetView().GetName();
        }
        
        public ClassDefinition GetClass()
        {
            return classList[photonView.GetClassId()];
        }

        public int GetTeam()
        {
            return photonView.GetTeam();
        }

        public TeamDefinition GetTeamDefinition()
        {
            return CharacterAppearance.Team.teamDefinition;
        }
        
        public int GetId()
        {
            // Need to verify this is the right way to do it
            return photonView.ViewID;
        }
    }
}