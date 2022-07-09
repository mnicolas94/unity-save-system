namespace SaveSystem
{
    public interface IPersistentCustomSerializable
    {
        byte[] WriteData(AssetGuidsDatabase database);
        void ReadData(byte[] data, AssetGuidsDatabase database);
    }
}