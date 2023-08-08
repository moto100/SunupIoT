// <copyright file="DevicesProxyReference.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DeviceModel
{
    using Sunup.Contract;

    /// <summary>
    /// Devices.
    /// </summary>
    public class DevicesProxyReference : IReference
    {
        private DevicesProxy devicesProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="DevicesProxyReference"/> class.
        /// </summary>
        /// <param name="devicesProxy">devicesProxy.</param>
        /// <param name="referenceName">referenceName.</param>
        public DevicesProxyReference(DevicesProxy devicesProxy, string referenceName)
        {
            this.ReferenceName = referenceName;
            this.devicesProxy = devicesProxy;
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
        /// Get data from device.
        /// </summary>
        /// <param name="deviceName">Device Name.</param>
        /// <param name="fieldName">Field Name.</param>
        /// <returns>return data from device.</returns>
        public dynamic GetData(string deviceName, string fieldName)
        {
            return this.devicesProxy.GetData(deviceName, fieldName);
        }
    }
}
