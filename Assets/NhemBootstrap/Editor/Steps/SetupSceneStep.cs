using NhemBootStrap.Editor.Core;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NhemBootstrap.Editor.Steps {
    public class SetupSceneStep : IBootstrapStep {
        public string Name => "Setup Scene";

        public bool CheckCompleted() {
            return true;
        }

        public void Execute(BootstrapContext context) {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);

            CreateRoot("[SYSTEMS]");
            CreateRoot("[UI]");
            CreateRoot("[MAP]");
            CreateRoot("[GAMEPLAY]");

            EditorSceneManager.SaveScene(scene, "Assets/_Project/Scenes/Main.unity");

            context.Log("Scene created with base structure");
        }

        private void CreateRoot(string name) {
            var go = new GameObject(name);
            go.transform.position = Vector3.zero;
        }
    }
}