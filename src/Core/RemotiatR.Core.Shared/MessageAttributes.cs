using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RemotiatR.Shared
{
    public class MessageAttributes
    {
        internal MessageAttributes() { }

        public Attributes RequestAttributes { get; } = new Attributes();
        public Attributes ResponseAttributes { get; } = new Attributes();
    }

    public class Attributes : Collection<Attribute>
    {
        public Attributes(IList<Attribute> attributes) : base(attributes) { }

        public Attributes() { }

        public void Add(string name, string value) => Add(new Attribute(name, value));

        public void Remove(string name)
        {
            var indicies = this.Get(name)
                .Select(x => IndexOf(x))
                .OrderByDescending(x => x)
                .ToArray();

            foreach (var index in indicies) RemoveAt(index);
        }

        public IEnumerable<Attribute> Get(string name) =>
            this.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToArray();
    }

    public class Attribute
    {
        public string Name { get; }

        public string Value { get; }

        public Attribute(string name, string value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
