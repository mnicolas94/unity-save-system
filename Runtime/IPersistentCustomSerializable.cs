using SaveSystem.GuidsResolve;

namespace SaveSystem
{
    public interface IPersistentCustomSerializable
    {
        byte[] WriteData(IGuidResolver database);
        void ReadData(byte[] data, IGuidResolver database);
    }
}