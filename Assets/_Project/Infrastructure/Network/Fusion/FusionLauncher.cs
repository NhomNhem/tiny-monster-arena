using System;
using System.Threading.Tasks;
using Fusion;
using R3;
using TinyMonsterArena.Application.Services.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TinyMonsterArena.Infrastructure.Network.Fusion {
    public class FusionLauncher : MonoBehaviour, INetworkService {
        private const string DefaultBootstrapScenePath = "Assets/_Project/_Scenes/UI.unity";
        private const string DefaultGameplayScenePath = "Assets/_Project/_Scenes/Arena.unity";

        [SerializeField] private string _bootstrapScenePath = DefaultBootstrapScenePath;
        [SerializeField] private string _gameplayScenePath = DefaultGameplayScenePath;

        private NetworkRunner _runner;
        private bool _isStartingGame;
        private readonly ReactiveProperty<string> _status = new("Ready");

        public ReadOnlyReactiveProperty<string> ConnectionStatus => _status;
        public NetworkRunner Runner => _runner;
        public event Action<NetworkRunner> RunnerInitialized;

        public async void StartGame(GameMode mode) {
            if (_isStartingGame) {
                return;
            }

            if (_runner != null) {
                _status.Value = _runner.IsRunning ? "Connected" : "Connecting...";
                return;
            }

            _isStartingGame = true;
            _runner = GetComponent<NetworkRunner>() ?? gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;
            RunnerInitialized?.Invoke(_runner);

            var sceneManager = GetComponent<NetworkSceneManagerDefault>();
            if (sceneManager == null) {
                sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();
            }

            _status.Value = "Connecting...";

            try {
                await EnsureLocalSceneLoadedAsync(_bootstrapScenePath);

                var gameplayScene = sceneManager.GetSceneRef(_gameplayScenePath);
                if (!gameplayScene.IsValid) {
                    _status.Value = $"Gameplay scene not found: {_gameplayScenePath}";
                    CleanupRunner();
                    return;
                }

                var sceneInfo = new NetworkSceneInfo();
                sceneInfo.AddSceneRef(gameplayScene, LoadSceneMode.Additive);

                var result = await _runner.StartGame(new StartGameArgs {
                    GameMode = mode,
                    SessionName = "Tiny Monster Arena Room",
                    Scene = sceneInfo,
                    SceneManager = sceneManager,
                    PlayerCount = 4
                });

                if (result.Ok) {
                    _status.Value = "Connected";
                } else {
                    _status.Value = "Failed: " + result.ShutdownReason;
                    CleanupRunner();
                }
            }
            catch (Exception e) {
                _status.Value = "Error occurred";
                Debug.LogException(e);
                CleanupRunner();
            }
            finally {
                _isStartingGame = false;
            }
        }

        private static async Task EnsureLocalSceneLoadedAsync(string scenePath) {
            if (string.IsNullOrWhiteSpace(scenePath)) {
                return;
            }

            var scene = SceneManager.GetSceneByPath(scenePath);
            if (scene.isLoaded) {
                return;
            }

            var operation = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
            if (operation == null) {
                throw new InvalidOperationException($"Unable to load local scene: {scenePath}");
            }

            while (!operation.isDone) {
                await Task.Yield();
            }
        }

        private void CleanupRunner() {
            if (_runner == null) {
                return;
            }

            Destroy(_runner);
            _runner = null;
        }
    }
}
