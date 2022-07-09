namespace SaveSystem.Serializers
{
    public interface ISerializer
    {
        byte[] Serialize(IPersistent obj, AssetGuidsDatabase guidsDatabase);
        void Deserialize(byte[] data, IPersistent obj, AssetGuidsDatabase guidsDatabase);
    }
}