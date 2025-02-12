using Entropy.Scripts.Player;
using Fusion;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.Player
{
    public class ClassController : NetworkBehaviour
    {
        public ClassDirectory ClassDirectory;
        [SerializeField] private ClassDefinition _classDefinition;

        [Networked, OnChangedRender(nameof(OnClassChanged))]
        public int ClassId { get; protected set; }
        // [Networked]
        // public int ClassIdQueued { get; protected set; } // Needs to be implemented

        private bool _hasInit = false;

        public ClassDefinition ClassDefinition
        {
            get
            {
                if (_classDefinition == null) 
                {
                    _classDefinition = ClassDirectory[ClassId];
                }
                
                return _classDefinition;
            }
            protected set => _classDefinition = value;
        }

        public PlayerController PlayerController { get; protected set; }
        public PlayerCollisionHandler PlayerCollisionHandler { get; protected set; }
        public GameManager GameManager { get; protected set; }
        
        private void Awake()
        {
            PlayerController = GetComponent<PlayerController>();
            PlayerCollisionHandler = GetComponent<PlayerCollisionHandler>();
        }

        private void Start()
        {
            Init();
        }
        
        private void Init()
        {
            if(_hasInit) return;
            
            GameManager = GameManager.GetInstance();

            _hasInit = true;
        }

        public void SetClassId(int classId)
        {
            ClassId = classId;
            OnClassChanged();
        }

        public void OnClassChanged()
        {
            ClassDefinition = ClassDirectory[ClassId];
        }

        // Apply class for everyone
        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
        public void RPC_ApplyClass(int classId, float modifier = 1f)
        {
            ClassId = classId;
            OnClassChanged();
            ApplyClass(modifier);
        }

        public void ApplyClass(float modifier = 1f)
        {
            Init();
            
            PlayerCollisionHandler playerCollisionHandler = GetComponent<PlayerCollisionHandler>();

            if (!playerCollisionHandler)
            {
                Debug.LogError("Player is missing a collision handler!  Can not apply class.");
                return;
            }

            if (ClassDefinition == null)
            {
                Debug.LogError("Could not find class definition for class " + ClassId);
            }
            
            PlayerController.maxHealth = (int)(ClassDefinition.maxHealth * modifier);
            PlayerController.fireRate = ClassDefinition.fireRate*(1/modifier);
            PlayerController.moveSpeed = ClassDefinition.moveSpeed * modifier;
            PlayerController.PlayerViewController.SetClassIcon(ClassDefinition.classIcon);
            playerCollisionHandler.armor = ClassDefinition.armor;
            playerCollisionHandler.damageAmtOnCollision = ClassDefinition.damageAmtOnCollision;
            
            PlayerController.SetMaxHealth();
            
            if(HasInputAuthority)
                GameManager.ui.CastUltimateButton.UpdateSpellIcon(ClassDefinition.ultimateIcon);
        }
    }
}