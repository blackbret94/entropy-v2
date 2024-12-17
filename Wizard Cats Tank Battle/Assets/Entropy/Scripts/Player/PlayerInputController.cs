using System.Collections.Generic;
using InputIcons;
using TanksMP;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Vashta.Entropy.UI;

namespace Entropy.Scripts.Player
{
    public class PlayerInputController : MonoBehaviour
    {
        [FormerlySerializedAs("Cursor")] public GameObject CursorGo;
        private PlayerInputActionsWCTB PlayerInputActions=>InitPlayerControls.PlayerInputActions;
        
        private Dictionary<PlayerInputType, PlayerInputAdapter> _inputAdapterDict;
        [SerializeField]
        private PlayerInputType _currentInputType = PlayerInputType.MKb;

        private Vector3 _lastMousePosition;
        private GamePanel _selectedPanel;

        private bool _fireIsHeldDown = false;

        private void OnEnable()
        {
            // Controls
            PlayerInputActions.Player.Move.Enable();
            PlayerInputActions.Player.Aim.Enable();

            InputAction iaFire = PlayerInputActions.Player.Fire;
            iaFire.Enable();
            iaFire.performed += Fire;
            iaFire.canceled += ReleaseFire;

            InputAction iaCastPowerup = PlayerInputActions.Player.CastPowerup;
            iaCastPowerup.Enable();
            iaCastPowerup.performed += CastPowerup;

            InputAction iaCastUltimate = PlayerInputActions.Player.CastUltimate;
            iaCastUltimate.Enable();
            iaCastUltimate.performed += CastUltimate;

            InputAction iaDropSpoon = PlayerInputActions.Player.DropSpoon;
            iaDropSpoon.Enable();
            iaDropSpoon.performed += DropSpoon;

            // UI
            InputAction iaClosePanel = PlayerInputActions.UI.Cancel;
            iaClosePanel.Enable();
            iaClosePanel.performed += CancelMenu;

            InputAction iaToggleSettings = PlayerInputActions.UI.ToggleSettings;
            iaToggleSettings.Enable();
            iaToggleSettings.performed += ToggleSettings;

            InputAction iaToggleClassSelection = PlayerInputActions.UI.ToggleChangeClass;
            iaToggleClassSelection.Enable();
            iaToggleClassSelection.performed += ToggleClassSelectionPanel;

            InputAction iaToggleScoreboard = PlayerInputActions.UI.ToggleScoreboard;
            iaToggleScoreboard.Enable();
            iaToggleScoreboard.performed += ToggleScoreboard;

            InputAction navigateMenu = PlayerInputActions.UI.Navigate;
            navigateMenu.Enable();

            InputAction submit = PlayerInputActions.UI.Submit;
            submit.Enable();
            submit.performed += SubmitMenu;

            InputAction secondaryMenu = PlayerInputActions.UI.MenuSecondaryAction;
            secondaryMenu.Enable();
            secondaryMenu.performed += MenuSecondary;

            InputAction zoom = PlayerInputActions.UI.Zoom;
            zoom.Enable();
            zoom.performed += Zoom;
        }

