using System.Collections;
using Entropy.Scripts.Player;
using Fusion;
using FusionHelpers;
using TanksMP;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Vashta.Entropy.Character;
using Vashta.Entropy.GameState;
using Vashta.Entropy.Network;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.StatusEffects;
using Vashta.Entropy.UI;
using Vashta.Entropy.UI.Minimap;

using NetworkInputData = Vashta.Entropy.Network.NetworkInputController.NetworkInputData;

namespace Vashta.Entropy.Player
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
    [RequireComponent(typeof(PlayerTeam))]
    [RequireComponent(typeof(PowerupController))]
    [RequireComponent(typeof(DeathController))]
    public class PlayerController : FusionPlayer
    {
        [Header("Stats")]
        public float acceleration = 30f;
        public float fireRate = 0.75f;
        public float moveSpeed = 8f;
        public float defaultMass = 1;
        
        [Networked] public string PlayerName { get; protected set; }
        public int TeamIndex => Team.TeamIndex;

        // Health
        [Networked, OnChangedRender(nameof(OnHealthChanged))]
        public int Health { get; protected set; } // accurate health
        public int DisplayHealth { get; private set; } // For showing immediate changes, resets ever render

        public int maxHealth { get; set; }
        public bool IsAlive { get; private set; } = true; // This replaced another variable called "isAlive" - need to make sure they weren't competing

        // Shield
        [Networked, OnChangedRender(nameof(OnShieldChanged))]
        public int Shield { get; private set; }

        public int maxShield = 5;
        
        // Loadout
        [Networked] public int Kills { get; set; }
        [Networked] public int Deaths { get; set; }
        [Networked] public float JoinTime { get; set; }
        public int PreferredTeamIndex => Team.PreferredTeamIndex;

        /// <summary>
        /// Current turret rotation and shooting direction.
        /// </summary>
        [Networked] public short turretRotation { get; set; }
        
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
        public DeathController DeathController { get; private set; }
        public ClassController ClassController { get; private set; }
        public CharacterAppearance CharacterAppearance;
        public NetworkManagerCustom NetworkManagerCustom { get; private set; }
        public PlayerTeam Team { get; private set; }
        public PowerupController PowerupController { get; private set; }

        //reference to this rigidbody
        #pragma warning disable 0649
		protected Rigidbody rb;
		#pragma warning restore 0649
        
        public bool IsLocal => (HasInputAuthority && !isBot);
        public ClassDefinition defaultClassDefinition;
        public GameObject characterRoot;

        private Vector3 _lastMousePos;

        protected float _lastSecondUpdate;
        private float _secondUpdateTime = 1f;
        
        private NetworkInputData _oldInput;
        
        public MinimapEntityControllerPlayer MinimapEntityControllerPlayer;
        
        private bool _hasLateInited = false;
        private Collider _collider;
        
        [Header("Data")]
        [FormerlySerializedAs("classList")] 
        public ClassDirectory classDirectory;
        public StatusEffectData StatusEffectApplyOnSpawn;

        public GameManager GameManager;
        
        public bool HasSpawned { get; private set; }
        public bool isBot = false;
        
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
            PowerupController = GetComponent<PowerupController>();
            InputController = GameManager.PlayerInputController;
            NetworkInputController = GetComponent<NetworkInputController>();
            DeathController = GetComponent<DeathController>();
            rb = GetComponent<Rigidbody>();
            _playerCurrencyRewarder = new PlayerCurrencyRewarder();
            Team = GetComponent<PlayerTeam>();
            NetworkManagerCustom = NetworkManagerCustom.GetInstance();
            _collider = GetComponent<Collider>();

            HasSpawned = true;
            StartCoroutine(SpawnedCR());
        }

        protected IEnumerator SpawnedCR()
        {
            // Join time
            _lastSecondUpdate = Runner.SimulationTime + .1f;

            if (HasStateAuthority)
            {
                JoinTime = Runner.SimulationTime;
                SetName();
            }

            bool justJoined = Mathf.Approximately(JoinTime, Runner.SimulationTime);

            // Will eventually need to move this into another method that is overriden by bots
            if (HasInputAuthority && !isBot)
            {
                // Local player logic
                GameManager.localPlayerController = this;
                GameManager.ui.CastPowerupButton.gameObject.SetActive(false);
                CharacterAppearance.SaveLoad.LoadLocal();
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

                GameManager.ui.fireButton.playerController = this;
            }
            else
            {
                // Remote player logic
                CharacterAppearance.LoadFromUUIDs();
            }
            
            PlayerViewController.SetName(PlayerName);
            GameManager.ui.GameLogPanel.EventPlayerJoined(PlayerName);
            PlayerList.Add(this);
            
            PlayerViewController.RefreshHealthSlider();
            
            Team.Init();
            while (!Team.TeamController.HasSpawned)
                yield return null;
            
            Team.Setup();
            
            // Move player to start position
            if (HasStateAuthority)
            {
                // Set position
                MoveToSpawn();
                
                // Set class
                ClassDefinition classDefinition = defaultClassDefinition ? defaultClassDefinition : classDirectory.RandomClass();
                ClassController.SetClassId(classDefinition.classId);
            }
            else if (!justJoined && Health <= 0)
            {
                // If the player is already dead when we join, reflect this
                // Might not be the best way to handle this
                SetIsAlive(false);
                PlayerDeath(null);
            }
            
            ClassController.ApplyClass(handicapModifier);
            
            // Apply status effect
            if (StatusEffectApplyOnSpawn && justJoined)
            {
                StatusEffectController.AddStatusEffect(StatusEffectApplyOnSpawn.Id, this);
            }
            
            PostSpawn();
        }

        public void SetIsAlive(bool isAlive)
        {
            Collider collider = GetComponent<Collider>();
            collider.enabled = isAlive;
            IsAlive = isAlive;
        }

        protected virtual void PostSpawn()
        {
            // Override in children to safely execute post-spawn 
        }

        protected virtual void SetName()
        {
            PlayerName = NetworkManagerCustom.LocalPlayerInfo.Name;
        }

        // Allows the player to freely respawn for 10 seconds after they joined the game.
        // Use SpawnController->PlayerCanRespawnFreely() to factor in everything, including bases.
        public bool RespawnIsFreeFromJointime()
        {
            return Runner.SimulationTime - JoinTime < 10f;
        }

        public override void InitNetworkState() { }

        public override void Render()
        {
            UpdateMass();
            
            // Delayed update
            if (Runner.SimulationTime >= _lastSecondUpdate + _secondUpdateTime)
            {
                LateInit();
            
                StatusEffectController.StatusEffectTick();
                _lastSecondUpdate = Runner.SimulationTime;
            }
            
            if (HasInputAuthority)
            {
                // rotate to cursor position
                Vector3 aimDelta = InputController.GetAdapter().GetTurretRotation(transform.position);
                MovementController.RotateTurret(aimDelta.normalized);
            }
            else
            {
                // Smoothly rotate towards networked rotation
                // float currentTurretRotation = turret.rotation.y;
                // float adjustedTurretRotation = turretRotation;
                //
                // if ((turretRotation > 260 && currentTurretRotation < 90) || (turretRotation < 90 && currentTurretRotation > 260))
                //     adjustedTurretRotation -= 360;
                //
                // float lerpedRotation = Mathf.Lerp(adjustedTurretRotation, currentTurretRotation, .5f);
                // // Debug.Log("Setting turret rotation: " + lerpedRotation);
                turret.rotation = Quaternion.Euler(0, turretRotation, 0);
            }
            
            // Delayed update
            // if (Runner.SimulationTime >= _lastSecondUpdate + _secondUpdateTime)
            // {
            //     LateInit();
            //
            //     // StatusEffectController.StatusEffectTick();
            //     _lastSecondUpdate = Runner.SimulationTime;
            // }

            // if (Health <= 0 && IsAlive)
            // {
            //     HandleKilled(null);
            // }
        }

        public void SetHealth(int health)
        {
            int clampedHealth = Mathf.Clamp(health, 0, maxHealth);
            
            if (HasStateAuthority)
            {
                Health = clampedHealth;
                OnHealthChanged();
            }

            DisplayHealth = clampedHealth;
            RefreshHealthView();
        }

        public void OnHealthChanged()
        {
            DisplayHealth = Health;
            RefreshHealthView();
        }

        public void RefreshHealthView()
        {
            PlayerViewController.SetHealth(Health, maxHealth);
            PlayerViewController.SetOvershield(Shield, maxShield);
        }

        public void SetMaxHealth()
        {
            Health = maxHealth;
            OnHealthChanged();
        }

        public void SetShield(int shield)
        {
            Shield = Mathf.Clamp(shield, 0, maxShield);
            OnShieldChanged();
        }

        public void OnShieldChanged()
        {
            PlayerViewController.SetOvershield(Shield, maxShield);
        }

        public void SetMaxShield()
        {
            Shield = maxShield;
            OnShieldChanged();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            GameManager.ui.GameLogPanel.EventPlayerLeft(PlayerName);
            PlayerList.Remove(this);
            base.Despawned(runner, hasState);
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
            if (!HasInputAuthority || isBot)
                return;
            
            if (NetworkInputController.fetchInput)
            {
                if (GetInput(out NetworkInputData inputData))
                {
                    MovementController.Move(Runner.DeltaTime, inputData.moveDirection.normalized);
                    MovementController.RotateTurret(inputData.aimDirection.normalized);
                    
                    // FIRE
                    if (inputData.IsDown(NetworkInputData.BUTTON_FIRE_PRIMARY))
                    {
                        CombatController.AttemptToShoot();
                    }

                    // POWERUP
                    if(inputData.IsDown(NetworkInputData.BUTTON_FIRE_POWERUP))
                        PowerupController.TryCastPowerup();
                        
                    
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
                else
                {
                    Debug.Log("No input data");
                }
            }
        }

        private void UpdateMass()
        {
            rb.mass = defaultMass * StatusEffectController.MassMultiplier;
        }
        
        /// <summary>
        /// Heal the player a specified amount
        /// </summary>
        public void Heal(int healAmount)
        {
            // handle health changes from DoTs/HoTs
            int health = Health;

            if (healAmount == 0)
                return;
            
            health += healAmount;
            
            SetHealth(health);
            
            if(healAmount < 0 || health < maxHealth)
                PlayerViewController.ShowDamageText(-healAmount, false, false);
        }

        // This should ONLY be called from CombatController and in the initial death check.  CombatController.KillPlayer()
        // should be used instead as
        // this method handles all of the game controller logic.  This method handles ONLY the player's response to dying.
        public void PlayerDeath(PlayerController killedByPlayerController, ushort deathFxId = 0)
        {
            DropCollectibles();
            
            // Increment deaths if outside of the base or killed by another player
            if (killedByPlayerController != null || !GameManager.SpawnController.PlayerCanRespawnFreely(this))
            {
                Deaths++;
            }

            //toggle visibility for player gameobject (on/off)
            characterRoot.SetActive(false);
            // killedBy = null;
            
            SetIsAlive(false);
            
            GameManager.TeamController.OnePassPlayerCheckToChangeTeams(this, false);
                
            if (HasInputAuthority)
            {
                GameManager.ui.HUD.PlayerDied();
            }
                
            //find original sender game object (killedBy)
            if (killedByPlayerController != null && killedByPlayerController.gameObject != null) killedBy = killedByPlayerController.gameObject;
                
            PlayerViewController.SpawnDeathFx(deathFxId);
                
            // Mark as grey on minimap
            if (MinimapEntityControllerPlayer)
            {
                MinimapEntityControllerPlayer.RenderAsDead();
            }
                
            StatusEffectController.ClearStatusEffects();
                
            if (killedBy != null)
            {
                PlayerController otherPlayerController = killedBy.GetComponent<PlayerController>();
                    
                otherPlayerController.UltimateController.RewardUltimateForKill();
                    
                // log
                GameManager.ui.GameLogPanel.EventPlayerKilled(PlayerName, GetTeamDefinition(), otherPlayerController.PlayerName, otherPlayerController.GetTeamDefinition());
                Debug.Log("Kill logged!");
                
                if (otherPlayerController != null && otherPlayerController != this)
                {
                    // play killer's death cry
                    AudioManager.Play3D(otherPlayerController.CharacterAppearance.Meow.AudioClip, transform.position);
                }
            }
            else
            {
                Debug.LogWarning("'Killed By' no one!");
            }

            // Exit here if game over
            if (GameManager.IsGameOver())
                return;

            // Local only
            if (HasInputAuthority)
            {
                CameraController.FollowKiller(killedBy);
                GameManager.SpawnController.HandleDeathSpawn(this);
            }

            if (HasInputAuthority || (isBot && HasStateAuthority))
            {
                // MoveToSpawn();
                GameManager.SpawnController.StartSpawnRoutine(this);
            }
        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        public void RPC_Respawn()
        {
            HandleRespawned();
        }

        public void MoveToSpawn()
        {
            // Move player to spawn
            if (HasInputAuthority || (isBot && HasStateAuthority))
            {
                Vector3 respawnPosition = GameManager.TeamController.GetSpawnPosition(TeamIndex);
                
                if (isBot)
                {
                    Debug.Log("Setting bot spawn position: " + respawnPosition);
                }
                
                // Debug.Log("Spawn Position: " + respawnPosition);
                //
                // float raycastDistance = 10f;
                // float clippingOffset = 0.01f; // Small vertical offset to avoid clipping
                //
                // Vector3 bottomOfCollider = respawnPosition + transform.rotation * _collider.bounds.center - Vector3.up * _collider.bounds.extents.y;
                //
                // // Raycast down from above the respawn point
                // if (Physics.Raycast(bottomOfCollider + Vector3.up * raycastDistance, Vector3.down, out RaycastHit hit, raycastDistance * 2f, LayerMask.GetMask("Ground")))
                // {
                //     // Adjust position so the bottom of the player sits right on the ground
                //
                //     Vector3 adjustedPosition = hit.point - transform.rotation * _collider.bounds.center + Vector3.up * _collider.bounds.extents.y + Vector3.up * clippingOffset;
                //     Debug.Log("Adjusted Position: " + adjustedPosition);
                //     rb.position = adjustedPosition;
                // }
                // else
                // {
                //     Debug.LogWarning("No ground found beneath respawn point!");
                //     rb.position = respawnPosition;
                // }
                
                rb.position = respawnPosition;
                // transform.position = respawnPosition;
                SnapToNavMesh(respawnPosition);
                
                if (isBot)
                {
                    // Debug.Log("Bot position set: " + transform.position);
                }
            }
            
        }
        
        public Vector3 SnapToNavMesh(Vector3 samplePosition)
        {
            float maxSampleDistance = 5f;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(samplePosition, out hit, maxSampleDistance, NavMesh.AllAreas))
            {
                float yOffset = .1f;
                
                Vector3 alignedPosition = new Vector3(
                    rb.position.x,
                    hit.position.y + yOffset,
                    rb.position.z
                );

                // transform.position = alignedPosition;
                rb.position = alignedPosition;
                return alignedPosition;
            }
            else
            {
                Debug.LogWarning("No NavMesh found near this position!");
                return transform.position;
            }
        }
        
        public void HandleRespawned()
        {
            ResetPlayerState();
            
            GameManager.TeamController.OnePassPlayerCheckToChangeTeams(this, false);
            SetIsAlive(true);
            characterRoot.SetActive(true);

            var @struct = DeathController.DeathStruct;
            @struct.expired = true;
            DeathController.DeathStruct = @struct;
            
            MoveToSpawn();
            
            // apply class
            StatusEffectController.RefreshCache();
            ClassController.ApplyClass(handicapModifier);
            PlayerViewController.ColorizePlayerForTeam();
                
            // Render as alive
            if (MinimapEntityControllerPlayer)
            {
                MinimapEntityControllerPlayer.RenderAsAlive();
            }
                
            // Show ultimates button
            if(HasInputAuthority)
                GameManager.ui.HUD.PlayerRespawned();
                
            // Apply status effect
            if (StatusEffectApplyOnSpawn)
            {
                StatusEffectController.AddStatusEffect(StatusEffectApplyOnSpawn.Id, this);
            }
            
            ResetTransform();
        }

        public void RewardForFlagCapture()
        {
            Kills += 10;
            
            if (!HasStateAuthority)
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
            
            if (!HasStateAuthority)
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
            if (HasInputAuthority)
            {
                CameraController.FollowPlayer(turret);
            }

            //reset forces modified by input
            MovementController.ResetTransform();
            
            //reset input left over
            if (HasInputAuthority)
            {
                GameManager.ui.controls[0].OnEndDrag(null);
                GameManager.ui.controls[1].OnEndDrag(null);
            }
        }
        
        // handles full logic for changing class.  ClassController.ApplyClass just handles class-specific changes.
        public void ChangeClass(ClassDefinition newClassDefinition, bool respawnPlayer, bool applyInstantly)
        {
            ClassController.SetClassId(newClassDefinition.classId);
            UIGame.GetInstance().ClassSelectionButton.UpdateIcon();
            
            if(applyInstantly)
                ClassController.RPC_ApplyClass(newClassDefinition.classId, handicapModifier);

            if (respawnPlayer && !GameManager.SpawnController.PlayerCanRespawnFreely(this))
            {
                DeathController.RPCKillPlayerForRespawn();
            }
        }
        
        public void ApplyStatusEffect(string statusEffectId, PlayerController effectOwnerPlayerController)
        {
            if (effectOwnerPlayerController == null)
            {
                Debug.Log("Player is null!");
                return;
            }

            StatusEffectController.AddStatusEffect(statusEffectId, effectOwnerPlayerController);
        }

        /// <summary>
        /// Shows, or updates, the powerup icon in the bottom-right corner
        /// </summary>
        /// <param name="powerupSessionId"></param>
        public void ShowPowerupIcon(ushort powerupSessionId)
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

        /// SECTION: HELPFUL GETTERS
        public ClassDefinition GetClass()
        {
            return ClassController.ClassDefinition;
        }
        
        public TeamDefinition GetTeamDefinition()
        {
            // Should probably move this to PlayerTeam
            return CharacterAppearance.GetTeamInstance().teamDefinition;
        }
        
        // Reset on death
        public void ResetPlayerState()
        {
            SetMaxHealth();
            Shield = 0;
            UltimateController.ClearUltimate();
        }
        
        /// <summary>
        /// Finds the remotely controlled Player game object of a specific player,
        /// by iterating over all Player components and searching for the matching creator.
        /// </summary>
        public static PlayerController GetPlayerGameObject(PlayerRef playerRef)
        {
            NetworkRunner runner = TanksMP.GameManager.GetInstance().Runner;
            if (!runner || !runner.IsRunning)
            {
                Debug.LogError("Runner is not running or hasn't been initiated!");
                return null;
            }
            
            if (runner.TryGetPlayerObject(playerRef, out NetworkObject playerObject))
            {
                PlayerController playerController = playerObject.GetComponent<PlayerController>();

                if (playerController == null)
                {
                    Debug.LogError("PlayerRef: " + playerRef.PlayerId + " does not contain Player component!");
                    return null;
                }

                return playerController;

            }
            else
            {
                Debug.LogError("Could not find PlayerRef: " + playerRef.PlayerId);
                return null;
            }
        }
    }
}