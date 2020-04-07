using System;
using System.Collections.Generic;

namespace RemotiatR.Server
{
    public interface IMessageHostInfo
    {
        Uri RootUri { get; }
        Type MessageSerializerType { get; }
        IReadOnlyDictionary<Type, IMessageInfo> MessageInfos { get; }
    }
}