        private void OnDisable()
        {
            // Controls
            PlayerInputActions.Player.Move.Disable();
            
            PlayerInputActions.Player.Fire.Disable();
            PlayerInputActions.Player.Fire.performed -= Fire;
            PlayerInputActions.Player.Fire.canceled -= ReleaseFire;
            
            PlayerInputActions.Player.Aim.Disable();
            
            PlayerInputActions.Player.CastPowerup.Disable();
            PlayerInputActions.Player.CastPowerup.performed -= CastPowerup;
            
            PlayerInputActions.Player.CastUltimate.Disable();
            PlayerInputActions.Player.CastUltimate.performed -= CastUltimate;
            
            PlayerInputActions.Player.DropSpoon.Disable();
            PlayerInputActions.Player.DropSpoon.performed -= DropSpoon;
            
            // UI
            PlayerInputActions.UI.Cancel.Disable();
            PlayerInputActions.UI.Cancel.performed -= CancelMenu;
            
            PlayerInputActions.UI.ToggleSettings.Disable();
            PlayerInputActions.UI.ToggleSettings.performed -= ToggleSettings;
            
            PlayerInputActions.UI.ToggleChangeClass.Disable();
            PlayerInputActions.UI.ToggleChangeClass.performed -= ToggleClassSelectionPanel;
            
            PlayerInputActions.UI.ToggleScoreboard.Disable();
            PlayerInputActions.UI.ToggleScoreboard.performed -= ToggleScoreboard;
            
            PlayerInputActions.UI.Navigate.Disable();
            
            PlayerInputActions.UI.Submit.Disable();
            PlayerInputActions.UI.Submit.performed -= SubmitMenu;
            
            PlayerInputActions.UI.MenuSecondaryAction.Disable();
            PlayerInputActions.UI.MenuSecondaryAction.performed -= MenuSecondary;
            
            PlayerInputActions.UI.Zoom.Disable();
            PlayerInputActions.UI.Zoom.performed -= Zoom;
        }

        private void Start()
        {
            _inputAdapterDict = new();
            _inputAdapterDict.Add(PlayerInputType.MKb, new PlayerInputAdapterMKB(this, PlayerInputActions));
            _inputAdapterDict.Add(PlayerInputType.Touch, new PlayerInputAdapterTouch(this, PlayerInputActions));
            _inputAdapterDict.Add(PlayerInputType.Gamepad, new PlayerInputAdapterGamepad(this, PlayerInputActions));

            _lastMousePosition = Input.mousePosition;

            Init();
        }

        private void Init()
        {
            switch (_currentInputType)
            {
                case PlayerInputType.Gamepad:
                    SetInputGamepad();
                    break;
                
                case PlayerInputType.MKb:
                    SetInputMKb();
                    break;
                
                case PlayerInputType.Touch:
                
                default:
                    break;
            }
        }

        public PlayerInputAdapter GetAdapter()
        {
            return _inputAdapterDict[_currentInputType];
        }

        public void SetInputType(PlayerInputType type)
        {
            _currentInputType = type;
        }

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            RefreshInput();
            GetAdapter().Update();
#endif
        }

        public void RefreshInput()
        {
            if (DetectMKBInput())
            {
                SetInputMKb();
            } else if (DetectGamepadInput())
            {
                SetInputGamepad();
            }
        }

        private void SetInputGamepad()
        {
            // Debug.Log("Detected Gamepad!");
            Cursor.lockState = CursorLockMode.Locked;
            ToggleCursor(false);
            SetInputType(PlayerInputType.Gamepad);

        }

        private void SetInputMKb()
        {
            // Debug.Log("Detected M+KB");
            Cursor.lockState = CursorLockMode.None;  
            ToggleCursor(true);
            SetInputType(PlayerInputType.MKb);
        }

        // Refine this later!
        private bool DetectGamepadInput()
        {
            if (
                Input.GetAxisRaw("Rotate X") != 0 || 
                Input.GetAxisRaw("Rotate Y") != 0 || 
                Input.GetAxisRaw("Fire Controller") != 0 ||
                Input.GetAxisRaw("HorizontalGamepad") != 0 ||
                Input.GetAxisRaw("VerticalGamepad") != 0)
            {
                return true;
            }

            return false;
        }
        
        private bool DetectMKBInput()
        {
            Vector3 mousePos = Input.mousePosition;
            bool mkbInput = Vector3.Distance(_lastMousePosition, mousePos) > .01f;
            _lastMousePosition = new Vector3(mousePos.x, mousePos.y, mousePos.z);
            return mkbInput;
        }

        private void ToggleCursor(bool enable)
        {
            CursorGo.SetActive(enable);
        }

