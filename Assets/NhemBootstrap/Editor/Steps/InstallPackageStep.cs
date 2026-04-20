using System.Collections.Generic;
using NhemBootStrap.Editor.Core;
using UnityEditor.PackageManager;
using System.Diagnostics;
using System.Linq;

namespace NhemBootstrap.Editor.Steps {
    public class InstallPackageStep : IBootstrapStep {
        public string Name => "Install Packages";
        public List<PackageInfoViewModel> Packages { get; } = new();

        public InstallPackageStep(List<string> packageNames) {
            foreach (var name in packageNames) {
                Packages.Add(new PackageInfoViewModel { Name = name, Selected = true });
            }
        }

        public bool CheckCompleted() {
            var installed = GetInstalledPackages();
            if (installed == null) return false;

            bool allSelectedCompleted = true;
            foreach (var p in Packages) {
                p.IsInstalled = installed.Contains(p.Name);
                if (p.Selected && !p.IsInstalled) {
                    allSelectedCompleted = false;
                }
            }

            return allSelectedCompleted;
        }

        public void Execute(BootstrapContext context) {
            var installed = GetInstalledPackages();
            
            foreach (var p in Packages) {
                if (p.Selected && (installed == null || !installed.Contains(p.Name))) {
                    Client.Add(p.Name);
                    context.Log($"Installing package: {p.Name}");
                }
            }
        }

        private HashSet<string> GetInstalledPackages() {
            var listRequest = Client.List(true);
            float timeout = 5f;
            var stopwatch = Stopwatch.StartNew();
            while (!listRequest.IsCompleted) {
                if (stopwatch.Elapsed.TotalSeconds > timeout) return null;
                System.Threading.Thread.Sleep(10);
            }

            if (listRequest.Status != StatusCode.Success) return null;

            return new HashSet<string>(listRequest.Result.Select(p => p.name));
        }
    }

    public class PackageInfoViewModel {
        public string Name;
        public bool Selected;
        public bool IsInstalled;
    }
}