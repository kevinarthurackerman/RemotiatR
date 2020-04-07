using System.Collections.Generic;

namespace RemotiatR.Shared
{
    public interface IMessageMetadata
    {
        IDictionary<string, string> RequestMetadata { get; }
        IDictionary<string, string> ResponseMetadata { get; }
    }
}
