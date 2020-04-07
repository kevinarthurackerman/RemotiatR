using System;
using System.Collections.Generic;

namespace RemotiatR.Client
{
    public interface IMessageHostInfo
    {
        Uri RootUri { get; }
        Type MessageSerializerType { get; }
        Type MessageTransportType { get; }
        IReadOnlyDictionary<Type, IMessageInfo> MessageInfos { get; }
    }
}
