using Fusion;
using R3;

namespace TinyMonsterArena.Application.Services.Interfaces {
    public interface INetworkService {
        /// <summary>
        /// The current status of the connection to the server.
        /// </summary>
        ReadOnlyReactiveProperty<string> ConnectionStatus { get; }
        /// <summary>
        /// Starts the game.
        /// </summary>
        /// <param name="mode"></param>
        void StartGame(GameMode mode);
    }
}