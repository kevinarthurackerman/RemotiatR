using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client
{
    public interface IMessageTransport
    {
        Task<Stream> SendRequest(Uri uri, Stream message, object messageObject, CancellationToken cancellationToken = default);
    }
}
