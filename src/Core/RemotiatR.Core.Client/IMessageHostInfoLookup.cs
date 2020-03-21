using System;
using System.Collections.Generic;

namespace RemotiatR.Client
{
    public interface IMessageHostInfoLookup : IReadOnlyDictionary<Type, IMessageHostInfo>
    {
    }
}
