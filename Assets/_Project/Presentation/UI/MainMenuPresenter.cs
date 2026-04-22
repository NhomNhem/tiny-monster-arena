using Fusion;
using R3;
using TinyMonsterArena.Application.Services.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace TinyMonsterArena.Presentation.UI {
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuPresenter : MonoBehaviour {
        [Inject] private readonly INetworkService _networkService;

        private VisualElement _root;
        private Button _btnJoin;
        private readonly CompositeDisposable _disposables = new();

        // TUYỆT ĐỐI không dùng OnEnable để truy cập Service được Inject
        private void Start() {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _btnJoin = _root.Q<Button>("btn-join");

            if (_networkService == null) {
                Debug.LogError("VContainer: INetworkService chưa được Inject vào MainMenuPresenter!");
                return;
            }

            _btnJoin.clicked += () => _networkService.StartGame(Fusion.GameMode.AutoHostOrClient);

            _networkService.ConnectionStatus
                .Subscribe(status => Debug.Log($"Status: {status}"))
                .AddTo(_disposables);
        }

        private void OnDestroy() => _disposables.Dispose();
    }
}