using System;
using System.Text;

namespace common.io
{
    public class IoBuffer
    {
        private const int INITCAPACITY = 100 * 1024; // 默认初始容量
        private const int INCREMENTSIZE = 2 * 1024;// 自动扩展时一次最少扩展多少字节

        private byte[] hb;
        private int mCurrentCapacity;  // 缓冲区的容量
        private int mLength;// 缓冲区中当前有效数据的长度
        private bool mExpandable;//是否可自动扩展容量
        private int mUppderCapacity;//可扩展到的最大容量-1会让无限扩展
        private int mReadPos;// 读指针的偏移位置（指向流中第一个有效字节）
        private int mWritePos;//写指针的偏移位置（指向流中最后一个有效字节）
        private bool isLittleEndian = true;//字节序
        private byte[] mSyncObj = new byte[0]; //用于加锁
        public IoBuffer()
            : this(INITCAPACITY)
        {

        }
        public IoBuffer(int capacity)
            : this(capacity, true)
        {

        }
        public IoBuffer(int capacity, bool expandable)
            : this(capacity, expandable, -1, null)
        {

        }
        public IoBuffer(int capacity, bool expandable, int maxCapacity, byte[] initBuffer)
        {
            if (capacity < 0)
            {
                mCurrentCapacity = INITCAPACITY;
            }
            else
            {
                mCurrentCapacity = capacity;
            }

            if (expandable && (maxCapacity != -1 && maxCapacity < capacity))
            {
                mExpandable = false;
                mUppderCapacity = -1;
            }
            else
            {
                mExpandable = expandable;
                mUppderCapacity = maxCapacity;
            }
            if (initBuffer == null)
            { 
                mLength = 0;
                hb = new byte[mCurrentCapacity];
                mReadPos = 0;
                mWritePos = 0;
            }
            else
            {
                mLength = initBuffer.Length;
                if (initBuffer.Length < mCurrentCapacity)
                {
                    hb = new byte[mCurrentCapacity];
                    Buffer.BlockCopy(initBuffer, 0, hb, 0, mLength);
                }
                else
                {
                    hb = initBuffer;
                }
                mReadPos = 0;
                mWritePos = Length;
            }
        }
        public IoBuffer Order(Boolean isLittleEndian)
        {
            this.isLittleEndian = isLittleEndian;
            return this;
        }
        public IoBuffer(byte[] hb, int mReadPos,int mWritePos)
        {
            this.hb = hb;
            this.mReadPos = mReadPos;
            this.mWritePos = mWritePos;
        }
        public static IoBuffer Wrap(byte[] hb)
        {
            return new IoBuffer(hb,0,hb.Length);
        }
        public static IoBuffer Allocate()
        {
            return new IoBuffer(INITCAPACITY);
        }
        public static IoBuffer Allocate(int capacity){
            return new IoBuffer(capacity);
        }
        public byte Read()
        {
            byte[] des = new byte[1];
            Read(des,0,1);
            return des[0];
        }
        public void WriteByte(byte input)
        {
            byte[] src = new byte[1];
            src[0] = input;
            Write(src, 0, 1);
        }
        public void WriteByts(byte[] input)
        {
            Write(input, 0, input.Length);
        }
        public void Write(byte input)
        {
            WriteByte(BufferType.BYTE);
            WriteByte(input);
        }
        public void Write(Boolean input)
        {
            byte rs = (byte)(input ? 1 : 0);
            WriteByte(BufferType.BOOLEAN);
            WriteByte(rs);
        }
        public short ReadShort()
        {
            byte[] des = new byte[2];
            int len = Read(des, 0, 2);
            if (len == 0)
                throw new IoBufferException(IoBufferException.lengthShort);
            short rs = BitConverter.ToInt16(des, 0);
            return isLittleEndian ? rs : System.Net.IPAddress.HostToNetworkOrder(rs);
        }
        public void Write(short input)
        {
            WriteByte(BufferType.SHORT);
            byte[] src = BitConverter.GetBytes(input);
            WriteByts(src);
        }
        public int ReadInt()
        {
            byte[] des = new byte[4];
            int len = Read(des, 0, 4);
            if (len == 0)
                throw new IoBufferException(IoBufferException.lengthShort);
            int rs = BitConverter.ToInt32(des, 0);
            return isLittleEndian ? rs : System.Net.IPAddress.HostToNetworkOrder(rs);
        }
        public void Write(int input)
        {
            WriteByte(BufferType.INT);
            byte[] src = BitConverter.GetBytes(input);
            WriteByts(src);
        }
        public void WriteInt(int input)
        {
            byte[] src = BitConverter.GetBytes(input);
            WriteByts(src);
        }
        public void WriteShort(short input)
        {
            byte[] src = BitConverter.GetBytes(input);
            WriteByts(src);
        }
        public float ReadFloat()
        {
            if (mReadPos < 0)
                return 0;
            if (hb.Length < mReadPos)
                throw new IoBufferException(IoBufferException.lengthShort);
            byte[] des = new byte[4];
            Read(des, 0, 4);
            float rs = BitConverter.ToSingle(des, 0);
            return rs;//TODO float字节序转换
        }
        public void Write(float input)
        {
            WriteByte(BufferType.FLOAT);
            byte[] src = BitConverter.GetBytes(input);
            WriteByts(src);
        }
        public long ReadLong()
        {
            byte[] des = new byte[8];
            int len = Read(des, 0, 8);
            if (len == 0)
                throw new IoBufferException(IoBufferException.lengthShort);
            long rs = BitConverter.ToInt64(des, 0);
            return isLittleEndian ? rs : System.Net.IPAddress.HostToNetworkOrder(rs);
        }
        public DateTime ReadDate()
        {
            long rs = ReadLong();
            return UtcTime(rs);
        }
        public static DateTime UtcTime(long date)
        {
            string timeStamp = date + "";
            if (timeStamp.Length > 10)
            {
                timeStamp = timeStamp.Substring(0, 10);
            }
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime now = dtStart.Add(toNow);
            return now;
        }
        public void Write(long input)
        {
            WriteByte(BufferType.LONG);
            byte[] src = BitConverter.GetBytes(input);
            WriteByts(src);
        }
        public void Write(DateTime input)
        {
            WriteByte(BufferType.DATE);
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            long v = (long)(input - startTime).TotalMilliseconds; // 相差毫秒数;
            byte[] src = BitConverter.GetBytes(v);
            WriteByts(src);
        }
        public byte[] ReadByteArr()
        {
            if (mReadPos < 0)
                return null;
            int len = ReadInt();
            byte[] des = new byte[len];
			if (len != 0)//非空串
			{
				len = Read(des, 0, len);
				if (len == 0)
					throw new IoBufferException(IoBufferException.lengthShort);
			}
            return des;
        }
        public void Write(byte[] input)
        {
            WriteByte(BufferType.BYTE_ARR);
			WriteInt(input.Length);
            WriteByts(input);
        }
        public string ReadString()
        {
            if (mReadPos < 0)
                return null;
            return Encoding.UTF8.GetString(ReadByteArr());
        }
        public void Write(string input)
        {
            byte[] src = Encoding.UTF8.GetBytes(input);
            WriteByte(BufferType.STRING);
            WriteInt(src.Length);
            WriteByts(src);
        }
        public byte[] readAll()
        {
            if (mReadPos < 0)
                return null;
            byte[] rs = new byte[mLength];
            Read(rs, mReadPos, mWritePos);
            return rs;
        }
        public int PeekInt(ref int value)
        {
            byte[] des = new byte[4];
            int len = Peek(des, 0, 4);
            if (len == 0)
                return -1;
            int rs = BitConverter.ToInt32(des, 0);
            value = isLittleEndian ? rs : System.Net.IPAddress.HostToNetworkOrder(rs);
            return 0;
        }
        public int Peek(byte[] dest, int offset, int count)
        {
            if (dest == null || offset < 0
                || count < 0
                || (dest.Length - offset) < count)
            {
                return 0;
            }

            lock (mSyncObj)
            {
                // 真正要读取的字节数
                if(mLength<count)
                {
                    return 0;
                }

                if (mReadPos < mWritePos)
                {
                    Buffer.BlockCopy(hb, mReadPos, dest, offset, count);
                }
                else
                {
                    int afterReadPosLen = mCurrentCapacity - mReadPos;
                    if (afterReadPosLen >= count)
                    {
                        Buffer.BlockCopy(hb, mReadPos, dest, offset, count);
                    }
                    else
                    {
                        Buffer.BlockCopy(hb, mReadPos, dest, offset, afterReadPosLen);
                        int restLen = count - afterReadPosLen;
                        Buffer.BlockCopy(hb, 0, dest, offset + afterReadPosLen, restLen);
                    }
                }

                return count;
            }
        }
        /// <summary>
        /// 从Buffer读取数据
        /// </summary>
        /// <param name="buffer">包含所读取到的字节。</param>
        /// <param name="offset">buffer 中的字节偏移量。</param>
        /// <param name="count">最多读取的字节数。</param>
        /// <returns>成功读取到的总字节数。可能为0</returns>
        public int Read(byte[] dest, int offset, int count)
        {
            if (dest == null || offset < 0
                || count < 0
                || (dest.Length - offset) < count)
            {
                return 0;
            }

            lock (mSyncObj)
            {
                // 真正要读取的字节数
                if(mLength<count)
                {
                    return 0;
                }

                ReadInternal(dest, offset, count);

                return count;
            }
        }
        /// <summary>
        /// 写入BUFFER
        /// </summary>
        /// <param name="buffer">源数据。</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">字节数。</param>
        public int Write(byte[] src, int offset, int count)
        {
            if (src == null || offset < 0
                         || count < 0
                         || (src.Length - offset) < count)
            {
                return 0;
            }

            lock (mSyncObj)
            {
                // 要往流中写入 buffer 中的数据，流的容量至少要是这么多
                int minCapacityNeeded = Length + count;

                // 如果需要扩展流则扩展流
                ExpandBufferIfNeed(minCapacityNeeded);
                // 如果无法再容纳下指定的字节数
                if (minCapacityNeeded <= mCurrentCapacity)
                {
                    this.WriteInternal(src, offset, count);
                }
                else
                {
                    throw new IndexOutOfRangeException("Buffer full, can't write any more.");
                }

                return count;
            }
        }
        public byte[] GetBuffer()
        {
            return hb;
        }
        public int Length
        {
            get
            {
                lock (mSyncObj)
                {
                    return mLength;
                }
            }
        }
        public bool IsFull()
        {
            return mLength == mCurrentCapacity;
        }

