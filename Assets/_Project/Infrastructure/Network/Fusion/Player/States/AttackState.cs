using Fusion.Addons.FSM;
using UnityEngine;

namespace TinyMonsterArena.Infrastructure.Network.Fusion.Player.States {
    public class AttackState : StateBehaviour {
        private PlayerNetwork _player;
        private bool _hitProcessed;
        private bool _attackRenderTriggered;
        private bool _recoveryOverrunLogged;

        protected override void OnInitialize() {
            _player = transform.root.GetComponentInChildren<PlayerNetwork>(true);
        }

        protected override void OnEnterState() {
            if (_player == null) {
                return;
            }

            _hitProcessed = false;
            _attackRenderTriggered = false;
            _recoveryOverrunLogged = false;
            _player.SetActionPhase(PlayerActionPhase.Windup);
            _player.SetLocomotionMode(_player.HasMoveIntent ? PlayerLocomotionMode.Moving : PlayerLocomotionMode.Idle);
            _player.KCC.SetInputDirection(Vector3.zero);
            _player.KCC.SetKinematicVelocity(Vector3.zero);
        }

        protected override void OnFixedUpdate() {
            if (_player == null) {
                return;
            }

            _player.SetLocomotionMode(_player.HasMoveIntent ? PlayerLocomotionMode.Moving : PlayerLocomotionMode.Idle);

            if (Machine.StateTime < _player.AttackWindupDuration) {
                _player.SetActionPhase(PlayerActionPhase.Windup);
            } else if (Machine.StateTime < _player.AttackWindupDuration + _player.AttackActiveDuration) {
                _player.SetActionPhase(PlayerActionPhase.Active);
            } else {
                _player.SetActionPhase(PlayerActionPhase.Recovery);
            }

            if (!_hitProcessed && Machine.StateTime >= _player.AttackWindupDuration) {
                _hitProcessed = true;
                // Execute combat logic
            }

            if (Machine.StateTime >= _player.AttackTotalDuration) {
                _player.SetActionPhase(PlayerActionPhase.None);

                if (_player.Input.Move.sqrMagnitude > 0.01f) {
                    if (!Machine.TryActivateState(_player.MoveState.StateId)) {
                        _player.LogStateTransitionFailure(nameof(AttackState), nameof(MoveState), "Recovery completed with move intent.");
                    }
                } else {
                    if (!Machine.TryActivateState(_player.IdleState.StateId)) {
                        _player.LogStateTransitionFailure(nameof(AttackState), nameof(IdleState), "Recovery completed without move intent.");
                    }
                }

                return;
            }

            if (!_recoveryOverrunLogged &&
                Machine.StateTime >= _player.AttackTotalDuration + _player.StateRecoveryGraceDuration) {
                _recoveryOverrunLogged = true;
                _player.LogRecoveryOverrun(nameof(AttackState), Machine.StateTime);

                if (_player.HasMoveIntent) {
                    if (!Machine.TryActivateState(_player.MoveState.StateId)) {
                        _player.LogStateTransitionFailure(nameof(AttackState), nameof(MoveState), "Recovery overrun fallback.");
                    }
                } else {
                    if (!Machine.TryActivateState(_player.IdleState.StateId)) {
                        _player.LogStateTransitionFailure(nameof(AttackState), nameof(IdleState), "Recovery overrun fallback.");
                    }
                }
            }
        }

        protected override void OnRender() {
            if (_player?.PlayerView == null) {
                return;
            }

            _player.PlayerView.SetLocomotion(_player.SimulationState.LocomotionMode == PlayerLocomotionMode.Moving);

            if (!_attackRenderTriggered && _player.SimulationState.ActionPhase == PlayerActionPhase.Windup) {
                _player.PlayerView.TriggerAttack();
                _attackRenderTriggered = true;
            }
        }
    }
}
