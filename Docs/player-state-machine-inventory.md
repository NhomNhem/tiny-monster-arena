## Current Flow Inventory

### Current top-level states
- `IdleState`: zeroes KCC input and velocity on enter, checks `Attack` first, then moves to `MoveState` if `Move` input is above epsilon, and triggers `PlayerView.PlayIdle()` in render enter.
- `MoveState`: drives KCC input, kinematic velocity, and facing directly from current move input, returns to `IdleState` when input stops, and can jump straight to `AttackState`.
- `AttackState`: zeroes movement on enter, owns attack timing with `_hitActiveTime` and `_totalDuration`, then returns to `MoveState` or `IdleState` at the end based on current move input.

### Responsibility mapping in the current implementation
- Locomotion domain:
  - `IdleState` owns idle detection and move-entry transition.
  - `MoveState` owns movement simulation and move-exit transition.
- Action domain:
  - `AttackState` owns wind-up, hit timing, recovery timing, and post-attack exit.
- Interrupt domain:
  - `IdleState` and `MoveState` can both transition to attack immediately.
  - `AttackState` suppresses movement until its total duration expires.
  - There is no explicit buffer contract in the state machine itself; buffering currently depends on `InputReader` plus direct state checks.

### Current coupling problems
- Gameplay transition completion is encoded inside state classes and timed by `Machine.StateTime`, but visual playback is still triggered directly from render enter methods.
- The state machine does not expose an authoritative simulation snapshot for presentation or debugging.
- Movement intent during attack recovery is inferred from live input instead of a documented buffer/interrupt contract.
- The current shape does not scale to upper-body-only combat because `AttackState` conceptually owns the whole body flow.

## Target Mapping For This Refactor

### Simulation contract
- Locomotion remains responsible for idle versus moving intent.
- Action state becomes phase-driven: `None`, `Windup`, `Active`, `Recovery`.
- Interrupt rules become explicit through simulation-authored checks:
  - attack start consumes buffered attack intent
  - movement resumes only when the action phase allows it
  - recovery overrun is treated as an invalid state and logged

### Presentation contract
- Presentation reacts to a simulation-authored snapshot instead of deciding gameplay exit timing.
- Animator layering and avatar mask work will consume locomotion mode and action phase as inputs in later tasks.
