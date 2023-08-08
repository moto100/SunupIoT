// <copyright file="InfluxDBClient.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.InfluxDBConnection
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using InfluxDB.Client;
    using InfluxDB.Client.Api.Domain;
    using InfluxDB.Client.Writes;
    using Sunup.Contract;

    /// <summary>
    /// MSSQLProxy.
    /// </summary>
    public sealed partial class InfluxDBClient
    {
        private string connectionString;
        private string organizationId;
        private string bucketName;
        private string measurement;
        private string accessToken;
        private InfluxDB.Client.InfluxDBClient influxDBClient;
        private bool isStarted = false;
        private WriteApiAsync writeApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfluxDBClient"/> class.
        /// </summary>
        /// <param name="connectionString">connectionString.</param>
        /// <param name="organizationId">organizationId.</param>
        /// <param name="bucketName">bucketName.</param>
        /// <param name="measurement">measurement.</param>
        /// <param name="accessToken">accessToken.</param>
        public InfluxDBClient(string connectionString, string organizationId, string bucketName, string measurement, string accessToken)
        {
            this.connectionString = connectionString;
            this.organizationId = organizationId;
            this.bucketName = bucketName;
            this.measurement = measurement;
            this.accessToken = accessToken;
        }

        /// <summary>
        /// Gets a value indicating whether isStarted.
        /// </summary>
        public bool IsStarted
        {
            get { return this.isStarted; }
        }

        /// <summary>
        /// Start.
        /// </summary>
        /// <returns>return true.</returns>
        public bool Start()
        {
            if (string.IsNullOrEmpty(this.connectionString) || string.IsNullOrEmpty(this.organizationId) ||
                string.IsNullOrEmpty(this.bucketName) || string.IsNullOrEmpty(this.measurement) || string.IsNullOrEmpty(this.accessToken))
            {
                return false;
            }

            try
            {
                this.influxDBClient = InfluxDBClientFactory.Create(this.connectionString, this.accessToken.ToCharArray());
                this.writeApi = this.influxDBClient.GetWriteApiAsync();
                Diagnostics.Logger.LogInfo($"[InfluxDBClient]Start >> Succeeded to create a InfluxDB client, connectionString : {this.connectionString} , organizationId : {this.organizationId} ,bucketName : {this.bucketName} ,measurement : {this.measurement} ,accessToken : {this.accessToken}.");
                this.isStarted = true;
            }
            catch (Exception ex)
            {
                this.isStarted = false;
                Diagnostics.Logger.LogWarning($"[InfluxDBClient]Start >> Failed to create a InfluxDB client, connectionString : {this.connectionString} , organizationId : {this.organizationId} ,bucketName : {this.bucketName} ,measurement : {this.measurement} ,accessToken : {this.accessToken}.", ex);
                Diagnostics.Logger.LogTrace($"[InfluxDBClient]Start >> Failed to create a InfluxDB client, connectionString : {this.connectionString} , organizationId : {this.organizationId} ,bucketName : {this.bucketName} ,measurement : {this.measurement} ,accessToken : {this.accessToken}.", ex);
            }

            return this.isStarted;
        }

        /// <summary>
        /// WriteObject.
        /// </summary>
        /// <param name="nodes">nodes.</param>
        /// <returns>true.</returns>
        public async Task WriteObject(VTQ[] nodes)
        {
            ////bool ret = false;
            if (this.isStarted && nodes != null && nodes.Length > 0)
            {
                ////return false;
                try
                {
                    var list = new List<PointData>();
                    foreach (var node in nodes)
                    {
                        if (!string.IsNullOrEmpty(node.Id) && node.Value != null)
                        {
                            ////Diagnostics.Logger.LogWarning($"[InfluxDBClient]WriteObject >> value is {node.Value}.");
                            if (node.DataType != 2 && node.DataType != 4)
                            {
                                double value = 0.0d;
                                bool parseFlag = false;
                                if (node.DataType == 1 || node.DataType == 5 || node.DataType == 6)
                                {
                                    parseFlag = double.TryParse(node.Value.ToString(), out value);
                                }

                                if (node.DataType == 3)
                                {
                                    parseFlag = true;
                                    var b = (bool)node.Value;
                                    if (b)
                                    {
                                        value = 1.0d;
                                    }
                                    else
                                    {
                                        value = 0.0d;
                                    }
                                }

                                if (parseFlag)
                                {
                                    var point = PointData.Measurement(this.measurement)
                                  .Tag("nodeid", node.Id)
                                  .Field("nodevalue", value)
                                  .Field("valuequality", node.Quality)
                                  .Field("datatype", node.DataType)
                                  .Timestamp(DateTime.UtcNow, WritePrecision.Ns);
                                    list.Add(point);
                                }
                            }
                        }
                    }

                    if (list.Count > 0)
                    {
                        await this.writeApi.WritePointsAsync(this.bucketName, this.organizationId, list);
                    }
                }
                catch (Exception ex)
                {
                    ////ret = false;
                    Diagnostics.Logger.LogWarning($"[InfluxDBClient]WriteObject >> Failed to write node value into InfluxDB.", ex);
                    Diagnostics.Logger.LogTrace($"[InfluxDBClient]WriteObject >> Failed to write node value into InfluxDB.", ex);
                }
            }
        }
    }
}
