using Fusion;
using UnityEngine;

namespace TinyMonsterArena.Infrastructure.Network.Fusion.Input {
    public struct NetworkInputData : INetworkInput{
        public Vector2 Move;
        public NetworkBool Attack;
    }
}