using Fusion;
using R3;

namespace TinyMonsterArena.Application.Services.Interfaces {
    public interface INetworkService {
        ReadOnlyReactiveProperty<string> ConnectionStatus { get; }
        void StartGame(GameMode mode);
    }
}