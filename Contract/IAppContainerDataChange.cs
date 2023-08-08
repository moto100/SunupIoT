// <copyright file="IAppContainerDataChange.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>
namespace Sunup.Contract
{
    using System.Collections.Generic;
    using Sunup.Contract;

    /// <summary>
    /// IAppContainerDataChange interface.
    /// </summary>
    public interface IAppContainerDataChange
    {
        /// <summary>
        /// OnDataChange.
        /// </summary>
        /// <param name="changedObject">changed object.</param>
        void OnDataChange(List<VTQ> changedObject);
    }
}
