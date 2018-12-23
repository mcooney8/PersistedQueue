namespace PersistedQueue.Sqlite
{
    public class DatabaseItem
    {
        public uint Key { get; set; }
        public byte[] SerializedItem { get; set;  }
    }
}
