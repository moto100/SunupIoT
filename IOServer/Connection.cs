// <copyright file="Connection.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using Sunup.Contract;
    using Sunup.Diagnostics;

    /// <summary>
    /// Connection.
    /// </summary>
    public partial class Connection
    {
        private string connectionId;
        private Application application;
        private List<Subscription> subscriptions;
        private int subscriptionIndex = -1;
        private object subscriptionIndexLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="connectionId">sessionId.</param>
        /// <param name="application">application.</param>
        public Connection(string connectionId, Application application)
        {
            this.connectionId = connectionId;
            this.application = application;
            this.subscriptions = new List<Subscription>();
        }

        /// <summary>
        /// Gets or sets Callback.
        /// </summary>
        public ICallback Callback { get; set; }

        /// <summary>
        /// Gets count of ConnectionType.
        /// </summary>
        public ConnectionType ConnectionType { get; private set; }

        /// <summary>
        /// Open connection.
        /// </summary>
        public void Open()
        {
            this.ConnectionType = ConnectionType.Opened;
            Diagnostics.Logger.LogTrace($"[Connection]Open >> ConnectionId : {this.connectionId}.");
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        /// <returns>task.</returns>
        public Task Close()
        {
            var task = Task.Run(() =>
            {
                this.ConnectionType = ConnectionType.Closed;
                Diagnostics.Logger.LogTrace($"[Connection]Closing >> ConnectionId : {this.connectionId}.");
                if (this.subscriptions != null)
                {
                    var count = this.subscriptions.Count;
                    for (var i = count - 1; i >= 0; i--)
                    {
                        var subscription = this.subscriptions[i];
                        if (subscription != null)
                        {
                            subscription.Stop();
                        }
                    }
                }

                Diagnostics.Logger.LogTrace($"[Connection]Closed >> ConnectionId : {this.connectionId}.");
            });

            return task;
        }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <param name="request">request.</param>
        /// <returns>response.</returns>
        public DataResponse ProcessRequest(string request)
        {
            DataResponse dataResponse = null;
            JObject jsonDocument = null;
            RequestFunction requestFunction = RequestFunction.Unknown;
            var requestStatus = this.VerifyRequest(request, out jsonDocument, out requestFunction);
            if (requestStatus != ResultStatus.Ok)
            {
                return this.ResponseError(requestStatus);
            }

            if (requestFunction == RequestFunction.Subscribe)
            {
                dataResponse = this.Subscribe(jsonDocument);
            }

            //// This method just for http pulling request.
            if (requestFunction == RequestFunction.GetSubscribedData)
            {
                dataResponse = this.GetSubscribedData(jsonDocument);
            }

            if (requestFunction == RequestFunction.Unsubscribe)
            {
                dataResponse = this.Unsubscribe(jsonDocument);
            }

            if (requestFunction == RequestFunction.WriteNode)
            {
                dataResponse = this.Write(jsonDocument);
            }

            return dataResponse;
        }

        private DataResponse Write(JObject jsonDocument)
        {
            DataResponse dataResponse = new DataResponse();
            var name = jsonDocument.Value<string>("name");
            var value = jsonDocument.Value<string>("value");
            this.application.AppContainerModel.WriteItem(name, value);
            dataResponse.FunctionType = (int)RequestFunction.WriteNode;
            dataResponse.Data = new
            {
            };

            return dataResponse;
        }

        private DataResponse GetSubscribedData(JObject jsonDocument)
        {
            DataResponse dataResponse = new DataResponse();
            var subscriptionId = jsonDocument["subscriptionId"] == null ? -1 : jsonDocument.Value<int>("subscriptionId");
            var subscription = this.subscriptions.Find(x => x.SubscriptionId == subscriptionId);
            if (subscription == null)
            {
                dataResponse = new DataResponse()
                {
                    Status = (int)ResultStatus.InvalidSubcriptionIdParameter,
                    Message = "Invalid subcriptionId parameter.",
                };

                return dataResponse;
            }
            else
            {
                dataResponse.FunctionType = (int)RequestFunction.GetSubscribedData;
                dataResponse.Data = new
                {
                    changedValues = subscription.GetChangedData(),
                };

                return dataResponse;
            }
        }

        private DataResponse Subscribe(JObject jsonDocument)
        {
            DataResponse dataResponse = new DataResponse();
            var subscriptionId = jsonDocument["subscriptionId"] == null ? -1 : jsonDocument.Value<int>("subscriptionId");
            var requestId = jsonDocument["requestId"] == null ? string.Empty : jsonDocument.Value<string>("requestId");
            var expressions = jsonDocument.Value<JArray>("expressions");
            var subscription = this.subscriptions.Find(x => x.SubscriptionId == subscriptionId);
            if (subscription == null)
            {
                lock (this.subscriptionIndexLock)
                {
                    subscriptionId = ++this.subscriptionIndex;
                }

                subscription = new Subscription(subscriptionId, this.application);
                this.subscriptions.Add(subscription);
                subscription.DataChangeCallback = this;
                var exps = new List<string>();
                foreach (var item in expressions)
                {
                    exps.Add(item.Value<string>());
                }

                if (exps.Count > 0)
                {
                    subscription.AddExpression(exps.ToArray());
                }

                subscription.Start();
            }
            else
            {
                var exps = new List<string>();
                foreach (var item in expressions)
                {
                    exps.Add(item.Value<string>());
                }

                if (exps.Count > 0)
                {
                    subscription.AddExpression(exps.ToArray());
                }
            }

            dataResponse.FunctionType = (int)RequestFunction.Subscribe;
            if (!string.IsNullOrEmpty(requestId))
            {
                dataResponse.Data = new
                {
                    subscriptionId = subscriptionId,
                    requestId = requestId,
                };
            }
            else
            {
                dataResponse.Data = new
                {
                    subscriptionId = subscriptionId,
                };
            }

            return dataResponse;
        }

        private DataResponse Unsubscribe(JObject jsonDocument)
        {
            DataResponse dataResponse = new DataResponse();
            var subscriptionId = jsonDocument["subscriptionId"] == null ? -1 : jsonDocument.Value<int>("subscriptionId");
            var expressions = jsonDocument["expressions"] == null ? new JArray() : jsonDocument.Value<JArray>("expressions");
            var subscription = this.subscriptions.Find(x => x.SubscriptionId == subscriptionId);
            if (subscription == null)
            {
                dataResponse = new DataResponse()
                {
                    Status = (int)ResultStatus.InvalidSubcriptionIdParameter,
                    Message = "Invalid subcriptionId parameter.",
                };

                return dataResponse;
            }
            else
            {
                var exps = new List<string>();
                foreach (var item in expressions)
                {
                    if (item.Type == JTokenType.String)
                    {
                        exps.Add(item.Value<string>());
                    }
                }

                if (exps.Count > 0)
                {
                    subscription.RemoveExpression(exps.ToArray());
                }
                else
                {
                    subscription.Stop();
                    this.subscriptions.Remove(subscription);
                }
            }

            dataResponse.FunctionType = (int)RequestFunction.Unsubscribe;
            dataResponse.Data = new
            {
                subscriptionId = subscriptionId,
            };

            return dataResponse;
        }

        private ResultStatus VerifyRequest(string request, out JObject jsonDocument, out RequestFunction requestFunction)
        {
            jsonDocument = null;
            requestFunction = RequestFunction.Unknown;
            if (string.IsNullOrEmpty(request))
            {
                Logger.LogError("[Connection] VerifyRequest >> Request is empty ot null.");
                return ResultStatus.InvalidJson;
            }

            ResultStatus resultStatus = ResultStatus.Ok;
            JObject document = null;
            try
            {
                document = JObject.Parse(request);
            }
            catch (Exception ex)
            {
                Logger.LogError("[Connection] VerifyRequest >> JObject.Parse Exception ", ex);
                return ResultStatus.InvalidJson;
            }

            if (document != null)
            {
                jsonDocument = document;
                string function = "Unknown";
                var found = document["function"] != null && document["function"].Type == JTokenType.String;
                if (!found)
                {
                    return ResultStatus.MissingFunction;
                }
                else
                {
                    function = document.Value<string>("function");
                    if (!Enum.TryParse(function, true, out requestFunction))
                    {
                        return ResultStatus.InvalidFunction;
                    }
                }

                if (requestFunction == RequestFunction.Subscribe)
                {
                    var hasExpressions = document["expressions"] != null && document["expressions"].Type == JTokenType.Array;
                    if (!hasExpressions)
                    {
                        return ResultStatus.MissingExpressionsParameter;
                    }
                    else
                    {
                        var expressions = document.Value<JArray>("expressions");
                        if (expressions.Count == 0)
                        {
                            return ResultStatus.InvalidExpressionsParameter;
                        }
                        else
                        {
                            foreach (var item in expressions)
                            {
                                if (item.Type != JTokenType.String)
                                {
                                    return ResultStatus.InvalidExpressionsParameter;
                                }
                            }
                        }
                    }

                    if (document["subscriptionId"] != null && document["subscriptionId"].Type != JTokenType.Integer)
                    {
                        return ResultStatus.InvalidSubcriptionIdParameter;
                    }

                    if (document["requestId"] != null && document["requestId"].Type != JTokenType.String)
                    {
                        return ResultStatus.InvalidRequestIdParameter;
                    }
                }
                else if (requestFunction == RequestFunction.Unsubscribe || requestFunction == RequestFunction.GetSubscribedData)
                {
                    var hasSubscriptionId = document["subscriptionId"] != null && document["subscriptionId"].Type == JTokenType.Integer;
                    if (!hasSubscriptionId)
                    {
                        return ResultStatus.MissingSubcriptionIdParameter;
                    }
                }
                else if (requestFunction == RequestFunction.WriteNode)
                {
                    var hasName = document["name"] != null && document["name"].Type == JTokenType.String;
                    if (!hasName)
                    {
                        return ResultStatus.MissingWrittenNameParameter;
                    }

                    var hasValue = document["value"] != null;
                    if (!hasValue)
                    {
                        return ResultStatus.MissingWrittenValueParameter;
                    }
                }
            }

            return resultStatus;
        }

        private DataResponse ResponseError(ResultStatus resultStatusType)
        {
            DataResponse dataResponse = null;
            switch (resultStatusType)
            {
                case ResultStatus.InvalidJson:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.InvalidJson,
                        Message = "Invalid json.",
                    };
                    break;
                case ResultStatus.MissingFunction:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.MissingFunction,
                        Message = "Missing function.",
                    };
                    break;
                case ResultStatus.InvalidFunction:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.InvalidFunction,
                        Message = "Invalid function.",
                    };
                    break;
                case ResultStatus.MissingExpressionsParameter:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.MissingExpressionsParameter,
                        Message = "Missing expressions parameter.",
                    };
                    break;
                case ResultStatus.InvalidExpressionsParameter:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.InvalidExpressionsParameter,
                        Message = "Invalid expressions parameter.",
                    };
                    break;
                case ResultStatus.InvalidSubcriptionIdParameter:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.InvalidSubcriptionIdParameter,
                        Message = "Invalid subcriptionId parameter.",
                    };
                    break;
                case ResultStatus.MissingSubcriptionIdParameter:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.MissingSubcriptionIdParameter,
                        Message = "Missing subcriptionId parameter.",
                    };
                    break;
                case ResultStatus.InvalidRequestIdParameter:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.InvalidRequestIdParameter,
                        Message = "Invalid requestId parameter.",
                    };
                    break;
                case ResultStatus.MissingWrittenNameParameter:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.MissingWrittenNameParameter,
                        Message = "Missing name parameter.",
                    };
                    break;
                case ResultStatus.MissingWrittenValueParameter:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.MissingWrittenValueParameter,
                        Message = "Missing value parameter.",
                    };
                    break;
            }

            return dataResponse;
        }
    }
}
