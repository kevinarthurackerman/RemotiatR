using System;
using System.Linq;

namespace ContosoUniversity.Shared
{
    public static class Defaults
    {
        public static Func<Type, Uri> UriBuilder => x =>
        {
            var url = x.FullName.Replace('.','/').Replace('+','-');

            return new Uri($"/api/{url}", UriKind.Relative);
        };
    }
}
