using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client
{
    public interface IMessageTransport
    {
        Task<object> SendRequest(object requestData, CancellationToken cancellationToken = default);
    }
}
