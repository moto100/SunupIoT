// <copyright file="DataResponse.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOGateway
{
    /// <summary>
    /// DataResponse.
    /// </summary>
    public class DataResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataResponse"/> class.
        /// </summary>
        public DataResponse()
        {
            this.Status = (byte)ResultStatus.Ok;
            this.Message = string.Empty;
            this.Data = null;
        }

        /// <summary>
        /// Gets or sets status.
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// Gets or sets message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets FunctionType.
        /// </summary>
        public int FunctionType { get; set; }

        /// <summary>
        /// Gets or sets data.
        /// </summary>
        public dynamic Data { get; set; }
    }
}
