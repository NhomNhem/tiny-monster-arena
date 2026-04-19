using UnityEditor;
using UnityEditor.Compilation;
using System.IO;
using System.Linq;
using NhemBootStrap.Editor.Core;

namespace NhemBootstrap.Editor.Steps {
    public class GenerateAsmdefStep : IBootstrapStep {
        public string Name => "Setup Clean Architecture Asmdefs";

        public bool CheckCompleted() {
            return Directory.Exists("Assets/_Project/Domain") &&
                   Directory.GetFiles("Assets/_Project/Domain", "*.asmdef").Length > 0;
        }

        public void Execute(BootstrapContext context) {
            string p = context.ProjectName;
            int created = 0;

            created += Write("Assets/_Project/Domain", $"{p}.Domain");

            created += Write("Assets/_Project/Application", $"{p}.Application",
                Filter($"{p}.Domain", "MessagePipe", "UniTask"));

            created += Write("Assets/_Project/Infrastructure", $"{p}.Infrastructure",
                Filter(
                    $"{p}.Domain",
                    $"{p}.Application",
                    $"{p}.Shared",
                    "FishNet.Runtime",
                    "MessagePipe",
                    "MessagePipe.VContainer",
                    "UniTask",
                    "ZLogger",
                    "Unity.InputSystem"
                ));

            created += Write("Assets/_Project/Presentation", $"{p}.Presentation",
                Filter(
                    $"{p}.Domain",
                    $"{p}.Application",
                    $"{p}.Shared",
                    "R3",
                    "R3.Unity",
                    "MessagePipe",
                    "UniTask"
                ));

            created += Write("Assets/_Project/Composition", $"{p}.Composition",
                Filter(
                    $"{p}.Domain",
                    $"{p}.Application",
                    $"{p}.Infrastructure",
                    $"{p}.Presentation",
                    $"{p}.Shared",
                    "VContainer",
                    "VContainer.Unity",
                    "FishNet.Runtime"
                ));

            created += Write("Assets/_Project/Shared", $"{p}.Shared",
                Filter("UniTask", "ZLogger", "R3", "R3.Unity"));

            AssetDatabase.Refresh();
            context.Log($"Asmdefs created: {created}");
        }

        // =========================

        int Write(string folder, string name, string[] refs = null) {
            string path = $"{folder}/{name}.asmdef";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            if (File.Exists(path)) {
                return 0;
            }

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

            File.WriteAllText(path, json);

            return 1;
        }

        string[] Filter(params string[] refs) {
            return refs.Where(IsInstalled).ToArray();
        }

        bool IsInstalled(string asm) {
            return CompilationPipeline.GetAssemblies().Any(a => a.name == asm);
        }
    }
}