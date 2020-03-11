using System;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client.MessageTransports
{
    public interface IMessageTransport
    {
        Task<object> SendRequest(Uri uri, object requestData, CancellationToken cancellationToken = default);
    }
}
