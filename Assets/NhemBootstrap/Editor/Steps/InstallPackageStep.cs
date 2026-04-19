using System.Collections.Generic;
using NhemBootStrap.Editor.Core;
using UnityEditor.PackageManager;

namespace NhemBootstrap.Editor.Steps {
    public class InstallPackageStep : IBootstrapStep {
        public string Name => "Install Package";
        private readonly List<string> _packages;
        
        public InstallPackageStep(List<string> packages) {
            _packages = packages;
        }

        public bool CheckCompleted() {
            var listRequest = Client.List(true);
            while (!listRequest.IsCompleted) { }
            
            if (listRequest.Status != StatusCode.Success) {
                return false;
            }

            var installedPackages = new HashSet<string>();
            foreach (var package in listRequest.Result) {
                installedPackages.Add(package.name);
            }

            foreach (var package in _packages) {
                if (!installedPackages.Contains(package)) {
                    return false;
                }
            }

            return true;
        }

        public void Execute(BootstrapContext context) {
            foreach (var package in _packages) {
                Client.Add(package);
                context.Log($"Installing package: {package}");
            }    
        }
    }
}