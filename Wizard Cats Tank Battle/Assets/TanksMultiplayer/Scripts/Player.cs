/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using System;
using System.Collections;
using Entropy.Scripts.Player;
using Fusion;
using UnityEngine;
using Vashta.Entropy.Character;
using Vashta.Entropy.GameState;
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
    [RequireComponent(typeof(CameraController))]
    [RequireComponent(typeof(PlayerViewController))]
    [RequireComponent(typeof(CombatController))]
    [RequireComponent(typeof(UltimateController))]
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(ClassController))]
    public class Player : NetworkBehaviour
    {
        [Header("Stats")]
        public float acceleration = 30f;
        public float fireRate = 0.75f;
        public float moveSpeed = 8f;
        public float defaultMass = 1;
        
        [Networked] public string PlayerName { get; private set; }
        [Networked] public int TeamIndex { get; set; }

        // Health
        public int Health
        {
            get => _health;
            set
            {
                _health = Mathf.Clamp(value, 0, maxHealth);
                PlayerViewController.SetHealth(value, maxHealth);
            }
        }

        public int maxHealth = 10;
        public bool IsAlive { get; set; } // This replaced another variable called "isAlive" - need to make sure they weren't competing

        // Shield
        public int Shield
        {
            get => _shield;
            set
            {
                _shield = Mathf.Clamp(value, 0, maxShield);
                PlayerViewController.SetOvershield(value, maxShield);
            }
        }

        public int maxShield = 5;
        
        // Loadout
        [Networked] public int Ammo { get; protected set; } // Is this needed anymore? Or should it be renamed?
        [Networked] public int Bullet { get; protected set; } // Rename this to powerup?
        public int Kills { get; set; }
        public int Deaths { get; set; }
        [Networked] public float JoinTime { get; protected set; }
        public int ClassId { get; protected set; } // Move to class Controller
        public int ClassIdQueued { get; protected set; }
        public int PreferredTeamIndex { get; set; }
        public int PowerupId { get; set; }

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
        /// Alters some stats to make the player more or less powerful.  Higher means more power.
        /// </summary>
        public float handicapModifier = 1f;

        /// <summary>
        /// Last player gameobject that killed this one.
        /// </summary>
        [HideInInspector]
        public GameObject killedBy;
        
        [Header("Controllers")]
        [HideInInspector]
        public StatusEffectController StatusEffectController;
        protected PlayerCurrencyRewarder _playerCurrencyRewarder;
        public PlayerInputController InputController { get; private set; }
        public CameraController CameraController { get; private set; }
        public PlayerViewController PlayerViewController { get; private set; }
        public CombatController CombatController { get; private set; }
        public UltimateController UltimateController { get; private set; }
        public MovementController MovementController { get; private set; }
        public ClassController ClassController { get; private set; }
        public CharacterAppearance CharacterAppearance;

        //reference to this rigidbody
        #pragma warning disable 0649
		protected Rigidbody rb;
		#pragma warning restore 0649
        
        public bool IsLocal => (HasInputAuthority && !isBot);
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
        
        // Spawn timer
        [HideInInspector]
        public float lastDeathTime = 0f;

        // public bool IsAlive = true;
        private bool _hasLateInited = false;
        
        [Header("Data")]
        public ClassList classList;
        public StatusEffectDirectory StatusEffectDirectory;
        public StatusEffectData StatusEffectApplyOnSpawn;

        public GameManager GameManager;
        
        public bool isBot = false;
        [SerializeField] private int _health;
        [SerializeField] private int _shield;

        //initialize server values for this player
        // Called on both Player and PlayerBot
        void Awake()
        {
            GameManager = GameManager.GetInstance();
            CameraController = GetComponent<CameraController>();
            PlayerViewController = GetComponent<PlayerViewController>();
            CombatController = GetComponent<CombatController>();
            UltimateController = GetComponent<UltimateController>();
            MovementController = GetComponent<MovementController>();
            ClassController = GetComponent<ClassController>();
            
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

            _lastSecondUpdate = Time.time + .1f;
            JoinTime = -Time.time;
            Kills = 0;
            Deaths = 0;
            IsAlive = true;
            ClassId = classDefinition.classId;
            
            ApplyClass();

            lastTransformUpdate = Time.time;
        }

        // TODO: Check if this does anything
        private IEnumerator RefreshHudCoroutine()
        {
            yield return new WaitForSeconds(.1f);
            PlayerViewController.RefreshHealthSlider();
        }

        public void SetMaxHealth()
        {
            Health = maxHealth;
        }

        public void SetMaxShield()
        {
            Shield = maxShield;
        }

        private void OnDestroy()
        {
            PlayerList.Remove(GetId());
            GameManager.ui.GameLogPanel.EventPlayerLeft(PlayerName);
        }

        /// <summary>
        /// Initialize synced values on every client.
        /// Initialize camera and input for this local client.
        /// </summary>
        void Start()
        {
            if (HasInputAuthority && !isBot)
            {
                //set a global reference to the local player
                GameManager.localPlayer = this;
                // InputController.PlayerFired += OnFire;
            }

            if (GameManager.TeamController.UsesTeams)
            {
                PlayerViewController.ColorizePlayerForTeam();
                GameManager.ui.GameLogPanel.EventPlayerChangedTeam(PlayerName, GetTeamDefinition());
            }
            
            PlayerViewController.SetName(PlayerName);
            
            GameManager.ui.GameLogPanel.EventPlayerJoined(PlayerName);

            ApplyClass();
            
            // refresh slider to fix render issues
            PlayerViewController.RefreshHealthSlider();
            
            //called only for this client 
            if (HasInputAuthority)
            {
                CameraController.SetTarget(turret);

                //initialize input controls for mobile devices
                //[0]=left joystick for movement, [1]=right joystick for shooting
#if !UNITY_STANDALONE && !UNITY_WEBGL
            GameManager.ui.controls[0].onDrag += MovementController.Move;
            GameManager.ui.controls[0].onDragEnd += MovementController.MoveEnd;

            // GameManager.ui.controls[1].onClick += Shoot;
            GameManager.ui.controls[1].onDragBegin += CombatController.ShootBegin;
            GameManager.ui.controls[1].onDrag += RotateTurret;
            GameManager.ui.controls[1].onDrag += CombatController.AttemptToShoot;
#endif

                GameManager.ui.fireButton.Player = this;
            }
            
            // Apply status effect
            if (StatusEffectApplyOnSpawn)
            {
                StatusEffectController.AddStatusEffect(StatusEffectApplyOnSpawn.Id, this);
            }
        }
        
        public void ApplyTeamChange()
        {
            PlayerViewController.ColorizePlayerForTeam();
            GameManager.ui.GameLogPanel.EventPlayerChangedTeam(PlayerName, GetTeamDefinition());
        }
        
        /// <summary>
        /// OBSOLETE in Fusion
        /// This method gets called whenever player properties have been changed on the network.
        /// </summary>
        // public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player player, ExitGames.Client.Photon.Hashtable playerAndUpdatedProps)
        // {
        //     //only react on property changes for this player
        //     if(player != photonView.Owner)
        //         return;
        //
        //     //update values that could change any time for visualization to stay up to date
        //     OnHealthChange(player.GetHealth());
        //     OnShieldChange(player.GetShield());
        //     
        //     OnAmmoChange(player.GetBullet(), player.GetAmmo());
        // }

        
        // OBSOLETE IN FUSION.  Re-write this
        //this method gets called multiple times per second, at least 10 times or more
        // void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        // {        
        //     if (stream.IsWriting)
        //     {             
        //         if (rb == null)
        //             return;
        //         
        //         //here we send the turret rotation angle to other clients
        //         stream.SendNext(turretRotation);
        //         
        //         // lag compensation
        //         stream.SendNext(rb.position);
        //         stream.SendNext(rb.velocity);
        //     }
        //     else
        //     {
        //         //here we receive the turret rotation angle from others and apply it
        //         networkTurretRotation = (short)stream.ReceiveNext();
        //         MovementController.OnTurretRotation();
        //         
        //         // lag compensation
        //         networkPosition = (Vector3)stream.ReceiveNext();
        //         networkVelocity = (Vector3)stream.ReceiveNext();
        //         
        //         if (rb == null)
        //             return;
        //         
        //         rb.velocity = networkVelocity;
        //         
        //         float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.timestamp));
        //
        //         float lagMultiplier = 2f;
        //         networkPosition += (networkVelocity * lag * lagMultiplier);
        //         lastTransformUpdate = Time.time;
        //     }
        // }
        
        protected virtual void Update()
        {
            // Delayed update
            if (Time.time >= _lastSecondUpdate + _secondUpdateTime)
            {
                LateInit();

                StatusEffectController.StatusEffectTick();
                _lastSecondUpdate = Time.time;
            }
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
            
            // Re-write using Fusion API
			//skip further calls for remote clients    
            
            // lag compensation
            // movement
            // rb.position = Vector3.MoveTowards(rb.position, networkPosition, Time.fixedDeltaTime);
            // rb.velocity = networkVelocity;
            //
            // // rotation
            // short targetRotation = networkTurretRotation;
            //
            // float diff = Mathf.Abs(turretRotation - targetRotation); 
            //
            // if (diff > 1f)
            // {
            //     // normalize direction
            //     if (turretRotation < 90 && targetRotation > 270)
            //         targetRotation -= 360;
            //
            //     if (turretRotation > 270 && targetRotation < 90)
            //     {
            //         targetRotation += 360;
            //     }
            //     
            //     // rotate
            //     float maxLerpTime = _maxTransformLerp * Mathf.Ceil(diff / 90);
            //     
            //     float time = (Time.time - lastTransformUpdate) / maxLerpTime;
            //     turretRotation = (short)(Mathf.RoundToInt(Mathf.Lerp(turretRotation, targetRotation, time)));
            //
            //     if (turretRotation > targetRotation)
            //         turretRotation = targetRotation;
            // }
            // else
            // {
            //     turretRotation = networkTurretRotation;
            // }
            //
            // MovementController.OnTurretRotation();
            //
            // return;
            
            
            //continously check for input on desktop platforms
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            
            //movement variables
            Vector2 moveDir = InputController.GetAdapter().GetMovementVector(out bool isMoving);
            Vector2 turnDir = InputController.GetAdapter().GetTurretRotation(transform.position);

            if (isMoving)
            {
                MovementController.Move(moveDir);
            }
            else
            {
                // Stop network velocity if not moving
                networkVelocity = Vector3.zero;
            }
            
            //rotate turret to look at the mouse direction
            MovementController.RotateTurret(turnDir);
            
            //shoot bullet on left mouse click
            if(InputController.GetAdapter().ShouldShoot())
                CombatController.AttemptToShoot();

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
        /// Obsolete.  This should be naturally eliminated as the rest of the code base is refactored
        /// </summary>
        public NetworkObject GetView()
        {
            return Object;
        }

        public void CmdTryChangeTeams(bool respawn)
        {
            if (PlayerCanRespawnFreely() || !IsAlive)
            {
                GameManager.RoomController.OnePassCheckChangeTeams(this, respawn);
            }
        }

        /// <summary>
        /// Server only.  Heal the player a specified amount
        /// </summary>
        public void Heal(int healAmount)
        {
            // handle health changes from DoTs/HoTs
            int health = Health;

            if (healAmount == 0)
                return;
            
            health += healAmount;
            
            Health = health;
            
            if(healAmount < 0 || health < maxHealth)
                PlayerViewController.ShowDamageText(-healAmount, false, false);
        }
        
        public void KillPlayer()
        {
            if (!IsAlive)
                return;
            
            CombatController.PlayerDeath(this, null);
        }

        public bool PlayerCanRespawnFreely()
        {
            // Check all potential team colliders
            GameManager gameManager = GameManager;
            foreach (var team in gameManager.TeamController.teams)
            {
                Collider col = team.freeClassChange.GetComponent<Collider>();
                    
                if (col == null)
                {
                    Debug.LogError("Team is missing a free respawn collider! " + TeamIndex);
                }
                else
                {
                    if (col.bounds.Contains(transform.position))
                        return true;
                }
            }
            
            return false;
        }
        
        public virtual void Respawn(Player player, string deathFxId = null)
        {
            lastDeathTime = Time.time;
            
            //toggle visibility for player gameobject (on/off)
            gameObject.SetActive(!gameObject.activeInHierarchy);
            bool isActive = gameObject.activeInHierarchy;
            killedBy = null;

            //the player has been killed
            if (!isActive)
            {
                HandleKilled(player, deathFxId);
            }
            
            // TODO: Update for fusion
            //send player back to the team area, this will get overwritten by the exact position from the client itself later on
            //we just do this to avoid players "popping up" from the position they died and then teleporting to the team area instantly
            //this is manipulating the internal PhotonTransformView cache to update the networkPosition variable
            GetComponent<PhotonTransformView>().OnPhotonSerializeView(new PhotonStream(false, new object[] { GameManager.TeamController.GetSpawnPosition(TeamIndex),
                                                                                                             Vector3.zero, Quaternion.identity }), new PhotonMessageInfo());
            
            // Player is alive
            if (isActive)
            {
               HandleRespawned();
            }
        }

        protected void HandleKilled(Player player, string deathFxId)
        {
            IsAlive = false;
                
            if (HasInputAuthority)
            {
                // Hide "Drop Flag" button if local player
                GameManager.ui.HUD.PlayerDied();
            }
                
            //find original sender game object (killedBy)
            if (player != null && player.gameObject != null) killedBy = player.gameObject;
                
            PlayerViewController.SpawnDeathFx(deathFxId);
                
            // Mark as grey on minimap
            if (MinimapEntityControllerPlayer)
            {
                MinimapEntityControllerPlayer.RenderAsDead();
            }
                
            StatusEffectController.ClearStatusEffects();
                
            if (killedBy != null)
            {
                Player otherPlayer = killedBy.GetComponent<Player>();
                    
                otherPlayer.UltimateController.RewardUltimateForKill();
                    
                // log
                GameManager.ui.GameLogPanel.EventPlayerKilled(PlayerName, GetTeamDefinition(), otherPlayer.PlayerName, otherPlayer.GetTeamDefinition());
                
                if (otherPlayer != null && otherPlayer != this)
                {
                    // play killer's death cry
                    AudioManager.Play3D(otherPlayer.CharacterAppearance.Meow.AudioClip, transform.position);
                }
            }

            // Local only
            if (HasInputAuthority)
            {
                CameraController.FollowKiller(killedBy);
                GameManager.SpawnController.DisplayDeath();
            }
        }

        protected void HandleRespawned()
        {
            IsAlive = true;
                
            // Move player to spawn
            transform.position = GameManager.TeamController.GetSpawnPosition(TeamIndex);

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
            if(HasInputAuthority)
                GameManager.ui.HUD.PlayerRespawned();
            

            IsAlive = true;
                
            // Apply status effect
            if (StatusEffectApplyOnSpawn)
            {
                StatusEffectController.AddStatusEffect(StatusEffectApplyOnSpawn.Id, this);
            }
            
            ResetTransform();
            
        }

        protected void RewardCoinsForKill()
        {
            // reward coins if the player is on a different team
            PlayerViewController.RewardCoins(_playerCurrencyRewarder.RewardForKill());
            
        }

        public void RewardForFlagCapture()
        {
            Kills += 10;
            
            if (!HasInputAuthority)
                return;
            
            GameManager.ui.DropCollectiblesButton.gameObject.SetActive(false);

            PlayerViewController.RewardCoins(_playerCurrencyRewarder.RewardForFlagCapture());
        }
        
        
        // Re-write so this uses an Input rather than RPC
        public void DropCollectibles()
        {
            //clean up collectibles on this player by letting them drop down
            Collectible[] collectibles = GetComponentsInChildren<Collectible>(true);
            for (int i = 0; i < collectibles.Length; i++)
            {
                // TODO: Re-write using Fusion
                PhotonNetwork.RemoveRPCs(collectibles[i].spawner.photonView);
                collectibles[i].spawner.photonView.RPC("Drop", RpcTarget.AllBuffered, transform.position);
            }
        }

        public void RewardForControlPointCapture()
        {
            Kills += 10;
            
            if (!HasInputAuthority)
                return;

            PlayerViewController.RewardCoins(_playerCurrencyRewarder.RewardForPointCapture());
        }
        
        /// <summary>
        /// Repositions in team area and resets camera & input variables.
        /// This should only be called for the local player.
        /// </summary>
        private void ResetTransform()
        {
            //start following the local player again
            CameraController.FollowPlayer(turret);
            
            //get team area and reposition it there
            // transform.position = GameManager.GetSpawnPosition(TeamId);

            //reset forces modified by input
            MovementController.ResetTransform();
            
            //reset input left over
            GameManager.ui.controls[0].OnEndDrag(null);
            GameManager.ui.controls[1].OnEndDrag(null);
        }
        
        // Re-write in Fusion
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
            ClassId = newClassDefinition.classId;
            UIGame.GetInstance().ClassSelectionButton.UpdateIcon();
            
            if(applyInstantly)
                ApplyClass();

            if(respawnPlayer && !PlayerCanRespawnFreely())
                KillPlayer();
        }
        
        private void ApplyClass()
        {
            PlayerCollisionHandler playerCollisionHandler = GetComponent<PlayerCollisionHandler>();

            if (!playerCollisionHandler)
            {
                Debug.LogError("Player is missing a collision handler!  Can not apply class.");
                return;
            }

            ClassDefinition classDefinition = classList[ClassId];

            if (classDefinition == null)
            {
                Debug.LogError("Could not find class definition for class " + ClassId);
            }

            ClassController.ApplyClass(this, playerCollisionHandler, classDefinition, handicapModifier);
            SetMaxHealth();
            
            if(HasInputAuthority)
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

        /// <summary>
        /// Shows, or updates, the powerup icon in the bottom-right corner
        /// </summary>
        /// <param name="powerupSessionId"></param>
        public void ShowPowerupIcon(int powerupSessionId)
        {
            if (!HasInputAuthority)
                return;
            
            HUDPanel.Get().ShowPowerupIcon(powerupSessionId);
        }
        /// <summary>
        /// Shows UI overlay announcing powerup
        /// </summary>
        /// <param name="powerupId"></param>
        public void CmdShowPowerupUI(int powerupId)
        {
            if (!HasInputAuthority)
                return;
            
            HUDPanel.Get().ShowPowerupUI(powerupId);
        }

        /// Section: ULTIMATES
        public void CastUltimate()
        {
            SpellData ultimateSpell = GetClass().ultimateSpell;
            UltimateController.ClearUltimate();
            
            if (!ultimateSpell)
            {
                Debug.LogError("Class with ID " + ClassId + " is missing an ultimate spell!");
                return;
            }

            ultimateSpell.Cast(this);
        }
        
        public void TryCastPowerup()
        {
            if (PowerupId > 0)
            {
                CastPowerup();
            }
            else
            {
                Debug.LogWarning("Tried to cast powerup with ID <=0: "+ PowerupId);
            }
        }
        
        public void CastPowerup()
        {
            if (PowerupId < 1)
            {
                Debug.LogError("Could not cast powerup, session ID: " + PowerupId);
            }
            
            StatusEffectData data = StatusEffectDirectory.GetBySessionId(PowerupId);

            if (!data)
            {
                Debug.LogError("Could not find powerup, session ID: " + PowerupId);
            }
            
            StatusEffectController.AddStatusEffect(data.Id, this);
            
            if (HasInputAuthority)
            {
                UIGame.GetInstance().CastPowerupButton.ClosePanel();
            }

            PowerupId = 0;
        }

        /// SECTION: HELPFUL GETTERS
        public ClassDefinition GetClass()
        {
            return classList[ClassId];
        }

        public int GetTeam() // Deprecated, remove
        {
            return TeamIndex;
        }

        public TeamDefinition GetTeamDefinition()
        {
            return CharacterAppearance.Team.teamDefinition;
        }
        
        public int GetId()
        {
            // Need to verify this is the right way to do it
            return Object.Id;
        }
        
        public void ResetPlayerState()
        {
            Bullet = 0;
            Health = maxHealth;
            Shield = 0;
            UltimateController.ClearUltimate();
        }
    }
}