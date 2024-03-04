using System.IO;

namespace SaveSystem.Storages
{
    public interface IStorageStream
    {
        bool TryGetStreamToRead(string profile, string key, out Stream stream);
        Stream GetStreamToWrite(string profile, string key);
    }
}