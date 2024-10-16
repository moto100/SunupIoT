// <copyright file="ServiceController.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServerHost.Controllers
{
    using System.IO;
    using Microsoft.AspNetCore.Mvc;
    using Sunup.Contract;
    using Sunup.IOServer;

    /// <summary>
    /// Service controller.
    /// </summary>
    [ApiController]
    [Route("Service")]
    public class ServiceController : ControllerBase
    {
        /// <summary>
        /// WWW.
        /// </summary>
        /// <returns>string.</returns>
        [HttpPost("WWW")]
        public DataResponse WWW()
        {
            var body = string.Empty;
            using (var reader = new StreamReader(this.Request.Body))
            {
             body = reader.ReadToEnd();
            }

            var sessionid = this.HttpContext.Session.Id;
            return IOServerAgent.Request(sessionid, body);
        }

        /// <summary>
        /// KeepAlive.
        /// </summary>
        /// <returns>string.</returns>
        [HttpGet("KeepAlive")]
        public DataResponse KeepAlive()
        {
            var ret = new DataResponse();
            ret.FunctionType = (int)RequestFunction.KeepAlive;
            ret.Message = "KeepAlive";
            ret.Data = Program.AppId;
            return ret;
        }

        /// <summary>
        /// Test post.
        /// </summary>
        /// <returns>Post.</returns>
        [HttpPost]
        public string Post()
        {
            var sessionId = this.HttpContext.Session.Id;
            return "Test Post:Session -> " + sessionId;
        }

        /// <summary>
        /// Test Get.
        /// </summary>
        /// <returns>Get.</returns>
        [HttpGet]
        public string Get()
        {
            var sessionId = this.HttpContext.Session.Id;
            return "Test Get:Session -> " + sessionId;
        }
    }
}
