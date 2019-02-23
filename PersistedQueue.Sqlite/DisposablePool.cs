using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.PersistedQueue.Sqlite
{
    internal class DisposablePool<T> : IDisposable where T : class, IDisposable
    {
        private readonly Func<T> factory;

        private System.Threading.SpinLock poolLock = new System.Threading.SpinLock(); // mutable struct; must not be readonly
        private readonly T[] pool;
        private int freeIndex = 0;

        public DisposablePool(Func<T> factory, int poolSize)
        {
            this.factory = factory;
            pool = new T[poolSize];
        }

        public T Rent()
        {
            T instance;
            if (!TryGet(out instance))
            {
                instance = factory();
            }
            return instance;
        }

        private bool TryGet(out T instance)
        {
            instance = null;
            bool lockTaken = false;
            try
            {
                poolLock.Enter(ref lockTaken);
                if (freeIndex < pool.Length)
                {
                    instance = pool[freeIndex];
                    pool[freeIndex] = null;
                    freeIndex++;
                }
            }
            finally
            {
                if (lockTaken)
                {
                    poolLock.Exit();
                }
            }
            return instance != null;
        }

        public void Return(T instance)
        {
            if (!TryPut(instance))
            {
                instance.Dispose();
            }
        }

        private bool TryPut(T instance)
        {
            bool lockTaken = false;
            try
            {
                poolLock.Enter(ref lockTaken);
                if (freeIndex == 0)
                {
                    return false;
                }
                freeIndex--;
                pool[freeIndex] = instance;
                return true;
            }
            finally
            {
                if (lockTaken)
                {
                    poolLock.Exit();
                }
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < pool.Length; i++)
            {
                pool[i]?.Dispose();
                pool[i] = null;
            }
        }
    }
}
