namespace NhemBootStrap.Editor.Core {

    public interface IBootstrapStep {
        string Name { get; }
        bool CheckCompleted();
        void Execute(BootstrapContext context);
    }
}