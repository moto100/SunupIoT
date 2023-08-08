// <copyright file="ResultStatus.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServer
{
    /// <summary>
    /// Result status type.
    /// </summary>
    public enum ResultStatus : byte
    {
        /// <summary>
        /// UnKnown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// InvalidSessionId
        /// </summary>
        InvalidSessionId = 1,

        /// <summary>
        /// InvalidJson
        /// </summary>
        InvalidJson = 2,

        /// <summary>
        /// MissingFunction
        /// </summary>
        MissingFunction = 3,

        /// <summary>
        /// InvalidFunction
        /// </summary>
        InvalidFunction = 4,

        /// <summary>
        /// MissingExpressionsParameter
        /// </summary>
        MissingExpressionsParameter = 5,

        /// <summary>
        /// InvalidExpressionsParameter
        /// </summary>
        InvalidExpressionsParameter = 6,

        /// <summary>
        /// FailedToInitailzieApp
        /// </summary>
        FailedToInitailzieApp = 7,

        /// <summary>
        /// ConnectionClosed
        /// </summary>
        ConnectionClosed = 8,

        /// <summary>
        /// MissingSubcriptionIdParameter
        /// </summary>
        MissingSubcriptionIdParameter = 9,

        /// <summary>
        /// InvalidSubcriptionIdParameter
        /// </summary>
        InvalidSubcriptionIdParameter = 10,

        /// <summary>
        /// MissingWrittenNameParameter
        /// </summary>
        MissingWrittenNameParameter = 11,

        /// <summary>
        /// MissingWrittenValueParameter
        /// </summary>
        MissingWrittenValueParameter = 12,

        /// <summary>
        /// InvalidRequestIdParameter
        /// </summary>
        InvalidRequestIdParameter = 13,

        /// <summary>
        /// Ok
        /// </summary>
        Ok = 200,
    }
}
