using System;
using System.Collections.Generic;
using System.Text;

namespace RemotiatR.Shared
{
    public interface IMessageMetadata
    {
        IDictionary<string, string> RequestMetadata { get; }
        IDictionary<string, string> ResponseMetadata { get; }
    }
}
