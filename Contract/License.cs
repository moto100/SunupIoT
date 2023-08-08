// <copyright file="License.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Contract
{
    using System;

    /// <summary>
    /// FileProcess.
    /// </summary>
    public class License
    {
        private static bool isLicenseValid = false;
        private static int dataPointNumber = 5;
        private static int connectionNumber = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="License"/> class.
        /// </summary>
        /// <param name="isValid">isValid.</param>
        /// <param name="pointNumber">pointNumber.</param>
        /// <param name="conNumber">conNumber.</param>
        public License(bool isValid, int pointNumber, int conNumber)
        {
            isLicenseValid = isValid;
            dataPointNumber = pointNumber;
            connectionNumber = conNumber;
        }

        /// <summary>
        /// Gets a value indicating whether isLicenseValid.
        /// </summary>
        public static bool IsLicenseValid
        {
            get { return isLicenseValid; }
        }

        /// <summary>
        /// Gets dataPointNumber.
        /// </summary>
        public static int DataPointNumber
        {
            get { return dataPointNumber; }
        }

        /// <summary>
        /// Gets ConectionNumber.
        /// </summary>
        public static int ConnectionNumber
        {
            get { return connectionNumber; }
        }
    }
}
