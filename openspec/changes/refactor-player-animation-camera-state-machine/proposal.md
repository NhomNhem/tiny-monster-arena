## Why

The current player state machine couples gameplay state, locomotion animation, attack timing, and scene camera behavior too tightly, which makes small fixes unstable and causes repeated regressions in multiplayer. We need a spec-driven redesign now because the existing ad-hoc flow is already blocking reliable movement, attack recovery, multi-scene boot, and multi-peer camera behavior.

## What Changes

- Redesign the player gameplay state machine around clear responsibilities for input intent, locomotion, attack windows, and state exit conditions.
- Introduce animation layering rules with avatar masks so upper-body combat can coexist with lower-body locomotion without locking the full character pose.
- Define a camera ownership and scene-layering model for multiplayer and multi-peer play mode so only the correct local camera/audio stack is active.
- Separate visual presentation rules from network simulation rules to reduce prefab hierarchy fragility and avoid animation or camera fixes leaking into gameplay authority code.
- Establish migration constraints for replacing the current state machine incrementally instead of rewriting all player logic in one step.

## Capabilities

### New Capabilities
- `player-gameplay-state-machine`: Defines the authoritative player state flow, transitions, interrupt rules, and ownership boundaries between simulation and presentation.
- `player-animation-and-camera-layering`: Defines animation layer masking, visual state synchronization, and local-only camera/audio activation rules for multiplayer scenes.

### Modified Capabilities

## Impact

- Affected code in `Assets/_Project/Infrastructure/Network/Fusion/Player/`, `Assets/_Project/Presentation/Player/`, and UI/network boot flow that currently starts gameplay from the menu scene.
- Animator controller, avatar mask assets, camera scene objects, and possibly player prefab composition will need refactoring.
- Multiplayer play mode setup in Fusion will need to align with the new camera and scene ownership rules.
- Future combat and movement features will depend on the new contracts, so this change should become the baseline for further player iteration.
