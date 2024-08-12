using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Vashta.Entropy.GameInput;
using Vashta.Entropy.UI;

namespace Entropy.Scripts.Player
{
    public class PlayerInputController : MonoBehaviour
    {
        [FormerlySerializedAs("Cursor")] public GameObject CursorGo;
        public InputDirectory InputDirectory;
        
        private Dictionary<PlayerInputType, PlayerInputAdapter> _inputAdapterDict;
        [SerializeField]
        private PlayerInputType _currentInputType = PlayerInputType.MKb;

        private Vector3 _lastMousePosition;
        private GamePanel _selectedPanel;

        private void Start()
        {
            _inputAdapterDict = new();
            _inputAdapterDict.Add(PlayerInputType.MKb, new PlayerInputAdapterMKB(InputDirectory, this));
            _inputAdapterDict.Add(PlayerInputType.Touch, new PlayerInputAdapterTouch(InputDirectory, this));
            _inputAdapterDict.Add(PlayerInputType.Gamepad, new PlayerInputAdapterGamepad(InputDirectory, this));

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
    }
}