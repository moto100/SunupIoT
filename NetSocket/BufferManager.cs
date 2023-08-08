// <copyright file="BufferManager.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource.NetSocket
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;

    /// <summary>
    /// BufferManager.
    /// </summary>
    public class BufferManager
    {
        private int numBytes;
        private byte[] buffer;
        private Stack<int> freeIndexPool;
        private int currentIndex;
        private int bufferSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferManager"/> class.
        /// </summary>
        /// <param name="totalBytes">totalBytes.</param>
        /// <param name="bufferSize">bufferSize.</param>
        public BufferManager(int totalBytes, int bufferSize)
        {
            this.numBytes = totalBytes;
            this.currentIndex = 0;
            this.bufferSize = bufferSize;
            this.freeIndexPool = new Stack<int>();
        }

        /// <summary>
        /// InitBuffer.
        /// </summary>
        public void InitBuffer()
        {
            this.buffer = new byte[this.numBytes];
            Console.WriteLine($"BufferManager this.buffer.length {this.buffer.Length} ");
        }

        /// <summary>
        /// 为上下文对象分配内存；
        /// 若栈中有数据则表明有内存曾今被回收过，则新的上下文对象就用这块内存
        /// 否则就连续进行分配，直到内存满为止.
        /// </summary>
        /// <param name="args">SocketAsyncEventArgs.</param>
        /// <returns>true.</returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (this.freeIndexPool.Count > 0)
            {
                args.SetBuffer(this.buffer, this.freeIndexPool.Pop(), this.bufferSize);
            }
            else
            {
                if ((this.numBytes - this.bufferSize) < this.currentIndex)
                {
                    return false;
                }

                ////Console.WriteLine($"BufferManager this.currentIndex {this.currentIndex} ");
                args.SetBuffer(this.buffer, this.currentIndex, this.bufferSize);
                this.currentIndex += this.bufferSize;
            }

            return true;
        }

        /// <summary>
        /// 从上下文中释放的内存都将放入管理池(栈)中.
        /// </summary>
        /// <param name="args">SocketAsyncEventArgs.</param>
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            this.freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }
}
