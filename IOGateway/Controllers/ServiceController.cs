// <copyright file="ServiceController.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOGateway.Controllers
{
    using System.IO;
    using Microsoft.AspNetCore.Mvc;
    using Sunup.IOGateway;

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
            this.ExtendSessionExpiration();
            var body = string.Empty;
            using (var reader = new StreamReader(this.Request.Body))
            {
             body = reader.ReadToEndAsync().Result;
            }

            var sessionid = this.HttpContext.Session.Id;
            ////return IOServerAgent.Request(sessionid, body);
            return new DataResponse();
        }

        /// <summary>
        /// Test post.
        /// </summary>
        /// <returns>Post.</returns>
        [HttpPost]
        public string Post()
        {
            this.ExtendSessionExpiration();
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
            this.ExtendSessionExpiration();
            var sessionId = this.HttpContext.Session.Id;
            return "Test Get:Session -> " + sessionId;
        }

        private void ExtendSessionExpiration()
        {
            this.HttpContext.Session.Set("Extend session expiration", new byte[] { 1, 2, 3, 4, 5 });
        }
    }
}
