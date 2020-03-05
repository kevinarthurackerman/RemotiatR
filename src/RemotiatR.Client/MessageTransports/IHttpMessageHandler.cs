using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client.MessageTransports
{
    public interface IHttpMessageHandler
    {
        Task<HttpResponseMessage> Handle(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken, HttpRequestHandlerDelegate next);
    }
}
