// <copyright file="TimeoutCounter.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Contract
{
    using System;

    /// <summary>
    /// TimeoutCounter.
    /// </summary>
    public class TimeoutCounter
    {
        private DateTime endDateTime;

        private int msecond;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutCounter"/> class.
        /// </summary>
        /// <param name="msecond">msecond.</param>
        public TimeoutCounter(int msecond)
        {
            this.msecond = msecond;
            this.endDateTime = DateTime.Now.AddMilliseconds(msecond);
        }

        /// <summary>
        /// Reset.
        /// </summary>
        public void Reset()
        {
            this.endDateTime = DateTime.Now.AddMilliseconds(this.msecond);
        }

        /// <summary>
        /// HasElapsed.
        /// </summary>
        /// <returns>true means elapsed.</returns>
        public bool HasElapsed()
        {
            return DateTime.Now > this.endDateTime;
        }
    }
}
