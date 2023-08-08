// <copyright file="ISubscriptionCallback.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServer
{
    using Sunup.Contract;

    /// <summary>
    /// ISubscriptionCallback interface.
    /// </summary>
    public interface ISubscriptionCallback
    {
        /// <summary>
        /// OnDataChange.
        /// </summary>
        /// <param name="changedObject">changed object.</param>
        /// <param name="subscriptionId">subscriptionId.</param>
        /// <param name="function">function.</param>
        void OnDataChange(VTQ[] changedObject, int subscriptionId, RequestFunction function);
    }
}
