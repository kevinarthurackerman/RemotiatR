using System.Collections.Generic;

namespace RemotiatR.Shared
{
    public class MessageAttributes
    {
        public Attributes RequestAttributes { get; } = new Attributes();
        public Attributes ResponseAttributes { get; } = new Attributes();
    }

    public class Attributes : Dictionary<string,string>
    {
        internal Attributes() { }
    }
}
