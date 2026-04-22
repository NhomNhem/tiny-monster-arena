using Fusion.Addons.FSM;
using UnityEngine;

namespace TinyMonsterArena.Infrastructure.Network.Fusion.Player.States {
    public class MoveState : StateBehaviour {
        private PlayerNetwork _player;

        protected override void OnInitialize() {
            _player = GetComponentInParent<PlayerNetwork>();
        }

        protected override void OnFixedUpdate() {
            var input = _player.Input;

            if (input.Move == Vector2.zero) {
                Machine.TryActivateState(_player.IdleState.StateId);
                return;
            }

            // Xử lý di chuyển KCC
            Vector3 moveDir = new Vector3(input.Move.x, 0, input.Move.y);
            _player.KCC.SetInputDirection(moveDir);
            
            // Xoay nhân vật (Advanced KCC hỗ trợ SetLookRotation)
            _player.KCC.SetLookRotation(Quaternion.LookRotation(moveDir));

            if (input.Attack) {
                Machine.TryActivateState(_player.AttackState.StateId);
            }
        }

        protected override void OnEnterStateRender() {
            _player.PlayerView.PlayRun();
        }
    }
}