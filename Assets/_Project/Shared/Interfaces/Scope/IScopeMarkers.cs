namespace TinyMonsterArena.Shared.Interfaces.Scope {
    /// <summary>
    /// Marker interface for project-level singleton services and configurations.
    /// Lifetime: Application lifetime.
    /// </summary>
    public interface IProjectScope { }

    /// <summary>
    /// Marker interface for per-player services and state.
    /// Lifetime: Created when player joins, destroyed when player leaves.
    /// </summary>
    public interface IPlayerScope { }

    /// <summary>
    /// Marker interface for main menu scene-specific services.
    /// Lifetime: Main menu scene lifetime.
    /// </summary>
    public interface IMainMenuScope { }

    /// <summary>
    /// Marker interface for gameplay scene-specific services.
    /// Lifetime: Gameplay scene lifetime.
    /// </summary>
    public interface IGameplayScope { }

    /// <summary>
    /// Marker interface for match-specific services and state.
    /// Lifetime: Created when match starts, destroyed when match ends.
    /// </summary>
    public interface IMatchLifetimeScope { }
}