using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client.MessageTransports
{
    public interface IHttpMessagePipelineHandler
    {
        Task<HttpResponseMessage> Handle(HttpRequestMessage httpRequestMessage, HttpRequestPipelineDelegate next, CancellationToken cancellationToken = default);
    }
}
