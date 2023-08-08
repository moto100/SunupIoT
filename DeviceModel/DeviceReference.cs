// <copyright file="DeviceReference.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DeviceModel
{
    using Sunup.Contract;

    /// <summary>
    /// DeviceReference.
    /// </summary>
    public class DeviceReference : IReference
    {
        private Device device;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceReference"/> class.
        /// </summary>
        /// <param name="device">device.</param>
        /// <param name="referenceName">referenceName.</param>
        public DeviceReference(Device device, string referenceName)
        {
            this.device = device;
            this.ReferenceName = referenceName;
            ////this.ReferenceNames = new string[2];
            ////this.ReferenceNames[0] = referenceName;
            ////this.ReferenceNames[1] = "Device";
        }

        /// <summary>
        /// Gets or sets ReferenceName.
        /// </summary>
        public string ReferenceName { get; set; }

        /// <summary>
        /// Gets or sets reference names.
        /// </summary>
        public string[] ReferenceNames { get; set; }

        /// <summary>
        /// Update Field Value.
        /// </summary>
        /// <param name="fieldName">device field name.</param>
        /// <param name="value">value.</param>
        public void SetField(string fieldName, object value)
        {
            this.device.SetField(fieldName, value);
            ////if (string.IsNullOrEmpty(fieldName))
            ////{
            ////    return;
            ////}

            ////var field = fieldName; ////.ToUpper();
            ////////if (this.fieldValues.ContainsKey(field))
            ////////{
            ////this[field] = value;
            ////////}
        }

        /// <summary>
        /// Get value for specified field name.
        /// </summary>
        /// <param name="fieldName">field name.</param>
        /// <returns>return value if has,or return null.</returns>
        public dynamic GetFieldValue(string fieldName)
        {
            return this.device.GetValue(fieldName);
        }

        /// <summary>
        /// Update Field Value.
        /// </summary>
        /// <param name="fieldName">device field name.</param>
        /// <param name="value">value.</param>
        public void SetWrittenField(string fieldName, object value)
        {
            this.device.SetWrittenField(fieldName, value);
            ////if (string.IsNullOrEmpty(fieldName))
            ////{
            ////    return;
            ////}

            ////var field = fieldName; ////.ToUpper();
            ////////if (this.fieldValues.ContainsKey(field))
            ////////{
            ////this[field] = value;
            ////////}
        }

        /// <summary>
        /// Get value for specified field name.
        /// </summary>
        /// <param name="fieldName">field name.</param>
        /// <returns>return value if has,or return null.</returns>
        public dynamic GetWrittenFieldValue(string fieldName)
        {
            return this.device.GetWrittenValue(fieldName);
        }
    }
}
