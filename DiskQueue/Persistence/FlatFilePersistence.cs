using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace PersistedQueue.Persistence
{
    public class FlatFilePersistence<T> : IPersistence<T>, IDisposable
    {
        private const char IndexFileEntrySeparator = '\n';
        private const char IndexFileValueSeparator = '|';

        private readonly string indexFilename;
        private readonly string itemFilename;
        private readonly BinaryFormatter binaryFormatter;
        private readonly object indexFileLock = new object();
        private readonly object itemFileLock = new object();

        private FileStream indexFileStream;
        private FileStream itemFileStream;
        private bool isDisposed;

        // TODO: In-memory dictionary to store partial or full index

        public FlatFilePersistence(string filename)
        {
            string directoryName = Path.GetDirectoryName(filename);
            string name = Path.GetFileName(filename);
            indexFilename = Path.Combine(directoryName, $"~{name}");
            itemFilename = filename;
            indexFileStream = new FileStream(indexFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            itemFileStream = new FileStream(itemFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            binaryFormatter = new BinaryFormatter();
        }

        public void Clear()
        {
            ResetIndexFile();
            ResetItemFile();
        }

        public IEnumerable<T> Load()
        {
            throw new NotImplementedException();
        }

        public T Load(uint key)
        {
            ByteRange byteRange = GetItemByteRange(key);
            if (byteRange.Equals(default(ByteRange)))
            {
                throw new Exception($"Unable to load key: {key}");
            }
            byte[] buffer = new byte[byteRange.Length];
            lock (itemFileLock)
            {
                itemFileStream.Seek(byteRange.Start, SeekOrigin.Begin);
                itemFileStream.Read(buffer, 0, byteRange.Length);
            }
            return (T)binaryFormatter.Deserialize(new MemoryStream(buffer));
        }

        public void Persist(uint key, T item)
        {
            long originalFileLength;
            long serializedItemlength;
            lock (itemFileLock)
            {
                originalFileLength = itemFileStream.Position;
                binaryFormatter.Serialize(itemFileStream, item);
                itemFileStream.Flush();
                serializedItemlength = itemFileStream.Position - originalFileLength;
            }
            WriteIndex(key, originalFileLength, serializedItemlength);
        }

        // TODO: This whole method needs to be rethought
        // - How to efficiently remove from index file
        // - How to efficiently remove from item file
        private static byte[] RemoveBytes = Encoding.ASCII.GetBytes("r");
        public void Remove(uint key)
        {
            ByteRange indexByteRange = GetIndexByteRange(key);
            lock (indexFileLock)
            {
                indexFileStream.Position = indexByteRange.Start;
                indexFileStream.Write(RemoveBytes, 0, RemoveBytes.Length);
                indexFileStream.Flush();
            }
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                indexFileStream.Dispose();
                itemFileStream.Dispose();
            }
        }

        ~FlatFilePersistence()
        {
            Dispose();
        }

        private void WriteIndex(uint key, long position, long length)
        {
            byte[] entry = CreateEntry(key, position, length);
            lock (indexFileLock)
            {
                // TODO: Depending on how remove ends up looking, I may want to check for a gap in the
                // file to fill in here instead of simply appending to the end
                indexFileStream.Seek(0, SeekOrigin.End);
                indexFileStream.Write(entry);
                indexFileStream.Flush();
            }
        }

        private byte[] CreateEntry(uint key, long position, long length)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(key);
            sb.Append(IndexFileValueSeparator);
            sb.Append(position);
            sb.Append(IndexFileValueSeparator);
            sb.Append(length);
            sb.Append(IndexFileEntrySeparator);
            return Encoding.ASCII.GetBytes(sb.ToString());
        }

        private ByteRange GetItemByteRange(uint key)
        {
            long start = -1;
            int length = -1;
            lock (indexFileLock)
            {
                StreamReader indexReader = new StreamReader(indexFileStream);
                while (!indexReader.EndOfStream)
                {
                    string entry = indexReader.ReadLine();
                    if (entry.StartsWith('r'))
                    {
                        continue;
                    }
                    string[] entryParts = entry.Split(IndexFileValueSeparator);
                    if (long.TryParse(entryParts[0], out long entryKey) && entryKey == key)
                    {
                        long.TryParse(entryParts[1], out start);
                        int.TryParse(entryParts[2], out length);
                        return new ByteRange(start, length);
                    }
                }
                return default(ByteRange);
            }
        }

        private ByteRange GetIndexByteRange(uint key)
        {
            lock (indexFileLock)
            {
                indexFileStream.Seek(0, SeekOrigin.Begin);
                StreamReader indexReader = new StreamReader(indexFileStream);
                long lastPosition = 0;
                while (!indexReader.EndOfStream)
                {
                    string entry = indexReader.ReadLine();
                    int entryLength = entry.Length + 1; // + 1 for newline character
                    if (entry.StartsWith('r'))
                    {
                        lastPosition += entryLength;
                        continue;
                    }
                    string[] entryParts = entry.Split(IndexFileValueSeparator);
                    if (long.TryParse(entryParts[0], out long entryKey) && entryKey == key)
                    {
                        return new ByteRange(lastPosition, entryLength);
                    }
                    lastPosition += entryLength;
                }
            }
            return default(ByteRange);
        }

        private void ResetIndexFile()
        {
            lock (indexFileLock)
            {
                indexFileStream.Dispose();
                File.Delete(indexFilename);
                indexFileStream = new FileStream(indexFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
        }

        private void ResetItemFile()
        {
            lock (itemFileLock)
            {
                itemFileStream.Dispose();
                File.Delete(itemFilename);
                itemFileStream = new FileStream(itemFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
        }

        private struct ByteRange
        {
            public ByteRange(long start, int length)
            {
                Start = start;
                Length = length;
            }
            public long Start { get; set; }
            public int Length { get; set; }
        }
    }
}
