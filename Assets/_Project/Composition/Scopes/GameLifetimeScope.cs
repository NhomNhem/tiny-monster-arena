using NhemDangFugBixs.Attributes;
using TinyMonsterArena.Shared.Interfaces.Scope;
using VContainer.Unity;

namespace TinyMonsterArena.Composition.Scopes {

    [LifetimeScopeFor(typeof(IGameplayScope))]
    public class GameLifetimeScope : LifetimeScope {
    }
}