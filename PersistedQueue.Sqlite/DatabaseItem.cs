namespace PersistedQueue.Sqlite
{
    public struct DatabaseItem
    {
        public uint Key { get; set; }
        public string SerializedItem { get; set;  }
    }
}
