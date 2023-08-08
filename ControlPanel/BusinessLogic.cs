// <copyright file="BusinessLogic.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ControlPanel
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Sunup.DBConnection;

    /// <summary>
    /// BusinessLogic.
    /// </summary>
    public class BusinessLogic
    {
        private MSSQLProxy dbProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessLogic"/> class.
        /// </summary>
        /// <param name="dbProxy">sessionId.</param>
        public BusinessLogic(MSSQLProxy dbProxy)
        {
            this.dbProxy = dbProxy;
        }

        /// <summary>
        /// GetRuntimeLog.
        /// </summary>
        /// <param name="level">level.</param>
        /// <param name="days">days.</param>
        /// <returns>string.</returns>
        public DataTable GetRuntimeLog(string level, int days)
        {
            if (days == -1000)
            {
                //// clear all log
                this.dbProxy.ExecuteNonQuery("DELETE Log");
                return new DataTable();
            }

            var sql = "SELECT * FROM Log";
            var para = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(level))
            {
                para.Add(new SqlParameter("level", level));
            }

            var date = DateTime.Now.AddDays(days * -1);
            para.Add(new SqlParameter("Date", date));

            for (int i = 0; i < para.Count; i++)
            {
                if (i == 0)
                {
                    sql += " WHERE ";
                }
                else
                {
                    sql += " AND ";
                }

                if (para[i].ParameterName == "Date")
                {
                    sql += $" {para[i].ParameterName} >= @{para[i].ParameterName} ";
                }
                else
                {
                    sql += $" {para[i].ParameterName} = @{para[i].ParameterName}";
                }
            }

            var dataSet = this.dbProxy.ExecuteDataSet(sql + " ORDER BY Date DESC", para.ToArray());
            ////return JsonConvert.SerializeObject(dataSet);
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                return dataSet.Tables[0];
            }
            else
            {
                return new DataTable();
            }
        }
    }
}
