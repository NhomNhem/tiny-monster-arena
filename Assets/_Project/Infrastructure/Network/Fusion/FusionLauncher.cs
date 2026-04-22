using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using NhemDangFugBixs.Attributes;
using R3;
using TinyMonsterArena.Application.Services.Interfaces;
using TinyMonsterArena.Shared.Interfaces.Scope;
using UnityEngine;

namespace TinyMonsterArena.Infrastructure.Network.Fusion {
    public class FusionLauncher : MonoBehaviour, INetworkRunnerCallbacks, INetworkService{

        [SerializeField] private NetworkSceneManagerDefault _sceneManager;
        
        private NetworkRunner _runner;
        private INetworkService _networkServiceImplementation;
        
        private readonly ReactiveProperty<string> _status = new("Ready");
        public ReadOnlyReactiveProperty<string> ConnectionStatus => _status;

        public async void StartGame(GameMode mode) {
            if (_runner == null)
                _runner = gameObject.AddComponent<NetworkRunner>();
            
            _runner.ProvideInput = true;
            _status.Value = "Connecting...";

            var result = await _runner.StartGame(new StartGameArgs {
                GameMode = mode,
                SessionName = "Tiny Monster Arena Room",
                SceneManager = _sceneManager,
                PlayerCount = 4
            });
            
            if (result.Ok)
                _status.Value = "Connected";
            else {
                _status.Value = "Failed to connect";
                Debug.LogError($"Failed to start game: {result.ShutdownReason}");
            }
        }
        
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {
        }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {
        }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
        }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {
        }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {
        }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {
        }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {
        }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {
        }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {
        }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {
        }
        public void OnInput(NetworkRunner runner, NetworkInput input) {
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
    }
}