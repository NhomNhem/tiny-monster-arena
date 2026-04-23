using Fusion.Addons.FSM;
using UnityEngine;

namespace TinyMonsterArena.Infrastructure.Network.Fusion.Player.States {
    public class MoveState : StateBehaviour {
        private PlayerNetwork _player;

        protected override void OnInitialize() {
            _player = transform.root.GetComponentInChildren<PlayerNetwork>(true);
        }

        protected override void OnEnterState() {
            _player.SetLocomotionMode(PlayerLocomotionMode.Moving);
            _player.SetActionPhase(PlayerActionPhase.None);
        }

        protected override void OnFixedUpdate() {
            if (_player == null) {
                return;
            }

            if (!_player.HasMoveIntent) {
                _player.SetLocomotionMode(PlayerLocomotionMode.Idle);
                _player.KCC.SetInputDirection(Vector3.zero);
                _player.KCC.SetKinematicVelocity(Vector3.zero);
                if (!Machine.TryActivateState(_player.IdleState.StateId)) {
                    _player.LogStateTransitionFailure(nameof(MoveState), nameof(IdleState), "Move intent ended.");
                }
                return;
            }

            _player.SetLocomotionMode(PlayerLocomotionMode.Moving);

            Vector3 moveDir = _player.MoveDirection;
            _player.KCC.SetInputDirection(moveDir);
            _player.KCC.SetKinematicVelocity(moveDir * _player.MoveSpeed);
            _player.KCC.SetLookRotation(Quaternion.LookRotation(moveDir));

            if (_player.TryConsumeBufferedAttack() && !Machine.TryActivateState(_player.AttackState.StateId)) {
                _player.LogStateTransitionFailure(nameof(MoveState), nameof(AttackState), "Buffered attack should interrupt locomotion.");
            }
        }

        protected override void OnRender() {
            if (_player?.PlayerView != null) {
                _player.PlayerView.SetLocomotion(true);
            }
        }
    }
}
