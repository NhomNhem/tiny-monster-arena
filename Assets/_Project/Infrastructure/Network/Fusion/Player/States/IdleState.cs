using Fusion.Addons.FSM;
using UnityEngine;

namespace TinyMonsterArena.Infrastructure.Network.Fusion.Player.States {
    public class IdleState : StateBehaviour {
        private PlayerNetwork _player;

        protected override void OnInitialize() {
            // Lấy ref từ cha (PlayerNetwork)
            _player = GetComponentInParent<PlayerNetwork>();
        }

        protected override void OnEnterState() {
            // Khi đứng yên, ta triệt tiêu hướng di chuyển của KCC
            if (_player.KCC != null) {
                _player.KCC.SetInputDirection(Vector3.zero);
            }
        }

        protected override void OnFixedUpdate() {
            var input = _player.Input;

            // 1. Ưu tiên chuyển sang Attack nếu có input
            if (input.Attack) {
                if (Machine.TryActivateState(_player.AttackState.StateId)) {
                    return;
                }
            }

            // 2. Chuyển sang Move nếu có di chuyển
            // Chúng ta dùng một khoảng epsilon nhỏ để tránh nhiễu joystick
            if (input.Move.sqrMagnitude > 0.01f) {
                Machine.TryActivateState(_player.MoveState.StateId);
            }
        }

        protected override void OnEnterStateRender() {
            // Thực thi animation Idle thông qua View
            if (_player.PlayerView != null) {
                _player.PlayerView.PlayIdle();
            }
        }
    }
}