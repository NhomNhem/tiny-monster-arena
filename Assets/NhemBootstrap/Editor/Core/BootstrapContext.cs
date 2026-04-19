using System.Collections.Generic;
using UnityEngine;

namespace NhemBootStrap.Editor.Core {
    public class BootstrapContext {
        public string ProjectPath;
        public string ProjectName;
        public readonly List<string> Logs = new();
        
        public void Log(string message) {
            Debug.Log(message);
            Logs.Add(message);
        }
    }
}