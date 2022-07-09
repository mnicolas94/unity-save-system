namespace SaveSystem
{
    public interface IPersistentCallbackReceiver
    {
        void OnBeforeSave(AssetGuidsDatabase guidsDatabase);
        void OnAfterSave();
        void OnBeforeLoad();
        void OnAfterLoad(AssetGuidsDatabase guidsDatabase);
    }
}