using System.Threading.Tasks;

namespace SaveSystem
{
    /// <summary>
    /// Exposes the same extension methods in <see cref="ScriptableObjectExtensions"/>
    /// </summary>
    public interface IPersistentAdapter
    {
        Task Save();

        Task<LoadReport> Load();

        Task<LoadReport> LoadOrCreate();

        Task<bool> IsSaved();

        Task DeleteData();
    }
}