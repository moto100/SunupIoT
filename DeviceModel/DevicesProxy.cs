// <copyright file="DevicesProxy.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DeviceModel
{
    using System;
    using System.Collections.Generic;
    using Sunup.Contract;
    using Sunup.DeviceModel;
    using Sunup.Diagnostics;
    using Sunup.ScriptExecutor;

    /// <summary>
    /// Devices.
    /// </summary>
    public class DevicesProxy : IDataChange
    {
        private Dictionary<string, Device> devices;
        private IDataChange notification;

        /// <summary>
        /// Initializes a new instance of the <see cref="DevicesProxy"/> class.
        /// </summary>
        public DevicesProxy()
        {
            this.devices = new Dictionary<string, Device>();
            this.Name = "DevicesProxy";
            this.DevicesProxyReference = new DevicesProxyReference(this, "DevicesProxy");
            ////this.ReferenceType = ReferenceType.GlobalVariable;
        }

        /////// <summary>
        /////// Gets or sets ReferenceType.
        /////// </summary>
        ////public ReferenceType ReferenceType { get; set; }

        /// <summary>
        /// Gets Value.
        /// </summary>
        public DevicesProxyReference DevicesProxyReference { get; }

        /// <summary>
        /// Gets devices.
        /// </summary>
        public Dictionary<string, Device> Devices
        {
            get { return this.devices; }
        }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get data from device.
        /// </summary>
        /// <param name="deviceName">Device Name.</param>
        /// <param name="fieldName">Field Name.</param>
        /// <returns>return data from device.</returns>
        public dynamic GetData(string deviceName, string fieldName)
        {
            if (string.IsNullOrEmpty(deviceName))
            {
                Logger.LogWarning($"[Devices Proxy]Try to get data from device >>Device name is null or empty.");
                return null;
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                Logger.LogWarning($"[Devices Proxy]Try to get data from device >>Field name is null or empty.");
                return null;
            }

            Logger.LogTrace($"[Devices Proxy]Try to get data from device >>Device name: {deviceName}, Field name: {fieldName}.");

            ////var deviceNameToUpper = deviceName.ToUpper();
            var device = this.GetDevice(deviceName);
            if (device != null)
            {
                return device.GetValue(fieldName);
            }

            return null;
        }

        /// <summary>
        /// Add Device.
        /// </summary>
        /// <param name="device">device object.</param>
        public void AddDevice(Device device)
        {
            if (device == null || string.IsNullOrEmpty(device.Name))
            {
                Logger.LogError($"[Devices Proxy]Add Device >>Name is null or empty.");
                return;
            }

            var deviceNameToUpper = device.Name.ToUpper();
            if (!this.devices.ContainsKey(deviceNameToUpper))
            {
                this.devices[deviceNameToUpper] = device;
                device.AddNotification(this);
                Logger.LogInfo($"[Devices Proxy]Added Device >>Name: {device.Name}.");
            }
        }

        /// <summary>
        /// Remove Device.
        /// </summary>
        /// <param name="deviceName">device name.</param>
        public void RemoveDevice(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName))
            {
                Logger.LogError($"[Devices Proxy]Remove Device >>Name is null or empty.");
                return;
            }

            var deviceNameToUpper = deviceName.ToUpper();
            var device = this.GetDevice(deviceNameToUpper);
            if (device != null)
            {
                device.RemoveNotification(this);
                this.devices.Remove(deviceNameToUpper);
                Logger.LogInfo($"[Devices Proxy]Remove Device >>Name: {deviceName}.");
            }
        }

        /// <summary>
        /// Get device by name.
        /// </summary>
        /// <param name="deviceName">device Name.</param>
        /// <returns>return device.</returns>
        public Device GetDevice(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName))
            {
                Logger.LogError($"[Devices Proxy]Get Device >>Name is null or empty.");
                return null;
            }

            if (this.devices.ContainsKey(deviceName.ToUpper()))
            {
                ////Logger.LogTrace($"[Devices Proxy]Get Device >>Name: {deviceName}.");
                return this.devices[deviceName.ToUpper()];
            }
            else
            {
                Logger.LogWarning($"[Devices Proxy]Get Device >>Don't have device : {deviceName}.");
            }

            return null;
        }

        /// <summary>
        /// Add notification.
        /// </summary>
        /// <param name="dataChange">Notification of IDataChange.</param>
        public void AddNotification(IDataChange dataChange)
        {
            this.notification = dataChange;
        }

        /// <summary>
        /// Implementation of IDataChange interface.
        /// </summary>
        public void OnDataChange()
        {
            //// get notification from device, and then notify container.
            Logger.LogTrace($"[Devices Proxy]Got Notification from devices>> the device: have new changed data.");
            this.Notify();
        }

        /// <summary>
        /// Run.
        /// </summary>
        public void Run()
        {
            if (this.devices != null)
            {
                foreach (var device in this.devices.Values)
                {
                    if (device != null)
                    {
                        device.Run();
                    }
                }
            }
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public void Stop()
        {
            if (this.devices != null)
            {
                foreach (var device in this.devices.Values)
                {
                    if (device != null)
                    {
                        device.Stop();
                    }
                }
            }
        }

        /// <summary>
        /// WriteItem.
        /// </summary>
        /// <param name="item">item.</param>
        public void WriteItem(WriteItem item)
        {
            var device = this.GetDevice(item.BoundDevice);
            if (device != null)
            {
                device.WriteItem(item);
            }
            else
            {
                Logger.LogWarning($"[Devices Proxy]Write Item >>Don't have device : {item.BoundDevice}.");
            }
        }

        private void Notify()
        {
            if (this.notification != null)
            {
                this.notification.OnDataChange();
            }
        }
    }
}
