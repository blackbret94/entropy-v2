#if ENABLE_INPUT_SYSTEM
using System;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cooldhands
{
    /// <summary>
    /// A component that creates a virtual <see cref="Mouse"/> device and drives its input from gamepad-style inputs. This effectively
    /// adds a software mouse cursor.
    /// </summary>
    /// <remarks>
    /// This component can be used with UIs that are designed for mouse input, i.e. need to be operated with a cursor.
    /// By hooking up the <see cref="InputAction"/>s of this component to gamepad input and directing <see cref="cursorTransform"/>
    /// to the UI transform of the cursor, you can use this component to drive an on-screen cursor.
    ///
    /// Note that this component does not actually trigger UI input itself. Instead, it creates a virtual <see cref="Mouse"/>
    /// device which can then be picked up elsewhere (such as by <see cref="InputSystemUIInputModule"/>) where mouse/pointer input
    /// is expected.
    ///
    /// Also note that if there is a <see cref="Mouse"/> added by the platform, it is not impacted by this component. More specifically,
    /// the system mouse cursor will not be moved or otherwise used by this component.
    /// </remarks>
    /// <seealso cref="Gamepad"/>
    /// <seealso cref="Mouse"/>
    public class UICursorVirtualMouseInput : MonoBehaviour, IUICursor
    {

        /// <summary>
        /// The cursor controller behaviour.
        /// </summary>
        public UICursorBehaviour Cursor
        {
            get => m_cursor;
            set
            {
                m_cursor = value;
            }
        }

        /// <summary>
        /// Multiplier for values received from <see cref="scrollWheelAction"/>.
        /// </summary>
        /// <value>Multiplier for scroll values.</value>
        public float scrollSpeed
        {
            get => m_ScrollSpeed;
            set => m_ScrollSpeed = value;
        }

        /// <summary>
        /// The virtual mouse device that the component feeds with input.
        /// </summary>
        /// <value>Instance of virtual mouse or <c>null</c>.</value>
        /// <remarks>
        /// This is only initialized after the component has been enabled for the first time. Note that
        /// when subsequently disabling the component, the property will continue to return the mouse device
        /// but the device will not be added to the system while the component is not enabled.
        /// </remarks>
        public Mouse virtualMouse => m_VirtualMouse;

        /// <summary>
        /// The Vector2 stick input that drives the mouse cursor, i.e. <see cref="Mouse.position"/> on
        /// <see cref="virtualMouse"/> and the <a
        /// href="https://docs.unity3d.com/ScriptReference/RectTransform-anchoredPosition.html">anchoredPosition</a>
        /// on <see cref="cursorTransform"/> (if set).
        /// </summary>
        /// <value>Stick input that drives cursor position.</value>
        /// <remarks>
        /// This should normally be bound to controls such as <see cref="Gamepad.leftStick"/> and/or
        /// <see cref="Gamepad.rightStick"/>.
        /// </remarks>
        public InputActionProperty stickAction
        {
            get => m_StickAction;
            set => SetAction(ref m_StickAction, value);
        }

        /// <summary>
        /// Optional button input that determines when <see cref="Mouse.leftButton"/> is pressed on
        /// <see cref="virtualMouse"/>.
        /// </summary>
        /// <value>Input for <see cref="Mouse.leftButton"/>.</value>
        public InputActionProperty clickAction
        {
            get => m_ClickAction;
            set
            {
                if (m_ButtonActionTriggeredDelegate != null)
                    SetActionCallback(m_ClickAction, m_ButtonActionTriggeredDelegate, false);
                SetAction(ref m_ClickAction, value);
                if (m_ButtonActionTriggeredDelegate != null)
                    SetActionCallback(m_ClickAction, m_ButtonActionTriggeredDelegate, true);
            }
        }

        /// <summary>
        /// Optional Vector2 value input that determines the value of <see cref="Mouse.scroll"/> on
        /// <see cref="virtualMouse"/>.
        /// </summary>
        /// <value>Input for <see cref="Mouse.scroll"/>.</value>
        /// <remarks>
        /// In case you want to only bind vertical scrolling, simply have a <see cref="Composites.Vector2Composite"/>
        /// with only <c>Up</c> and <c>Down</c> bound and <c>Left</c> and <c>Right</c> deleted or bound to nothing.
        /// </remarks>
        public InputActionProperty scrollWheelAction
        {
            get => m_ScrollWheelAction;
            set => SetAction(ref m_ScrollWheelAction, value);
        }

        private Vector3 m_currentMousePosition;
        private bool m_isSystemMouseMoving = false;
        private float m_referenceResolutionWidth = 800f;
        private bool m_FirstEnabled = false;
        private GameObject m_lastGameObjectOver;
        private float m_ScreenWidth;
        private float m_ScreenHeight;

        private bool m_UpdateCursor = true;
        private bool m_SetPosition = false;
        private Vector2 m_GoToPosition = Vector2.zero; 

        MouseState mouseState;

        public Vector2 CursorPosition { 
            get
            {
                 if (m_cursor != null)
                    return m_cursor.RectTransform.position; 
                return Vector2.zero; 
            } 
            set{
                SetCursorPosition(value);    
            }
        }
        internal void SetCursorPosition(Vector2 position)
        {
            m_SetPosition = true;
            m_GoToPosition = position;
        }

        protected void OnEnable()
        {
            TryGetHardwareCursor();
            UICursorController.current = this;
            m_FirstEnabled = true;
            m_ScreenWidth = Screen.width;
            m_ScreenHeight = Screen.height;

            // Add mouse device.
            if (m_VirtualMouse == null)
                m_VirtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
            else if (!m_VirtualMouse.added)
                InputSystem.AddDevice(m_VirtualMouse);

            // Hook into input update.
            if (m_AfterInputUpdateDelegate == null)
                m_AfterInputUpdateDelegate = OnAfterInputUpdate;
            InputSystem.onAfterUpdate += m_AfterInputUpdateDelegate;

            // Hook into actions.
            if (m_ButtonActionTriggeredDelegate == null)
                m_ButtonActionTriggeredDelegate = OnButtonActionTriggered;
            SetActionCallback(m_ClickAction, m_ButtonActionTriggeredDelegate, true);
        
            // Enable actions.
            m_StickAction.action?.Enable();
            m_ClickAction.action?.Enable();
            m_ScrollWheelAction.action?.Enable();
        }

        protected void OnDisable()
        {
            UICursorController.current = null;
            m_FirstEnabled = false;
            // Disable actions.
            if(m_StickAction.reference == null)
                m_StickAction.action?.Disable();
            
            if(m_ClickAction.reference == null)
                m_ClickAction.action?.Disable();

            if(m_ScrollWheelAction.reference == null)
                m_ScrollWheelAction.action?.Disable();

            // Unhock from actions.
            if (m_ButtonActionTriggeredDelegate != null)
            {
                SetActionCallback(m_ClickAction, m_ButtonActionTriggeredDelegate, false);
            }

            // Remove mouse device.
            if (m_VirtualMouse != null && m_VirtualMouse.added)
                InputSystem.RemoveDevice(m_VirtualMouse);

            // Remove ourselves from input update.
            if (m_AfterInputUpdateDelegate != null)
                InputSystem.onAfterUpdate -= m_AfterInputUpdateDelegate;
        }

        private void TryGetHardwareCursor()
        {
            var devices = InputSystem.devices;
            for (var i = 0; i < devices.Count; ++i)
            {
                var device = devices[i];
                if (device.native && device is Mouse mouse)
                {
                    m_SystemMouse = mouse;
                    break;
                }
            }
        }

        private void ProcessOverGameObject(PointerEventData pointerEvent)
		{
            //Debug.Log(PointerEventData.pointerCurrentRaycast);
            if (pointerEvent != null && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject ()) {
				if (m_lastGameObjectOver != pointerEvent.pointerCurrentRaycast.gameObject) {
				
					var clickHandler = ExecuteEvents.GetEventHandler<IPointerDownHandler> (pointerEvent.pointerCurrentRaycast.gameObject);
					if (clickHandler == null) {
						clickHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler> (pointerEvent.pointerCurrentRaycast.gameObject);
					}
					if (clickHandler != null) {
						m_cursor.ClickableElement = pointerEvent.pointerCurrentRaycast.gameObject;
					} else {
						m_cursor.ClickableElement = null;
					}
					m_cursor.OnClickableElementChanged();
					m_lastGameObjectOver = pointerEvent.pointerCurrentRaycast.gameObject;
				}
			} else {
				if (m_lastGameObjectOver != null) {
					m_cursor.ClickableElement = null;
					m_cursor.OnClickableElementChanged();
				}
				m_lastGameObjectOver = null;
			}
        }

        /// <summary>
        /// Callback sent to all game objects when the player gets or loses focus.
        /// </summary>
        /// <param name="focusStatus">The focus state of the application.</param>
        void OnApplicationFocus(bool focusStatus)
        {
            m_UpdateCursor = focusStatus;
        }

        private void UpdateMotion()
        {
            if (m_VirtualMouse == null)
                return;
            
            m_currentMousePosition = Vector3.zero;
            m_isSystemMouseMoving = false;

            if(m_SystemMouse != null)
            {
                m_currentMousePosition = m_SystemMouse.position.ReadValue();
                var currentMouseDelta = m_SystemMouse.delta.ReadValue();
                m_isSystemMouseMoving = currentMouseDelta.magnitude > 0.25f;
            }

            // Set initial cursor position.
            if (m_FirstEnabled == true)
            {
                m_FirstEnabled = false;
                var position = m_cursor.RectTransform.position;
                InputState.Change(m_VirtualMouse.position, position);
            }

            // Read current stick value.
            var stickAction = m_StickAction.action;
            if (stickAction == null)
                return;
            var stickValue = stickAction.ReadValue<Vector2>();
            bool motionStop = Mathf.Approximately(0, stickValue.x) && Mathf.Approximately(0, stickValue.y);
            if((motionStop && m_isSystemMouseMoving) || m_SystemMouse == null)
            {
                motionStop = false;
            }

            if (!motionStop || m_SetPosition)
            {
                var currentTime = InputState.currentTime;

                float speedThisFrame = (m_ScreenWidth * m_cursor.m_Speed) / m_referenceResolutionWidth;

                var delta = (speedThisFrame * stickValue) * Time.unscaledDeltaTime;
                
                //var currentPosition = m_VirtualMouse.position.ReadValue();
                var currentPosition = (Vector2)m_cursor.RectTransform.position;

                if(m_SystemMouse != null && m_isSystemMouseMoving)
                {
                    delta = m_currentMousePosition - ((Vector3)currentPosition);
                }

                if(m_SetPosition)
                {
                    delta = m_GoToPosition - currentPosition;
                    m_SetPosition = false;
                }

                // Update position.
                var newPosition = currentPosition + delta;

                newPosition.x = Mathf.Clamp(newPosition.x, 0, m_ScreenWidth);
                newPosition.y = Mathf.Clamp(newPosition.y, 0, m_ScreenHeight);

                ////REVIEW: the fact we have no events on these means that actions won't have an event ID to go by; problem?
                InputState.Change(m_VirtualMouse.position, newPosition);
                InputState.Change(m_VirtualMouse.delta, delta);

                ProcessOverGameObject(UICursorOverRaycaster.LastPointerEventData);

                // Update software cursor transform, if any.
                if (m_cursor != null)
                    m_cursor.RectTransform.position = newPosition;

            }

            // Update scroll wheel.
            var scrollAction = m_ScrollWheelAction.action;
            if (scrollAction != null)
            {
                var scrollValue = scrollAction.ReadValue<Vector2>();
                scrollValue.x *= m_ScrollSpeed;
                scrollValue.y *= m_ScrollSpeed;

                InputState.Change(m_VirtualMouse.scroll, scrollValue);
            }
        }

        [Header("Cursor")]
        [SerializeField] private UICursorBehaviour m_cursor;

        [Header("Motion")]
        [SerializeField] private float m_ScrollSpeed = 45;

        [Space(10)]
        [SerializeField] private InputActionProperty m_StickAction;
        [SerializeField] private InputActionProperty m_ClickAction;
        [SerializeField] private InputActionProperty m_ScrollWheelAction;

        private Mouse m_VirtualMouse;
        private Mouse m_SystemMouse;
        private Action m_AfterInputUpdateDelegate;
        private Action<InputAction.CallbackContext> m_ButtonActionTriggeredDelegate;

        private void SendEventAction(InputAction.CallbackContext context)
        {
            if (m_VirtualMouse == null)
                return;

            // The button controls are bit controls. We can't (yet?) use InputState.Change to state
            // the change of those controls as the state update machinery of InputManager only supports
            // byte region updates. So we just grab the full state of our virtual mouse, then update
            // the button in there and then simply overwrite the entire state.
            var action = context.action;
            MouseButton? button = null;
            if (action == m_ClickAction.action)
            {
                button = MouseButton.Left;
            }

            if (button != null)
            {
                var isPressed = context.control.IsPressed();
                if(m_VirtualMouse.leftButton.isPressed != isPressed || context.canceled)
                {
                    m_VirtualMouse.CopyState<MouseState>(out mouseState);
                    mouseState.WithButton(button.Value, isPressed);

                    InputState.Change(m_VirtualMouse, mouseState);

                    //released
                    if(m_cursor != null && !isPressed)
                    {
                        m_cursor.OnClick();
                        //InputState.Change(m_VirtualMouse.position, m_cursor.RectTransform.position);
                        //ProcessOverGameObject(UICursorOverRaycaster.LastPointerEventData);
                    }
                }
            }
        }

        private void OnButtonActionTriggered(InputAction.CallbackContext context)
        {
            if(EventSystem.current != null && EventSystem.current.sendNavigationEvents == false)
            {
                SendEventAction(context);
            }
        }

        private static void SetActionCallback(InputActionProperty field, Action<InputAction.CallbackContext> callback, bool install = true)
        {
            var action = field.action;
            if (action == null)
                return;

            if (install)
            {
                //action.started += callback;
                action.performed += callback;
                //action.canceled += callback;
            }
            else
            {
                //action.started -= callback;
                action.performed -= callback;
                //action.canceled -= callback;
            }
        }

        private static void SetAction(ref InputActionProperty field, InputActionProperty value)
        {
            var oldValue = field;
            field = value;

            if (oldValue.reference == null)
            {
                var oldAction = oldValue.action;
                if (oldAction != null && oldAction.enabled)
                {
                    oldAction.Disable();
                    if (value.reference == null)
                        value.action?.Enable();
                }
            }
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            m_ScreenWidth = Screen.width;
            m_ScreenHeight = Screen.height;
        }

        private void OnAfterInputUpdate()
        {
            if(EventSystem.current != null && EventSystem.current.sendNavigationEvents == false)
            {
                if(m_UpdateCursor)
                {
                    UpdateMotion(); 
                }
            }
        }
    }
}
#endif