using RemotiatR.Shared;
using System;

namespace RemotiatR.Client
{
    public interface IMessageInfo
    {
        Type MessageType { get; }
        Uri EndpointPath { get; }
        MediatorTypes MediatorType { get; }
        IMessageHostInfo HostInfo { get; }
    }
}
