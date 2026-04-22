using Fusion.Addons.FSM;
using TinyMonsterArena.Application.UseCases;
using UnityEngine;
using VContainer;

namespace TinyMonsterArena.Infrastructure.Network.Fusion.Player.States {
    public class AttackState : StateBehaviour {
        private PlayerNetwork _player;

        [SerializeField] private float _totalDuration = 0.6f;
        [SerializeField] private float _hitActiveTime = 0.2f;
        private bool _hitProcessed;

        protected override void OnInitialize() {
            _player = GetComponentInParent<PlayerNetwork>();
        }

        protected override void OnEnterState() {
            _hitProcessed = false;
            // Dừng di chuyển KCC
            _player.KCC.SetInputDirection(Vector3.zero);
        }

        protected override void OnFixedUpdate() {
            // SỬ DỤNG Machine.StateTime (Theo file StateMachine.cs bạn gửi)
            if (!_hitProcessed && Machine.StateTime >= _hitActiveTime) {
                _hitProcessed = true;
                // Execute combat logic
            }

            if (Machine.StateTime >= _totalDuration) {
                Machine.TryActivateState(_player.IdleState.StateId);
            }
        }

        protected override void OnEnterStateRender() {
            _player.PlayerView.PlayAttack();
        }
    }
}