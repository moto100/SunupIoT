// <copyright file="IReference.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Contract
{
    /// <summary>
    ///  Reference.
    /// </summary>
    public interface IReference
   {
        /////// <summary>
        /////// Gets or sets reference type.
        /////// </summary>
        ////ReferenceType ReferenceType { get; set; }

        /////// <summary>
        /////// Gets or sets value.
        /////// </summary>
        ////object Value { get; set; }

        /////// <summary>
        /////// Gets or sets attribute name.
        /////// </summary>
        ////string Name { get; set; }

        /// <summary>
        /// Gets or sets reference name.
        /// </summary>
        string ReferenceName { get; set; }

        /// <summary>
        /// Gets or sets reference names.
        /// </summary>
        string[] ReferenceNames { get; set; }

        /////// <summary>
        /////// Add notification.
        /////// </summary>
        /////// <param name="dataChange">Instance of IDataChange interface.</param>
        ////void AddNotification(IDataChange dataChange);
    }
}
