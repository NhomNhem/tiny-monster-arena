using System.Collections.Generic;

namespace NhemBootStrap.Editor.Core {
    public class BootstrapRunner {
        private readonly List<IBootstrapStep> _steps;

        public BootstrapRunner(List<IBootstrapStep> steps) {
            _steps = steps;
        }

        public void Run(BootstrapContext context) {
            bool allSucceeded = true;
            
            foreach (var step in _steps) {
                try {
                    context.Log($"▶ Running: {step.Name}");
                    step.Execute(context);
                }
                catch (System.Exception ex) {
                    context.Log($"❌ Failed: {step.Name} - {ex.Message}");
                    allSucceeded = false;
                    // Continue with other steps even if one fails
                }
            }

            if (allSucceeded) {
                context.Log("✅ Bootstrap Completed");
            }
            else {
                context.Log("⚠️ Bootstrap Completed with Errors");
            }
        }
    }
}