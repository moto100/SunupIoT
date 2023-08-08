// <copyright file="ResultStatus.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Contract
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
        /// InvalidJsonString
        /// </summary>
        InvalidJsonString = 14,

        /// <summary>
        /// MissingData
        /// </summary>
        MissingData = 12,

        /// <summary>
        /// FailedToSaveFile
        /// </summary>
        FailedToSaveFile = 13,

        /// <summary>
        /// FailedToSaveFile
        /// </summary>
        FailedToReadFile = 14,

        /// <summary>
        /// MissingProjectId
        /// </summary>
        MissingProjectId = 15,

        /// <summary>
        /// SucceedToStartAppProcess
        /// </summary>
        SucceedToStartAppProcess = 16,

        /// <summary>
        /// FailedToStartAppProcess
        /// </summary>
        FailedToStartAppProcess = 17,

        /// <summary>
        /// FailedToCopyFile
        /// </summary>
        FailedToCopyFile = 18,

        /// <summary>
        /// FileIsNotExisting
        /// </summary>
        FileIsNotExisting = 19,

        /// <summary>
        /// ProjectInfoIsNotExisting
        /// </summary>
        ProjectInfoIsNotExisting = 20,

        /// <summary>
        /// FailedToStopRunningApp
        /// </summary>
        FailedToStopRunningApp = 21,

        /// <summary>
        /// MissingUsernamePassword
        /// </summary>
        MissingUsernamePassword = 22,

        /// <summary>
        /// InvalidUsernamePassword
        /// </summary>
        InvalidUsernamePassword = 23,

        /// <summary>
        /// Ok
        /// </summary>
        Ok = 200,
    }
}
