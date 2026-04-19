using System.Collections.Generic;
using UnityEngine;

namespace NhemBootstrap.Editor.Config {
    [CreateAssetMenu(menuName = "Nhem/Bootstrap Config")]
    public class BootstrapConfig : ScriptableObject
    {
        [Header("Packages")]
        public List<string> packages;

        [Header("Folders")]
        public List<string> folders;

        [Header("Asmdef Names")]
        public List<string> asmdefs;
    }
}