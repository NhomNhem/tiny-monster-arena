using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

using VContainer;

namespace TinyMonsterArena.Infrastructure.Network.Fusion.Spawning {
    public class NetworkPlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks {
        [SerializeField] private NetworkPrefabRef _playerPrefab;
        [SerializeField] private Transform[] _playerSpawnPoint;

        private FusionLauncher _launcher;
        private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new();
        private NetworkRunner _registeredRunner;
        private bool _isSubscribedToLauncher;

        [Inject]
        public void Construct(FusionLauncher launcher) {
            _launcher = launcher;
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
        
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {
        }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {
        }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
            if (runner.IsServer) {
                if (_spawnedPlayers.ContainsKey(player)) {
                    Debug.LogWarning($"[Spawner] Player {player} already has a spawned object. Skipping spawn.");
                    return;
                }
                SpawnPlayer(runner, player);
            }
        }
        
        private void SpawnPlayer(NetworkRunner runner, PlayerRef player) {
            if (_playerSpawnPoint == null || _playerSpawnPoint.Length == 0) {
                Debug.LogError("[Spawner] Cannot spawn player: _playerSpawnPoint is null or empty");
                return;
            }

            if (!_playerPrefab.IsValid) {
                Debug.LogError("[Spawner] Cannot spawn player: _playerPrefab is not assigned or invalid");
                return;
            }

            int index = player.RawEncoded % _playerSpawnPoint.Length;
            Vector3 spawnPosition = _playerSpawnPoint[index].position;
            
            Debug.Log($"[Spawner] Spawning player {player} tại {spawnPosition}");

            var playerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            if (playerObject == null) {
                Debug.LogError("[Spawner] Failed to spawn player object");
                return;
            }
            
            _spawnedPlayers.Add(player, playerObject);
            Debug.Log($"[Spawner] Successfully spawned player {player}");
        }
        
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
            if (runner.IsServer && _spawnedPlayers.TryGetValue(player, out var obj)) {
                runner.Despawn(obj);
                _spawnedPlayers.Remove(player);
            }
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
            Debug.Log($"[Spawner] Connected to server: {runner.LocalPlayer}");
            if (runner.IsServer && !_spawnedPlayers.ContainsKey(runner.LocalPlayer)) {
                SpawnPlayer(runner, runner.LocalPlayer);
            }
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
            Debug.Log("[Spawner] Registered callbacks");
            _registeredRunner = runner;
        }
    }
}
