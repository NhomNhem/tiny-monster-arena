namespace TinyMonsterArena.Infrastructure.Network.Fusion.Player {
    public enum PlayerLocomotionMode {
        Idle,
        Moving
    }

    public enum PlayerActionPhase {
        None,
        Windup,
        Active,
        Recovery
    }

    public readonly struct PlayerSimulationState {
        public PlayerSimulationState(
            PlayerLocomotionMode locomotionMode,
            PlayerActionPhase actionPhase,
            bool hasMoveIntent,
            bool hasBufferedAttack) {
            LocomotionMode = locomotionMode;
            ActionPhase = actionPhase;
            HasMoveIntent = hasMoveIntent;
            HasBufferedAttack = hasBufferedAttack;
        }

        public PlayerLocomotionMode LocomotionMode { get; }
        public PlayerActionPhase ActionPhase { get; }
        public bool HasMoveIntent { get; }
        public bool HasBufferedAttack { get; }

        public bool IsInAction => ActionPhase != PlayerActionPhase.None;
        public bool CanMove => ActionPhase is PlayerActionPhase.None or PlayerActionPhase.Recovery;
        public bool CanStartAttack => ActionPhase == PlayerActionPhase.None;

        public PlayerSimulationState WithLocomotion(PlayerLocomotionMode locomotionMode) => new(locomotionMode, ActionPhase, HasMoveIntent, HasBufferedAttack);

        public PlayerSimulationState WithActionPhase(PlayerActionPhase actionPhase) => new(LocomotionMode, actionPhase, HasMoveIntent, HasBufferedAttack);

        public PlayerSimulationState WithIntent(bool hasMoveIntent, bool hasBufferedAttack) => new(LocomotionMode, ActionPhase, hasMoveIntent, hasBufferedAttack);
    }
}
