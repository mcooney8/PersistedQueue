using System;
using System.Collections.Generic;
using System.Text;

namespace PersistedQueue
{
    public class PersistedQueueConfiguration
    {
        public int MaxItemsInMemory { get; set; }
        public bool DeferLoad { get; set; }
        public bool PersistAllItems { get; set; }
    }
}
