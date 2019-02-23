# PersistentQueue
A queue implementation that keeps all data persisted. The main benefits/goal of this is to allow for only part of the queue items to stay in memory as well as allow for the queue to be reloaded from persistence at any point.

# Example Usage
```c#
// Setup a queue using a flat file as the persistence mechanism
IPersistence<int> sqlitePersistence = new SqlitePersistence<int>("persist.db");
PersistedQueue<int> persistentQueue = new PersistedQueue<int>(sqlitePersistence);

// And then it can be used like any normal queue
persistedQueue.Enqueue(1);
persistedQueue.Peek();
int i = persistedQueue.Dequeue();

// One additional functionality is the ability to load the existing data from persistence
// By default it loads in the constructor, but it can be deferred
PersistedQueueConfiguration config = new PersistedQueueConfiguration { DeferLoad = true };
PersistedQueue<int> persistentQueue = new PersistedQueue<int>(sqlitePersistence, config);
persistedQueue.Load();
```

# Custom Persistence
One of the main things I wanted to accomplish with this library is allow you to use your own persistence and not have to add additional dependencies that you wouldn't need. All you need to do implement your own persistence is implement the IPersistence interface and pass it as a constructor parameter.
A simple (but useless) example is the in memory one included for testing:
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
