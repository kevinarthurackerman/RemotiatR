using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.MessageTransport.Http.Client
{
    public interface IHttpMessagePipelineHandler
    {
        Task<HttpResponseMessage> Handle(HttpRequestMessage httpRequestMessage, HttpRequestPipelineDelegate next, CancellationToken cancellationToken = default);
    }
}
