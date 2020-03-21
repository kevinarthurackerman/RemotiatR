using System.Collections.Generic;

namespace RemotiatR.Shared
{
    public class MessageMetadata
    {
        public IDictionary<string, string> RequestMetadata { get; } = new Dictionary<string, string>();
        public IDictionary<string, string> ResponseMetadata { get; } = new Dictionary<string, string>();
    }
}
