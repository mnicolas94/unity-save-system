using SaveSystem.GuidsResolve;

namespace SaveSystem
{
    public interface IPersistentCallbackReceiver
    {
        void OnBeforeSave(IGuidResolver guidsDatabase);
        void OnAfterSave();
        void OnBeforeLoad();
        void OnAfterLoad(IGuidResolver guidsDatabase);
    }
}