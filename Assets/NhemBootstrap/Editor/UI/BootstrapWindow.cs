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

        [MenuItem("Tools/Nhem Bootstrap")]
        public static void Open() {
            GetWindow<BootstrapWindow>();
        }

        public void CreateGUI() {
            var root = rootVisualElement;

            var nameField = new TextField("Project Name");
            nameField.value = _projectName;
            nameField.RegisterValueChangedCallback(e => _projectName = e.newValue);

            _list = new ScrollView();
            _progress = new ProgressBar();
            _log = new ScrollView();

            var runBtn = new Button(Run) { text = "Run Bootstrap" };

            root.Add(nameField);
            root.Add(_list);
            root.Add(_progress);
            root.Add(runBtn);
            root.Add(_log);

            Init();
            Draw();
        }

        void Init() {
            _steps = new List<StepViewModel> {
                Create(new CreateFolderStep()),
                Create(new GenerateAsmdefStep())
            };
        }

        StepViewModel Create(IBootstrapStep step) {
            return new StepViewModel {
                Step = step,
                Completed = step.CheckCompleted()
            };
        }

        void Draw() {
            _list.Clear();

            foreach (var vm in _steps) {
                var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };

                var toggle = new Toggle { value = !vm.Completed };
                toggle.SetEnabled(!vm.Completed);

                toggle.RegisterValueChangedCallback(e => vm.Enabled = e.newValue);

                var name = new Label(vm.Step.Name) { style = { flexGrow = 1 } };
                var status = new Label(vm.Completed ? "✅" : "⏳");

                row.Add(toggle);
                row.Add(name);
                row.Add(status);

                _list.Add(row);
            }
        }

        void Run() {
            var ctx = new BootstrapContext { ProjectName = _projectName };

            int total = _steps.Count;
            int done = 0;

            foreach (var vm in _steps) {
                if (!vm.Enabled) continue;

                if (vm.Step.CheckCompleted()) {
                    Log($"Skip: {vm.Step.Name}");
                    continue;
                }

                vm.Step.Execute(ctx);

                done++;
                _progress.value = (float)done / total * 100f;
            }

            Log("DONE");
            Draw();
        }

        void Log(string msg) {
            _log.Add(new Label(msg));
            Debug.Log(msg);
        }
    }
}