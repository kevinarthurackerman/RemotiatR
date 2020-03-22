using System;
using RemotiatR.Shared;

namespace RemotiatR.Server
{
    public interface IMessageInfo
    {
        public Type MessageType { get; }
        public Uri EndpointPath { get; }
        public MediatorTypes MediatorType { get; }
        IMessageHostInfo HostInfo { get; }
    }
}
