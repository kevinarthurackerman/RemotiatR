using System.Threading.Tasks;

namespace RemotiatR.Shared
{
    public delegate Task<object> MessagePipelineDelegate(object message);
}
