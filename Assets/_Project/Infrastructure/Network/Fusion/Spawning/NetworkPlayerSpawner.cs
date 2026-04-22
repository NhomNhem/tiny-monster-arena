using Fusion;
using UnityEngine;

namespace TinyMonsterArena.Infrastructure.Network.Fusion.Spawning {
    public class NetworkPlayerSpawner : SimulationBehaviour, IPlayerJoined {
        [SerializeField] private NetworkPrefabRef _playerPrefab;
        [SerializeField] private Transform[] _playerSpawnPoint;
        
        private NetworkDictionary<PlayerRef, NetworkObject> _spawnedPlayers = new();
        
        public void PlayerJoined(PlayerRef player) {
            if (Runner.IsServer) {
                SpawnPlayer(player);
            }
        }

        private void SpawnPlayer(PlayerRef player) {
            int index = player.RawEncoded % _playerSpawnPoint.Length;
            Vector3 spawnPosition = _playerSpawnPoint[index].position;
            
            Debug.Log($"Spawning player {player} at position {spawnPosition}");

            var playerObject = Runner.Spawn(
                _playerPrefab,
                spawnPosition,
                Quaternion.identity,
                player);
            
            _spawnedPlayers.Add(player, playerObject);
        }
    }
}