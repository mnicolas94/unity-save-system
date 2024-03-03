using System.Threading.Tasks;

namespace SaveSystem.Storages
{
    public interface IStorage
    {
        /// <summary>
        /// Whether a key's data is stored for a given profile.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ExistsData(string profile, string key);
        
        /// <summary>
        /// Writes a key's data for a specific profile.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task Write(string profile, string key, byte[] data);
        
        /// <summary>
        /// Reads a key's data for a given profile. Returns a tuple with a bool value that indicates whether the
        /// operation was successful and the data itself.  
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<(bool, byte[])> Read(string profile, string key);
        
        /// <summary>
        /// Deletes a key's data for a specific profile.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task Delete(string profile, string key);
    }
}