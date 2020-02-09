using System.Net.Http;
using System.Threading.Tasks;

namespace RemotiatR.Client.MessageSenders
{
    public delegate Task<HttpResponseMessage> HttpRequestHandlerDelegate();
}
