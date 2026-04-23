# AGENTS.md

## Tiny Monster Arena – AI Agent Guide

This document summarizes essential architecture, workflows, and conventions for AI coding agents working in this codebase. All guidance is derived from actual project structure and code, not generic advice.

---

### 1. Big Picture Architecture
- **Clean Architecture:**
  - Layers: Application, Domain, Infrastructure, Presentation, Shared, Composition (see `_Project/`)
  - **Dependency Injection:** All services are registered via VContainer scopes. No singletons.
  - **Networking:** Photon Fusion 2, Host Authority. Client prediction, server reconciliation, lag compensation.
  - **UI/Feedback:** R3 for reactive event flow, DOTween for animation/juice, ChocDino.UIFX for UI effects.
- **Scene Layers:** SYSTEMS (NetworkRunner, DI scopes), UI (HUD, Killfeed), MAP (Tilemap), GAMEPLAY (Player, Projectiles)

### 2. Developer Workflows
- **Build/Run:** Standard Unity Editor workflow. No custom build scripts.
- **Dependencies:**
  - .NET libs via NuGet (`Assets/NuGet.config`, `Assets/packages.config`)
  - Unity plugins in `Assets/Plugins/`, `Assets/Packages/`
- **DOTween:** After import/upgrade, always run "Tools > Demigiant > DOTween Utility Panel" and press "Setup DOTween..."
- **DI Scopes:** Extend via `LifetimeScope` classes in `_Project/Composition/Scopes/`. Register services in `Configure(IContainerBuilder builder)`.

### 3. Project-Specific Conventions
- **No MonoBehaviour Singletons:** Use DI and marker interfaces for all cross-scene state/services (`IScopeMarkers.cs`).
- **Interface-Driven:** All services (e.g., `IAudioService`) are interface-based and implemented in Infrastructure (e.g., `BroAudioService`).
- **Audio:** Use `AudioKey` and `AudioType` enums for all audio logic. Never reference asset paths or IDs directly.
- **Scene Lifetime:** Use marker interfaces (`IProjectScope`, `IGameplayScope`, etc.) to control service lifetimes.
- **Pure Logic:** Domain logic must be in pure C# classes, not MonoBehaviours.

#### Audio & DI (BroAudio, NhemdangFugBigTooling)
- Audio sử dụng BroAudio framework, mọi truy cập audio đều qua abstraction `IAudioService` (không dùng singleton).
- Dự án dùng NhemdangFugBigTooling với Source Generator (SG) hỗ trợ wiring DI cho VContainer: chỉ cần tạo interface + implementation đúng chuẩn, SG sẽ tự động đăng ký vào scope tương ứng.
- Khi thêm service mới (audio effect, pooling...), chỉ cần tạo interface + implementation, SG sẽ tự động wiring nếu tuân thủ convention.
- Khi gọi audio, luôn dùng `IAudioService.PlaySFX`, `PlayBGM`, ... và truyền enum `AudioKey`, `AudioType` thay vì asset path.

### 4. Integration Points & Cross-Component Patterns
- **Networking:** All networked objects/components must use Photon Fusion’s `[Networked]` attribute and follow Host/Client authority split.
- **UI/FX:** Use ChocDino.UIFX and R3 for all UI feedback and event-driven flows.
- **Audio:** All audio routed through `IAudioService` abstraction, implemented by BroAudio.
- **External Libraries:** DOTween (animation/juice), Odin Inspector (editor tooling), R3 (reactive), UniTask (async), VContainer (DI).

### 5. Key Files & Directories
- `_Project/Composition/Scopes/` – DI scope definitions (e.g., `ProjectLifetimeScope.cs`)
- `_Project/Composition/Installers/` – Service installers for DI
- `_Project/Application/Services/Interfaces/` – Service contracts (e.g., `IAudioService.cs`)
- `_Project/Infrastructure/Audio/` – Service implementations (e.g., `BroAudioService.cs`)
- `_Project/Shared/Audio/AudioType.cs` – Audio enums
- `Docs/tiny_monster_arena_gdd_final.md` – High-level design and architecture rationale

### 6. Build, Test, and Lint Commands
- **Build:** Standard Unity Editor build process. Use File > Build Settings.
- **Test:** No custom test scripts found. Use Unity Test Runner (Window > General > Test Runner) for EditMode and PlayMode tests.
- **Lint:** No automated linting tools configured. Follow code style guidelines below.
- **Run Single Test:** In Test Runner, select specific test and click "Run Selected".

### 7. Code Style Guidelines
#### Imports & Formatting
- **File-scoped namespaces** (C# 10+) used exclusively
- **Import order:** Project namespaces → External libraries → System namespaces → Unity namespaces
- **No #region directives** used in codebase
- **K&R brace style**: Opening braces on same line as declaration
- **4-space indentation** (Visual Studio default)

#### Naming Conventions
- **Classes:** PascalCase (`PlayerEntity`, `FusionLauncher`)
- **Interfaces:** PascalCase with "I" prefix (`IAudioService`, `INetworkService`)
- **Fields:** camelCase with `_` prefix (`_runner`, `_status`)
- **Properties:** PascalCase (`Health`, `IsDead`)
- **Methods:** PascalCase (`PlaySFX()`, `TakeDamage()`)
- **Enums:** Singular names (`AudioType`), PascalCase values with underscores (`AudioKey.UI_Click`)
- **Variables:** camelCase (`playerCount`, `isConnected`)

#### Types & Structure
- **Value objects/small data:** Use `struct` with `readonly` modifiers
- **Entities/larger objects:** Use `class`
- **Interfaces** placed in `Application/Services/Interfaces/`
- **Implementations** placed in `Infrastructure/` layer
- **Null handling:** Use nullable types (`Vector3?`) and null-conditional checks
- **Error handling:** Try-catch blocks (no Result type pattern observed)

#### Async & Reactive Patterns
- **UniTask** available for asynchronous operations
- **async void** used for Unity event handlers (fire-and-forget)
- **R3 Observable** for reactive programming patterns
- **Observable.Timer** for delayed actions with disposables

#### DI & Service Patterns
- **VContainer** for dependency injection
- **LifetimeScope** classes in `_Project/Composition/Scopes/`
- **Constructor injection** for service dependencies
- **Marker interfaces** for scope control (`IProjectScope`, `IGameplayScope`, etc.)

#### Comments & Documentation
- **XML Documentation Comments** (`///`) on public interface members
- **Inline comments** (`//`) for implementation notes
- **No trailing whitespace**; keep lines under 120 characters when possible

### 8. Architecture Notes
All services follow dependency injection through VContainer. No service locators or singletons are used. Networking follows host-authority model with client prediction. UI uses reactive programming with R3 for event flows and DOTween for animations.

See [Docs/tiny_monster_arena_gdd_final.md] for complete design rationale.