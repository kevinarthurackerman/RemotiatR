using System.Net.Http;
using System.Threading.Tasks;

namespace RemotiatR.MessageTransport.Http.Client
{
    public delegate Task<HttpResponseMessage> HttpRequestPipelineDelegate();
}
