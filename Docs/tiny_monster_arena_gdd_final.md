# TINY MONSTER ARENA – FINAL GDD (CV SHOWCASE)

---

# 1. PROJECT OVERVIEW

**Genre:** 2D Top-down Multiplayer Arena Brawler  
**Platform:** PC (Unity Standalone)  
**Session Size:** 2–4 Players  

**Core Goal:**
- Showcase Multiplayer Architecture (Photon Fusion 2)
- Demonstrate Clean Code + DI (VContainer 2.0)
- Reactive UI/Event Flow (R3)
- Strong Gamefeel (DOTween + Juice)

---

# 2. CORE GAME LOOP

1. Player nhập tên → Join Lobby
2. Matchmaking (2–4 players)
3. Spawn vào Arena
4. Combat diễn ra (Last Man Standing)
5. Slow-motion Finish + Winner Highlight
6. Result Screen → Rematch hoặc Exit

## 2.1 Loop Enhancements (Retention)

- **Shrinking Arena:** Vòng bo thu nhỏ theo thời gian
- **Random Buff Spawn:** Speed / Damage / Shield
- **Environmental Hazards:** Spike / Lava tiles xuất hiện ngẫu nhiên

---

# 3. COMBAT DESIGN (GAMEFEEL FOCUS)

## 3.1 Combat Phases

### Anticipation
- Squash/Stretch animation
- Wind-up delay (0.1–0.2s)

### Active
- Fast hit execution
- Input Buffering (queue input trong 100ms)

### Recovery
- Hit-stop (freeze frame 0.05–0.1s)
- Vulnerable window

---

## 3.2 Feedback Systems

- Hit Flash (Shader)
- Screen Shake (damage-based)
- Particle Burst (impact direction)
- Damage Number Popup (R3 driven)

---

## 3.3 Combat Systems

### Stagger System
- Hit liên tục → build stagger meter
- Full → Groggy (stun ngắn 0.5s)

### Dynamic Knockback
- Force = BaseForce × DistanceModifier × SkillMultiplier

### Lag Compensation
- Server validates hit based on past tick state

---

# 4. CHARACTER DESIGN (MECHANIC-DRIVEN)

## Skeleton (Heavy)
- High damage, medium HP, slow
- **Whirlwind:** Charge → spin AoE knockback
- Mechanic: Charge-based attack (hold input)

## Goblin (Rogue)
- Fast, low HP
- **Dash Stab:** Dash với invulnerability frames
- Mechanic: I-frame timing skill-based

## Slime (Tank)
- High HP, slow
- **Sticky Aura:** Slow enemies in radius
- Passive: Split into 2 mini slimes once on death

---

# 5. NETWORK ARCHITECTURE

## 5.1 Model

- **Mode:** Host Authority (Fusion Host Mode)
- Host = State Authority

## 5.2 Responsibilities

### Client
- Input gửi lên (Input Authority)
- Local Prediction (movement)

### Host
- Validate hit
- Apply damage
- Sync state via [Networked]

---

## 5.3 Key Techniques

- Client-side prediction
- Server reconciliation
- Lag compensation (rewind tick)

---

## 5.4 Edge Cases

- Player disconnect → chuyển sang spectator hoặc end match
- Late join → spectator only
- Packet delay → prediction + correction

---

# 6. SCENE ARCHITECTURE

## 6.1 Scene Layers

### [SYSTEMS]
- NetworkRunner
- SceneLifetimeScope
- InputProvider

### [UI]
- HUD
- Killfeed
- Damage Popup (R3 binding)

### [MAP]
- Tilemap
- Colliders
- Spawn Points

### [GAMEPLAY]
- Player (NetworkObject)
- Projectiles

---

# 7. TECH STACK

- **Engine:** Unity
- **Networking:** Photon Fusion 2
- **DI:** VContainer 2.0
- **Reactive:** R3
- **Async:** UniTask
- **Tweening:** DOTween

---

# 8. IMPLEMENTATION PLAN

## Phase 1 – Foundation
- Setup VContainer RootScope
- Setup NetworkRunner
- Scene structure

## Phase 2 – Movement
- NetworkCharacterController
- Client Prediction

## Phase 3 – Combat
- Hitbox/Hurtbox
- Damage System (pure C#)
- R3 Event Flow (OnHit)

## Phase 4 – Juice & Polish
- DOTween feedback
- Screenshake
- UI animation

## Phase 5 – Optimization
- Object Pooling (VContainer)
- Reduce network variables

---

# 9. SOFTWARE ARCHITECTURE PRINCIPLES

- No Singleton
- Dependency Injection only
- Pure logic separated from MonoBehaviour
- Interface-based communication

---

# 10. CV BOOST FEATURES

## MUST
- Gameplay GIF
- Architecture Diagram
- Clean Code Snippets

## ADVANCED (Optional)
- Basic AI Bot (fill match)
- Replay System (tick-based)

---

# 11. README STRUCTURE (FOR RECRUITER)

1. Project Overview
2. Gameplay GIF
3. Tech Stack
4. Architecture Diagram
5. Key Challenges & Solutions
6. How to Run

---

# 12. FINAL NOTE

This project is designed as a **Technical Showcase**, not a full commercial game.

Focus:
- Code quality
- Architecture clarity
- Multiplayer correctness
- Gamefeel impact

---

**END OF DOCUMENT**

