## ADDED Requirements

### Requirement: Player animation SHALL separate locomotion and combat layering
The system SHALL support a base locomotion animation layer and a masked action layer so upper-body combat playback does not replace lower-body locomotion unnecessarily.

#### Scenario: Lower body keeps locomotion during upper-body action
- **WHEN** the player is moving while a masked upper-body attack animation is active
- **THEN** the lower body SHALL continue to evaluate locomotion motion while the upper body plays the combat action

#### Scenario: Idle pose returns when no action layer is active
- **WHEN** the player has no locomotion input and no active masked combat action
- **THEN** the animator SHALL resolve to the idle locomotion pose without requiring an attack-specific reset state

### Requirement: Avatar mask assets SHALL be defined as part of the player presentation contract
The system SHALL define which bones are controlled by each action layer and SHALL use avatar mask assets that match the production player rig.

#### Scenario: Combat layer only overrides masked bones
- **WHEN** a combat action layer is active
- **THEN** only the bones included by its avatar mask SHALL be overridden by that layer

#### Scenario: Mask validation catches incompatible rig coverage
- **WHEN** a new avatar mask is assigned to the player action layer
- **THEN** the integration workflow SHALL verify that required upper-body bones for the current player rig are present

### Requirement: Multiplayer scene cameras and listeners SHALL be local-only presentation resources
The system SHALL ensure only the owning local presentation stack keeps camera, audio listener, and equivalent singleton visual resources active in multiplayer and multi-peer play mode.

#### Scenario: Multi-peer mode suppresses duplicate scene camera output
- **WHEN** multiple runner clones load the arena scene in multi-peer mode
- **THEN** only the local presentation owner SHALL keep its camera and audio listener enabled

#### Scenario: UI and arena additive boot preserves local view ownership
- **WHEN** the game starts from the UI bootstrap and loads the arena scene additively for multiplayer
- **THEN** local camera ownership SHALL remain deterministic and SHALL not depend on which scene happened to be active first
