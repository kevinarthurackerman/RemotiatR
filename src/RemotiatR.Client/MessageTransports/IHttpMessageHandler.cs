using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client.MessageTransports
{
    public interface IHttpMessageHandler
    {
        Task<HttpResponseMessage> Handle(HttpRequestMessage httpRequestMessage, HttpRequestHandlerDelegate next, CancellationToken cancellationToken = default);
    }
}
