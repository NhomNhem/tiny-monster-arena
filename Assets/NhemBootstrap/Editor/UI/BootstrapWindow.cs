using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using NhemBootStrap.Editor.Core;
using NhemBootstrap.Editor.Steps;

namespace NhemBootstrap.Editor {
    public class BootstrapWindow : EditorWindow {
        private List<StepViewModel> _steps = new();
        private ScrollView _list;
        private ScrollView _log;
        private ProgressBar _progress;

        private string _projectName = "TinyMonsterArena";
        private bool _forceUpdateAsmdef = false;

        [MenuItem("Tools/Nhem Bootstrap")]
        public static void Open() {
            GetWindow<BootstrapWindow>();
        }

        public void CreateGUI() {
            var root = rootVisualElement;
            
            var nameField = new TextField("Project Name");
            nameField.value = _projectName;
            nameField.RegisterValueChangedCallback(e => _projectName = e.newValue);

            var forceToggle = new Toggle("Force Update Asmdefs");
            forceToggle.value = _forceUpdateAsmdef;
            forceToggle.RegisterValueChangedCallback(e => {
                _forceUpdateAsmdef = e.newValue;
                OnRefreshClick(); // Trigger refresh to update "Setup Clean Architecture Asmdefs" status
            });
            
            _list = new ScrollView();
            _progress = new ProgressBar();
            _log = new ScrollView();
            
            var refreshBtn = new Button(OnRefreshClick) { text = "Refresh Status" };
            var runBtn = new Button(Run) { text = "Apply Changes / Fix" };
            
            root.Add(nameField);
            root.Add(forceToggle);
            root.Add(_list);
            root.Add(_progress);
            root.Add(refreshBtn);
            root.Add(runBtn);
            root.Add(_log);
            
            Init();
            Draw();
            
            // Log that GUI was created
            Log("BootstrapWindow GUI created");
        }

        void Init() {
            var packages = new List<string> {
                "com.fishnet.fishnet",
                "com.enemic.messagepipe",
                "cysharp.unitask",
                "com.vcontainer.vcontainer",
                "com.vcontainer.vcontainer.unity",
                "com.github.xinaoranged.zlogger",
                "com.nmediacorp.r3",
                "com.nmediacorp.r3.unity"
            };
            
            _steps = new List<StepViewModel> {
                Create(new CreateFolderStep()),
                Create(new GenerateAsmdefStep()),
                Create(new InstallPackageStep(packages))
            };
            
            // Auto-select uninstalled packages on init
            foreach (var vm in _steps) {
                if (vm.Step is InstallPackageStep packageStep) {
                    foreach (var pkg in packageStep.Packages) {
                        if (!pkg.IsInstalled) pkg.Selected = true;
                    }
                }
            }
        }

        StepViewModel Create(IBootstrapStep step) {
            var completed = step.CheckCompleted();
            Log($"Creating StepViewModel for {step.Name}: Completed={completed}, Enabled={!completed}");
            return new StepViewModel {
                Step = step,
                Completed = completed,
                Enabled = !completed
            };
        }

        void OnRefreshClick() {
            foreach (var vm in _steps) {
                vm.Completed = vm.Step.CheckCompleted();
                // Special for GenerateAsmdefStep: if force is on, it's never "completed" (needs fix)
                if (vm.Step is GenerateAsmdefStep && _forceUpdateAsmdef) {
                    vm.Completed = false;
                }
                
                vm.Enabled = !vm.Completed;
                
                // Auto-select uninstalled packages on refresh
                if (vm.Step is InstallPackageStep packageStep) {
                    foreach (var pkg in packageStep.Packages) {
                        if (!pkg.IsInstalled) pkg.Selected = true;
                    }
                }
            }
            Draw();
            Log("Status refreshed");
        }

        void Draw() {
            _list.Clear();

            foreach (var vm in _steps) {
                var row = new VisualElement { style = { flexDirection = FlexDirection.Column, marginBottom = 5 } };
                var header = new VisualElement { style = { flexDirection = FlexDirection.Row, alignItems = Align.Center } };

                var toggle = new Toggle { value = !vm.Completed };
                toggle.SetEnabled(!vm.Completed);
                toggle.RegisterValueChangedCallback(e => vm.Enabled = e.newValue);
                vm.Enabled = toggle.value;

                var name = new Label(vm.Step.Name) { style = { flexGrow = 1 } };
                name.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
                var status = new Label(vm.Completed ? "✅" : "⏳");

                header.Add(toggle);
                header.Add(name);
                header.Add(status);
                row.Add(header);

                // Special handling for InstallPackageStep to show sub-packages
                if (vm.Step is InstallPackageStep packageStep) {
                    var pkgList = new VisualElement { style = { marginLeft = 20, marginTop = 2 } };
                    foreach (var pkg in packageStep.Packages) {
                        var pkgRow = new VisualElement { style = { flexDirection = FlexDirection.Row, alignItems = Align.Center } };
                        
                        var pkgToggle = new Toggle { value = pkg.Selected };
                        pkgToggle.SetEnabled(!pkg.IsInstalled);
                        pkgToggle.RegisterValueChangedCallback(e => {
                            pkg.Selected = e.newValue;
                            // Update parent step status if needed
                            vm.Completed = vm.Step.CheckCompleted();
                            status.text = vm.Completed ? "✅" : "⏳";
                        });
                        
                        var pkgName = new Label(pkg.Name) { style = { flexGrow = 1, fontSize = 11 } };
                        var pkgStatus = new Label(pkg.IsInstalled ? "✓" : "");
                        
                        pkgRow.Add(pkgToggle);
                        pkgRow.Add(pkgName);
                        pkgRow.Add(pkgStatus);
                        pkgList.Add(pkgRow);
                    }
                    row.Add(pkgList);
                }

                _list.Add(row);
            }
        }

        void Run() {
            var ctx = new BootstrapContext { 
                ProjectName = _projectName,
                ForceUpdateAsmdef = _forceUpdateAsmdef
            };

            int total = _steps.Count;
            int done = 0;

            foreach (var vm in _steps) {
                if (!vm.Enabled) continue;

                Log($"Checking step: {vm.Step.Name} - Completed: {vm.Step.CheckCompleted()}");

                if (vm.Step.CheckCompleted()) {
                    Log($"Skip: {vm.Step.Name}");
                    continue;
                }

                try {
                    Log($"Executing step: {vm.Step.Name}");
                    vm.Step.Execute(ctx);

                    done++;
                    _progress.value = (float)done / total * 100f;
                    Log($"Completed step: {vm.Step.Name}");
                }
                catch (System.Exception ex) {
                    Log($"❌ Error in {vm.Step.Name}: {ex.Message}");
                    Log($"Stack trace: {ex.StackTrace}");
                    // Continue with other steps even if one fails
                }
            }

            Log($"Finished all steps. Done: {done}/{total}");
            Log("DONE");
            Draw();
        }

        void Log(string msg) {
            _log.Add(new Label(msg));
            Debug.Log(msg);
        }
    }
}