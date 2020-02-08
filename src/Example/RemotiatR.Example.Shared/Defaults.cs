using System;
using System.Linq;

namespace RemotiatR.Example.Shared
{
    public static class Defaults
    {
        public static Func<Type, Uri> UriBuilder => x =>
             {
                 var segments = x.FullName.Split('.').Last().Split('+').First().Split('_');

                 if (segments.Length != 2) throw new InvalidOperationException("Handled type must contain two segments separated by '_'");

                 return new Uri($"/api/{segments[1]}/{segments[0]}", UriKind.Relative);
             };
    }
}
