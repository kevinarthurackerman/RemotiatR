using System.Net.Http;
using System.Threading.Tasks;

namespace RemotiatR.Client.MessageTransports
{
    public delegate Task<HttpResponseMessage> HttpRequestHandlerDelegate();
}
