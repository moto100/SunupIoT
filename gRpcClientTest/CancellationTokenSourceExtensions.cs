using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gRpcClientTest
{
    public static class CancellationTokenSourceExtensions
    {
        public static bool TryCancel(this CancellationTokenSource cancellationTokenSource, bool throwOnFirstException = false)
        {
            if (cancellationTokenSource == null)
            {
                return false;
            }

            try
            {
                // Checking the _IsCancellationRequested_ here will not throw an
                // "ObjectDisposedException" as the getter of the property "Token" will do!
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return false;
                }

                cancellationTokenSource.Cancel(throwOnFirstException);
                return true;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }
    }
}
