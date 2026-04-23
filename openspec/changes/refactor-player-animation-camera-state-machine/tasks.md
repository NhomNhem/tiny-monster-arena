## 1. Gameplay State Contract

- [x] 1.1 Inventory the current player state flow and map existing `Idle`, `Move`, and `Attack` responsibilities to locomotion, action, and interrupt domains.
- [x] 1.2 Define the authoritative state contract in code-facing types so gameplay state no longer depends on animator clip state for transition completion.
- [x] 1.3 Implement buffered input and explicit recovery/interrupt rules for locomotion and attack transitions.
- [x] 1.4 Add targeted runtime validation or debug logging for invalid transitions and stuck recovery states.

## 2. Animation Layering

- [x] 2.1 Audit the current animator controller parameters, transitions, and attack playback assumptions that conflict with layered animation.
- [x] 2.2 Create avatar mask assets for the production player rig and document which bones belong to locomotion versus combat layers.
- [x] 2.3 Refactor the animator controller to use a base locomotion layer plus masked upper-body action layer.
- [x] 2.4 Update presentation binding so gameplay state drives animator layer parameters without feeding gameplay logic back from clip playback.

## 3. Camera and Local Presentation Ownership

- [ ] 3.1 Define the source of truth for local presentation ownership in multiplayer and multi-peer mode.
- [ ] 3.2 Refactor arena camera/audio activation so only the local presentation owner keeps singleton visual and listener components enabled.
- [ ] 3.3 Align additive `UI` + `Arena` scene boot with the new local camera ownership rules.
- [ ] 3.4 Verify scene-level camera/light behavior in Fusion multi-peer play mode and remove temporary scene hacks that are no longer needed.

## 4. Cleanup and Verification

- [ ] 4.1 Remove obsolete state logic, animator transitions, and prefab wiring that the new contracts replace.
- [ ] 4.2 Run focused multiplayer smoke tests for move, attack, attack recovery, and scene boot across host/client and multi-peer editor mode.
- [ ] 4.3 Document the final player architecture boundaries so future combat features extend the new state machine instead of bypassing it.
