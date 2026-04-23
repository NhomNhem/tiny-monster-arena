using TinyMonsterArena.Presentation.UI;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace TinyMonsterArena.Composition.Scopes {
    public class BootstrapLifetimeScope : LifetimeScope {
        [SerializeField] private UIDocument _uiDocument;
        
        protected override void Configure(IContainerBuilder builder) {
            builder.RegisterComponent(_uiDocument);

            builder.RegisterEntryPoint<MainMenuPresenter>(Lifetime.Singleton);
        }
    }
}
