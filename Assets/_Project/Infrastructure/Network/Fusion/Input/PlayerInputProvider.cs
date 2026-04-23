using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using TinyMonsterArena.Infrastructure.Input;
using UnityEngine;
using UnityEngine.InputSystem;

using VContainer;

namespace TinyMonsterArena.Infrastructure.Network.Fusion.Input {
    public class PlayerInputProvider : MonoBehaviour, INetworkRunnerCallbacks {
        private InputReader _inputReader;
        private FusionLauncher _launcher;
        private NetworkRunner _registeredRunner;
        private bool _isSubscribedToLauncher;

        [Inject]
        public void Construct(FusionLauncher launcher) {
            _launcher = launcher;
        }

        private void Awake() {
            _inputReader = GetComponent<InputReader>();
        }

        private void Start() {
            TryResolveLauncherAndRegister();
        }

        private void Update() {
            if (_registeredRunner == null) {
                TryResolveLauncherAndRegister();
            }
        }

        private void OnDestroy() {
            if (_launcher != null && _isSubscribedToLauncher) {
                _launcher.RunnerInitialized -= OnRunnerInitialized;
                _isSubscribedToLauncher = false;
            }

            if (_registeredRunner != null) {
                _registeredRunner.RemoveCallbacks(this);
                _registeredRunner = null;
            }
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token) { }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

        public void OnInput(NetworkRunner runner, NetworkInput input) {
            var data = new NetworkInputData() {
                Move = _inputReader.Move,
                Attack = _inputReader.ConsumeAttack()
            };

            input.Set(data);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {
        }
        public void OnConnectedToServer(NetworkRunner runner) {
        }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
        }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {
        }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {
        }
        public void OnSceneLoadDone(NetworkRunner runner) {
        }
        public void OnSceneLoadStart(NetworkRunner runner) {
        }

        private void OnRunnerInitialized(NetworkRunner runner) {
            TryRegisterCallbacks(runner);
        }

        private void TryResolveLauncherAndRegister() {
            if (_launcher == null) {
                _launcher = FindFirstObjectByType<FusionLauncher>(FindObjectsInactive.Include);
            }

            if (_launcher == null) {
                return;
            }

            if (!_isSubscribedToLauncher) {
                _launcher.RunnerInitialized += OnRunnerInitialized;
                _isSubscribedToLauncher = true;
            }

            TryRegisterCallbacks(_launcher.Runner);
        }

        private void TryRegisterCallbacks(NetworkRunner runner) {
            if (runner == null || ReferenceEquals(_registeredRunner, runner)) {
                return;
            }

            if (_registeredRunner != null) {
                _registeredRunner.RemoveCallbacks(this);
            }

            runner.AddCallbacks(this);
            _registeredRunner = runner;
        }
    }
    
}

