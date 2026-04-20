using UnityEditor;
using UnityEditor.Compilation;
using System.IO;
using System.Linq;
using NhemBootStrap.Editor.Core;

namespace NhemBootstrap.Editor.Steps {
    public class GenerateAsmdefStep : IBootstrapStep {
        public string Name => "Setup Clean Architecture Asmdefs";

        public bool CheckCompleted() {
            // If force update is enabled via some state, we might want to return false here
            // But IBootstrapStep doesn't have access to context in CheckCompleted
            // We'll rely on the ViewModel.Enabled state in BootstrapWindow
            return Directory.Exists("Assets/_Project/Domain") &&
                   Directory.GetFiles("Assets/_Project/Domain", "*.asmdef").Length > 0;
        }

        public void Execute(BootstrapContext context) {
            string p = context.ProjectName;
            bool force = context.ForceUpdateAsmdef;
            int created = 0;

            // Prepare list of internal asmdefs to check
            string[] internalAsms = {
                $"{p}.Domain",
                $"{p}.Application",
                $"{p}.Infrastructure",
                $"{p}.Presentation",
                $"{p}.Composition",
                $"{p}.Shared"
            };

            created += Write("Assets/_Project/Domain", $"{p}.Domain", null, force);

            created += Write("Assets/_Project/Application", $"{p}.Application",
                Filter(internalAsms, $"{p}.Domain", "MessagePipe", "UniTask"), force);

            created += Write("Assets/_Project/Infrastructure", $"{p}.Infrastructure",
                Filter(internalAsms, 
                    $"{p}.Domain",
                    $"{p}.Application",
                    $"{p}.Shared",
                    "FishNet.Runtime",
                    "MessagePipe",
                    "MessagePipe.VContainer",
                    "UniTask",
                    "ZLogger",
                    "Unity.InputSystem"
                ), force);

            created += Write("Assets/_Project/Presentation", $"{p}.Presentation",
                Filter(internalAsms, 
                    $"{p}.Domain",
                    $"{p}.Application",
                    $"{p}.Shared",
                    "R3",
                    "R3.Unity",
                    "MessagePipe",
                    "UniTask"
                ), force);

            created += Write("Assets/_Project/Composition", $"{p}.Composition",
                Filter(internalAsms, 
                    $"{p}.Domain",
                    $"{p}.Application",
                    $"{p}.Infrastructure",
                    $"{p}.Presentation",
                    $"{p}.Shared",
                    "VContainer",
                    "VContainer.Unity",
                    "FishNet.Runtime"
                ), force);

            created += Write("Assets/_Project/Shared", $"{p}.Shared",
                Filter(internalAsms, "UniTask", "ZLogger", "R3", "R3.Unity"), force);

            AssetDatabase.Refresh();
            context.Log($"Asmdefs processed: {created} (Force: {force})");
        }

        // =========================

        int Write(string folder, string name, string[] refs = null, bool force = false) {
            string path = $"{folder}/{name}.asmdef";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            refs ??= new string[] { };

            string refsJson = refs.Length == 0
                ? "[]"
                : "[\n" + string.Join(",\n", refs.Select(r => $"    \"{r}\"")) + "\n  ]";

            string json =
                $@"{{
  ""name"": ""{name}"",
  ""rootNamespace"": ""{name}"",
  ""references"": {refsJson},
  ""autoReferenced"": false
}}";

            if (File.Exists(path)) {
                if (!force) return 0;
                
                // If force, check if content is different
                string existing = File.ReadAllText(path);
                if (existing.Replace("\r\n", "\n") == json.Replace("\r\n", "\n")) return 0;
            }

            File.WriteAllText(path, json);

            return 1;
        }

        string[] Filter(string[] internalAsms, params string[] refs) {
            return refs.Where(r => IsInstalled(r, internalAsms)).ToArray();
        }

        bool IsInstalled(string asm, string[] internalAsms) {
            // 1. If it's one of our internal asmdefs, assume it will exist
            if (internalAsms.Contains(asm)) return true;

            // 2. Check if it's already compiled by Unity
            if (CompilationPipeline.GetAssemblies().Any(a => a.name == asm)) return true;

            // 3. Fallback: search for any .asmdef file with this name in the project
            // This helps if Unity hasn't compiled it yet but the file exists
            string[] guids = AssetDatabase.FindAssets(asm + " t:asmdef");
            foreach (var guid in guids) {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == asm) return true;
            }

            return false;
        }
    }
}