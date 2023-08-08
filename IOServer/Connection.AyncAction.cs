// <copyright file="Connection.AyncAction.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServer
{
    using System;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Sunup.Contract;

    /// <summary>
    /// Connection.
    /// </summary>
    public partial class Connection : ISubscriptionCallback
    {
        /// <summary>
        /// Receive data from window view.
        /// </summary>
        /// <param name="changedObject">changedObject.</param>
        /// <param name="subscriptionId">subscriptionId.</param>
        /// <param name="function">function.</param>
        public void OnDataChange(VTQ[] changedObject, int subscriptionId, RequestFunction function)
        {
            if (subscriptionId > -1 && changedObject != null && changedObject.Length > 0)
            {
                DataResponse dataResponse = new DataResponse();
                dataResponse.FunctionType = (int)function;
                dataResponse.Data = new
                {
                    subscriptionId = subscriptionId,
                    changedValues = changedObject,
                };

                if (this.Callback != null)
                {
                    var content = JsonConvert.SerializeObject(dataResponse);
                    this.Callback.MessageCallback(content);
                }
            }
        }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <param name="request">request.</param>
        /// <returns>representing the asynchronous operation.</returns>
        public Task ProcessAyncRequest(string request)
        {
            return Task.Run(() =>
            {
                DataResponse dataResponse = null;
                JObject jsonDocument = null;
                RequestFunction requestFunction = RequestFunction.Unknown;
                var requestStatus = this.VerifyRequest(request, out jsonDocument, out requestFunction);
                if (requestStatus != ResultStatus.Ok)
                {
                    dataResponse = this.ResponseError(requestStatus);
                }
                else
                {
                    if (requestFunction == RequestFunction.Subscribe)
                    {
                        dataResponse = this.Subscribe(jsonDocument);
                    }

                    if (requestFunction == RequestFunction.Unsubscribe)
                    {
                        dataResponse = this.Unsubscribe(jsonDocument);
                    }

                    if (requestFunction == RequestFunction.WriteNode)
                    {
                        dataResponse = this.Write(jsonDocument);
                    }
                }

                if (this.Callback != null && dataResponse != null)
                {
                    var content = JsonConvert.SerializeObject(dataResponse);
                    this.Callback.MessageCallback(content);
                }
            });
        }
    }
}
