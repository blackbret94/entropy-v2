/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using System.Collections;
using Entropy.Scripts.Player;
using Fusion;
using FusionHelpers;
using UnityEngine;
using Vashta.Entropy.Character;
using Vashta.Entropy.Network;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.Spells;
using Vashta.Entropy.StatusEffects;
using Vashta.Entropy.UI;
using Vashta.Entropy.UI.Minimap;

using NetworkInputData = Vashta.Entropy.Network.NetworkInputController.NetworkInputData;

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
    [RequireComponent(typeof(NetworkInputController))]
    public class Player : FusionPlayer
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
        public bool IsAlive { get; set; } = true; // This replaced another variable called "isAlive" - need to make sure they weren't competing

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
        public NetworkInputController NetworkInputController { get; private set; }
        public CameraController CameraController { get; private set; }
        public PlayerViewController PlayerViewController { get; private set; }
        public CombatController CombatController { get; private set; }
        public UltimateController UltimateController { get; private set; }
        public MovementController MovementController { get; private set; }
        public ClassController ClassController { get; private set; }
        public CharacterAppearance CharacterAppearance;
        public NetworkManagerCustom NetworkManagerCustom { get; private set; }

        //reference to this rigidbody
        #pragma warning disable 0649
		protected Rigidbody rb;
		#pragma warning restore 0649
        
        public bool IsLocal => (HasInputAuthority && !isBot);
        public ClassDefinition defaultClassDefinition;

        private Vector3 _lastMousePos;

        protected float _lastSecondUpdate;
        private float _secondUpdateTime = 1f;

        private float _initTime;
        private NetworkInputData _oldInput;
        
        public MinimapEntityControllerPlayer MinimapEntityControllerPlayer;
        
        public override void InitNetworkState() {}

        public string CharacterAppearanceSerialized { get; set; }
        
        // Spawn timer
        [HideInInspector]
        public float lastDeathTime = 0f;
        private bool _hasLateInited = false;
        
        [Header("Data")]
        public ClassList classList;
        public StatusEffectDirectory StatusEffectDirectory;
        public StatusEffectData StatusEffectApplyOnSpawn;

        public GameManager GameManager;
        
        public bool isBot = false;
        [SerializeField] private int _health;
        [SerializeField] private int _shield;
        
        public override void Spawned()
        {
            base.Spawned();
            
            // Load dependencies
            GameManager = GameManager.GetInstance();
            CameraController = GetComponent<CameraController>();
            PlayerViewController = GetComponent<PlayerViewController>();
            CombatController = GetComponent<CombatController>();
            UltimateController = GetComponent<UltimateController>();
            MovementController = GetComponent<MovementController>();
            ClassController = GetComponent<ClassController>();
            InputController = GameManager.PlayerInputController;
            NetworkInputController = GetComponent<NetworkInputController>();
            rb = GetComponent<Rigidbody>();
            _playerCurrencyRewarder = new PlayerCurrencyRewarder();
            NetworkManagerCustom = NetworkManagerCustom.GetInstance();

            PlayerName = NetworkManagerCustom.LocalPlayerInfo.Name;
            Debug.Log("Setting player name: " + PlayerName);
            
            // Local player logic
            if (HasInputAuthority && !isBot)
            {
                //set a global reference to the local player
                GameManager.localPlayer = this;
            }
            
            PlayerList.Add(this);
            
            StartCoroutine(RefreshHudCoroutine());
            
            if (IsLocal)
            {
                GameManager.ui.CastPowerupButton.gameObject.SetActive(false);
            }
            
            ClassDefinition classDefinition = defaultClassDefinition ? defaultClassDefinition : classList.RandomClass();

            _lastSecondUpdate = Time.time + .1f;
            JoinTime = -Time.time;
            ClassId = classDefinition.classId;
            
            ApplyClass();
            
            if (IsLocal)
            {
                CharacterAppearance.SaveLoad.Load();
            }
            
            PlayerViewController.SetName(PlayerName);
            GameManager.ui.GameLogPanel.EventPlayerJoined(PlayerName);
            
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

            StartCoroutine(FirstSpawnCoroutine());
        }

        protected virtual IEnumerator FirstSpawnCoroutine()
        {
            // This works for now: TODO: replace with real entry screen where the player can pick teams 
            yield return new WaitForSeconds(4f);
            if (HasInputAuthority)
            {
                // Move player
                Vector3 startPos = GameManager.TeamController.GetSpawnPosition(TeamIndex);
                // transform.position = startPos;
                
                Debug.Log("Setting position: " + startPos + " for team index " + TeamIndex);
                // KillPlayer();
            }
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
            PlayerList.Remove(this);
            GameManager.ui.GameLogPanel.EventPlayerLeft(PlayerName);
        }
        
        public void ApplyTeamChange()
        {
            PlayerViewController.ColorizePlayerForTeam();
            GameManager.ui.GameLogPanel.EventPlayerChangedTeam(PlayerName, GetTeamDefinition());
        }
        
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

        public override void FixedUpdateNetwork()
        {
            UpdateMass();
            
            if (NetworkInputController.fetchInput)
            {
                if (GetInput(out NetworkInputData inputData))
                {
                    MovementController.Move(inputData.moveDirection.normalized);
                    MovementController.RotateTurret(inputData.aimDirection.normalized);
                    
                    // FIRE
                    if (inputData.IsDown(NetworkInputData.BUTTON_FIRE_PRIMARY))
                        CombatController.AttemptToShoot();
                    
                    // POWERUP
                    if(inputData.IsDown(NetworkInputData.BUTTON_FIRE_POWERUP))
                        TryCastPowerup();
                        
                    
                    // ULTIMATE
                    if (inputData.IsDown(NetworkInputData.BUTTON_FIRE_ULTIMATE))
                    {
                        bool couldCast = UltimateController.TryCastUltimate();

                        if (!couldCast)
                        {
                            GameManager.ui.SfxController.PlayUltimateNotReady();
                        }
                    }
                        
                    
                    // DROP FLAG
                    if (inputData.IsDown(NetworkInputData.BUTTON_DROP_FLAG))
                    {
                        DropCollectibles();
                        UIGame.GetInstance().DropCollectiblesButton.gameObject.SetActive(false);
                    }
                    
#if UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
// Move dir and turn dir are from inputData
				GameManager.ui.controls[0].position = moveDir;
				GameManager.ui.controls[1].position = turnDir;
#endif

                    _oldInput = inputData;
                }
            }
        }

        private void UpdateMass()
        {
            rb.mass = defaultMass * StatusEffectController.MassMultiplier;
        }

        public void CmdTryChangeTeams(bool respawn)
        {
            if (PlayerCanRespawnFreely() || !IsAlive)
            {
                GameManager.TeamController.OnePassPlayerCheckToChangeTeams(this, respawn);
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
                Collider col = team.freeClassChangeArea.GetComponent<Collider>();
                    
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
        
        public virtual void Respawn(Player killedByPlayer, string deathFxId = null)
        {
            lastDeathTime = Time.time;
            
            //toggle visibility for player gameobject (on/off)
            gameObject.SetActive(!gameObject.activeInHierarchy);
            bool isActive = gameObject.activeInHierarchy;
            killedBy = null;

            //the player has been killed
            if (!isActive)
            {
                HandleKilled(killedByPlayer, deathFxId);
            }
            
            //send player back to the team area, this will get overwritten by the exact position from the client itself later on
            //we just do this to avoid players "popping up" from the position they died and then teleporting to the team area instantly
            //this is manipulating the internal PhotonTransformView cache to update the networkPosition variable
            transform.position = GameManager.TeamController.GetSpawnPosition(TeamIndex);
            // GetComponent<PhotonTransformView>().OnPhotonSerializeView(new PhotonStream(false, new object[] { GameManager.TeamController.GetSpawnPosition(TeamIndex),
                                                                                                             // Vector3.zero, Quaternion.identity }), new PhotonMessageInfo());
            
            // Player is alive
            if (isActive)
            {
               HandleRespawned();
            }
        }

        protected void HandleKilled(Player killedByPlayer, string deathFxId)
        {
            IsAlive = false;
                
            if (HasInputAuthority)
            {
                // Hide "Drop Flag" button if local player
                GameManager.ui.HUD.PlayerDied();
            }
                
            //find original sender game object (killedBy)
            if (killedByPlayer != null && killedByPlayer.gameObject != null) killedBy = killedByPlayer.gameObject;
                
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
                collectibles[i].spawner.Drop(transform.position);
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
        
        // Move to OnSpawned()
        // public void OnPhotonInstantiate(PhotonMessageInfo info)
        // {
        //     CharacterAppearanceSerializable characterAppearanceSerializable = null;
        //     
        //     try
        //     {
        //         characterAppearanceSerializable = CharacterAppearanceSerializable.Decrypt(
        //                 (string)info.Sender.CustomProperties[Vashta.Entropy.SaveLoad.PrefsKeys.characterAppearance]);
        //     }
        //     catch (Exception e)
        //     {
        //         characterAppearanceSerializable = new CharacterAppearanceSerializable();
        //         Debug.LogWarning("Warning!  Could not load character from Custom Properties. " + e);
        //     }
        //
        //     CharacterAppearance characterAppearance = GetComponentInChildren<CharacterAppearance>();
        //     characterAppearance.LoadFromSerialized(characterAppearanceSerializable);
        // }

        public void SetClass(ClassDefinition newClassDefinition, bool respawnPlayer, bool applyInstantly)
        {
            ClassId = newClassDefinition.classId;
            UIGame.GetInstance().ClassSelectionButton.UpdateIcon();
            
            if(applyInstantly)
                ApplyClass();

            if(respawnPlayer && !PlayerCanRespawnFreely())
                KillPlayer();
        }
        
        protected void ApplyClass()
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
        
        public void ApplyStatusEffect(string statusEffectId, Player effectOwnerPlayer)
        {
            if (effectOwnerPlayer == null)
            {
                Debug.Log("Player is null!");
                return;
            }

            StatusEffectController.AddStatusEffect(statusEffectId, effectOwnerPlayer);
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
            return CharacterAppearance.GetTeamInstance().teamDefinition;
        }
        
        // Reset on death
        public void ResetPlayerState()
        {
            Bullet = 0;
            Health = maxHealth;
            Shield = 0;
            UltimateController.ClearUltimate();
        }
    }
}