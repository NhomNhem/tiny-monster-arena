using System.IO;
using NhemBootStrap.Editor.Core;
using UnityEditor;

namespace NhemBootstrap.Editor.Steps {
    public class CreateFolderStep : IBootstrapStep {
        public string Name => "Setup Clean Architecture Folders";

        public bool CheckCompleted() {
            return AssetDatabase.IsValidFolder("Assets/_Project/Domain")
                   && AssetDatabase.IsValidFolder("Assets/_Project/Application")
                   && AssetDatabase.IsValidFolder("Assets/_Project/Infrastructure");
        }

        public void Execute(BootstrapContext context) {
            int created = 0;

            string[] folders = {
                // ── Domain ─────────────────
                "Assets/_Project",
                "Assets/_Project/Domain",
                "Assets/_Project/Domain/Entities",
                "Assets/_Project/Domain/ValueObjects",
                "Assets/_Project/Domain/Repositories",

                // ── Application ────────────
                "Assets/_Project/Application",
                "Assets/_Project/Application/UseCases",
                "Assets/_Project/Application/Services",
                "Assets/_Project/Application/Messages",

                // ── Infrastructure ─────────
                "Assets/_Project/Infrastructure",
                "Assets/_Project/Infrastructure/Network",
                "Assets/_Project/Infrastructure/Network/Fishnet",
                "Assets/_Project/Infrastructure/Physics",
                "Assets/_Project/Infrastructure/Input",
                "Assets/_Project/Infrastructure/Logging",

                // ── Presentation ───────────
                "Assets/_Project/Presentation",
                "Assets/_Project/Presentation/Player",
                "Assets/_Project/Presentation/HUD",

                // ── Composition ────────────
                "Assets/_Project/Composition",
                "Assets/_Project/Composition/Scopes",
                "Assets/_Project/Composition/Installers",

                // ── Shared ─────────────────
                "Assets/_Project/Shared",
                "Assets/_Project/Shared/Extensions",
                "Assets/_Project/Shared/Constants",
                "Assets/_Project/Shared/Logging",

                // ── Assets ─────────────────
                "Assets/_Art",
                "Assets/_Art/Characters",
                "Assets/_Art/UI",
                "Assets/_Art/VFX",

                "Assets/_Audio",
                "Assets/_Audio/SFX",
                "Assets/_Audio/BGM",

                "Assets/_Scenes",
                "Assets/_Scenes/Dev",
                "Assets/_Scenes/Gameplay"
            };

            foreach (var path in folders) {
                if (MakeFolder(path))
                    created++;
            }

            AssetDatabase.Refresh();

            context.Log($"Created {created} folders (Clean Architecture)");
        }

        private bool MakeFolder(string path) {
            if (AssetDatabase.IsValidFolder(path))
                return false;

            string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
            string name = Path.GetFileName(path);

            if (!AssetDatabase.IsValidFolder(parent))
                MakeFolder(parent);

            AssetDatabase.CreateFolder(parent, name);
            return true;
        }
    }
}