// <copyright file="ICallback.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Contract
{
    /// <summary>
    /// WebAgentCallback interface.
    /// </summary>
    public interface ICallback
    {
        /// <summary>
        /// MessageCallback.
        /// </summary>
        /// <param name="result">changed object.</param>
        void MessageCallback(string result);
    }
}
