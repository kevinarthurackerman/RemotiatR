using System;
using System.Collections.Generic;

namespace RemotiatR.Server
{
    public interface IMessageHostInfoLookup : IReadOnlyDictionary<Uri, IMessageHostInfo>
    {
    }
}
