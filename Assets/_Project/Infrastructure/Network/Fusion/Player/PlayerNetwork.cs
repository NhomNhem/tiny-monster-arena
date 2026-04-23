using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using Fusion.Addons.KCC;
using TinyMonsterArena.Infrastructure.Network.Fusion.Input;
using TinyMonsterArena.Infrastructure.Network.Fusion.Player.States;
using TinyMonsterArena.Presentation.Player;
using UnityEngine;

namespace TinyMonsterArena.Infrastructure.Network.Fusion.Player {
    public class PlayerNetwork : NetworkBehaviour, IStateMachineOwner {
        public KCC KCC;
        public PlayerView PlayerView;
        
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _attackWindupDuration = 0.1f;
        [SerializeField] private float _attackActiveDuration = 0.1f;
        [SerializeField] private float _attackRecoveryDuration = 0.15f;
        [SerializeField] private float _attackBufferDuration = 0.2f;
        [SerializeField] private float _stateRecoveryGraceDuration = 0.1f;
        [SerializeField] private bool _enableStateDebugLogs = true;

        public IdleState IdleState;
        public MoveState MoveState;
        public AttackState AttackState;

        public NetworkInputData Input { get; private set; }
        public float MoveSpeed => _moveSpeed;
        public float AttackWindupDuration => _attackWindupDuration;
        public float AttackActiveDuration => _attackActiveDuration;
        public float AttackRecoveryDuration => _attackRecoveryDuration;
        public float AttackTotalDuration => _attackWindupDuration + _attackActiveDuration + _attackRecoveryDuration;
        public float StateRecoveryGraceDuration => _stateRecoveryGraceDuration;
        public bool IsStateDebugLoggingEnabled => _enableStateDebugLogs;
        public PlayerSimulationState SimulationState { get; private set; }
        public bool HasMoveIntent => Input.Move.sqrMagnitude > 0.01f;
        public Vector3 MoveDirection => HasMoveIntent
            ? new Vector3(Input.Move.x, 0f, Input.Move.y).normalized
            : Vector3.zero;
        public bool HasBufferedAttack => _attackBufferRemaining > 0f;

        private StateMachine<StateBehaviour> _locomotionMachine;
        private float _attackBufferRemaining;

        public override void FixedUpdateNetwork() {
            Input = GetInput(out NetworkInputData input) ? input : default;

            TickBufferedInput();
            UpdateIntentSnapshot();
        }

        public void CollectStateMachines(List<IStateMachine> stateMachines) {
            _locomotionMachine ??= CreateLocomotionMachine();
            stateMachines.Add(_locomotionMachine);
        }

        private StateMachine<StateBehaviour> CreateLocomotionMachine() {
            var locomotionMachine = new StateMachine<StateBehaviour>(
                "Locomotion",
                IdleState,
                MoveState,
                AttackState);

            locomotionMachine.SetDefaultState(IdleState.StateId);
            return locomotionMachine;
        }

        public void SetLocomotionMode(PlayerLocomotionMode locomotionMode) {
            SimulationState = SimulationState.WithLocomotion(locomotionMode);
        }

        public void SetActionPhase(PlayerActionPhase actionPhase) {
            SimulationState = SimulationState.WithActionPhase(actionPhase);
        }

        public bool TryConsumeBufferedAttack() {
            if (!HasBufferedAttack) {
                return false;
            }

            _attackBufferRemaining = 0f;
            UpdateIntentSnapshot();
            return true;
        }

        public void LogStateTransitionFailure(string fromState, string toState, string reason) {
            if (!_enableStateDebugLogs) {
                return;
            }

            Debug.LogWarning(
                $"[PlayerState] Failed to transition {fromState} -> {toState}. " +
                $"Reason: {reason}. ActionPhase={SimulationState.ActionPhase}, " +
                $"Locomotion={SimulationState.LocomotionMode}, " +
                $"HasMoveIntent={SimulationState.HasMoveIntent}, HasBufferedAttack={SimulationState.HasBufferedAttack}",
                this);
        }

        public void LogRecoveryOverrun(string stateName, float stateTime) {
            if (!_enableStateDebugLogs) {
                return;
            }

            Debug.LogWarning(
                $"[PlayerState] {stateName} exceeded its recovery window. " +
                $"StateTime={stateTime:F3}, Expected<={AttackTotalDuration + _stateRecoveryGraceDuration:F3}",
                this);
        }

        private void TickBufferedInput() {
            if (Input.Attack) {
                _attackBufferRemaining = _attackBufferDuration;
                return;
            }

            if (_attackBufferRemaining <= 0f) {
                return;
            }

            _attackBufferRemaining = Mathf.Max(0f, _attackBufferRemaining - Runner.DeltaTime);
        }

        private void UpdateIntentSnapshot() {
            SimulationState = SimulationState.WithIntent(HasMoveIntent, HasBufferedAttack);
        }
    }
}
