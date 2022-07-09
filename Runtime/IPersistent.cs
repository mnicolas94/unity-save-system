namespace SaveSystem
{
    public interface IPersistent
    {
        string PersistentFileName { get; }

        void ResetToDefault();

        /// <summary>
        /// If a file from a previous version is loaded, this method is called for specific backward compatibility
        /// loading logic. Returns whether its compatible or not.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool ReadPreviousVersion(string version, byte[] data);
    }
}