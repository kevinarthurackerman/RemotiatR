using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RemotiatR.Client
{
    internal class MessageHostInfoLookup : ReadOnlyDictionary<Type, IMessageHostInfo>, IMessageHostInfoLookup
    {
        public MessageHostInfoLookup(IEnumerable<IMessageHostInfo> messageHostInfos) : base(
            messageHostInfos.SelectMany(x => x.MessageInfos.Values).ToDictionary(x => x.MessageType, x => x.HostInfo)
        )
        {
        }
    }
}
