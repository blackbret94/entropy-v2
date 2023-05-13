/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using System;
using System.Collections.Generic;
using System.Linq;
using Entropy.Scripts.Audio;
using Entropy.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.EventSystems;
using Vashta.Entropy.Character;
using Vashta.Entropy.SaveLoad;
using EckTechGames.FloatingCombatText;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.StatusEffects;
using Vashta.Entropy.UI.ClassSelectionPanel;

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

        public float counterDamageMod = 1.50f;
        public float sameClassDamageMod = .60f;

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
        /// Clip to play when a shot has been fired.
        /// </summary>
        public AudioClip shotClip;

        /// <summary>
        /// Clip to play on player death.
        /// </summary>
        public AudioClip explosionClip;

        /// <summary>
        /// Object to spawn on shooting.
        /// </summary>
        public GameObject shotFX;

        /// <summary>
        /// Object to spawn on player death.
        /// </summary>
        public GameObject defaultDeathFx;

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
        /// Last player gameobject that killed this one.
        /// </summary>
        [HideInInspector]
        public GameObject killedBy;

        /// <summary>
        /// Reference to the camera following component.
        /// </summary>
        [HideInInspector]
        public FollowTarget camFollow;

        public PlayerCollisionHandler PlayerCollisionHandler;
        public PlayerMovementAudioAnimationController PlayerMovementAudioAnimationController;
        public StatusEffectController StatusEffectController;

        //timestamp when next shot should happen
        private float nextFire;

        public float TimeToNextFire => nextFire - Time.time;
        public float FractionFireReady => Mathf.Min(1-(TimeToNextFire / fireRate), 1);

        public bool LoadClass;

        protected PlayerCurrencyRewarder _playerCurrencyRewarder;

        //reference to this rigidbody
        #pragma warning disable 0649
		private Rigidbody rb;
		#pragma warning restore 0649
        
        public bool IsLocal => GameManager.GetInstance().localPlayer == this;
        public ClassDefinition defaultClassDefinition;
        public ClassList classList;
        // public ClassDefinition classDefinition { get; private set; }

        public int PreferredTeamIndex = -1; // -1 indicates random

        private Vector3 _lastMousePos;

        private float _lastSecondUpdate;
        private float _secondUpdateTime = 1f;

        private float _initTime;

        private static Dictionary<int, Player> _playersByViewId = new ();

        public BulletDictionary BulletDictionary;

        //initialize server values for this player
        void Awake()
        {
            _playersByViewId.Add(GetId(), this);
            
            ClassDefinition classDefinition = defaultClassDefinition ? defaultClassDefinition : classList.RandomClass();
            photonView.SetClassId(classDefinition.classId);
            ApplyClass();
            
            //only let the master do initialization
            if(!PhotonNetwork.IsMasterClient)
                return;
            
            _lastSecondUpdate = Time.time + 2f;
            photonView.SetJoinTime(Time.time);

            GetView().SetKills(0);
            GetView().SetDeaths(0);
        }

        public void LoadClassCallback(int classId, bool forceRefresh)
        {
            photonView.SetClassId(classId);
            
            if(forceRefresh)
                ApplyClass();
        }

        private void SetMaxHealth()
        {
            //set players current health value after joining
            GetView().SetHealth(maxHealth);
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

        /// <summary>
        /// Initialize synced values on every client.
        /// Initialize camera and input for this local client.
        /// </summary>
        void Start()
        {
            if (GameManager.GetInstance().UsesTeams)
                ColorizePlayerForTeam();

            //set name in label
            label.text = GetView().GetName();
            
            //call hooks manually to update
            OnHealthChange(GetView().GetHealth());
            OnShieldChange(GetView().GetShield());

            _playerCurrencyRewarder = new PlayerCurrencyRewarder();

            //called only for this client 
            if (!photonView.IsMine)
                return;

			//set a global reference to the local player
            GameManager.GetInstance().localPlayer = this;

			//get components and set camera target
            rb = GetComponent<Rigidbody>();
            camFollow = Camera.main.GetComponent<FollowTarget>();
            camFollow.target = turret;

			//initialize input controls for mobile devices
			//[0]=left joystick for movement, [1]=right joystick for shooting
            #if !UNITY_STANDALONE && !UNITY_WEBGL
            GameManager.GetInstance().ui.controls[0].onDrag += Move;
            GameManager.GetInstance().ui.controls[0].onDragEnd += MoveEnd;

            GameManager.GetInstance().ui.controls[1].onClick += Shoot;
            // GameManager.GetInstance().ui.controls[1].onDragBegin += ShootBegin;
            GameManager.GetInstance().ui.controls[1].onDrag += RotateTurret;
            GameManager.GetInstance().ui.controls[1].onDrag += Shoot;
            #endif

            GameManager.GetInstance().ui.fireButton.Player = this;
        }
        
        private void ColorizePlayerForTeam()
        {
            //get corresponding team and colorize renderers in team color
            Team team = GameManager.GetInstance().teams[GetView().GetTeam()];
            CharacterAppearance.Team = team;
            CharacterAppearance.ColorizeCart();
            
            label.color = team.material.color;
        }

        private void AttemptToChangeTeams()
        {
            if (PreferredTeamIndex == -1 || PreferredTeamIndex == GetView().GetTeam() || !GameManager.GetInstance().TeamHasVacancy(PreferredTeamIndex))
                return;
            
            GetView().SetTeam(PreferredTeamIndex);
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
        }

        
        //this method gets called multiple times per second, at least 10 times or more
        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {        
            if (stream.IsWriting)
            {             
                //here we send the turret rotation angle to other clients
                stream.SendNext(turretRotation);
            }
            else
            {   
                //here we receive the turret rotation angle from others and apply it
                this.turretRotation = (short)stream.ReceiveNext();
                OnTurretRotation();
            }
        }

        
        // This needs to be on a second timer
        private void Update()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (Time.time < _lastSecondUpdate + _secondUpdateTime)
                    return;
                
                int health = GetView().GetHealth();

                health += (int)StatusEffectController.HealthPerSecond;
            
                if (health <= 0)
                    // killed the player
                    PlayerDeath(StatusEffectController.LastDotAppliedBy, null);

                _lastSecondUpdate = Time.time;
            }
        }

        //continously check for input on desktop platforms
        #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        void FixedUpdate()
		{
			//skip further calls for remote clients    
            if (!photonView.IsMine)
            {
                //keep turret rotation updated for all clients
                OnTurretRotation();
                return;
            }

            //movement variables
            Vector2 moveDir;
            Vector2 turnDir;

            //reset moving input when no arrow keys are pressed down
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            {
                moveDir.x = 0;
                moveDir.y = 0;
            }
            else
            {
                //read out moving directions and calculate force
                moveDir.x = Input.GetAxis("Horizontal");
                moveDir.y = Input.GetAxis("Vertical");
                Move(moveDir);
            }

            //cast a ray on a plane at the mouse position for detecting where to shoot 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.up);
            float distance = 0f;
            Vector3 hitPos = Vector3.zero;
            //the hit position determines the mouse position in the scene
            if (plane.Raycast(ray, out distance))
            {
                hitPos = ray.GetPoint(distance) - transform.position;
            }

            //we've converted the mouse position to a direction
            turnDir = new Vector2(hitPos.x, hitPos.z);

            //rotate turret to look at the mouse direction
            RotateTurret(new Vector2(hitPos.x, hitPos.z));
            
            

            //shoot bullet on left mouse click
            if (Input.GetButton("Fire1") && !EventSystem.current.IsPointerOverGameObject())
                Shoot();

			//replicate input to mobile controls for illustration purposes
			#if UNITY_EDITOR
				GameManager.GetInstance().ui.controls[0].position = moveDir;
				GameManager.GetInstance().ui.controls[1].position = turnDir;
			#endif
        }
        #endif
            
      
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
            Vector3 movementDir = transform.forward * ((moveSpeed +  StatusEffectController.MovementSpeedModifier) * StatusEffectController.MovementSpeedMultiplier * Time.deltaTime);
            //apply vector to rigidbody position
            rb.MovePosition(rb.position + movementDir);
        }


        //on movement drag ended
        void MoveEnd()
        {
            //reset rigidbody physics values
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }


        //rotates turret to the direction passed in
        void RotateTurret(Vector2 direction = default(Vector2))
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
            nextFire = Time.time + 0.1f;
        }


        //shoots a bullet in the direction passed in
        //we do not rely on the current turret rotation here, because we send the direction
        //along with the shot request to the server to absolutely ensure a synced shot position
        protected void Shoot(Vector2 direction = default(Vector2))
        {
            float fireRateMod = fireRate * StatusEffectController.AttackRateModifier;
            
            //if shot delay is over  
            if (Time.time > nextFire)
            {
                //set next shot timestamp
                nextFire = Time.time + fireRateMod;
                
                //send current client position and turret rotation along to sync the shot position
                //also we are sending it as a short array (only x,z - skip y) to save additional bandwidth
                short[] pos = new short[] { (short)(shotPos.position.x * 10), (short)(shotPos.position.z * 10)};
                //send shot request with origin to server
                // Debug.Log(turretRotation);
                this.photonView.RPC("CmdShoot", RpcTarget.AllViaServer, pos, turretRotation);
            }
        }

        //called on the server first but forwarded to all clients
        [PunRPC]
        protected void CmdTakeDamage(int damage, bool attackerIsCounter=false, bool attackerIsSame=false)
        {
            // Show damage

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
            
            // animate
            PlayerAnimator.TakeDamage();
        }

        //called on the server first but forwarded to all clients
        [PunRPC]
        protected void CmdShoot(short[] position, short angle)
        {   
            //get current bullet type
            int currentBullet = GetView().GetBullet();

            //calculate center between shot position sent and current server position (factor 0.6f = 40% client, 60% server)
            //this is done to compensate network lag and smoothing it out between both client/server positions
            Vector3 shotCenter = Vector3.Lerp(shotPos.position, new Vector3(position[0]/10f, shotPos.position.y, position[1]/10f), 0.6f);
            Quaternion syncedRot = turret.rotation = Quaternion.Euler(0, angle, 0);

            //spawn bullet using pooling
            GameObject obj = PoolManager.Spawn(bullets[currentBullet], shotCenter, syncedRot);
            Bullet bullet = obj.GetComponent<Bullet>();
            bullet.SpawnNewBullet();
            bullet.owner = gameObject;
            bullet.ClassDefinition = classList[photonView.GetClassId()];
            bullet.damage = Mathf.CeilToInt(bullet.damage * StatusEffectController.DamageOutputModifier);
            
            // animate
            PlayerAnimator.Attack();

            //check for current ammunition
            //let the server decrease special ammunition, if present
            if (PhotonNetwork.IsMasterClient && currentBullet != 0)
            {
                //if ran out of ammo: reset bullet automatically
                GetView().DecreaseAmmo(1);
            }

            //send event to all clients for spawning effects
            if (shotFX || shotClip)
                RpcOnShot();
        }


        //called on all clients after bullet spawn
        //spawn effects or sounds locally, if set
        protected void RpcOnShot()
        {
            if (shotFX) PoolManager.Spawn(shotFX, shotPos.position, Quaternion.identity);
            if (shotClip) AudioManager.Play3D(shotClip, shotPos.position, 0.1f);
        }


        //hook for updating turret rotation locally
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
            healthSlider.value = Mathf.Max(0,(float)value / maxHealth);
        }


        //hook for updating shield locally
        //(the actual value updates via player properties)
        protected void OnShieldChange(int value)
        {
            float val = Mathf.Max(0, (float)value / maxShield);
            
            shieldSlider.value = val;
            shieldSlider.gameObject.SetActive(val > .001f);

        }

        /// <summary>
        /// Server only: calculate damage to be taken by the Player,
        /// triggers score increase and respawn workflow on death.
        /// </summary>
        public void TakeDamage(int damage, Player other)
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
            
            if (health <= 0)
                // killed the player
                PlayerDeath(other, null);
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
            
            health -= damage;
            
            if (health <= 0)
                //bullet killed the player
                PlayerDeath(bullet.owner.GetComponent<Player>(), bullet);
            else
            {
                //we didn't die, set health to new value
                GetView().SetHealth(health);
                this.photonView.RPC("CmdTakeDamage", RpcTarget.AllViaServer, damage, attackerIsCounter, attackerIsSame);
            }
        }

        private int CalculateDamageTaken(Bullet bullet, out bool attackerIsCounter, out bool attackerIsSame)
        {
            float calculatedDamage = bullet.damage;

            // Check class modifiers
            if (bullet.ClassDefinition == null)
            {
                Debug.LogWarning("Warning! No class definition assigned to bullet");
            }

            attackerIsCounter = bullet.ClassDefinition.IsCounter(photonView.GetClassId());
            attackerIsSame = bullet.ClassDefinition.classId == photonView.GetClassId();

            if (attackerIsCounter)
            {
                calculatedDamage *= counterDamageMod;
            }

            if (attackerIsSame)
            {
                calculatedDamage *= sameClassDamageMod;
            }
            
            // Check defense modifier
            calculatedDamage += StatusEffectController.DamageTakenModifier;
            
            // don't allow healing
            return Mathf.Max(0, Mathf.RoundToInt(calculatedDamage));
        }

        /// <summary>
        /// Server-only.  Handles player death
        /// </summary>
        /// <param name="other"></param>
        private void PlayerDeath(Player other, Bullet killingBlow)
        {
            GetView().IncrementDeaths();

            //the game is already over so don't do anything
            if(GameManager.GetInstance().IsGameOver()) return;

            //get killer and increase score for that enemy team
            if (other != null)
            {
                int otherTeam = other.GetView().GetTeam();
                if (GetView().GetTeam() != otherTeam)
                    GameManager.GetInstance().AddScore(ScoreType.Kill, otherTeam);
                else
                {
                    if (!PlayerCanRespawnFreely())
                        GameManager.GetInstance().RemoveScore(ScoreType.Kill, otherTeam);
                }

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
            GetView().SetBullet(0);

            //clean up collectibles on this player by letting them drop down
            Collectible[] collectibles = GetComponentsInChildren<Collectible>(true);
            for (int i = 0; i < collectibles.Length; i++)
            {
                PhotonNetwork.RemoveRPCs(collectibles[i].spawner.photonView);
                collectibles[i].spawner.photonView.RPC("Drop", RpcTarget.AllBuffered, transform.position);
            }

            //tell the dead player who killed them (owner of the bullet)
            short senderId = 0;
            if (other != null)
                senderId = (short)other.GetComponent<PhotonView>().ViewID;

            this.photonView.RPC("RpcRespawn", RpcTarget.All, senderId, killingBlow?killingBlow.GetId():0);
        }

        private bool PlayerCanRespawnFreely()
        {
            float countdownMax = ClassSelectionPanel.Instance.TimerLength;

            if (Time.time <= photonView.GetJoinTime() + countdownMax)
            {
                return true;
            }
            
            return false;
        }


        //called on all clients on both player death and respawn
        //only difference is that on respawn, the client sends the request
        [PunRPC]
        protected virtual void RpcRespawn(short senderId, int bulletId)
        {
            
            Bullet killingBlowBullet = BulletDictionary[bulletId];
            
            //toggle visibility for player gameobject (on/off)
            gameObject.SetActive(!gameObject.activeInHierarchy);
            bool isActive = gameObject.activeInHierarchy;
            killedBy = null;

            //the player has been killed
            if (!isActive)
            {
                StatusEffectController.ClearStatusEffects();
                
                //find original sender game object (killedBy)
                PhotonView senderView = senderId > 0 ? PhotonView.Find(senderId) : null;
                if (senderView != null && senderView.gameObject != null) killedBy = senderView.gameObject;

                if (killedBy != null)
                {
                    Player killedByPlayer = killedBy.GetComponent<Player>();

                    if (killedByPlayer != null)
                        killedByPlayer.GetView().IncrementKills();
                }

                //detect whether the current user was responsible for the kill, but not for suicide
                //yes, that's my kill: increase local kill counter
                Player localPlayer = GameManager.GetInstance().localPlayer;
                if (this != localPlayer && killedBy == GameManager.GetInstance().localPlayer.gameObject)
                {
                    Text[] killCounter = GameManager.GetInstance().ui.killCounter;
                    killCounter[0].text = GetView().GetKills().ToString();
                    killCounter[0].GetComponent<Animator>().Play("Animation");

                    RewardCoins();
                }
                
                //spawn death particles
                GameObject explosion = killingBlowBullet != null ? killingBlowBullet.deathFx : defaultDeathFx;
                if(explosion != null)
                    PoolManager.Spawn(explosion, transform.position, transform.rotation);

                //play sound clip on player death
                // play killer's death cry
                if (killedBy != null)
                {
                    Player player = killedBy.GetComponent<Player>();
                
                    if (player != null && player != this)
                    {
                        AudioManager.Play3D(player.CharacterAppearance.Meow.AudioClip, transform.position);
                    }
                }
            }

            if (PhotonNetwork.IsMasterClient)
            {
                // Check if the player is trying to change teams
                AttemptToChangeTeams();
                
                //send player back to the team area, this will get overwritten by the exact position from the client itself later on
                //we just do this to avoid players "popping up" from the position they died and then teleporting to the team area instantly
                //this is manipulating the internal PhotonTransformView cache to update the networkPosition variable
                GetComponent<PhotonTransformView>().OnPhotonSerializeView(new PhotonStream(false, new object[] { GameManager.GetInstance().GetSpawnPosition(GetView().GetTeam()),
                                                                                                                 Vector3.zero, Quaternion.identity }), new PhotonMessageInfo());
            }

            if (isActive)
            {
                // apply class
                ApplyClass();
            }

            //further changes only affect the local client
            if (!photonView.IsMine)
                return;

            //local player got respawned so reset states
            if (isActive == true)
            {
                ResetPosition();
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

        protected void RewardCoins()
        {
            // reward coins if the player is on a different team
            int coinsRewarded = _playerCurrencyRewarder.RewardForKill();
            OverlayCanvasController.instance.ShowCombatText(gameObject, CombatTextType.CoinReward, "+"+coinsRewarded);

            // play coin reward sound
            GameManager.GetInstance().ui.SfxController.PlayCoinEarnedSound();
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
        public void ResetPosition()
        {
            //start following the local player again
            camFollow.target = turret;
            camFollow.SetNormalCam();
            camFollow.HideMask(false);

            //get team area and reposition it there
            transform.position = GameManager.GetInstance().GetSpawnPosition(GetView().GetTeam());

            //reset forces modified by input
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
            //reset input left over
            GameManager.GetInstance().ui.controls[0].OnEndDrag(null);
            GameManager.GetInstance().ui.controls[1].OnEndDrag(null);
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

        public void SetClass(ClassDefinition newClassDefinition, bool respawnPlayer)
        {
            photonView.SetClassId(newClassDefinition.classId);

            // Respawn if apply now
            if(respawnPlayer)
                TakeDamage(maxHealth*100, this);
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
            ClassApplier.ApplyClass(this, playerCollisionHandler, classDefinition, handicapModifier);
            SetMaxHealth();
            ReplaceClassMissile();
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
    }
}