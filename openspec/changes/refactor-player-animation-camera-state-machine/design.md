## Context

The current player flow mixes three concerns in the same implementation surface: authoritative network state, animator state playback, and scene-level camera/audio activation. Recent fixes showed that movement, attack, scene boot, and visual hierarchy issues are tightly coupled, so small changes in one area easily regress another. The project also now depends on additive `UI` and `Arena` scenes in multiplayer, which raises the cost of keeping animation and camera logic ad-hoc.

The redesign has to fit the existing Unity + Fusion architecture:
- network simulation remains host authoritative and is driven from `PlayerNetwork` and Fusion callbacks
- presentation remains in the prefab and animator/controller layer
- multi-peer play mode must not allow every runner clone to keep its own scene camera, light, and audio listener active

## Goals / Non-Goals

**Goals:**
- Define a state machine contract that separates simulation state, player intent, and presentation reactions.
- Move locomotion and attack blending to animation layers with avatar masks so upper-body combat does not freeze lower-body movement.
- Establish a local-only camera/audio policy that works in additive scenes and Fusion multi-peer mode.
- Make the migration incremental so the current player prefab and controller can be replaced in slices without a full rewrite.

**Non-Goals:**
- Rebuild the entire combat system, hit detection, or weapon pipeline.
- Introduce a full animation graph tooling framework beyond what Unity Animator layers and Fusion already provide.
- Replace Fusion KCC or change the authority model.
- Solve all future camera features such as spectator, kill-cam, or replay mode in this change.

## Decisions

### 1. Split gameplay state from animation state
The redesigned player flow will treat the gameplay state machine as the source of truth for simulation, while animation state becomes a projection of simulation state plus local visual context.

Rationale:
- The current setup lets animator/controller behavior influence gameplay recovery too directly.
- Attack completion and locomotion recovery should be governed by explicit simulation exit conditions, not by whichever clip is still playing.

Alternatives considered:
- Keep one combined state machine and tighten transition checks. Rejected because it preserves the same coupling that caused recent bugs.
- Let animator state drive gameplay through animation events. Rejected because it becomes brittle under prediction, interruption, and multiplayer latency.

### 2. Use intent-driven substates with explicit interrupt rules
The new machine will model locomotion, action, and interruption separately:
- locomotion handles idle and move
- action handles attack wind-up, active, and recovery
- interruption rules decide whether movement input can continue, be buffered, or be ignored

Rationale:
- This removes the current “one big state owns everything” pattern.
- Buffered actions and movement resumption become explicit and testable.

Alternatives considered:
- Keep only `Idle`, `Move`, and `Attack` top-level states. Rejected because it hides timing rules inside state code and scales poorly once more actions are added.

### 3. Introduce animator layers with avatar masks
Animator output will be split into at least:
- a base locomotion layer for full-body movement and idle
- an upper-body combat layer masked to torso/arms for attack playback

Rationale:
- This allows the character to preserve run/idle lower-body motion while upper-body attack animation plays.
- It removes the need for attack clips to take over the whole body when only weapon action should override the pose.

Alternatives considered:
- Continue using a single locomotion layer with direct transitions into attack. Rejected because it caused full-body lock and poor recovery behavior.
- Switch to a Playables-based graph immediately. Rejected for now because it increases migration cost without first proving the target behavior contract.

### 4. Treat scene camera/audio as local presentation, not shared gameplay state
The arena scene camera, audio listener, and other singleton scene visuals will be governed by local-visibility rules in multi-peer mode. Long term, the preferred shape is a dedicated local presentation camera rig rather than a permanent scene gameplay camera.

Rationale:
- Cameras and listeners are local presentation concerns and should never be implicitly active for every runner clone.
- This aligns with additive `UI` + `Arena` scenes, where gameplay scene loading must not decide local view ownership on its own.

Alternatives considered:
- Keep one always-on scene camera in `Arena`. Rejected because multi-peer clones duplicate it.
- Attach the entire camera rig directly under the networked player. Rejected as a first step because it couples local view composition to network prefab ownership too tightly.

### 5. Migrate in vertical slices
Implementation should proceed in slices:
1. codify the new state contracts and transition rules
2. adapt the animator controller and avatar masks
3. move camera/local listener ownership to the presentation layer
4. remove obsolete logic and transitions from the old controller/state code

Rationale:
- The current system is already live enough that a rewrite-in-place is too risky.
- Vertical slices let us verify multiplayer, animation, and movement behavior after each stage.

## Risks / Trade-offs

- [Animator/controller complexity increases] → Mitigate by limiting the first pass to a small number of explicit layers and documented parameters.
- [State duplication between simulation and presentation] → Mitigate by making gameplay state authoritative and keeping presentation as a one-way mapping.
- [Migration leaves temporary overlap between old and new logic] → Mitigate by introducing feature slices with removal checkpoints in tasks.
- [Local camera ownership remains ambiguous in edge cases] → Mitigate by specifying a single source of truth for “local presentation active” and validating it in multi-peer play mode.
- [Avatar masks may not match the current rig cleanly] → Mitigate by validating mask coverage against the existing knight model before wiring final transitions.

## Migration Plan

1. Define the new state taxonomy, buffered input rules, and transition ownership in code-facing design notes.
2. Create the animator layer plan, required parameters, and avatar mask asset list.
3. Introduce local presentation activation rules for camera/audio in additive scene boot.
4. Refactor the current player controller and presentation binding incrementally, preserving existing movement/attack behavior until each slice passes.
5. Remove obsolete transitions, parameters, and full-body attack assumptions from the legacy animator and state logic.

Rollback strategy:
- Keep the old controller/state implementation available until the new layered setup passes multiplayer smoke tests.
- Switch back to the legacy animator/controller if avatar mask integration breaks locomotion unexpectedly.

## Open Questions

- Should attack buffering be limited to one queued action, or can it support a short combo chain in the same architecture?
- Do we want a persistent local camera rig loaded from `UI`, or should `Arena` own a camera prefab that is activated only for the local player?
- Is the next combat feature set expected to include more upper-body-only actions such as aim, block, or cast, which would justify additional animator layers now?
