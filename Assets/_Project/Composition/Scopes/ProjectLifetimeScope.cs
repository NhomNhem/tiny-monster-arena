using NhemDangFugBixs.Attributes;
using Sirenix.OdinInspector;
using TinyMonsterArena.Infrastructure.Network.Fusion;
using TinyMonsterArena.Shared.Interfaces.Scope;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TinyMonsterArena.Composition.Scopes {
    
    [LifetimeScopeFor(typeof(IProjectScope))]
    public class ProjectLifetimeScope : LifetimeScope {
        [SerializeField, Required] private FusionLauncher _fusionLauncherPrefab;
        
        protected override void Configure(IContainerBuilder builder) {
            builder.RegisterComponentInNewPrefab(_fusionLauncherPrefab, Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();
        }
    }

}