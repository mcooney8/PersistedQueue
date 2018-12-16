using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace PersistedQueue.Persistence
{
    public class FlatFilePersistence<T> : IPersistence<T>
    {
        private const string IndexFilename = "index";
        private const char IndexFileEntrySeparator = '\n';
        private const char IndexFileValueSeparator = '|';
        
        private readonly FileStream indexFileStream;
        private readonly FileStream itemFileStream;
        private readonly FileInfo itemFileInfo;
        private readonly BinaryFormatter binaryFormatter;
        private readonly object indexFileLock = new object();
        private readonly object itemFileLock = new object();

        // TODO: In-memory dictionary to store partial or full index

        public FlatFilePersistence(string filename)
        {
            string directoryName = Path.GetDirectoryName(filename);
            string indexFilename = Path.Combine(directoryName, IndexFilename);
            indexFileStream = new FileStream(indexFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            itemFileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            itemFileInfo = new FileInfo(filename);
            binaryFormatter = new BinaryFormatter();
        }

        public void Clear()
        {
            indexFileStream.SetLength(0);
            itemFileStream.SetLength(0);
        }

        public IEnumerable<T> Load()
        {
            throw new NotImplementedException();
        }

        public T Load(long key)
        {
            ByteRange byteRange = GetItemByteRange(key);
            byte[] buffer = new byte[byteRange.Length];
            lock (itemFileLock)
            {
                itemFileStream.Seek(byteRange.Start, SeekOrigin.Begin);
                itemFileStream.Read(buffer, 0, byteRange.Length);
            }
            return (T)binaryFormatter.Deserialize(new MemoryStream(buffer));
        }

        public void Persist(long key, T item)
        {
            long originalFileLength;
            long serializedItemlength;
            lock (itemFileLock)
            {
                originalFileLength = itemFileInfo.Length;
                binaryFormatter.Serialize(itemFileStream, item);
                itemFileInfo.Refresh();
                serializedItemlength = itemFileInfo.Length - originalFileLength;
            }
            WriteIndex(key, originalFileLength, serializedItemlength);
        }

        // TODO: This whole method needs to be rethought
        // - How to efficiently remove from index file
        // - How to efficiently remove from item file
        public void Remove(long key)
        {
            ByteRange indexByteRange = GetIndexByteRange(key);
            lock (indexFileLock)
            {
                byte[] buffer = new byte[indexByteRange.Length];
                indexFileStream.Position = indexByteRange.Start;
                indexFileStream.Write(buffer, 0, indexByteRange.Length);
            }
        }

        private void WriteIndex(long key, long position, long length)
        {
            ReadOnlySpan<byte> entryToWrite = CreateEntry(key, position, length);
            lock (indexFileLock)
            {
                // TODO: Depending on how remove ends up looking, I may want to check for a gap in the
                // file to fill in here instead of simply appending to the end
                indexFileStream.Seek(0, SeekOrigin.End);
                indexFileStream.Write(entryToWrite);
                indexFileStream.Flush();
            }
        }

        private ReadOnlySpan<byte> CreateEntry(long key, long position, long length)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(key);
            sb.Append(IndexFileValueSeparator);
            sb.Append(position);
            sb.Append(IndexFileValueSeparator);
            sb.Append(length);
            sb.Append(IndexFileEntrySeparator);
            return Encoding.ASCII.GetBytes(sb.ToString()).AsSpan();
        }

        private ByteRange GetItemByteRange(long key)
        {
            long start = -1;
            int length = -1;
            lock (indexFileLock)
            {
                StreamReader indexReader = new StreamReader(indexFileStream);
                while (!indexReader.EndOfStream)
                {
                    string entry = indexReader.ReadLine();
                    if (!entry.Contains(IndexFileValueSeparator))
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

        private ByteRange GetIndexByteRange(long key)
        {
            lock (indexFileLock)
            {
                indexFileStream.Seek(0, SeekOrigin.Begin);
                StreamReader indexReader = new StreamReader(indexFileStream);
                long lastPosition = 0;
                while (!indexReader.EndOfStream)
                {
                    string entry = indexReader.ReadLine();
                    string[] entryParts = entry.Split(IndexFileValueSeparator);
                    if (long.TryParse(entryParts[0], out long entryKey) && entryKey == key)
                    {
                        long length = indexReader.BaseStream.Position - lastPosition;
                        return new ByteRange(lastPosition, (int)length);
                    }
                }
            }
            return default(ByteRange);
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

        ~FlatFilePersistence()
        {
            indexFileStream.Dispose();
            itemFileStream.Dispose();
        }
    }
}