        public void SetSelectedPanel(GamePanel panel)
        {
            if (!panel)
            {
                Debug.Log("Attempted to set active panel to a game panel that does not exist!");
                return;
            }

            _selectedPanel = panel;
        }

        public GamePanel GetSelectedGamePanel()
        {
            return _selectedPanel;
        }

        public bool GameplayActionsBlocked()
        {
            if (!_selectedPanel)
                return false;
            
            if (!_selectedPanel.isActiveAndEnabled)
            {
                _selectedPanel = null;
                return false;
            }

            return _selectedPanel.BlockGameplayWhenSelected;
        }

        private void Fire(InputAction.CallbackContext context)
        {
            _fireIsHeldDown = true;
        }

        private void ReleaseFire(InputAction.CallbackContext context)
        {
            _fireIsHeldDown = false;
        }

        public bool GetFireIsHeldDown()
        {
            if (GameplayActionsBlocked())
                return false;
            
            return _fireIsHeldDown;
        }

        private void CastPowerup(InputAction.CallbackContext context)
        {
            if (GameplayActionsBlocked())
                return;
            
            TanksMP.Player player = PlayerList.GetLocalPlayer();

            if (player != null)
            {
                player.TryCastPowerup();
            }
        }

        private void CastUltimate(InputAction.CallbackContext context)
        {
            if (GameplayActionsBlocked())
                return;
            
            TanksMP.Player player = PlayerList.GetLocalPlayer();
            
            if (player != null)
            {
                bool couldCast = player.UltimateController.TryCastUltimate();
            
                if (!couldCast)
                    GameManager.GetInstance().ui.SfxController.PlayUltimateNotReady();
            }
        }

        private void DropSpoon(InputAction.CallbackContext context)
        {
            if (GameplayActionsBlocked())
                return;
            
            TanksMP.Player player = PlayerList.GetLocalPlayer();
            
            if (player != null)
            {
                player.CommandDropCollectibles();
                UIGame.GetInstance().DropCollectiblesButton.gameObject.SetActive(false);
            }
        }

        protected void SubmitMenu(InputAction.CallbackContext context)
        {
            GamePanel selectedPanel = GetSelectedGamePanel();
            if (selectedPanel)
            {
                selectedPanel.UI_Primary();
            }
        }
        
        protected void CancelMenu(InputAction.CallbackContext context)
        {
            GamePanel[] gamePanels = Object.FindObjectsByType<GamePanel>(FindObjectsSortMode.None);

            foreach (GamePanel gamePanel in gamePanels)
            {
                gamePanel.CloseByHotkey();
            }
        }

        public void MenuSecondary(InputAction.CallbackContext context)
        {
            GamePanel selectedPanel = GetSelectedGamePanel();
            if (selectedPanel)
            {
                selectedPanel.UI_Tertiary();
            }
        }

        protected void ToggleClassSelectionPanel(InputAction.CallbackContext context)
        {
            if (UIGame.GetInstance().ClassSelectionPanel.isActiveAndEnabled)
            {
                CancelMenu(context);
            }
            else
            {
                CancelMenu(context);
                UIGame.GetInstance().ClassSelectionPanel.TogglePanel();
            }
        }

        protected void ToggleSettings(InputAction.CallbackContext context)
        {
            if (UIGame.GetInstance().SettingsPanel.isActiveAndEnabled)
            {
                CancelMenu(context);
            }
            else
            {
                CancelMenu(context);
                UIGame.GetInstance().SettingsPanel.TogglePanel();
            }
        }

        protected void ToggleScoreboard(InputAction.CallbackContext context)
        {
            if (UIGame.GetInstance().ScoreboardPanel.isActiveAndEnabled)
            {
                CancelMenu(context);
            }
            else
            {
                CancelMenu(context);
                UIGame.GetInstance().ScoreboardPanel.TogglePanel();
            }
        }

        protected void Zoom(InputAction.CallbackContext context)
        {
            
        }
    }
}