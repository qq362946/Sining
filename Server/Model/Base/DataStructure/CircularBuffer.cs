using System;
using System.Collections.Generic;
using System.IO;

namespace Sining.DataStructure
{
    /// <summary>
    /// 环形缓存（自动扩充、不会收缩缓存、所以不要用这个操作过大的IO流）
    /// 1、环大小8192，溢出的会自动增加环的大小。
    /// 2、每个块都是一个环形缓存，当溢出的时候会自动添加到下一个环中。
    /// 3、当读取完成后用过的环会放在缓存中，不会销毁掉。
    /// </summary>
    public class CircularBuffer : Stream
    {
        public int ChunkSize = 8192; // 环形缓存块大小
        
        private readonly Queue<byte[]> _bufferQueue = new Queue<byte[]>();

        private readonly Queue<byte[]> _bufferCache = new Queue<byte[]>();
        
        private byte[] _lastBuffer;

        public int FirstIndex { get; set; }
        
        public int LastIndex { get; set; }

        public override long Length
        {
            get
            {
                if (_bufferQueue.Count == 0) return 0;

                return (_bufferQueue.Count - 1) * ChunkSize + LastIndex - FirstIndex;
            }
        }

        public byte[] First
        {
            get
            {
                if (_bufferQueue.Count == 0)
                {
                    AddLast();
                }
                return _bufferQueue.Peek();
            }
        }

        public byte[] Last
        {
            get
            {
                if (_bufferQueue.Count == 0)
                {
                    AddLast();
                }
                return _lastBuffer;
            }
        }
        public void AddLast()
        {
            var buffer = _bufferCache.Count > 0 ? _bufferCache.Dequeue() : new byte[ChunkSize];
            
            _bufferQueue.Enqueue(buffer);
            _lastBuffer = buffer;
        }

        public void RemoveFirst()
        {
            _bufferCache.Enqueue(_bufferQueue.Dequeue());
        }

        public void Read(Stream stream, int count)
        {
            if (count > Length)
            {
                throw new Exception($"bufferList length < count, {Length} {count}");
            }

            var copyCount = 0;
            while (copyCount < count)
            {
                var n = count - copyCount;
                if (ChunkSize - FirstIndex > n)
                {
                    stream.Write(First, FirstIndex, n);
                    FirstIndex += n;
                    copyCount += n;
                }
                else
                {
                    stream.Write(First, FirstIndex, ChunkSize - FirstIndex);
                    copyCount += ChunkSize - FirstIndex;
                    FirstIndex = 0;
                    RemoveFirst();
                }
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer.Length < offset + count)
            {
                throw new Exception($"buffer length < count, buffer length: {buffer.Length} {offset} {count}");
            }

            var length = Length;
            if (length < count)
            {
                count = (int)length;
            }
            
            var copyCount = 0;
            while (copyCount < count)
            {
                var copyLength = count - copyCount;

                if (ChunkSize - FirstIndex > copyLength)
                {
                    Array.Copy(First,
                        FirstIndex,
                        buffer,
                        copyCount + offset, copyLength);
                    
                    FirstIndex += copyLength;
                    copyCount += copyLength;
                    continue;
                }

                Array.Copy(First,
                    FirstIndex,
                    buffer,
                    copyCount + offset,
                    ChunkSize - FirstIndex);

                copyCount += ChunkSize - FirstIndex;
                FirstIndex = 0;
                
                RemoveFirst();
            }
            
            return count;
        }
        
        public void Write(Stream stream)
        {
            var count = (int)(stream.Length - stream.Position);
			        
            var copyCount = 0;
            while (copyCount < count)
            {
                if (LastIndex == ChunkSize)
                {
                    AddLast();
                    LastIndex = 0;
                }

                var n = count - copyCount;
                if (ChunkSize - LastIndex > n)
                {
                    stream.Read(Last, LastIndex, n);
                    LastIndex += count - copyCount;
                    copyCount += n;
                }
                else
                {
                    stream.Read(Last, LastIndex, ChunkSize - LastIndex);
                    copyCount += ChunkSize - LastIndex;
                    LastIndex = ChunkSize;
                }
            }
        }

        public void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var copyCount = 0;

            while (copyCount < count)
            {
                if (ChunkSize == LastIndex)
                {
                    AddLast();
                    LastIndex = 0;
                }

                var byteLength = count - copyCount;
 
                if (ChunkSize - LastIndex > byteLength)
                {
                    Array.Copy(buffer,
                        copyCount + offset,
                        Last,
                        LastIndex, byteLength);
                    LastIndex += byteLength;
                    copyCount += byteLength;
                }
                else
                {
                    Array.Copy(buffer,
                        copyCount + offset,
                        _lastBuffer,
                        LastIndex,
                        ChunkSize - LastIndex);
                    copyCount += ChunkSize - LastIndex;
                    LastIndex = ChunkSize;
                }
            }
        }

        public override void Flush()
        {
            throw new System.NotImplementedException();
        }
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override bool CanRead { get; } = true;
        public override bool CanSeek { get; } = false;
        public override bool CanWrite { get; } = true;
        public override long Position { get; set; }

        public void Clear()
        {
            _bufferQueue.Clear();
            _lastBuffer = null;
            FirstIndex = 0;
            LastIndex = 0;
        }
    }
}