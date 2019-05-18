# PersistedQueue
A queue implementation that keeps all data persisted. The main benefits/goal of this is to allow for only part of the queue items to stay in memory as well as allow for the queue to be reloaded from persistence at any point.

# Example Usage
```c#
// Setup a queue using sqlite as the persistence mechanism
IPersistence<int> sqlitePersistence = new SqlitePersistence<int>("persist.db");
PersistedQueue<int> persistentQueue = new PersistedQueue<int>(sqlitePersistence);

// And then it can be used like any normal queue
persistedQueue.Enqueue(1);
persistedQueue.Peek();
int i = persistedQueue.Dequeue();

// If you want more control over what gets persisted and when or whether data is loaded from persistence
// you can specify a config in the constructor
PersistedQueueConfiguration config = new PersistedQueueConfiguration
{
    MaxItemsInMemory = 100, // Defaults to 1024, is the number of items that will be kept in memory for faster access
    DeferLoad = true, // Defaults to false, allows loading of items already persisted to be deferred until later, when ready the client can call Load()
    PersistAllItems = true // Defaults to true, if enabled means that every item inserted will get persisted, if disabled, only the items that cannot fit into the in-memory items will be persisted
};
PersistedQueue<int> persistentQueue = new PersistedQueue<int>(sqlitePersistence, config);
persistedQueue.Load();
```

# Persistence Options
So far, there are 2 separate persistence implementations included as separate projects in this repo (and eventually separate nuget packages when I get to that part).
1. [Sqlite](https://github.com/mcooney8/PersistentQueue/tree/master/PersistedQueue.Sqlite) (utilizing [Sqlite.Fast](https://github.com/zmj/sqlite-fast/tree/master/Sqlite.Fast))
2. [Nosql](https://github.com/mcooney8/PersistentQueue/tree/master/PersistedQueue.Nosql) (utilizing [DBreeze](https://github.com/hhblaze/DBreeze))

# Custom Persistence
One of the main things I wanted to accomplish with this library is allow you to use your own persistence and not have to add additional dependencies that you wouldn't need. All you need to do implement your own persistence is implement the IPersistence interface and pass it as a constructor parameter.
A simple (but pointless) example is the in memory one included for testing:
```c#
public class InMemoryPersistence<T> : IPersistence<T>
{
    Dictionary<uint, T> items = new Dictionary<uint, T>();

    public T Load(uint key)
    {
        return items[key];
    }

    public void Persist(uint key, T item)
    {
        items[key] = item;
    }

    public void Remove(uint key)
    {
        items.Remove(key);
    }

    public void Clear()
    {
        items.Clear();
    }

    public IEnumerable<T> Load()
    {
        return new List<T>();
    }

    public void Dispose()
    {
    }
}
```
