﻿using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gRpcClientTest
{
    public class NamedPipesConnectionFactory
    {
        private readonly string pipeName;

        public NamedPipesConnectionFactory(string pipeName)
        {
            this.pipeName = pipeName;
        }

        public async ValueTask<Stream> ConnectAsync(SocketsHttpConnectionContext _,
            CancellationToken cancellationToken = default)
        {
            var clientStream = new NamedPipeClientStream(
                serverName: ".",
                pipeName: this.pipeName,
                direction: PipeDirection.InOut,
                options: PipeOptions.WriteThrough | PipeOptions.Asynchronous,
                impersonationLevel: TokenImpersonationLevel.Anonymous);

            try
            {
                await clientStream.ConnectAsync(cancellationToken).ConfigureAwait(false);
                return clientStream;
            }
            catch
            {
                clientStream.Dispose();
                throw;
            }
        }
    }
}
