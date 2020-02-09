using System.Net.Http;
using System.Threading.Tasks;

namespace RemotiatR.Client
{
    public delegate Task<HttpResponseMessage> HttpRequestHandlerDelegate();
}
