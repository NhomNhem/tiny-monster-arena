## Knight Controller Audit

### Current controller shape
- Controller: `Assets/_Project/_Animation/Knight.controller`
- Parameters:
  - `MoveSpeed` float
  - `Attack` trigger
- Layers:
  - `Base Layer` only
- States:
  - `Locomotion` blend tree driven by `MoveSpeed`
  - `Attack` clip state
- Transitions:
  - `Any State -> Attack` on `Attack`
  - `Attack -> Locomotion` on exit time

### Conflicts with the target architecture
- Attack is still authored as a full-body state on the only animator layer.
- `Any State -> Attack` means every pose can be interrupted into the same full-body clip without distinguishing wind-up, active, or recovery intent.
- Both `Locomotion` and `Attack` have `m_WriteDefaultValues: 1`, which increases the chance of pose stomping when layered animation is introduced.
- There is no dedicated upper-body layer or mask, so movement and combat cannot blend by design.
- Presentation still uses one trigger (`Attack`) instead of phase-aware parameters that can map cleanly from gameplay state.

## Rig Inventory For Avatar Mask

### Source prefab
- `Assets/KayKit/Characters/KayKit - Adventurers (for Unity)/Prefabs/Characters/Knight.prefab`

### Relevant rig paths
- `Rig_Medium/root`
- `Rig_Medium/root/hips`
- `Rig_Medium/root/hips/spine`
- `Rig_Medium/root/hips/spine/chest`
- `Rig_Medium/root/hips/spine/chest/head`
- `Rig_Medium/root/hips/spine/chest/upperarm.l`
- `Rig_Medium/root/hips/spine/chest/upperarm.l/lowerarm.l`
- `Rig_Medium/root/hips/spine/chest/upperarm.l/lowerarm.l/wrist.l`
- `Rig_Medium/root/hips/spine/chest/upperarm.l/lowerarm.l/wrist.l/hand.l`
- `Rig_Medium/root/hips/spine/chest/upperarm.r`
- `Rig_Medium/root/hips/spine/chest/upperarm.r/lowerarm.r`
- `Rig_Medium/root/hips/spine/chest/upperarm.r/lowerarm.r/wrist.r`
- `Rig_Medium/root/hips/spine/chest/upperarm.r/lowerarm.r/wrist.r/hand.r`
- `Rig_Medium/root/hips/upperleg.l`
- `Rig_Medium/root/hips/upperleg.l/lowerleg.l/foot.l/toes.l`
- `Rig_Medium/root/hips/upperleg.r`
- `Rig_Medium/root/hips/upperleg.r/lowerleg.r/foot.r/toes.r`

### Upper-body combat mask plan
- Include:
  - `spine`
  - `chest`
  - `head`
  - both arm chains through hands/handslots
- Exclude:
  - `hips`
  - both leg chains
  - root motion chain below the spine

### Resulting asset
- `Assets/_Project/_Animation/Masks/KnightUpperBody.mask`
