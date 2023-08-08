// <copyright file="Session.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ControlPanel
{
    /// <summary>
    /// Session.
    /// </summary>
    public class Session
    {
        private string sessionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="sessionId">sessionId.</param>
        public Session(string sessionId)
        {
            this.sessionId = sessionId;
        }
    }
}
