// <copyright file="Helper.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Contract
{
    using System;

    /// <summary>
    /// Object Copier.
    /// </summary>
    public static class Helper
    {
        /////// <summary>
        /////// Parse data value .
        /////// </summary>
        /////// <param name="dataTypeStr">The type of data.</param>
        /////// <param name="dataValueStr">The value of data.</param>
        /////// <returns>The copied object.</returns>
        ////public static DataValue ParseDataValue(string dataTypeStr, string dataValueStr)
        ////{
        ////    DataType dataType = (DataType)byte.Parse(dataTypeStr);
        ////    DataValue dataValue = null;
        ////    switch (dataType)
        ////    {
        ////        case DataType.Integer:
        ////            dataValue = new DataValue(int.Parse(dataValueStr));
        ////            break;
        ////        case DataType.Bool:
        ////            dataValue = new DataValue(bool.Parse(dataValueStr));
        ////            break;
        ////        case DataType.String:
        ////            dataValue = new DataValue(dataValueStr);
        ////            break;
        ////        case DataType.DateTime:
        ////            dataValue = new DataValue(DateTime.Parse(dataValueStr));
        ////            break;
        ////    }

        ////    return dataValue;
        ////}

        /// <summary>
        /// Parse data value .
        /// </summary>
        /// <param name="dataType">The type of data.</param>
        /// <param name="dataValueStr">The value of data.</param>
        /// <returns>The copied object.</returns>
        public static object ParseDataValue(DataType dataType, string dataValueStr)
        {
            object dataValue = null;
            switch (dataType)
            {
                case DataType.Integer:
                    dataValue = int.Parse(dataValueStr);
                    break;
                case DataType.Bool:
                    dataValue = bool.Parse(dataValueStr);
                    break;
                case DataType.String:
                    dataValue = dataValueStr;
                    break;
                case DataType.DateTime:
                    dataValue = DateTime.Parse(dataValueStr);
                    break;
            }

            return dataValue;
        }
    }
}
