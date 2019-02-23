namespace PersistedQueue.Sqlite
{
    public struct DatabaseItem
    {
        public uint Key { get; set; }
        public byte[] SerializedItem { get; set;  }
    }
}
