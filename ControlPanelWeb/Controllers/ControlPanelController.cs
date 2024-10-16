// <copyright file="ControlPanelController.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ControlPanelWeb.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Sunup.Contract;
    using Sunup.ControlPanel;

    /// <summary>
    /// ControlPanel controller.
    /// </summary>
    [ApiController]
    [Route("ControlPanel")]
    public class ControlPanelController : ControllerBase
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
             body = reader.ReadToEndAsync().Result;
            }

            var sessionid = this.HttpContext.Session.Id;
            return AdminAgent.Request(sessionid, body);
        }

        /// <summary>
        /// WWW.
        /// </summary>
        /// <returns>string.</returns>
        [HttpPost("UploadFile")]
        public DataResponse UploadFile()
        {
            var body = string.Empty;
            var content = string.Empty;
            var projectId = this.Request.Form.ContainsKey("projectId") ? this.Request.Form["projectId"].ToString() : string.Empty;
            var function = this.Request.Form.ContainsKey("function") ? this.Request.Form["function"].ToString() : string.Empty;

            var file = this.Request.Form.Files.Count > 0 ? this.Request.Form.Files[0] : null;
            if (file != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    var contentArray = memoryStream.ToArray();
                    content = System.Text.Encoding.UTF8.GetString(contentArray);
                }
            }

            Newtonsoft.Json.Linq.JObject jsonDocument = new Newtonsoft.Json.Linq.JObject();
            jsonDocument["function"] = function;
            jsonDocument["projectId"] = projectId;
            jsonDocument["data"] = Newtonsoft.Json.Linq.JObject.Parse(content);
            body = jsonDocument.ToString();
            var sessionid = this.HttpContext.Session.Id;
            return AdminAgent.Request(sessionid, body);
        }

        /// <summary>
        /// Download.
        /// </summary>
        /// <returns>FileResult.</returns>
        [HttpPost("DownloadFile")]
        [ProducesResponseType(typeof(FileResult), 200)]
        public async Task<FileResult> DownloadFile()
        {
            var body = string.Empty;
            using (var reader = new StreamReader(this.Request.Body))
            {
             body = reader.ReadToEndAsync().Result;
            }

            var sessionid = this.HttpContext.Session.Id;
            var dataResponse = AdminAgent.Request(sessionid, body);
            FileStreamResult actionresult = null;
            if (dataResponse.Status == (int)ResultStatus.Ok && dataResponse.FunctionType == (int)RequestFunction.DownloadProject)
            {
                string fileContent = Convert.ToString(dataResponse.Data);
                var file = await this.GetStream(fileContent);
                actionresult = new FileStreamResult(file, new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
                actionresult.FileDownloadName = "project.json";
            }

            return actionresult;
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

        private async Task<Stream> GetStream(string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            await writer.FlushAsync();
            stream.Position = 0;
            return stream;
        }
    }
}
