namespace PersistedQueue
{
    public class PersistedQueueConfiguration
    {
        /// <summary>
        /// The max number of items the queue will place in memory for quicker access. Defaults to 1024.
        /// </summary>
        public int MaxItemsInMemory { get; set; }

        /// <summary>
        /// If DeferLoad is set to true, the existing persisted items will not be loaded until/unless the
        /// Load method is called. Defaults to false.
        /// </summary>
        public bool DeferLoad { get; set; }

        /// <summary>
        /// If PersistAllItems is set to true, all items will be persisted, if set to false, only items that 
        /// don't fit into the memory buffer will be persisted. Defaults to true.
        /// </summary>
        public bool PersistAllItems { get; set; }
    }
}
