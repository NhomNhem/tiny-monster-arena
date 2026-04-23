## ADDED Requirements

### Requirement: Player gameplay state SHALL be simulation-authored
The system SHALL maintain player gameplay state from authoritative simulation rules rather than deriving it from animator clip playback or scene object state.

#### Scenario: Simulation decides locomotion state
- **WHEN** a player has movement input and no blocking action state
- **THEN** the simulation state machine SHALL enter locomotion movement regardless of the currently playing visual clip

#### Scenario: Simulation decides attack recovery
- **WHEN** an attack reaches its recovery completion condition
- **THEN** the simulation state machine SHALL exit the action state based on gameplay rules without waiting for the animator to choose the next clip

### Requirement: Player actions SHALL define explicit interrupt and recovery rules
The system SHALL define for each player action whether movement can continue, be suppressed, or be resumed during wind-up, active, and recovery windows.

#### Scenario: Movement resumes after recoverable attack
- **WHEN** a player is in attack recovery and the action definition allows locomotion resume
- **THEN** the state machine SHALL transition back to locomotion without requiring a separate manual reset

#### Scenario: Blocking window suppresses locomotion
- **WHEN** a player is in an action phase marked as non-interruptible
- **THEN** movement intent SHALL not directly force the state machine out of that action phase

### Requirement: Player input intent SHALL be buffered predictably
The system SHALL support explicit buffering rules for movement and action input so that transition timing is deterministic and testable.

#### Scenario: Buffered attack is consumed once
- **WHEN** the player submits an attack input during a valid buffer window
- **THEN** the next eligible action transition SHALL consume that buffered attack exactly once

#### Scenario: Continuous move intent persists across action recovery
- **WHEN** the player continues holding movement input through an action recovery window
- **THEN** locomotion intent SHALL remain available for the first valid post-action transition
