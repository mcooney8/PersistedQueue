# PersistentQueue
A queue implementation that keeps all data persisted. The main benefits/goal of this is to allow for only part of the queue items to stay in memory as well as allow for the queue to be reloaded from persistence at any point.

# Example Usage
```c#
// Setup a queue using a flat file as the persistence mechanism
IPersistence<int> flatFilePersistence = new FlatFilePersistence<int>("file.persistence");
PersistentQueue<int> persistentQueue = new PersistentQueue<int>(flatFilePersistence, maxItemsInMemory: 1024);

// And then it can be used like any normal queue
persistentQueue.Enqueue(1);
persistentQueue.Peek();
int i = persistentQueue.Dequeue();

// One additional functionality is the ability to load the existing data from persistence
// By default it loads in the constructor, but it can be deferred
PersistentQueue<int> persistentQueue = new PersistentQueue<int>(flatFilePersistence, maxItemsInMemory: 1024, deferLoad: true);
persistentQueue.Load();
```
