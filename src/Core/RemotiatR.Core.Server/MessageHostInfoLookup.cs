using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RemotiatR.Server
{
    internal class MessageHostInfoLookup : ReadOnlyDictionary<Uri, IMessageHostInfo>, IMessageHostInfoLookup
    {
        public MessageHostInfoLookup(IEnumerable<IMessageHostInfo> messageHostInfos) : base(
            messageHostInfos.ToDictionary(x => x.RootUri)
        )
        {
        }
    }
}
