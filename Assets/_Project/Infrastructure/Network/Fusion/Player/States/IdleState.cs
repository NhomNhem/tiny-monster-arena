using Fusion.Addons.FSM;
using UnityEngine;

namespace TinyMonsterArena.Infrastructure.Network.Fusion.Player.States {
    public class IdleState : StateBehaviour {
        private PlayerNetwork _player;

        protected override void OnInitialize() {
            _player = transform.root.GetComponentInChildren<PlayerNetwork>(true);
        }

        protected override void OnEnterState() {
            _player.SetLocomotionMode(PlayerLocomotionMode.Idle);
            _player.SetActionPhase(PlayerActionPhase.None);

            if (_player?.KCC != null) {
                _player.KCC.SetInputDirection(Vector3.zero);
                _player.KCC.SetKinematicVelocity(Vector3.zero);
            }
        }

        protected override void OnFixedUpdate() {
            if (_player == null) {
                return;
            }

            var input = _player.Input;

            if (_player.TryConsumeBufferedAttack()) {
                if (Machine.TryActivateState(_player.AttackState.StateId)) {
                    return;
                }

                _player.LogStateTransitionFailure(nameof(IdleState), nameof(AttackState), "Buffered attack was available.");
            }

            if (input.Move.sqrMagnitude > 0.01f) {
                if (!Machine.TryActivateState(_player.MoveState.StateId)) {
                    _player.LogStateTransitionFailure(nameof(IdleState), nameof(MoveState), "Move intent should activate locomotion.");
                }
            }
        }

        protected override void OnRender() {
            if (_player?.PlayerView != null) {
                _player.PlayerView.SetLocomotion(false);
            }
        }
    }
}
