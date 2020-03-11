using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Shared
{
    public interface IMessagePipelineHandler
    {
        Task<object> Handle(object message, MessagePipelineDelegate next, CancellationToken cancellationToken = default);
    }
}
