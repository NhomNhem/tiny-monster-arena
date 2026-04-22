using TinyMonsterArena.Composition.Installers.Interfaces;
using UnityEngine;
using VContainer;

namespace TinyMonsterArena.Composition.Installers {
    public class AudioInstaller : IAudioInstaller{

        public void Install(object rawBuilder) {
            var builder = (IContainerBuilder) rawBuilder;
        }
    }
}