using System;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client
{
    public interface IMessageSender
    {
        Task<object> SendRequest(Uri uri, object requestData, Type requestType, Type responseType, CancellationToken cancellationToken);
    }
}