        public bool IsEmpty()
        {
            return mLength == 0;
        }
        public void Reset()
        {
            mLength = 0;
            mReadPos = 0;
            mWritePos = 0;
        }
        private void ReadInternal(byte[] dest, int offset, int count)
        {
            if (mReadPos < mWritePos)
            {
                Buffer.BlockCopy(hb, mReadPos, dest, offset, count);
            }
            else
            {
                int afterReadPosLen = mCurrentCapacity - mReadPos;

                if (afterReadPosLen >= count)
                {
                    Buffer.BlockCopy(hb, mReadPos, dest, offset, count);
                }
                else
                {
                    Buffer.BlockCopy(hb, mReadPos, dest, offset, afterReadPosLen);
                    int restLen = count - afterReadPosLen;
                    Buffer.BlockCopy(hb, 0, dest, offset + afterReadPosLen, restLen);
                }
            }

            MoveReadPos(count);
        }
        public void MoveReadPos(int count)
        {
            mReadPos += count;
            mReadPos %= mCurrentCapacity;
            mLength -= count;

            //重置读取，判断更方便
            if (mLength == 0)
            {
                mReadPos = mWritePos = 0;
            }
        }
        private void WriteInternal(byte[] src, int offset, int count)
        {
            if (mReadPos > mWritePos)
            {
                Buffer.BlockCopy(src, offset, hb, mWritePos, count);
            }
            else
            {
                int afterWritePosLen = mCurrentCapacity - mWritePos;
                if (afterWritePosLen >= count)
                {
                    Buffer.BlockCopy(src, offset, hb, mWritePos, count);
                }
                else
                {
                    Buffer.BlockCopy(src, offset, hb, mWritePos, afterWritePosLen);
                    int restLen = count - afterWritePosLen;
                    Buffer.BlockCopy(src, offset + afterWritePosLen, hb, 0, restLen);
                }
            }

            mWritePos += count;
            mWritePos %= mCurrentCapacity;
            mLength += count;
        }
        private void ExpandBufferIfNeed(int minSize)
        {
            if (!mExpandable)
            {
                return;
            }

            // 不需要扩展
            if (mCurrentCapacity >= minSize)
            {
                return;
            }

            // 无法扩展
            if (mUppderCapacity != -1 && (mUppderCapacity - mCurrentCapacity) < INCREMENTSIZE)
            {
                return;
            }

            // 计算要扩展几块（INCREMENTSIZE 的倍数）
            int blocksNum = (int)Math.Ceiling((double)(minSize - mCurrentCapacity) / INCREMENTSIZE);

            byte[] buffNew = new byte[mCurrentCapacity + blocksNum * INCREMENTSIZE];

			Buffer.BlockCopy(hb, 0, buffNew, 0, mCurrentCapacity);
            hb = buffNew;
            mCurrentCapacity = buffNew.Length;
        }
    
    }
}
