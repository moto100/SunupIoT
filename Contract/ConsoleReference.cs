// <copyright file="ConsoleReference.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Contract
{
    using System;

    /// <summary>
    /// Devices.
    /// </summary>
    public class ConsoleReference : IReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleReference"/> class.
        /// </summary>
        public ConsoleReference()
        {
            this.ReferenceName = "Console";
        }

        /// <summary>
        /// Gets or sets reference name.
        /// </summary>
        public string ReferenceName { get; set; }

        /// <summary>
        /// Gets or sets reference names.
        /// </summary>
        public string[] ReferenceNames { get; set; }

        /// <summary>
        /// log message.
        /// </summary>
        /// <param name="message">Device Name.</param>
        public void Log(object message)
        {
            Console.WriteLine(message);
        }
    }
}
