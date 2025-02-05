using System;
using System.Collections.Generic;
using Entropy.Scripts.Player;
using Fusion;
using Fusion.Sockets;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Player;

namespace Vashta.Entropy.Network
{
    public class NetworkInputController : NetworkBehaviour, INetworkRunnerCallbacks
    {
        // Can use this to disable input
        public static bool fetchInput = true;
        
        private GameManager _gameManager;
        private PlayerInputController _playerInputController;
        private PlayerController _playerController;
        private NetworkInputData _inputData;
        private Vector2 _moveDelta;
        private Vector2 _aimDelta;

        private uint _buttonReset;
        private uint _buttonSample;
        
        public override void Spawned()
        {
            // Load dependencies
            _gameManager = GameManager.GetInstance();
            _playerController = GetComponent<PlayerController>();
            _playerInputController = _gameManager.PlayerInputController;

            if (Object.HasInputAuthority)
            {
                Runner.AddCallbacks(this);
            }
            
            Debug.Log("Spawned [" + this + "] IsClient=" + Runner.IsClient + " IsServer=" + Runner.IsServer + " IsInputSrc=" + Object.HasInputAuthority + " IsStateSrc=" + Object.HasStateAuthority);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (_playerController!=null && _playerController.IsAlive)
            {
                _inputData.aimDirection = _aimDelta.normalized;
                _inputData.moveDirection = _moveDelta.normalized;
                _inputData.Buttons = _buttonSample;
                _buttonReset |= _buttonSample; // This effectively delays the reset of the read button flags until next Update() in case we're ticking faster than we're rendering
            }

            // Hand over the data to Fusion
            input.Set(_inputData);
            _inputData.Buttons = 0;
        }

        private void Update()
        {
            // if (!Object.HasInputAuthority)
            // {
            //     return;
            // }

            if (!_playerInputController)
            {
                Debug.Log("PlayerInputController not found!");
                return;
            }
            
            // Resets sample values
            _buttonSample &= ~_buttonReset;

            // Only capture input if gameplay actions are not blocked by panels
            if (!_playerInputController.GameplayActionsBlocked())
            {
                if (_playerInputController.FireButton.IsDown)
                    _buttonSample |= NetworkInputData.BUTTON_FIRE_PRIMARY;

                if (_playerInputController.PowerupButton.IsDown)
                    _buttonSample |= NetworkInputData.BUTTON_FIRE_POWERUP;

                if (_playerInputController.UltimateButton.IsDown)
                    _buttonSample |= NetworkInputData.BUTTON_FIRE_ULTIMATE;

                if (_playerInputController.DropSpoonButton.IsDown)
                    _buttonSample |= NetworkInputData.BUTTON_DROP_FLAG;
                
                _moveDelta = _playerInputController.GetAdapter().GetMovementVector(out bool isMoving);
                _aimDelta = _playerInputController.GetAdapter().GetTurretRotation(_playerController.transform.position);
            }
        }
        
        public struct NetworkInputData : INetworkInput
        {
            public const uint BUTTON_FIRE_PRIMARY = 1 << 0; // 0001
            public const uint BUTTON_FIRE_POWERUP = 1 << 1; // 0010
            public const uint BUTTON_FIRE_ULTIMATE = 1 << 2; // 0100
            public const uint BUTTON_DROP_FLAG = 1 << 3; // 1000

            public uint Buttons;
            public Vector2 aimDirection;
            public Vector2 moveDirection;

            public bool IsUp(uint button)
            {
                return IsDown(button) == false;
            }

            public bool IsDown(uint button)
            {
                return (Buttons & button) == button;
            }

            public bool WasPressed(uint button, NetworkInputData oldInput)
            {
                return (oldInput.Buttons & button) == 0 && (Buttons&button)==button;
            }
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {}
		
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {}

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
    }
}