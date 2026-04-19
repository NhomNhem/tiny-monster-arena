using System.Collections.Generic;

namespace NhemBootStrap.Editor.Core {
    public class BootstrapRunner {
        private readonly List<IBootstrapStep> _steps;

        public BootstrapRunner(List<IBootstrapStep> steps) {
            _steps = steps;
        }

        public void Run(BootstrapContext context) {
            foreach (var step in _steps) {
                context.Log($"▶ Running: {step.Name}");
                step.Execute(context);
            }

            context.Log("✅ Bootstrap Completed");
        }
    }
}