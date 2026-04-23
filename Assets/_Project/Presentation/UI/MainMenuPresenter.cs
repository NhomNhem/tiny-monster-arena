using System;
using Fusion;
using R3;
using TinyMonsterArena.Application.Services.Interfaces;
using UnityEngine.UIElements;
using VContainer.Unity;

namespace TinyMonsterArena.Presentation.UI {
    public class MainMenuPresenter : IStartable, IDisposable {
        private readonly INetworkService _networkService;
        private readonly UIDocument _uiDocument;
        private readonly CompositeDisposable _disposables = new();

        private Button _btnJoin;

        public MainMenuPresenter(INetworkService networkService, UIDocument uiDocument) {
            _networkService = networkService;
            _uiDocument = uiDocument;
        }

        public void Start() {
            var root = _uiDocument.rootVisualElement;
            _btnJoin = root.Q<Button>("btn-join");
            var statusLabel = root.Q<Label>("status-label");

            if (_btnJoin == null) {
                return;
            }

            _btnJoin.clicked += OnJoinClicked;

            _networkService.ConnectionStatus
                .Subscribe(status => {
                    if (statusLabel != null) {
                        statusLabel.text = status;
                    }

                    _btnJoin.SetEnabled(status is not "Connecting..." and not "Connected");
                })
                .AddTo(_disposables);
        }

        private void OnJoinClicked() {
            _btnJoin.SetEnabled(false);
            _networkService.StartGame(GameMode.AutoHostOrClient);
        }

        public void Dispose() {
            _disposables.Dispose();
            if (_btnJoin != null) {
                _btnJoin.clicked -= OnJoinClicked;
            }
        }
    }
}
